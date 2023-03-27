using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Demonizer;

public class DemonizerBuilder
{
	private readonly HashSet<Assembly> _assemblies = new();
	private IServiceCollection? _serviceCollection;

	public DemonizerBuilder AddDemosFromThisAssembly()
	{
		_assemblies.Add(Assembly.GetCallingAssembly());

		return this;
	}
	
	public DemonizerBuilder AddDemosFromAssembly(Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly);
		_assemblies.Add(assembly);

		return this;
	}

	public DemonizerBuilder AddServices(IServiceCollection serviceCollection)
	{
		ArgumentNullException.ThrowIfNull(serviceCollection);
		_serviceCollection = serviceCollection;

		return this;
	}

	public Demonizer Build()
	{
		_serviceCollection ??= new ServiceCollection();
		return new(_assemblies, _serviceCollection.BuildServiceProvider());
	}
}