using Backend_portafolio.Entities;
using Backend_portafolio.Datos;
using Microsoft.AspNetCore.Identity;

namespace Backend_portafolio.Services
{
    public class UsersStore : IUserStore<User>, IUserEmailStore<User>, IUserPasswordStore<User>
    {
        public readonly IRepositoryUsers _repositoryUsers;
        public UsersStore(IRepositoryUsers repositoryUsers)
        {
            _repositoryUsers = repositoryUsers;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            user.id = await _repositoryUsers.CrearUsuario(user);

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await _repositoryUsers.BuscarUsuarioPorEmail(normalizedEmail);
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _repositoryUsers.BuscarUsuarioPorEmail(normalizedUserName);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.email);
        }

        public async Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return true;
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.emailNormalizado = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IdentityResult> IUserStore<User>.CreateAsync(User user, CancellationToken cancellationToken)
        {
            user.id = await _repositoryUsers.CrearUsuario(user);
            return IdentityResult.Success;
        }

        Task<IdentityResult> IUserStore<User>.DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {

        }

        async Task<User> IUserStore<User>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _repositoryUsers.BuscarPorId(Int32.Parse(userId));
        }

        async Task<User> IUserStore<User>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _repositoryUsers.BuscarUsuarioPorEmail(normalizedUserName);
        }

        Task<string> IUserStore<User>.GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IUserPasswordStore<User>.GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.passwordHash);
        }

        Task<string> IUserStore<User>.GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.id.ToString());
        }

        Task<string> IUserStore<User>.GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.email);
        }

        Task<bool> IUserPasswordStore<User>.HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IUserStore<User>.SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.emailNormalizado = normalizedName;
            return Task.CompletedTask;
        }

        Task IUserPasswordStore<User>.SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.passwordHash = passwordHash;
            return Task.CompletedTask;
        }

        Task IUserStore<User>.SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IdentityResult> IUserStore<User>.UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await _repositoryUsers.EditarUsuario(user);

            return IdentityResult.Success;
        }
    }
}
