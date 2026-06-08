using AutoMapper;
using NutriTEC.Application.DTOs.Auth;
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

    public async Task<AuthServiceResult<RegisterClientResponse>> RegisterClientAsync(
        RegisterClientRequest request,
        CancellationToken cancellationToken)
    {
        // Registration normalizes text before mapping so AutoMapper can remain convention-based.
        NormalizeRegistrationRequest(request);

        // Email uniqueness is a business rule enforced before the registration is persisted.
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return AuthServiceResult<RegisterClientResponse>.EmailConflict("El correo electronico ya esta registrado.");
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
        return AuthServiceResult<RegisterClientResponse>.Success(response);
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
