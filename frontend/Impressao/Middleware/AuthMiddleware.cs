
namespace Impressao.Middleware
{
    public class AuthMiddleware
    {
        private RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var req = context.Request;

            if (context.Session.Get("User Id") == null && req.Path.ToString() != "/Auth/Login")
            {
                context.Response.Redirect("/Auth/Login");
            }

            await _next(context);

        }
    }
}
