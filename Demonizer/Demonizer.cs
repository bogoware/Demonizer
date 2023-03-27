using System.Diagnostics;
using System.Reflection;
using System.Text;
using Demonizer.Commands;
using Demonizer.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Demonizer;

public sealed class Demonizer
{
	private readonly List<Assembly> _assemblies;
	private readonly IServiceProvider _serviceProvider;

	internal Demonizer(IEnumerable<Assembly> assemblies, IServiceProvider serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(assemblies);
		ArgumentNullException.ThrowIfNull(serviceProvider);
		_assemblies = assemblies.ToList();
		_serviceProvider = serviceProvider;
	}

	public int Run(string[] args)
	{
		var cliServices = new ServiceCollection();
		cliServices.AddSingleton(new RunDemosConfiguration(_assemblies, _serviceProvider));
		var registrar = new TypeRegistrar(cliServices);
		var app = new CommandApp<RunDemosCommand>(registrar);
		return app.Run(args);
		
	}

	
}