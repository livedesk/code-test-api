using code_test_api.Services;

namespace code_test_api.Models
{
    public class InMemoryUserStore
    {
        private readonly List<User> users = new List<User>();

        /// <summary>
        /// Add a user to the store
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void AddUser(string username, string email, string password)
        {
            // Get the hash and salt for the given password
            Utils.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            // Add new user to the store
            users.Add(new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
        }

        /// <summary>
        /// Authenticate the username by username and password
        /// Returns the user or null
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public User? AuthenticateUser(string userName, string password)
        {
            // Find user on username
            var foundUser = users.FirstOrDefault(user => user.Username == userName);

            // Verify the password is valid
            if (foundUser != null)
            {
                var passwordValid = Utils.VerifyPasswordHash(password, foundUser.PasswordHash, foundUser.PasswordSalt);

                if (passwordValid)
                    return foundUser;
            }

            return null;
        }

        /// <summary>
        /// Gets a user by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User? GetUser(string userName)
        {
            // Find user on username
            var foundUser = users.FirstOrDefault(user => user.Username == userName);

            return foundUser;
        }
    }
}
