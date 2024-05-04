namespace Domain.Shared;

public class AdminOptions
{
    public const string SectionName = "Admin";

    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
