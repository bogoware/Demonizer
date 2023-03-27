using System.Reflection;

namespace Demonizer.Commands;

internal sealed class RunDemosConfiguration
{
	/// <summary>
	/// Assemblies scanned for Demos.
	/// </summary>
	public readonly List<Assembly> Assemblies;
	/// <summary>
	/// <see cref="IServiceProvider"/> used for Demos' DI.
	/// </summary>
	public readonly IServiceProvider ServiceProvider;

	public RunDemosConfiguration(List<Assembly> assemblies, IServiceProvider serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(assemblies);
		ArgumentNullException.ThrowIfNull(serviceProvider);
		Assemblies = assemblies;
		ServiceProvider = serviceProvider;
	}
}