namespace Orbit.WebApi.Core.Common
{
    /// <summary>
    /// Authorize response message
    /// </summary>
    public static class AuthorizeResponseMessage
    {
        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>
        /// Authorization has been denied for this request.
        /// </value>
        public static ResponseError Default
        {
            get
            {
                return new ResponseError(401, "Authorization has been denied for this request.");
            }
        }

        /// <summary>
        /// Gets the require authorization.
        /// </summary>
        /// <value>
        /// The require authorization.
        /// </value>
        public static ResponseError RequireAuthorization
        {
            get
            {
                return new ResponseError(400, "Request require authorization");
            }
        }

        /// <summary>
        /// Gets the missing access token.
        /// </summary>
        /// <value>
        /// The missing access token.
        /// </value>
        public static ResponseError MissingAccessToken
        {
            get
            {
                return new ResponseError(400, "Missing access token");
            }
        }

        /// <summary>
        /// Gets the invalid bearer token.
        /// </summary>
        /// <value>
        /// The invalid bearer token.
        /// </value>
        public static ResponseError InvalidBearerToken
        {
            get
            {
                return new ResponseError(401, "Invalid bearer token received");
            }
        }

        /// <summary>
        /// Gets the user session expired.
        /// </summary>
        /// <value>
        /// The user session expired.
        /// </value>
        public static ResponseError UserSessionExpired
        {
            get
            {
                return new ResponseError(403, "The client's session has already expired");
            }
        }

        /// <summary>
        /// Gets the token expired.
        /// </summary>
        /// <value>
        /// The token expired.
        /// </value>
        public static ResponseError TokenExpired
        {
            get
            {
                return new ResponseError(401, "The Token has expired");
            }
        }

        /// <summary>
        /// Gets the access denied.
        /// </summary>
        /// <value>
        /// The access denied.
        /// </value>
        public static ResponseError AccessDenied
        {
            get
            {
                return new ResponseError(401, "Access Denied");
            }
        }

        /// <summary>
        /// Gets the no principal.
        /// </summary>
        /// <value>
        /// The no principal.
        /// </value>
        public static ResponseError NoPrincipal
        {
            get
            {
                return new ResponseError(401, "No current principal");
            }
        }

        /// <summary>
        /// Gets the altered credential.
        /// </summary>
        /// <value>
        /// The altered credential.
        /// </value>
        public static ResponseError AlteredCredential
        {
            get
            {
                return new ResponseError(401, "Your password has been changed.");
            }
        }
    }

    /// <summary>
    /// Class to encapsulate response error
    /// </summary>
    public class ResponseError
    {
        /// <summary>
        /// The error response message
        /// </summary>
        private ErrorResponseMessage errorResponseMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public ResponseError(int code, string message)
        {
            errorResponseMessage = new ErrorResponseMessage(code, message);
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public ErrorResponseMessage Error
        {
            get
            {
                return errorResponseMessage;
            }
        }
    }

    /// <summary>
    /// Class to encapsulate error response message.
    /// </summary>
    public class ErrorResponseMessage
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponseMessage"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="message">The message.</param>
        public ErrorResponseMessage(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
