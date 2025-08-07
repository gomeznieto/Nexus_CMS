using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend_portafolio.Constants;
using Backend_portafolio.Models;

namespace Backend_portafolio.Services
{
    public interface ILayoutService
    {
        public Task<UserHomeLayoutFormModel> GetLayoutForm();
    }
    public class LayoutsService : ILayoutService
    {
        private readonly IHomeSectionService _homeSectionService;
        private readonly IUsersService _usersService;
        public LayoutsService(IHomeSectionService homeSectionService, IUsersService usersService)
        {
            _usersService = usersService;
            _homeSectionService = homeSectionService;
        }

        // --- GET ---
        public async Task<UserHomeLayoutFormModel> GetLayoutForm()
        {
            try
            {
                // Usuario actual
                var currentUser = await _usersService.GetDataUser();

                // Secciones del Home Creadas
                var homeSections = await _homeSectionService.GetByUserAsync(currentUser.Id);


                // Lista para el dropdown de la vista
                var options = new List<SelectListItem>
                {
                    new {"About", $"{SectionTypes.UserAbout}"},
                    new {"Social Networks", $"{SectionTypes.SocialNetworks}"},
                    new {"Hobbies", $"{SectionTypes.UserHobbies}"},
                    new {"Bio", $"{SectionTypes.Bio}"},
                };

                // Seccion a la lista del dropdown
                foreach (var section in homeSections)
                {
                    options.add(new
                    {
                        Text = $"HomeSection - {section.Name}",
                        Value = $"{section.Id}"
                    });
                }

                return new UserHomeLayoutFormModel
                {
                    UserId = currentUser.Id,
                    SectionOptions = options
                };
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error al crear la vista");
            }
        }
    }
}