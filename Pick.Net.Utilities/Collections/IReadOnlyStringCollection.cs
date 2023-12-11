namespace Pick.Net.Utilities.Collections;

public interface IReadOnlyStringCollection : IReadOnlyCollection<string>
{
	bool Contains(string value);

	bool Contains(ReadOnlySpan<char> value);
}
