using Backend_portafolio.Constants;
using Backend_portafolio.Datos;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend_portafolio.Services
{
    public interface ILayoutService
    {
        public Task<UserHomeLayoutFormModel> GetLayoutForm(int userId);
        public Task SaveLayoutForm(UserHomeLayoutFormModel model, int userId);
    }
    public class LayoutsService : ILayoutService
    {
        private readonly IHomeSectionService _homeSectionService;
        private readonly IRepositoryLayout _repositoryLayout;

        public LayoutsService(
            IHomeSectionService homeSectionService, 
            IRepositoryLayout repositoryLayout
            )
        {
            _repositoryLayout = repositoryLayout;
            _homeSectionService = homeSectionService;
        }

        // --- GET ---
        public async Task<UserHomeLayoutFormModel> GetLayoutForm(int userId)
        {
            try
            {             

                // Secciones del Home Creadas
                var homeSections = await _homeSectionService.GetByUserAsync(userId);


                // Lista para el dropdown de la vista
                var options = new List<SelectListItem>
                {
                    new ($"{SectionTypes.UserAbout}", $"{userId}"),
                    new ($"{SectionTypes.SocialNetworks}", $"{userId}"),
                    new ( $"{SectionTypes.UserHobbies}", $"{userId}"),
                    new ($"{SectionTypes.Bio}", $"{userId}"),
                };

                // Seccion a la lista del dropdown
                foreach (var section in homeSections)
                {
                    options.Add(new SelectListItem
                    {
                        Text = $"{section.Name}",
                        Value = $"{section.Id}"
                    });
                }

                // Obtener secciones del usuario
                var userSections = await _repositoryLayout.GetLayoutHomeSectionsAsync(userId);

                return new UserHomeLayoutFormModel
                {
                    Sections = userSections.ToList(),
                    SectionOptions = options
                };
            }
            catch (Exception)
            {
                throw new Exception("Error al crear la vista");
            }
        }

        // --- POST ---
        public async Task SaveLayoutForm(UserHomeLayoutFormModel model, int userId)
        {
            try
            {

                var userSections = await _repositoryLayout.GetLayoutHomeSectionsAsync(userId);

                foreach (var section in model.Sections)
                {
                    section.UserId = userId;
                    if (userSections.Any(s => s.Id == section.Id))
                    {
                        await _repositoryLayout.UpdateLayoutSectionAsync(section);
                    }
                    else
                    {

                        await _repositoryLayout.CreateLayoutSectionAsync(section);
                        
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        // DELETE

    }
}