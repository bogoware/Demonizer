namespace Demonizer;

[AttributeUsage(AttributeTargets.Class)]
public class DemoAttribute : Attribute
{
	public string? Name { get; set; }
	public string? Description { get; set; }
	public bool Enabled { get; set; } = true;
	public int Order { get; set; }
}