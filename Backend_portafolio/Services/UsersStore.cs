using Backend_portafolio.Entities;
using Backend_portafolio.Datos;
using Microsoft.AspNetCore.Identity;
using Backend_portafolio.Models;
using AutoMapper;

namespace Backend_portafolio.Services
{
    public class UsersStore : IUserStore<UserViewModel>, IUserEmailStore<UserViewModel>, IUserPasswordStore<UserViewModel>
    {
        public readonly IRepositoryUsers _repositoryUsers;
        private readonly IMapper _mapper;

        public UsersStore(IRepositoryUsers repositoryUsers, IMapper mapper)
        {
            _repositoryUsers = repositoryUsers;
            _mapper = mapper;
        }

        public async Task<IdentityResult> CreateAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            user.id = await _repositoryUsers.CrearUsuario(_mapper.Map<User>(user));

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<UserViewModel> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return _mapper.Map<UserViewModel>(await _repositoryUsers.BuscarUsuarioPorEmail(normalizedEmail));
        }

        public Task<UserViewModel> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserViewModel> FindByNameAsync(string usernameNormalizado, CancellationToken cancellationToken)
        {
            return _mapper.Map<UserViewModel>(await _repositoryUsers.BuscarUsuarioPorUsername(usernameNormalizado));
        }

        public Task<string> GetEmailAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.email);
        }

        public async Task<bool> GetEmailConfirmedAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return true;
        }

        public Task<string> GetNormalizedEmailAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.id.ToString());
        }

        public Task<string> GetUserNameAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(UserViewModel user, string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(UserViewModel user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(UserViewModel user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.emailNormalizado = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(UserViewModel user, string normalizedName, CancellationToken cancellationToken)
        {
            user.usernameNormalizado = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(UserViewModel user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IdentityResult> IUserStore<UserViewModel>.CreateAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            user.id = await _repositoryUsers.CrearUsuario(_mapper.Map<User>(user));
            return IdentityResult.Success;
        }

        Task<IdentityResult> IUserStore<UserViewModel>.DeleteAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {

        }

        async Task<UserViewModel> IUserStore<UserViewModel>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return _mapper.Map<UserViewModel>(await _repositoryUsers.BuscarPorId(Int32.Parse(userId)));
        }

        async Task<UserViewModel> IUserStore<UserViewModel>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return _mapper.Map<UserViewModel>(await _repositoryUsers.BuscarUsuarioPorUsername(normalizedUserName));
        }

        Task<string> IUserStore<UserViewModel>.GetNormalizedUserNameAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IUserPasswordStore<UserViewModel>.GetPasswordHashAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.passwordHash);
        }

        Task<string> IUserStore<UserViewModel>.GetUserIdAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.id.ToString());
        }

        Task<string> IUserStore<UserViewModel>.GetUserNameAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.username);
        }

        Task<bool> IUserPasswordStore<UserViewModel>.HasPasswordAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IUserStore<UserViewModel>.SetNormalizedUserNameAsync(UserViewModel user, string normalizedName, CancellationToken cancellationToken)
        {
            user.usernameNormalizado = normalizedName;
            return Task.CompletedTask;
        }

        Task IUserPasswordStore<UserViewModel>.SetPasswordHashAsync(UserViewModel user, string passwordHash, CancellationToken cancellationToken)
        {
            user.passwordHash = passwordHash;
            return Task.CompletedTask;
        }

        Task IUserStore<UserViewModel>.SetUserNameAsync(UserViewModel user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<IdentityResult> IUserStore<UserViewModel>.UpdateAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            await _repositoryUsers.EditarUsuario(_mapper.Map<User>(user));

            return IdentityResult.Success;
        }
    }
}
