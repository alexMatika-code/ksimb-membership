namespace ksimb_membership.Modules.Security;

public sealed class SecuritySettings
{
    public Guid Id { get; set; }

    public required string AdminSecretHash { get; set; }
}
