using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Backend_portafolio.Entities
{
    public class Format
    {
        public int id { get; set; }
        [Display(Name = "Formato de Entrada")]
        [Remote(action: "VerificarExisteFormato", controller: "Formats")]
        public string name { get; set; }
        public int user_id { get; set; }
    }
}
