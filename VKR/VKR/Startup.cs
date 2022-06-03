using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Sample;


namespace VKR
{
    public class Startup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services, IPlatform platform)
        {
            services.AddSingleton<SampleSqliteConnection>();

            //services.UseGeofencing<GeofenceDelegate>();
            // if you need some very real-time geofencing, you want to use below - this will really hurt your user's battery
            services.UseGpsDirectGeofencing<GeofenceDelegate>();

            services.UseNotifications();
            // we use this in the example, it isn't needed for geofencing in general
            //services.UseGps();
        }
    }
}