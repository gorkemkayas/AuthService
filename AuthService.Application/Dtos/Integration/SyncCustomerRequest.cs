namespace AuthService.Application.Dtos.Integration;

public sealed record SyncCustomerRequest(
    Guid ExternalUserId,
    string Email,
    string FirstName,
    string LastName);