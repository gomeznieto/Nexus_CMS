namespace Backend_portafolio.Models
{
	public class ListPostViewModel
	{
        public string format { get; set; }
        public IEnumerable<Post> posts { get; set; }
    }
}
