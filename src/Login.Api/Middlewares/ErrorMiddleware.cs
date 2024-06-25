namespace Login.Api.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Starting");
                await _next.Invoke(context);
                _logger.LogInformation("Finished with success");
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        ErrorId = errorId,
                        Messages = new List<string> { "Ocorreu um erro inesperado. Contate o suporte." }
                    });

                    _logger.LogInformation(ex, "Finished with error");
                }
            }
        }
    }
}
