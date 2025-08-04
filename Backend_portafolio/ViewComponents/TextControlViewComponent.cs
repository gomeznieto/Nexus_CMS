using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.ViewComponents
{
    public class TextControlViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string fieldName, string fieldValue, EditMode editMode)
        {
            var model = new TextControlModel
            {
                FieldName = fieldName,
                FieldValue = fieldValue,
                EditMode = editMode
            };

            return View(model);
        }
    }
}
