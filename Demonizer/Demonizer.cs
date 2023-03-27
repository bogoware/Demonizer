using System.Reflection;
using Spectre.Console;

namespace Demonizer;

public sealed class Demonizer
{
	private List<Assembly> _assemblies;
	private IServiceProvider? _serviceProvider;
	public bool HasServiceProvider => _serviceProvider != null;

	internal Demonizer(IEnumerable<Assembly> assemblies, IServiceProvider? serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(assemblies);
		_assemblies = assemblies.ToList();
		_serviceProvider = serviceProvider;
	}
	
	public void Run(string[] args)
	{
		AnsiConsole.Markup("[underline red]Hello[/] World!");
	}
}
