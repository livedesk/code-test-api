using code_test_api.Dtos;
using code_test_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;



namespace code_test_api.Services
{
    public interface IUserService
    {
        public UserForReturnDto Register(UserForRegisterDto model);
        public string? Login(UserForLoginDto model);
        public UserForReturnDto? GetUser(string userName);        
    }
    public class UserService : IUserService
    {
        private readonly InMemoryUserStore _inMemoryUserStore;
        private readonly Utils _utils;

        public UserService(InMemoryUserStore inMemoryUserStore, Utils utils)
        {
            _inMemoryUserStore = inMemoryUserStore;
            _utils = utils;
        }


        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserForReturnDto Register(UserForRegisterDto model)
        {
            try
            {
                _inMemoryUserStore.AddUser(model.Username, model.Email, model.Password);

                return new UserForReturnDto() { Email = model.Email, Username = model.Username };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            // add user to the user store

        }

        /// <summary>
        /// Login a user
        /// Returns the Email and Password if user is authenticated or null if not authenticated
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string? Login(UserForLoginDto model)
        {
            try
            {
                User? authenticatedUser = _inMemoryUserStore.AuthenticateUser(model.Username, model.Password);

                // guard not auth'd user
                if (authenticatedUser == null)
                    return null;

                // Get JWT Token from the auth'd user
                var jwtToken = _utils.CreateJWT(authenticatedUser);

                return jwtToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public UserForReturnDto? GetUser(string userName)
        {
            var user = _inMemoryUserStore.GetUser(userName);

            if (user == null)
                return null;

            // could use some object mapping tool here instead!!!
            return new UserForReturnDto() { Username = user.Username, Email = user.Email };
        }
    }
}
