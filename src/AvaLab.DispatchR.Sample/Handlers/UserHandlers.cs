using AvaLab.DispatchR.Abstraction;
using AvaLab.DispatchR.Sample.Commands;
using AvaLab.DispatchR.Sample.Services;

namespace AvaLab.DispatchR.Sample.Handlers;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public CreateUserCommandHandler(IUserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(CreateUserCommand command, CancellationToken ct)
    {
        _logger.Log($"Creating user: {command.Username}");
        
        var user = new User(
            Id: await _userRepository.GetNextIdAsync(),
            Username: command.Username,
            Email: command.Email
        );

        await _userRepository.AddUserAsync(user);
        
        _logger.Log($"User created successfully: {user.Id}");
    }
}

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public UpdateUserCommandHandler(IUserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        _logger.Log($"Updating user {command.UserId} email to: {command.Email}");
        
        var user = await _userRepository.GetUserByIdAsync(command.UserId);
        if (user == null)
        {
            _logger.Log($"User {command.UserId} not found");
            return false;
        }

        var updatedUser = user with { Email = command.Email };
        await _userRepository.UpdateUserAsync(updatedUser);
        
        _logger.Log($"User {command.UserId} updated successfully");
        return true;
    }
}

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User?>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public GetUserByIdQueryHandler(IUserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        _logger.Log($"Querying user by ID: {query.UserId}");
        
        var user = await _userRepository.GetUserByIdAsync(query.UserId);
        
        if (user == null)
        {
            _logger.Log($"User {query.UserId} not found");
        }
        else
        {
            _logger.Log($"User found: {user.Username}");
        }
        
        return user;
    }
}

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IEnumerable<User>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public GetAllUsersQueryHandler(IUserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken ct)
    {
        _logger.Log("Querying all users");
        
        var users = await _userRepository.GetAllUsersAsync();
        
        _logger.Log($"Found {users.Count()} users");
        
        return users;
    }
}
