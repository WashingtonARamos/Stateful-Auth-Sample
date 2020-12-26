namespace Stateful_Auth_Sample.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public UserRole Role { get; set; }

        public enum UserRole
        {
            Reader = 0,
            Creator = 1
        }
    }
}
