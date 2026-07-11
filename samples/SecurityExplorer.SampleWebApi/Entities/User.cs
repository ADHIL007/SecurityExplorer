namespace SecurityExplorer.SampleWebApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public required string Password { get; set; }

        public string Department { get; set; } = String.Empty;

        public string address { get; set; }
        //public HashSet<string> Permissions { get; set; } = new HashSet<string>();
    }
}
