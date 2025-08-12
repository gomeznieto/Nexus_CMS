using Backend_portafolio.Constants;
using Backend_portafolio.Datos;
using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Backend_portafolio.Services
{
    public interface ILayoutService
    {
        Task DeleteHomeLayoutSection(int id);
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
                    new ($"{SectionTypesNames.UserAbout}", $"{SectionTypeIds.UserAbout}"),
                    new ($"{SectionTypesNames.Bio}", $"{SectionTypeIds.Bio}"),
                    new ( $"{SectionTypesNames.UserHobbies}", $"{SectionTypeIds.UserHobbies}"),
                    new ($"{SectionTypesNames.SocialNetworks}", $"{SectionTypeIds.SocialNetworks}"),
                };

                // Seccion a la lista del dropdown
                foreach (var section in homeSections)
                {
                    options.Add(new SelectListItem
                    {
                        Text = $"Post - {section.Name}",
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

                    if (section.SectionType.Contains("Post"))
                    {
                        var parts = section.SectionType.Split('-');
                        section.SectionType = parts[1].TrimStart().Trim();
                    }

                    if (userSections.Any(s => s.Id == section.Id) && section.Status == Constants.LayoutItemStatus.Modified)
                    {
                        await _repositoryLayout.UpdateLayoutSectionAsync(section);
                    }
                    else
                    {
                        if(section.Status == Constants.LayoutItemStatus.New)
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
        public async Task DeleteHomeLayoutSection(int id)
        {
            try
            {
                var section = await _repositoryLayout.GetLayoutSectionByIdAsync(id);

                if(section == null)
                    throw new Exception("No se ha encontrado la seccion del layout");

                await _repositoryLayout.DeleteLayoutSectionAsync(id);

                // Modificamos el orden de las demas secciones
                var userSections = await _repositoryLayout.GetLayoutHomeSectionsAsync(section.UserId);
                int order = 1;

                foreach (var item in userSections.OrderBy(s => s.DisplayOrder))
                {
                    if (item.DisplayOrder != order)
                    {
                        item.DisplayOrder = order;
                        await _repositoryLayout.UpdateLayoutSectionAsync(item);
                    }
                    order++;
                }
            }
            catch (Exception)
            {
                throw new Exception("Error al eliminar la seccion del layout");
            }
        }
    }
}