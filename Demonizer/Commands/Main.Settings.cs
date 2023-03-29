using System.ComponentModel;
using Spectre.Console.Cli;

namespace Demonizer.Commands;

internal sealed partial class Main
{
	public sealed class Settings: CommandSettings
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
}