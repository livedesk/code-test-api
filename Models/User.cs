namespace code_test_api.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        //public string Password { get; set; }     Upgraded to use Hash/Salt instead 
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
    public class StringList
    {
        public List<string> Items { get; set; }
    }

    public class StringListService
    {
        public List<string> Items { get; } = new List<string>();
    }
}
