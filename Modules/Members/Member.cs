namespace ksimb_membership.Modules.Members;

public sealed class Member
{
    public Guid Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string FullName => FirstName + " " + LastName;

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    //OIB
    public required string PersonalIdentityNumber { get; set; }

    public College College { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public MembershipStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Gender Gender { get; set; }

    public bool IsAdmin { get; set; }
}

public enum College
{
    NisamStudent,
    FER,
    FSB,
    FKIT,
    FPZ,
    AF,
    GF,
    GEOD,
    GRF,
    RGN,
    TTF,
    EFZG,
    PFZG,
    FFZG,
    FPZG,
    FHS,
    ERF,
    UFZG,
    KIF,
    FFRZ,
    MEF,
    SFZG,
    FBF,
    VEF,
    PMF,
    PBF,
    AGR,
    FŠDT,
    KBF,
    ADU,
    ALU,
    MUZA,
    Furešto
}

public enum Gender
{
    Muško,
    Žensko
}

public enum MembershipStatus
{
    Pending,
    Active,
    Denied
}