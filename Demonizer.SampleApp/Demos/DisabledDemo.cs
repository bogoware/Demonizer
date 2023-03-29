using Spectre.Console;

namespace Demonizer.SampleApp.Demos;

[Demo(Enabled = false, Description = "This demo is disabled", Order = 3)]
internal class DisabledDemo : IDemo
{
	public void Run(string[] args) => AnsiConsole.MarkupLine("I am a [red]disabled[/] demo :)");
}