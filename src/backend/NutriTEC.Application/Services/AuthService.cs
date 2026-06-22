using AutoMapper;
using NutriTEC.Application.DTOs.Auth;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Auth;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Application.Interfaces.Nutritionists;
using NutriTEC.Application.Interfaces.Users;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly INutritionistRepository _nutritionistRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IClientRepository clientRepository,
        INutritionistRepository nutritionistRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _nutritionistRepository = nutritionistRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<RegisterClientResponse> RegisterClientAsync(
        RegisterClientRequest request,
        CancellationToken cancellationToken)
    {
        // Registration normalizes text before mapping so AutoMapper can remain convention-based.
        NormalizeRegistrationRequest(request);

        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("El correo electronico ya esta registrado.");
        }

        var user = _mapper.Map<User>(request);
        user.HashPassword = _passwordHasher.HashPassword(request.Password);

        var client = _mapper.Map<Client>(request);
        var initialMeasure = _mapper.Map<Measure>(request);
        initialMeasure.MeasureDateTime = DateTime.Today;

        await _clientRepository.RegisterClientAsync(user, client, initialMeasure, cancellationToken);

        return new RegisterClientResponse
        {
            UserId = user.UserId,
            ClientId = client.ClientId,
            Age = user.Age,
            Email = user.Email,
            FullName = $"{user.Name} {user.LastName}",
            Message = "Cliente registrado correctamente."
        };
    }

    public async Task<RegisterNutritionistResponse> RegisterNutritionistAsync(
        RegisterNutritionistRequest request,
        CancellationToken cancellationToken)
    {
        NormalizeNutritionistRequest(request);

        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("El correo electronico ya esta registrado.");
        }

        if (await _nutritionistRepository.IdNumberExistsAsync(request.IdNumber, cancellationToken))
        {
            throw new ConflictException("El numero de cedula ya esta registrado.");
        }

        var user = new User
        {
            Name = request.Name,
            LastName = request.LastName,
            Birthday = request.Birthday,
            Email = request.Email,
            HashPassword = _passwordHasher.HashPassword(request.Password)
        };

        var nutritionist = new Nutritionist
        {
            IdNumber = request.IdNumber,
            Weight = request.Weight,
            BodyMassIndex = request.BodyMassIndex,
            Address = request.Address,
            Photo = request.Photo,
            EncryptedCreditCard = request.EncryptedCreditCard,
            // El cobro siempre es por tarjeta; la frecuencia la elige el nutricionista.
            PaymentMethod = "CARD",
            BillingFrequency = request.BillingFrequency
        };

        await _nutritionistRepository.RegisterNutritionistAsync(user, nutritionist, cancellationToken);

        return new RegisterNutritionistResponse
        {
            UserId = user.UserId,
            NutritionistCode = nutritionist.NutritionistCode,
            Age = user.Age,
            Email = user.Email,
            FullName = $"{user.Name} {user.LastName}",
            Message = "Nutricionista registrado correctamente."
        };
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.HashPassword))
        {
            throw new UnauthorizedException("Credenciales invalidas.");
        }

        // Check client first, then nutritionist, to determine account type.
        var client = await _clientRepository.GetByUserIdAsync(user.UserId, cancellationToken);
        if (client is not null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var activePlanAssignment = await _clientRepository.GetActivePlanAssignmentAsync(
                client.ClientId,
                today,
                cancellationToken);

            return new LoginResponse
            {
                UserId = user.UserId,
                ClientId = client.ClientId,
                NutritionistCode = null,
                Age = user.Age,
                Email = user.Email,
                FullName = $"{user.Name} {user.LastName}",
                AccountType = "Client",
                ActivePlan = CreateActivePlanSummary(activePlanAssignment),
                Message = "Inicio de sesion correcto."
            };
        }

        var nutritionist = await _nutritionistRepository.GetByUserIdAsync(user.UserId, cancellationToken);
        if (nutritionist is not null)
        {
            return new LoginResponse
            {
                UserId = user.UserId,
                ClientId = null,
                NutritionistCode = nutritionist.NutritionistCode,
                Age = user.Age,
                Email = user.Email,
                FullName = $"{user.Name} {user.LastName}",
                AccountType = "Nutritionist",
                ActivePlan = null,
                Message = "Inicio de sesion correcto."
            };
        }

        throw new UnauthorizedException("Credenciales invalidas.");
    }

    private static void NormalizeRegistrationRequest(RegisterClientRequest request)
    {
        request.Name = request.Name.Trim();
        request.LastName = request.LastName.Trim();
        request.Country = request.Country.Trim();
        request.Email = request.Email.Trim().ToLowerInvariant();
    }

    private static void NormalizeNutritionistRequest(RegisterNutritionistRequest request)
    {
        request.Name = request.Name.Trim();
        request.LastName = request.LastName.Trim();
        request.Email = request.Email.Trim().ToLowerInvariant();
        request.IdNumber = request.IdNumber.Trim();
        request.Address = request.Address.Trim();
        request.BillingFrequency = request.BillingFrequency.Trim().ToUpperInvariant();
    }

    private static ActivePlanSummaryResponse? CreateActivePlanSummary(PlanAssignment? activePlanAssignment)
    {
        if (activePlanAssignment is null)
        {
            return null;
        }

        return new ActivePlanSummaryResponse
        {
            AssignmentId = activePlanAssignment.AssignmentId,
            PlanId = activePlanAssignment.PlanId,
            PlanName = activePlanAssignment.NutritionPlan.PlanName,
            TotalCalories = activePlanAssignment.NutritionPlan.TotalCalories,
            NutritionistCode = activePlanAssignment.NutritionPlan.NutritionistCode,
            StartDate = activePlanAssignment.StartDate,
            EndDate = activePlanAssignment.EndDate,
            AssignmentStatus = activePlanAssignment.AssignmentStatus
        };
    }
}
