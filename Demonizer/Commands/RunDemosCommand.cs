using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace Demonizer.Commands;

internal sealed class RunDemosCommand : Command<RunDemosCommand.Settings>
{
	public sealed class Settings : CommandSettings
	{
		[CommandArgument(0, "[names]")]
		[Description("The names of the demo to run. If missing all demos are run.")]
		public string[]? Names { get; set; }

		[CommandOption("-a|--all")]
		[Description("By default only non disabled demos are executed. This flag will include all.")]
		public bool IncludeDisabled { get; set; }

		[CommandOption("-l|--list")]
		[Description("Just list the demos and their configuration.")]
		public bool ListOnly { get; set; }
	}

	private readonly RunDemosConfiguration _config;

	public RunDemosCommand(RunDemosConfiguration config)
	{
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
				RunDemos(context, settings);
			}
		}
		catch (Exception ex)
		{
			AnsiConsole.WriteException(ex);
		}

		return 0; // Success
	}

	private void ListDemos([NotNull] CommandContext context, [NotNull] Settings settings)
	{
		var demos = ScanForDemos(_config.Assemblies);
		var grid = new Grid();

		// Add columns 
		grid.AddColumns(4);

		// Header 
		grid.AddRow(
			new Text("Seq").LeftJustified(),
			new Markup("[bold]Name[/]").LeftJustified(),
			new Markup("[bold]Description[/]").LeftJustified(),
			new Markup("[bold]Enabled[/]").LeftJustified()
		);
		
		foreach (var demo in demos) // Rows
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

		AnsiConsole.Write(grid);
	}

	private void RunDemos([NotNull] CommandContext context, [NotNull] Settings settings)
	{
		var demos = ScanForDemos(_config.Assemblies);
		foreach (var demo in demos)
		{
			if (!settings.IncludeDisabled && !demo.IsEnabled) continue;
			using var scope = _config.ServiceProvider.CreateScope();
			RunDemoHeader(demo);
			RunDemo(demo, context.Remaining.Raw.ToArray(), scope);
			RunDemoFooter(demo);
		}
	}

	private bool ConfigurationIsInvalid()
	{
		var invalid = false;
		if (_config.Assemblies.Count == 0)
		{
			invalid = true;
			AnsiConsole.MarkupLine($"[bold red]No assembly provided[/]: You should at least add an assembly for scan");
		}

		return invalid;
	}

	private static void RunDemoHeader(DemoDescriptor demo)
	{
		var title = new StringBuilder();
		title.Append($"[blue]{demo.Name}[/]");
		if (!string.IsNullOrWhiteSpace(demo.Description))
			title.Append($" ([green]{demo.Description}[/])");

		var rule = new Rule
		{
			Title = title.ToString(),
			Justification = Justify.Left,
			Border = BoxBorder.Double
		};
		AnsiConsole.Write(rule);
	}

	private static void RunDemoFooter(DemoDescriptor demo)
	{
		AnsiConsole.WriteLine();
	}

	private static void RunDemo(DemoDescriptor demo, string[] args, IServiceScope scope)
	{
		var instance = ActivatorUtilities.CreateInstance(scope.ServiceProvider, demo.Type);
		var method = demo.Type.GetMethod(nameof(IDemo.Run), BindingFlags.Public | BindingFlags.Instance);
		method!.Invoke(instance, new object?[] { args });
	}

	private static List<DemoDescriptor> ScanForDemos(IEnumerable<Assembly> assemblies)
	{
		ArgumentNullException.ThrowIfNull(assemblies);

		var descriptors =
			from a in assemblies.ToList()
			from t in a.GetTypes()
			where t.IsClass && t.IsAssignableTo(typeof(IDemo))
			select GetDescriptor(t);
		var list = descriptors.ToList();
		list.Sort();
		return list;
	}

	private static DemoDescriptor GetDescriptor(Type t) => t.GetCustomAttribute<DemoAttribute>() switch
	{
		null => new()
		{
			Type = t,
			Name = t.Name,
			Description = string.Empty,
			IsEnabled = true,
			Order = -1
		},
		var attr => new()
		{
			Type = t,
			Name = attr.Name ?? t.Name,
			Description = attr.Description ?? string.Empty,
			IsEnabled = attr.Enabled,
			Order = attr.Order
		}
	};
}