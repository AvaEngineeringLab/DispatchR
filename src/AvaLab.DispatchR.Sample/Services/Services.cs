using AvaLab.DispatchR.Sample.Commands;

namespace AvaLab.DispatchR.Sample.Services;

public interface IUserRepository
{
    Task<int> GetNextIdAsync();
    Task AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(int userId);
    Task UpdateUserAsync(User user);
    Task<IEnumerable<User>> GetAllUsersAsync();
}

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public Task<int> GetNextIdAsync()
    {
        return Task.FromResult(_nextId++);
    }

    public Task AddUserAsync(User user)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task<User?> GetUserByIdAsync(int userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        return Task.FromResult(user);
    }

    public Task UpdateUserAsync(User user)
    {
        var index = _users.FindIndex(u => u.Id == user.Id);
        if (index >= 0)
        {
            _users[index] = user;
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return Task.FromResult<IEnumerable<User>>(_users);
    }
}

public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}
