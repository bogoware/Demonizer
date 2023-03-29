using Demonizer.Commands;
using FluentAssertions;
using Spectre.Console.Cli;
using Spectre.Console.Testing;
using Xunit.Abstractions;

namespace Demonizer.Tests;

public class RunCommandTests
{
	private readonly ITestOutputHelper _testOutput;
	public RunCommandTests(ITestOutputHelper output) => _testOutput = output;

	[Fact]
	public void Flag_l_lists_all_demos()
	{
		// Arrange
		var sut = GetCommandAppTester();

		// Act
		var result = sut.Run(new[] { "-l" });

		// Assert
		_testOutput.WriteLine(result.Output);
		result.ExitCode.Should().Be(0);
		result.Settings.Should().BeOfType<Main.Settings>().Which.ListOnly.Should().BeTrue();
		result.Output.Should().Contain("HelloWorld");
		result.Output.Should().Contain("HelloEurope");
		result.Output.Should().Contain("HelloWorldDisabled");
	}

	[Fact]
	public void Simple_run_will_execute_only_enabled_demos()
	{
		// Arrange
		var sut = GetCommandAppTester();

		// Act
		var result = sut.Run();

		// Assert
		_testOutput.WriteLine(result.Output);
		result.ExitCode.Should().Be(0);
		var settings = result.Settings.Should().BeOfType<Main.Settings>().Which;

		settings.Names.Should().BeNull();
		settings.ListOnly.Should().BeFalse();
		settings.IncludeDisabled.Should().BeFalse();
		
		result.Output.Should().Contain("HelloWorld");
		result.Output.Should().Contain("HelloEurope");
		result.Output.Should().NotContain("HelloWorldDisabled");
	}
	
	[Fact]
	public void Named_run_will_execute_only_the_demo_matching()
	{
		// Arrange
		var sut = GetCommandAppTester();

		// Act
		var result = sut.Run("HelloWorld");

		// Assert
		_testOutput.WriteLine(result.Output);
		result.ExitCode.Should().Be(0);
		var settings = result.Settings.Should().BeOfType<Main.Settings>().Which;

		settings.Names.Should().BeEquivalentTo("HelloWorld");
		settings.ListOnly.Should().BeFalse();
		settings.IncludeDisabled.Should().BeFalse();
		
		result.Output.Should().Contain("HelloWorld");
		result.Output.Should().NotContain("HelloWorldEurope");
	}
	
	[Fact]
	public void Notify_no_demo_found()
	{
		// Arrange
		var sut = GetCommandAppTester();

		// Act
		var result = sut.Run("ThisDemoDoesntExist");

		// Assert
		_testOutput.WriteLine(result.Output);
		result.ExitCode.Should().Be(-1);

		result.Output.Should().Contain("no demo found matching your criteria.");
	}

	private static CommandAppTester GetCommandAppTester()
	{
		var builder = new DemonizerBuilder()
			.AddDemosFromThisAssembly()
			.Build();
		var sut = new CommandAppTester(builder.GetTypeRegistrar());
		sut.Configure(config =>
		{
			config.PropagateExceptions();
		});
		sut.SetDefaultCommand<Main>();
		return sut;
	}
}