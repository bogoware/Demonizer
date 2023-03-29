using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

// ReSharper disable ClassNeverInstantiated.Global

namespace Demonizer.Commands;

internal sealed partial class Main : Command<Main.Settings>
{
	private readonly Configuration _config;
	private readonly IAnsiConsole _console;

	public Main(IAnsiConsole console, Configuration config)
	{
		_console = console;
		_config = config;
	}

	public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
	{
		try
		{
			if (ConfigurationIsInvalid())
			{
				return -1;
			}

			if (settings.ListOnly)
			{
				ListDemos(context, settings);
			}
			else
			{
				return RunDemos(context, settings);
			}
		}
		catch (Exception ex)
		{
			_console.WriteException(ex);
		}

		return 0; // Success
	}

	private void ListDemos([NotNull] CommandContext context, [NotNull] Settings settings)
	{
		var grid = new Grid();
		grid.AddColumns(4);
		// Header 
		grid.AddRow(
			new Text("Seq").LeftJustified(),
			new Markup("[bold]Name[/]").LeftJustified(),
			new Markup("[bold]Description[/]").LeftJustified(),
			new Markup("[bold]Enabled[/]").LeftJustified()
		);

		foreach (var demo in _config.Demos) // Rows
		{
			var title = new StringBuilder();
			title.Append($"[blue]{demo.Name}[/]");
			if (!string.IsNullOrWhiteSpace(demo.Description))
				title.Append($" ([green]{demo.Description}[/])");

			grid.AddRow(
				new Text(demo.Order.ToString()).RightJustified(),
				new Markup($"[bold blue]{demo.Name}[/]").LeftJustified(),
				new Text(demo.Description).LeftJustified(),
				new Markup(demo.IsEnabled ? "[green]Enabled[/]" : "[red]Disabled[/]").LeftJustified()
			);
		}

		_console.Write(grid);
	}

	private int RunDemos([NotNull] CommandContext context, [NotNull] Settings settings)
	{
		var demos = _config.Demos.ToList();
		var requiredDemos = settings.Names?.Where(n => !string.IsNullOrWhiteSpace(n)).ToHashSet();
		if (requiredDemos != null && requiredDemos.Any())
		{
			demos = demos.Where(d => requiredDemos.Contains(d.Name)).ToList();
		}

		if (!demos.Any())
		{
			_console.MarkupLine($"[bold red]INVOCATION ERROR[/]: no demo found matching your criteria.");
			_console.MarkupLine($"Check available demos with --list option");
			return -1;
		}

		foreach (var demo in demos)
		{
			if (!settings.IncludeDisabled && !demo.IsEnabled) continue;
			using var scope = _config.ServiceProvider.CreateScope();
			RunDemoHeader(demo);
			RunDemo(demo, context.Remaining.Raw.ToArray(), scope);
			RunDemoFooter(demo);
		}

		return 0;
	}

	private bool ConfigurationIsInvalid()
	{
		var invalid = false;
		if (_config.Demos.Count == 0)
		{
			invalid = true;
			_console.MarkupLine($"[bold red]CONFIGURATION ERROR[/]: No demos provided.");
		}

		return invalid;
	}

	private void RunDemoHeader(DemoDescriptor demo)
	{
		var sb = new StringBuilder();
		sb.Append($">>> [blue]{demo.Name}[/]");
		if (!string.IsNullOrWhiteSpace(demo.Description))
			sb.Append($" ([green]{demo.Description}[/])");
		sb.Append(" START");

		var rule = new Rule
		{
			Title = sb.ToString(),
			Justification = Justify.Left,
			Border = BoxBorder.Double
		};
		_console.MarkupLine(sb.ToString());
	}

	private void RunDemoFooter(DemoDescriptor demo)
	{
		var sb = new StringBuilder();
		sb.Append($">>> [blue]{demo.Name}[/]");
		sb.Append(" END");
		_console.MarkupLine(sb.ToString());
		_console.WriteLine("");
	}

	private static void RunDemo(DemoDescriptor demo, string[] args, IServiceScope scope)
	{
		var instance = ActivatorUtilities.CreateInstance(scope.ServiceProvider, demo.Type);
		var method = demo.Type.GetMethod(nameof(IDemo.Run), BindingFlags.Public | BindingFlags.Instance);
		method!.Invoke(instance, new object?[] { args });
	}
}