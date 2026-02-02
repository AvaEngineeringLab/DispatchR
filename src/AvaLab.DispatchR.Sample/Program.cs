using AvaLab.DispatchR;
using AvaLab.DispatchR.Abstraction;
using AvaLab.DispatchR.MicrosoftExtensionsDI;
using AvaLab.DispatchR.Sample.Commands;
using AvaLab.DispatchR.Sample.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AvaLab.DispatchR.Sample;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("=== DispatchR Sample Application ===\n");

        // Setup Dependency Injection
        var services = new ServiceCollection();

        // Register DispatchR and scan this assembly for handlers
        services.AddDispatchR(cfg =>
        {
            cfg.RegisterAssembly(Assembly.GetExecutingAssembly());
        });

        // Register application services
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<ILogger, ConsoleLogger>();

        var serviceProvider = services.BuildServiceProvider();
        var dispatcher = serviceProvider.GetRequiredService<IDispatchR>();

        Console.WriteLine("DispatchR initialized successfully!\n");

        // Demo: Create users
        Console.WriteLine("--- Creating Users ---");
        await dispatcher.Send(new CreateUserCommand("john_doe", "john@example.com"));
        await dispatcher.Send(new CreateUserCommand("jane_smith", "jane@example.com"));
        await dispatcher.Send(new CreateUserCommand("bob_wilson", "bob@example.com"));

        Console.WriteLine();

        // Demo: Query all users
        Console.WriteLine("--- Querying All Users ---");
        var allUsers = await dispatcher.Query(new GetAllUsersQuery());
        foreach (var user in allUsers)
        {
            Console.WriteLine($"  - {user.Id}: {user.Username} ({user.Email})");
        }

        Console.WriteLine();

        // Demo: Query specific user
        Console.WriteLine("--- Querying Specific User ---");
        var specificUser = await dispatcher.Query(new GetUserByIdQuery(2));
        if (specificUser != null)
        {
            Console.WriteLine($"  Found: {specificUser.Username} - {specificUser.Email}");
        }

        Console.WriteLine();

        // Demo: Update user
        Console.WriteLine("--- Updating User ---");
        var updateResult = await dispatcher.Send(new UpdateUserCommand(2, "jane.updated@example.com"));
        Console.WriteLine($"  Update result: {updateResult}");

        Console.WriteLine();

        // Demo: Verify update
        Console.WriteLine("--- Verifying Update ---");
        var updatedUser = await dispatcher.Query(new GetUserByIdQuery(2));
        if (updatedUser != null)
        {
            Console.WriteLine($"  Updated email: {updatedUser.Email}");
        }

        Console.WriteLine();

        // Demo: Query non-existent user
        Console.WriteLine("--- Querying Non-Existent User ---");
        var nonExistentUser = await dispatcher.Query(new GetUserByIdQuery(999));
        Console.WriteLine($"  Result: {(nonExistentUser == null ? "Not found" : "Found")}");

        Console.WriteLine();

        // Demo: Update non-existent user
        Console.WriteLine("--- Updating Non-Existent User ---");
        var failedUpdate = await dispatcher.Send(new UpdateUserCommand(999, "test@example.com"));
        Console.WriteLine($"  Update result: {failedUpdate}");

        Console.WriteLine();
        Console.WriteLine("=== Demo Complete ===");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}