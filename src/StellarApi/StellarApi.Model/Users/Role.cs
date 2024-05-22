namespace StellarApi.Model.Users
{
    /// <summary>
    /// Represents a role for a User in the application.
    /// </summary>
    public enum Role
    {
        // The admin user is the most powerfull user.
        Admin,
        // The user is the simpliest role, it can do some things, but with limitations.
        User
    }
}
