namespace AuthService.Application.Interfaces.Contexts
{
    public interface IClientContext
    {
        string ClientType { get; }
        string? DeviceId { get; }
    }
}
