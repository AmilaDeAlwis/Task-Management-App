using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using System.Text;
using TaskManagementApi.Interfaces;

namespace TaskManagementApi.Filter
{
    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly IUserService _userService;
        private readonly ILogger<BasicAuthFilter> _logger;

        public BasicAuthFilter(IUserService userService, ILogger<BasicAuthFilter> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Handle Authorization using Basic Authentication.
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (!context.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    SetUnauthorizedResult(context);
                    return;
                }

                var authHeader = AuthenticationHeaderValue.Parse(context.HttpContext.Request.Headers["Authorization"]);

                if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    SetUnauthorizedResult(context);
                    return;
                }

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length != 2)
                {
                    SetUnauthorizedResult(context);
                    return;
                }

                var username = credentials[0];
                var password = credentials[1];

                var user = _userService.GetUserByUsernameAsync(username).Result;
                if (user == null || !_userService.VerifyPasswordAsync(user, password).Result)
                {
                    SetUnauthorizedResult(context);
                    return;
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Basic Authorization header format.");
                context.Result = new BadRequestObjectResult(new { Message = "Invalid Authorization header format." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during basic authentication.");
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private void SetUnauthorizedResult(AuthorizationFilterContext context)
        {
            context.Result = new UnauthorizedResult();
            context.HttpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"TaskManagement\"");
        }
    }

    public class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute() : base(typeof(BasicAuthFilter)) { }
    }
}
