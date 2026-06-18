using AutoMapper;
using NutriTEC.Application.DTOs.Auth;
using NutriTEC.Application.Exceptions;
using NutriTEC.Application.Interfaces.Auth;
using NutriTEC.Application.Interfaces.Clients;
using NutriTEC.Application.Interfaces.Users;
using NutriTEC.Domain.Entities;

namespace NutriTEC.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IClientRepository clientRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _clientRepository = clientRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<RegisterClientResponse> RegisterClientAsync(
        RegisterClientRequest request,
        CancellationToken cancellationToken)
    {
        // Registration normalizes text before mapping so AutoMapper can remain convention-based.
        NormalizeRegistrationRequest(request);

        // Email uniqueness is a business rule enforced before the registration is persisted.
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("El correo electronico ya esta registrado.");
        }

        // Mapping builds the domain objects while the service applies registration-specific decisions.
        var user = _mapper.Map<User>(request);
        user.HashPassword = _passwordHasher.HashPassword(request.Password);
        user.Age = CalculateAge(request.Birthday);

        var client = _mapper.Map<Client>(request);
        var initialMeasure = _mapper.Map<Measure>(request);
        initialMeasure.MeasureDateTime = DateTime.UtcNow;

        // The repository persists the related records in one transaction to avoid partial registrations.
        await _clientRepository.RegisterClientAsync(user, client, initialMeasure, cancellationToken);

        var response = CreateRegisterClientResponse(user, client);
        return response;
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        // Login normalizes only the lookup key before reading persisted credentials.
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.HashPassword))
        {
            throw new UnauthorizedException("Credenciales invalidas.");
        }

        // The current flow supports clients by checking the related client profile table.
        var client = await _clientRepository.GetByUserIdAsync(user.UserId, cancellationToken);

        if (client is null)
        {
            throw new UnauthorizedException("Credenciales invalidas.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var activePlanAssignment = await _clientRepository.GetActivePlanAssignmentAsync(
            client.ClientId,
            today,
            cancellationToken);

        var response = CreateLoginResponse(user, client, activePlanAssignment);
        return response;
    }

    private static void NormalizeRegistrationRequest(RegisterClientRequest request)
    {
        // The service owns workflow normalization before persistence and duplicate-email checks.
        request.Name = request.Name.Trim();
        request.LastName = request.LastName.Trim();
        request.Country = request.Country.Trim();
        request.Email = request.Email.Trim().ToLowerInvariant();
    }

    private static RegisterClientResponse CreateRegisterClientResponse(User user, Client client)
    {
        // Registration responses expose only safe account/profile identifiers and confirmation text.
        return new RegisterClientResponse
        {
            UserId = user.UserId,
            ClientId = client.ClientId,
            Email = user.Email,
            FullName = $"{user.Name} {user.LastName}",
            Message = "Cliente registrado correctamente."
        };
    }

    private static LoginResponse CreateLoginResponse(
        User user,
        Client client,
        PlanAssignment? activePlanAssignment)
    {
        // Login responses combine account, client profile, and optional active plan context.
        return new LoginResponse
        {
            UserId = user.UserId,
            ClientId = client.ClientId,
            Email = user.Email,
            FullName = $"{user.Name} {user.LastName}",
            AccountType = "Client",
            ActivePlan = CreateActivePlanSummary(activePlanAssignment),
            Message = "Inicio de sesion correcto."
        };
    }

    private static ActivePlanSummaryResponse? CreateActivePlanSummary(PlanAssignment? activePlanAssignment)
    {
        // A null active plan keeps login successful while clearly signaling no current assignment.
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

    private static int CalculateAge(DateOnly birthday)
    {
        // Age is stored because the current SQL schema requires it alongside the birth date.
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthday.Year;

        if (birthday > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
