namespace Demonizer.SampleApp;

[Demo(Description = "The simplest demo", Order = 0)]
public class HelloWorldDemo: IDemo
{
	public void Run(string[] args) => Console.WriteLine("Hello world");
}