namespace AvaLab.DispatchR.Abstraction
{
    public interface ICommand
    { }

    public interface ICommand<out TResponse> : ICommand
    { }
}