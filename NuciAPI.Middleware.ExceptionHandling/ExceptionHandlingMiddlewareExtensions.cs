using Microsoft.AspNetCore.Builder;

namespace NuciAPI.Middleware.ExceptionHandling
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseNuciApiExceptionHandling(
            this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}