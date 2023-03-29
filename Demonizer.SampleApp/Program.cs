using Demonizer.SampleApp.Demos;
using Demonizer.SampleApp.Services;

var demonizerApp = new DemonizerBuilder()
	.ConfigureServices(services => // Configure IoC for DI
	{
		services.AddSingleton<NowTimeService>();
	})
	.AddDemo<HelloWorldDemo>()  // Adding explicitly demos
	.AddDemosFromThisAssembly() // Discover demos from assemblies
	.Build();                   // Build the app

return demonizerApp.Run(args); // Tun the App