using static Stateful_Auth_Sample.Models.User;

namespace Stateful_Auth_Sample.Authentication
{
    public class Roles
    {

        public const string Reader = nameof(UserRole.Reader);
        public const string Creator = nameof(UserRole.Creator);

    }
}
