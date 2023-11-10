namespace DotNetUtilities.Collections;

public class ReadOnlyMap<TKey, TValue> : BaseMap<TKey, TValue>
	where TKey : notnull
	where TValue : class
{
	public override bool IsReadOnly => true;

	public ReadOnlyMap() : this(0, null) { }

	public ReadOnlyMap(int capacity) : this(capacity, null) { }

	public ReadOnlyMap(IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

	public ReadOnlyMap(int capacity, IEqualityComparer<TKey>? comparer) : base(capacity, comparer) { }

	public ReadOnlyMap(Dictionary<TKey, TValue> values) : base(new(values, values.Comparer)) { }
}
