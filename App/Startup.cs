namespace App;

public class Startup
{
    public static void Services(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
    }

    public static void App(WebApplication app)
    {
        app.UseAuthorization();

        app.MapControllers();
    }
}