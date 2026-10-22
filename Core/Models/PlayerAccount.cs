namespace DungeonsAndDragqueens.Core.Models;

public class PlayerAccount
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = "";
    public string NormalizedUserName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public int Gold { get; set; }
    public bool HasAutoFight { get; set; }
    public bool HasAutoCamp { get; set; }
}
