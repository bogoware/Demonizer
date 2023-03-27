using Demonizer;
using Demonizer.SampleApp;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<NowTimeService>();

return new DemonizerBuilder()
	.AddServices(services)
	.AddDemosFromThisAssembly()
	.Build()
	.Run(args);