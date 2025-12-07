using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;

namespace OmniRepo.Infrastructure.Identity;
[DapperAot]

public class ApplicationUserStore(SqliteConnection connection)
    : IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>
{
    
    public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        const string sql = "INSERT INTO Users (Id, UserName, Email, PasswordHash) VALUES (@Id, @UserName, @Email, @PasswordHash)";
        connection.Execute(sql, user);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        const string sql = "UPDATE Users SET UserName=@UserName, Email=@Email, PasswordHash=@PasswordHash WHERE Id=@Id";
        connection.Execute(sql, user);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM Users WHERE Id=@Id";
        connection.Execute(sql, new { user.Id });
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Users WHERE Id=@Id";
        var user = connection.QuerySingleOrDefault<ApplicationUser>(sql, new { Id = userId });
        return Task.FromResult(user);
    }

    public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Users WHERE UPPER(UserName)=@UserName";
        var user = connection.QuerySingleOrDefault<ApplicationUser>(sql, new { UserName = normalizedUserName });
        return Task.FromResult(user);
    }

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);
    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken) { user.UserName = userName; return Task.CompletedTask; }
    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName?.ToUpper());
    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken) { user.PasswordHash = passwordHash; return Task.CompletedTask; }
    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);
    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash != null);

    public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken) { user.Email = email; return Task.CompletedTask; }
    public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email);
    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(true);
    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Users WHERE UPPER(Email)=@Email";
        var user = connection.QuerySingleOrDefault<ApplicationUser>(sql, new { Email = normalizedEmail });
        return Task.FromResult(user);
    }
    public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.Email?.ToUpper());
    public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken) => Task.CompletedTask;

    public void Dispose() { }
}

public class ApplicationRoleStore : IRoleStore<ApplicationRole>
{
    private readonly IDbConnection _connection;

    public ApplicationRoleStore(IDbConnection connection)
    {
        _connection = connection;
    }

    public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        const string sql = "INSERT INTO Roles (Id, Name) VALUES (@Id, @Name)";
        _connection.Execute(sql, role);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        const string sql = "UPDATE Roles SET Name=@Name WHERE Id=@Id";
        _connection.Execute(sql, role);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM Roles WHERE Id=@Id";
        _connection.Execute(sql, new { role.Id });
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<ApplicationRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Roles WHERE Id=@Id";
        var role = _connection.QuerySingleOrDefault<ApplicationRole>(sql, new { Id = roleId });
        return Task.FromResult(role);
    }

    public Task<ApplicationRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM Roles WHERE UPPER(Name)=@Name";
        var role = _connection.QuerySingleOrDefault<ApplicationRole>(sql, new { Name = normalizedRoleName });
        return Task.FromResult(role);
    }

    public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken) => Task.FromResult(role.Id);
    public Task<string?> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) => Task.FromResult(role.Name);
    public Task SetRoleNameAsync(ApplicationRole role, string? roleName, CancellationToken cancellationToken) { role.Name = roleName; return Task.CompletedTask; }
    public Task<string?> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) => Task.FromResult(role.Name?.ToUpper());
    public Task SetNormalizedRoleNameAsync(ApplicationRole role, string? normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
    public void Dispose() { }
}
