using Spectre.Console;

namespace Demonizer.SampleApp.Demos;

[Demo(Description = "The simplest demo", Order = 0)]
internal class HelloWorldDemo: IDemo
{
	
	public void Run(string[] args) => AnsiConsole.MarkupLine($"Hello [red]World[/]");
}