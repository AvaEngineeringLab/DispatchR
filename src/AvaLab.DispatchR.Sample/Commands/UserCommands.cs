using AvaLab.DispatchR.Abstraction;

namespace AvaLab.DispatchR.Sample.Commands;

// Command without response
public record CreateUserCommand(string Username, string Email) : ICommand;

// Command with response
public record UpdateUserCommand(int UserId, string Email) : ICommand<bool>;

// Query
public record GetUserByIdQuery(int UserId) : IQuery<User?>;

public record GetAllUsersQuery : IQuery<IEnumerable<User>>;

// Domain model
public record User(int Id, string Username, string Email);
