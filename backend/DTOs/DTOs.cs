namespace TaskManager.DTOs
{
    //TODO: ADD DTO ALL Records here

    public record RegisterRequest
    (
        string Email,
        string Password
    );

    public record RegisterResponse
    (
        int Id,
        string Email,
        string Token
    );

}
