namespace AvaLab.DispatchR.Abstraction
{
    /// <summary>
    /// Strongly-typed dispatch API (avoids Task<T> casting issues).
    /// </summary>
    public interface IDispatchR
    {
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);

        Task Send(ICommand command, CancellationToken ct = default);

        Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
    }
}