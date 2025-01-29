namespace Backend_portafolio.Entities
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string usernameNormalizado { get; set; }
        public int role { get; set; }
        public string img { get; set; }
        public string cv { get; set; }
        public string about { get; set; }
        public string hobbies { get; set; }
        public string email { get; set; }
        public string emailNormalizado { get; set; }
        public string passwordHash { get; set; }
        public string apiKey { get; set; }

    }
}
