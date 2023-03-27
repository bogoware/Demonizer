using Spectre.Console.Cli;

namespace Demonizer.Infrastructure;

internal sealed class TypeResolver : ITypeResolver, IDisposable
{
	private readonly IServiceProvider _provider;

	public TypeResolver(IServiceProvider provider)
	{
		ArgumentNullException.ThrowIfNull(provider);
		_provider = provider;
	}

	public object? Resolve(Type? type)
	{
		if (type == null)
		{
			return null!;
		}

		return _provider.GetService(type);
	}

	public void Dispose()
	{
		if (_provider is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}
}