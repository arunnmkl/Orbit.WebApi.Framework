using System;
using System.Collections.Generic;
using Orbit.WebApi.Core.Dependency;
using Orbit.WebApi.Security.Models;
using Orbit.WebApi.Security.Models.Chat;

namespace Orbit.WebApi.Security
{
    /// <summary>
    /// User manager
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class UserManager : IDisposable
    {
        /// <summary>
        /// The authentication repo
        /// </summary>
        private readonly AuthSqlRepository authRepo;

        /// <summary>
        /// The security command
        /// </summary>
        private ISecurityCommand securityCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager" /> class.
        /// </summary>
        public UserManager()
        {
            authRepo = new AuthSqlRepository(AuthContext.AuthDal);
            securityCommand = DependencyResolverContainer.Resolve<ISecurityCommand>();
        }

        /// <summary>
        /// Authenticates the username password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// user identity
        /// </returns>
        public UserIdentity AuthenticateUsernamePassword(string userName, string password)
        {
            if (securityCommand != null)
            {
                if (securityCommand.ValidateUsernameAndPassword(userName, password) == false)
                {
                    return null;
                }

                password = securityCommand.Encrypt(password);
            }
            else
            {
                password = SecureString.Encrypt(password);
            }

            var result = authRepo.AuthenticateUsernamePassword(userName, password);

            if (securityCommand != null && result != null)
            {
                result.FullName = securityCommand.GetUserFullName(result.UserId);
            }

            if (securityCommand != null && result != null)
            {
                result.UserCulture = securityCommand.GetUserCulture(result.UserId);
            }

            return result;
        }

        /// <summary>
        /// Finds the authentication client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>
        /// authentication client information
        /// </returns>
        public AuthClient FindAuthClient(string clientId)
        {
            return authRepo.FindAuthClient(clientId);
        }

        /// <summary>
        /// Finds the login provider.
        /// </summary>
        /// <param name="authProvider">The authentication provider.</param>
        /// <returns>
        /// user identity details
        /// </returns>
        public UserIdentity FindLoginProvider(AuthProvider authProvider)
        {
            return authRepo.FindLoginProvider(authProvider);
        }

        /// <summary>
        /// Adds the refresh token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// success or failure
        /// </returns>
        public bool AddRefreshToken(RefreshToken token)
        {
            return authRepo.AddRefreshToken(token);
        }

        /// <summary>
        /// Removes the refresh token.
        /// </summary>
        /// <param name="tokenId">The token identifier.</param>
        /// <returns>
        /// success or failure
        /// </returns>
        public bool RemoveRefreshToken(string tokenId)
        {
            return authRepo.RemoveRefreshToken(tokenId);
        }

        /// <summary>
        /// Finds the refresh token.
        /// </summary>
        /// <param name="hashedTokenId">The hashed token identifier.</param>
        /// <returns>
        /// refresh token
        /// </returns>
        public RefreshToken FindRefreshToken(string hashedTokenId)
        {
            return authRepo.FindRefreshToken(hashedTokenId);
        }

        /// <summary>
        /// Gets all refresh tokens.
        /// </summary>
        /// <returns>
        /// all refresh tokens
        /// </returns>
        public IList<RefreshToken> GetAllRefreshTokens()
        {
            return authRepo.GetAllRefreshTokens();
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns>
        /// user id
        /// </returns>
        public long AddUser(string username, string password, string emailAddress = "", bool enabled = true)
        {
            if (securityCommand != null)
            {
                password = securityCommand.Encrypt(password);
            }
            else
            {
                password = SecureString.Encrypt(password);
            }

            return authRepo.AddUser(username, password, emailAddress, enabled);
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="authProvider">The authentication provider.</param>
        /// <returns>
        /// inserted state.
        /// </returns>
        public bool AddNewUserLoginProvider(AuthProvider authProvider)
        {
            return authRepo.AddNewUserLoginProvider(authProvider);
        }

        /// <summary>
        /// Finds the user by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// user identity details
        /// </returns>
        public UserIdentity FindUserByUserId(long userId)
        {
            var result = authRepo.GetAuthenticatedUserByUserId(userId);
            if (securityCommand != null && result != null)
            {
                result.FullName = securityCommand.GetUserFullName(result.UserId);
            }

            if (securityCommand != null && result != null)
            {
                result.UserCulture = securityCommand.GetUserCulture(result.UserId);
            }
            return result;
        }

        /// <summary>
        /// Gets the user permissions.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IList<ResourceAccessRule> GetUserPermissions(long userId)
        {
            return authRepo.GetUserPermissions(userId);
        }

        /// <summary>
        /// Gets the user resource permission.
        /// </summary>
        /// <param name="securityIds">The security ids.</param>
        /// <returns>
        /// user resource permissions
        /// </returns>
        public IList<ResourceAccessRule> GetUserResourcePermission(IList<Guid> securityIds)
        {
            return authRepo.GetUserResourcePermission(securityIds);
        }

        /// <summary>
        /// Saves the user authentication token.
        /// </summary>
        /// <param name="userAuthToken">The user authentication token.</param>
        /// <returns>
        /// saved state
        /// </returns>
        public bool SaveUserAuthToken(UserAuthToken userAuthToken)
        {
            return authRepo.SaveUserAuthToken(userAuthToken);
        }

        /// <summary>
        /// Gets the user authentication token.
        /// </summary>
        /// <param name="userAuthToken">The user authentication token.</param>
        /// <returns>
        /// the user authentication token details
        /// </returns>
        public UserAuthToken GetUserAuthToken(UserAuthToken userAuthToken)
        {
            return authRepo.GetUserAuthToken(userAuthToken);
        }

        /// <summary>
        /// Sets the token expires.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userAuthTokenId">The user authentication token identifier.</param>
        /// <returns>
        /// row count
        /// </returns>
        public int SetTokenExpires(long userId, string userAuthTokenId)
        {
            return authRepo.SetTokenExpires(userId, userAuthTokenId);
        }

        /// <summary>
        /// Generates the authentication token.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="checkExistence">if set to <c>true</c> [check existence].</param>
        /// <param name="killOldSession">if set to <c>true</c> [kill old session].</param>
        /// <returns>
        /// the new authentication token
        /// </returns>
        public string GenerateAuthToken(string username, bool checkExistence = false, bool killOldSession = false)
        {
            return authRepo.GenerateAuthToken(username, checkExistence, killOldSession);
        }

        /// <summary>
        /// Gets the user authentication token by identifier.
        /// </summary>
        /// <param name="userAuthTokenId">The user authentication token identifier.</param>
        /// <returns>
        /// the user authentication token details
        /// </returns> 
        public UserAuthToken GetUserAuthTokenById(string userAuthTokenId)
        {
            return authRepo.GetUserAuthTokenById(userAuthTokenId);
        }

        /// <summary>
        /// Gets the password timestamp.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>the password timestamp</returns>                                     
        public long GetPasswordTimestamp(long userId)
        {
            return authRepo.GetPasswordTimestamp(userId);
        }

        /// <summary>
        /// Saves the chat history.
        /// </summary>
        /// <param name="history">The history.</param>
        /// <returns>true if chat history successfully updated, otherwise false.</returns>
        public bool SaveChatHistory(ChatHistory history)
        {
            return authRepo.SaveChatHistory(history);
        }

        /// <summary>
        /// Gets the chat history.
        /// </summary>
        /// <param name="user1">The user1.</param>
        /// <param name="user2">The user2.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>the chat history</returns>
        public IList<ChatHistory> GetChatHistory(string user1, string user2, string searchText = null)
        {
            return authRepo.GetChatHistory(user1, user2, searchText);
        }

        /// <summary>
        /// Gets the associated chat users.
        /// </summary>
        /// <param name="securityId">The security identifier.</param>
        /// <returns>the associated chat users</returns>
        public IList<ChatUser> GetAssociatedChatUsers(Guid securityId)
        {
            return authRepo.GetAssociatedChatUsers(securityId);
        }

        /// <summary>
        /// Gets the user identifier by name.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>the user identifier</returns>
        public long FindUserIdByUsername(string username)
        {
            return authRepo.FindUserIdByUsername(username);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.authRepo.Dispose();
            }
        }
    }
}
