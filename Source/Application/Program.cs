using Application.Models.DirectoryServices.Protocols;
using Application.Models.DirectoryServices.Protocols.Configuration;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting host ...");

try
{
	var builder = WebApplication.CreateBuilder(args);

	builder.Host.UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
	{
		loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
		loggerConfiguration.ReadFrom.Services(serviceProvider);
	});

	builder.Services.AddControllersWithViews();
	builder.Services.AddSingleton<ILdapConnectionFactory, LdapConnectionFactory>();
	builder.Services.AddSingleton<ILdapConnectionOptionsParser, LdapConnectionOptionsParser>();
	builder.Services.Configure<DirectoryOptions>(builder.Configuration.GetSection("Directory"));

	var app = builder.Build();

	app.UseStaticFiles();
	app.UseRouting();
	app.MapDefaultControllerRoute();

	app.Run();
}
catch(Exception exception)
{
	Log.Fatal(exception, "Host terminated unexpectedly.");
}
finally
{
	Log.Information("Stopping host ...");
	Log.CloseAndFlush();
}