namespace Demonizer.SampleApp;

[Demo(Description = "This demo shows how injection works.", Order = 1)]
public class NowDemo: IDemo
{
	private readonly NowTimeService _timeService;

	public NowDemo(NowTimeService timeService)
	{
		_timeService = timeService;
	}
	public void Run(string[] args) => Console.WriteLine($"Now is {_timeService.Now()}");
}