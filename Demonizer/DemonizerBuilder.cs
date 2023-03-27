using System.Reflection;

namespace Demonizer;

public class DemonizerBuilder
{
	private readonly HashSet<Assembly> _assemblies = new();
	private IServiceProvider? _serviceProvider;
	
	public DemonizerBuilder AddDemosFromExecutingAssembly()
	{
		_assemblies.Add(Assembly.GetExecutingAssembly());

		return this;
	}
	
	public DemonizerBuilder AddDemosFromAssembly(Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly);
		_assemblies.Add(assembly);

		return this;
	}

	public DemonizerBuilder AddServiceProvider(IServiceProvider serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(serviceProvider);
		_serviceProvider = serviceProvider;

		return this;
	}

	public Demonizer Build()
	{
		if (_assemblies.Count == 0) AddDemosFromExecutingAssembly();
		return new Demonizer(_assemblies, _serviceProvider);
	}
}