using Spectre.Console;
using Spectre.Console.Testing;

namespace Demonizer.Tests.Commands;

[Demo(Name = "HelloWorldDisabled", Description = "HelloWorldDescription", Order = 666, Enabled = false)]
public class HelloWorldAnnotated : IDemo
{
	public void Run(string[] args)
	{
		
	}
}