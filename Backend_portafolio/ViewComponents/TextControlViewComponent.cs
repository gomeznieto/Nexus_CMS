using Backend_portafolio.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_portafolio.ViewComponents
{
    public class TextControlViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string fieldName, string fieldValue)
        {
            var model = new TextControlModel
            {
                FieldName = fieldName,
                FieldValue = fieldValue
            };

            return View(model);
        }
    }
}
