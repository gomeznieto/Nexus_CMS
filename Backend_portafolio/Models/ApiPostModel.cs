namespace Backend_portafolio.Models
{
	public class ApiPostModel
	{
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string cover { get; set; }
        public string Format{ get; set; }
        public List<ApiMediaModel> images { get; set; }
        public List<ApiLinkModel> links { get; set; }
        public List<ApiCategoryModel> categories { get; set; }
    }
}
