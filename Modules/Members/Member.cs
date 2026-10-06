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

    public required string BirthPlace { get; set; }

    public string College { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public MembershipStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Gender Gender { get; set; }

    public bool IsAdmin { get; set; }

    public Guid? ApproverId { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public int? MemberCardNumber { get; set; }

    public bool IsCardCreated { get; set; }
}

public enum College
{
    NisamStudent,
    AF,
    ADU,
    AGR,
    ALU,
    EFZG,
    ERF,
    FBF,
    FER,
    FFRZ,
    FFZG,
    FHS,
    FKIT,
    FPZ,
    FPZG,
    FSB,
    FŠDT,
    GEOD,
    GF,
    GRF,
    KBF,
    KIF,
    MEF,
    MUZA,
    PBF,
    PFZG,
    PMF,
    RGN,
    SFZG,
    TTF,
    UFZG,
    VEF,
    Drugo
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