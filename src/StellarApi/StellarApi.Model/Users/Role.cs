namespace StellarApi.Model.Users
{
    /// <summary>
    /// Represents a role for a User in the application.
    /// </summary>
    public enum Role
    {
        // The administrator user is the most powerfull user.
        Administrator,
        // The member is the simpliest role, it can do some things, but with limitations.
        Member
    }
}
