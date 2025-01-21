namespace Backend_portafolio.Models
{
	public class ApiPostViewModel
	{
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string cover { get; set; }
        public string format{ get; set; }
        public List<ApiMediaViewModel> media { get; set; }
        public List<ApiLinkViewModel> links { get; set; }
        public List<ApiCategoryViewModel> categories { get; set; }
    }
}
