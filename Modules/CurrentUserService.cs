namespace ksimb_membership.Modules;

public interface ICurrentUserService
{
    public void SetCurrentUser(Guid id);

    public Guid? GetCurrentUser();
}

internal sealed class CurrentUserService : ICurrentUserService
{
    private Guid? _currentUser;

    public void SetCurrentUser(Guid id)
    {
        _currentUser = id;
    }

    public Guid? GetCurrentUser()
    {
        return _currentUser;
    }
}