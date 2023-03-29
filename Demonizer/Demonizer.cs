using Demonizer.Commands;
using Demonizer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Demonizer;

public sealed class Demonizer
{
	private readonly HashSet<DemoDescriptor> _demoDescriptors;
	private readonly IServiceProvider _serviceProvider;
	private readonly string? _appName;

	internal Demonizer(HashSet<DemoDescriptor> demoDescriptors, IServiceProvider serviceProvider, string? appName)
	{
		ArgumentNullException.ThrowIfNull(demoDescriptors);
		ArgumentNullException.ThrowIfNull(serviceProvider);
		_demoDescriptors = demoDescriptors;
		_serviceProvider = serviceProvider;
		_appName = appName;
	}

	public int Run(string[] args)
	{
		var app = new CommandApp<Main>(GetTypeRegistrar());
		if (_appName != null)
		{
			app.Configure(conf => { conf.SetApplicationName(_appName); });
		}
		return app.Run(args);
	}

	/// <summary>
	/// Return an  suitable to initialize an <see cref="CommandApp"/> or Spectre.Console.Testing.CommandAppTester.
	/// </summary>
	/// <returns></returns>
	internal ITypeRegistrar GetTypeRegistrar()
	{
		var cliServices = new ServiceCollection();
		cliServices.AddSingleton(new Main.Configuration(_demoDescriptors, _serviceProvider));
		return new TypeRegistrar(cliServices);
	}
}