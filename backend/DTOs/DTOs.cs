namespace TaskManager.DTOs
{
    //TODO: ADD DTO ALL Records here

    public record AuthRequest
    (
        string Email,
        string Password
    );

    public record AuthResponse
    (
        string message,
        int subId,
        string Email,
        string Token
    );

}
