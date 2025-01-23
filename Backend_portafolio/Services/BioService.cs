using AutoMapper;
using Backend_portafolio.Datos;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend_portafolio.Services
{
    public interface IBioService
    {
        Task CreateBio(BioViewModel viewModel);
        Task DeleteBio(int id);
        Task EditBio(BioViewModel viewModel);
        Task<List<Bio>> GetAllBio(int userID = 0);
        Task<Bio> GetBioById(int id);
        Task<BioViewModel> GetBioViewModel(BioViewModel viewModel = null);
    }
    public class BioService : IBioService
    {

        private readonly IUsersService _usersService;
        private readonly IRepositoryBio _repositoryBio;


        public BioService(
            IUsersService usersService,
            IRepositoryBio repositoryBio
        )
        {
            _usersService = usersService;
            _repositoryBio = repositoryBio;
        }

        //****************************************************
        //*********************** GETS ***********************
        //****************************************************
        public async Task<List<Bio>> GetAllBio(int userID = 0)
        {
            try
            {
                if (userID == 0)
                    userID = _usersService.ObtenerUsuario();

                return (await _repositoryBio.Obtener(userID)).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<BioViewModel> GetBioViewModel(BioViewModel viewModel = null)
        {
            try
            {
                viewModel = viewModel ?? new BioViewModel();
                var usuarioID = _usersService.ObtenerUsuario();
                viewModel.user_id = usuarioID;
                viewModel.Bios = (await GetAllBio()).OrderByDescending(x => x.year).ToList();
                return viewModel;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<Bio> GetBioById(int id)
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();
                return await _repositoryBio.ObtenerPorId(id, usuarioID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //****************************************************
        //********************** CREATE **********************
        //****************************************************

        public async Task CreateBio(BioViewModel viewModel)
        {
            try
            {
                // Validar usuario
                var userID = _usersService.ObtenerUsuario();
                if (userID != viewModel.user_id)
                    throw new ApplicationException("No puedes crear una bio para otro usuario");

                await _repositoryBio.Agregar(viewModel);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        //****************************************************
        //********************** DELETE **********************
        //****************************************************

        public async Task DeleteBio(int id)
        {
            try
            {
                var usuarioID = _usersService.ObtenerUsuario();
                var bio = await _repositoryBio.ObtenerPorId(id, usuarioID);

                // Validar que la bio exista
                if (bio == null)
                    throw new ApplicationException("Bio no encontrada");

                // Validar que el usuario sea el dueño de la bio
                if (bio.user_id != usuarioID)
                    throw new ApplicationException("No puedes eliminar una bio de otro usuario");

                await _repositoryBio.Borrar(id, usuarioID);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //****************************************************
        //*********************** EDIT ***********************
        //****************************************************

        public async Task EditBio(BioViewModel viewModel)
        {
            try
            {
                //Validar usuario
                var usuarioID = _usersService.ObtenerUsuario();
                if (usuarioID != viewModel.user_id)
                    throw new ApplicationException("No puedes editar una bio de otro usuario");

                //Validar que la bio exista
                var bio = await _repositoryBio.ObtenerPorId(viewModel.id, usuarioID);
                if (bio == null)
                    throw new ApplicationException("Bio no encontrada");

                await _repositoryBio.Editar(viewModel);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
