namespace Demonizer;
internal class DemoDescriptor : IComparable<DemoDescriptor>, IEquatable<DemoDescriptor>
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

	public bool Equals(DemoDescriptor? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		// ReSharper disable once CheckForReferenceEqualityInstead.1
		return Type.Equals(other.Type);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((DemoDescriptor)obj);
	}

	public override int GetHashCode() => Type.GetHashCode();

	public static bool operator ==(DemoDescriptor? left, DemoDescriptor? right) => Equals(left, right);

	public static bool operator !=(DemoDescriptor? left, DemoDescriptor? right) => !Equals(left, right);
}