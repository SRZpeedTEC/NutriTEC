using NutriTEC.Application.DTOs.Admin;
using NutriTEC.Application.DTOs.Auth;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Admin;
using NutriTEC.Application.Interfaces.Auth;

namespace NutriTEC.Application.Services;

public class AdminService : IAdminService
{
    private const string ActiveStatus = "ACTIVE";
    private const string PendingReviewStatus = "PENDING_REVIEW";
    private const string RejectedStatus = "REJECTED";

    private readonly IAdminRepository _adminRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AdminService(
        IAdminRepository adminRepository,
        IPasswordHasher passwordHasher)
    {
        _adminRepository = adminRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AdminLoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        // Admin login uses the admin stored procedure, then verifies the hash with the existing hasher.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var admin = await _adminRepository.GetAdminLoginByEmailAsync(normalizedEmail, cancellationToken);

        if (admin is null || !_passwordHasher.VerifyPassword(request.Password, admin.HashPassword))
        {
            throw new UnauthorizedException("Credenciales invalidas.");
        }

        return new AdminLoginResponse
        {
            UserId = admin.UserId,
            AdminId = admin.AdminId,
            Age = CalculateAge(admin.Birthday),
            Email = admin.Email,
            FullName = $"{admin.Name} {admin.LastName}",
            AccountType = "Admin",
            ActivePlan = null,
            Message = "Inicio de sesion de administrador correcto."
        };
    }

    public async Task<IReadOnlyCollection<AdminProductResponse>> GetProductsAsync(
        string? productStatus,
        CancellationToken cancellationToken)
    {
        var normalizedStatus = NormalizeOptionalStatus(productStatus);
        return await _adminRepository.GetProductsAsync(normalizedStatus, cancellationToken);
    }

    public async Task<AdminBillingReportResponse> GetBillingReportAsync(
        AdminBillingReportRequest request,
        CancellationToken cancellationToken)
    {
        NormalizeBillingReportRequest(request);
        var charges = await _adminRepository.GetBillingChargesAsync(request, cancellationToken);
        var nutritionists = charges
            .GroupBy(charge => new
            {
                charge.BillingFrequency,
                charge.NutritionistCode,
                charge.NutritionistName,
                charge.NutritionistEmail,
                charge.PaymentMethod,
                charge.CreditCardNumber,
                charge.PricePerPatient,
                charge.DiscountRate
            })
            .Select(group =>
            {
                var patients = group
                    .OrderBy(charge => charge.ClientName)
                    .ThenBy(charge => charge.ClientId)
                    .Select(charge => new AdminBillingPatientChargeResponse
                    {
                        ClientId = charge.ClientId,
                        ClientName = charge.ClientName,
                        ActiveFrom = charge.ActiveFrom,
                        ActiveTo = charge.ActiveTo,
                        ActiveDays = charge.ActiveDays,
                        ProrationFactor = charge.ProrationFactor,
                        AmountBeforeDiscount = charge.AmountBeforeDiscount,
                        Amount = charge.AmountBeforeDiscount
                    })
                    .ToList();

                var totalBeforeDiscount = RoundMoney(patients.Sum(patient => patient.AmountBeforeDiscount));
                var discountApplied = RoundMoney(totalBeforeDiscount * group.Key.DiscountRate);
                var finalAmount = totalBeforeDiscount - discountApplied;

                return new AdminBillingNutritionistChargeResponse
                {
                    BillingFrequency = group.Key.BillingFrequency,
                    NutritionistCode = group.Key.NutritionistCode,
                    NutritionistName = group.Key.NutritionistName,
                    NutritionistEmail = group.Key.NutritionistEmail,
                    PaymentMethod = group.Key.PaymentMethod,
                    CreditCardNumber = group.Key.CreditCardNumber,
                    PricePerPatient = group.Key.PricePerPatient,
                    DiscountRate = group.Key.DiscountRate,
                    PatientCount = patients.Count,
                    TotalAmountBeforeDiscount = totalBeforeDiscount,
                    DiscountApplied = discountApplied,
                    FinalAmount = finalAmount,
                    TotalAmount = finalAmount,
                    Patients = patients
                };
            })
            .OrderBy(nutritionist => nutritionist.BillingFrequency)
            .ThenBy(nutritionist => nutritionist.NutritionistName)
            .ThenBy(nutritionist => nutritionist.NutritionistCode)
            .ToList();

        var cycleDays = (request.CycleEndDate.Date - request.CycleStartDate.Date).Days + 1;
        var totalAmountBeforeDiscount = nutritionists.Sum(nutritionist => nutritionist.TotalAmountBeforeDiscount);
        var totalDiscountApplied = nutritionists.Sum(nutritionist => nutritionist.DiscountApplied);
        var finalAmount = nutritionists.Sum(nutritionist => nutritionist.FinalAmount);

        return new AdminBillingReportResponse
        {
            Frequency = request.Frequency,
            CycleStartDate = request.CycleStartDate.Date,
            CycleEndDate = request.CycleEndDate.Date,
            CycleDays = cycleDays,
            PricePerPatient = request.PricePerPatient,
            NutritionistCount = nutritionists.Count,
            PatientChargeCount = nutritionists.Sum(nutritionist => nutritionist.PatientCount),
            TotalAmountBeforeDiscount = totalAmountBeforeDiscount,
            TotalDiscountApplied = totalDiscountApplied,
            FinalAmount = finalAmount,
            TotalAmount = finalAmount,
            NutritionistsByBillingFrequency = nutritionists
                .GroupBy(nutritionist => nutritionist.BillingFrequency)
                .ToDictionary(
                    group => group.Key,
                    group => (IReadOnlyCollection<AdminBillingNutritionistChargeResponse>)group.ToList()),
            Nutritionists = nutritionists
        };
    }

