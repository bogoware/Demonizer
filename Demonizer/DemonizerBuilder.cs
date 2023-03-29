using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable MemberCanBePrivate.Global

namespace Demonizer;

public class DemonizerBuilder
{
	private readonly HashSet<DemoDescriptor> _demos = new();
	public IServiceCollection ServiceCollection { get; } = new ServiceCollection();
	public string AppName { get; private set; } = null!;

	/// <summary>
	/// Sets the app name used to generate help messages.
	/// </summary>
	public DemonizerBuilder SetAppName(string name)
	{
		ArgumentNullException.ThrowIfNull(name);
		AppName = name;
		return this;
	}
	
	/// <summary>
	/// Add a demo
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	/// <exception cref="InvalidCastException">In case the <see cref="type"/> doesn't inherit from <see cref="IDemo"/></exception>
	public DemonizerBuilder AddDemo(Type type)
	{
		ArgumentNullException.ThrowIfNull(type);

		if (!type.IsAssignableTo(typeof(IDemo)))
		{
			throw new InvalidCastException();
		}

		DemoDescriptor descriptor = type.GetCustomAttribute<DemoAttribute>() switch
		{
			null => new()
			{
				Type = type,
				Name = type.Name,
				Description = string.Empty,
				IsEnabled = true,
				Order = -1
			},
			var attr => new()
			{
				Type = type,
				Name = attr.Name ?? type.Name,
				Description = attr.Description ?? string.Empty,
				IsEnabled = attr.Enabled,
				Order = attr.Order
			}
		};

		_demos.Add(descriptor);

		return this;
	}

	/// <summary>
	/// Add a demo
	/// </summary>
	public DemonizerBuilder AddDemo<T>() where T : IDemo => AddDemo(typeof(T));

	/// <summary>
	/// Add all the demos present in the calling assembly.
	/// </summary>
	public DemonizerBuilder AddDemosFromThisAssembly() =>
		AddDemosFromAssembly(Assembly.GetCallingAssembly());

	/// <summary>
	/// Add all the demos present in the <see cref="assembly"/>.
	/// </summary>
	public DemonizerBuilder AddDemosFromAssembly(Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly);

		var demoTypes = (from t in assembly.GetTypes().ToList()
			where t.IsClass && t.IsAssignableTo(typeof(IDemo))
			select t).ToList();

		foreach (var demo in demoTypes)
		{
			AddDemo(demo);
		}

		return this;
	}

	/// <summary>
	/// Configure services for DI
	/// <seealso cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="configurator"></param>
	/// <returns></returns>
	public DemonizerBuilder ConfigureServices(Action<IServiceCollection> configurator)
	{
		configurator(ServiceCollection);
		return this;
	}

	/// <summary>
	/// Build the <see cref="Demonizer"/>.
	/// </summary>
	/// <returns></returns>
	public Demonizer Build() => new(_demos, ServiceCollection.BuildServiceProvider(), AppName);
}