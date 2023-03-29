namespace Demonizer.Commands;

internal sealed partial class Main
{
	internal sealed class Configuration
	{
		/// <summary>
		/// Demos to execute.
		/// </summary>
		public readonly List<DemoDescriptor> Demos;

		/// <summary>
		/// <see cref="IServiceProvider"/> used for Demos' DI.
		/// </summary>
		public readonly IServiceProvider ServiceProvider;

		public Configuration(IEnumerable<DemoDescriptor> demos, IServiceProvider serviceProvider)
		{
			ArgumentNullException.ThrowIfNull(demos);
			ArgumentNullException.ThrowIfNull(serviceProvider);
			Demos = demos.ToList();
			ServiceProvider = serviceProvider;
		}
	}
}