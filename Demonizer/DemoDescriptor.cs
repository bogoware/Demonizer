namespace Demonizer;
internal class DemoDescriptor : IComparable<DemoDescriptor>
{
	public required Type Type { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	public int Order { get; init; }
	public bool IsEnabled { get; init; } = true;

	public int CompareTo(DemoDescriptor? other)
	{
		if (ReferenceEquals(this, other)) return 0;
		if (ReferenceEquals(null, other)) return 1;
		var orderCompare = Order.CompareTo(other.Order);
		if (orderCompare != 0) return orderCompare;
		return string.Compare(Name, other.Name, StringComparison.Ordinal);
	}
}