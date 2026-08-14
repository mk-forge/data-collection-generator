using TupleGeneratorGUI.Components;
using TupleGeneratorGUI.Services;

namespace TupleGeneratorGUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddScoped<AuthService>();
            builder.Services.AddSingleton<LDAPAuthService>();
            builder.Services.AddScoped<BusyStateService>();
            builder.Services.AddScoped<StateService>();
            builder.Services.AddScoped<ProtectedSessionStorage>();
            builder.Services.AddScoped<LocalStorageSyncService>();
            builder.Services.AddSingleton<PageMetadataService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAntiforgery();
            app.MapStaticAssets();

            app.MapGet("/favicon.ico", () => Results.Redirect("/favicon.png"));

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