    public async Task<AdminProductResponse> GetProductByBarCodeAsync(
        string barCode,
        CancellationToken cancellationToken)
    {
        var normalizedBarCode = NormalizeBarCode(barCode);
        var product = await _adminRepository.GetProductByBarCodeAsync(normalizedBarCode, cancellationToken);

        return product ?? throw new NotFoundException("No se encontro el producto solicitado.");
    }

    public Task<AdminProductMutationResponse> ApproveProductAsync(
        string barCode,
        int adminId,
        CancellationToken cancellationToken)
    {
        return ReviewProductAsync(
            barCode,
            adminId,
            ActiveStatus,
            "Producto aprobado correctamente.",
            cancellationToken);
    }

    public Task<AdminProductMutationResponse> RejectProductAsync(
        string barCode,
        int adminId,
        CancellationToken cancellationToken)
    {
        return ReviewProductAsync(
            barCode,
            adminId,
            RejectedStatus,
            "Producto rechazado correctamente.",
            cancellationToken);
    }

    private async Task<AdminProductMutationResponse> ReviewProductAsync(
        string barCode,
        int adminId,
        string newStatus,
        string message,
        CancellationToken cancellationToken)
    {
        var normalizedBarCode = NormalizeBarCode(barCode);

        if (adminId <= 0)
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [nameof(adminId)] = new[] { "El identificador del administrador debe ser mayor que 0." }
            });
        }

        if (!await _adminRepository.AdminExistsAsync(adminId, cancellationToken))
        {
            throw new NotFoundException("No se encontro el administrador que revisa el producto.");
        }

        if (await _adminRepository.GetProductByBarCodeAsync(normalizedBarCode, cancellationToken) is null)
        {
            throw new NotFoundException("No se encontro el producto solicitado.");
        }

        var product = await _adminRepository.UpdateProductStatusByAdminAsync(
            normalizedBarCode,
            adminId,
            newStatus,
            cancellationToken);

        return new AdminProductMutationResponse
        {
            Message = message,
            Product = product ?? throw new NotFoundException("No se encontro el producto solicitado.")
        };
    }

    private static string? NormalizeOptionalStatus(string? productStatus)
    {
        if (string.IsNullOrWhiteSpace(productStatus))
        {
            return null;
        }

        var normalizedStatus = productStatus.Trim().ToUpperInvariant();

        if (normalizedStatus is not ActiveStatus
            and not PendingReviewStatus
            and not RejectedStatus)
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [nameof(productStatus)] = new[]
                {
                    "El estado del producto debe ser ACTIVE, PENDING_REVIEW o REJECTED."
                }
            });
        }

        return normalizedStatus;
    }

    private static string NormalizeBarCode(string barCode)
    {
        if (string.IsNullOrWhiteSpace(barCode))
        {
            throw new ApplicationValidationException(new Dictionary<string, string[]>
            {
                [nameof(barCode)] = new[] { "El codigo de barras es obligatorio." }
            });
        }

        return barCode.Trim();
    }

    private static void NormalizeBillingReportRequest(AdminBillingReportRequest request)
    {
        request.Frequency = string.IsNullOrWhiteSpace(request.Frequency)
            ? null
            : request.Frequency.Trim().ToUpperInvariant();
        request.CycleStartDate = request.CycleStartDate.Date;
        request.CycleEndDate = request.CycleEndDate.Date;
        if (request.PricePerPatient.HasValue)
        {
            request.PricePerPatient = RoundMoney(request.PricePerPatient.Value);
        }
    }

    private static decimal RoundMoney(decimal amount)
    {
        return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    private static int CalculateAge(DateOnly birthday)
    {
        if (birthday == default)
        {
            return 0;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthday.Year;
        return birthday > today.AddYears(-age) ? age - 1 : age;
    }
}
