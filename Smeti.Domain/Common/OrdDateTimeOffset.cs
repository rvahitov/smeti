using LanguageExt;
using LanguageExt.TypeClasses;

namespace Smeti.Domain.Common;

public readonly struct OrdDateTimeOffset : Ord<DateTimeOffset>
{
    public Task<int> GetHashCodeAsync(DateTimeOffset x) => GetHashCode(x).AsTask();

    public int GetHashCode(DateTimeOffset x) => x.GetHashCode();

    public Task<bool> EqualsAsync(DateTimeOffset x, DateTimeOffset y) => Equals(x, y).AsTask();

    public bool Equals(DateTimeOffset x, DateTimeOffset y) => x.Equals(y);

    public Task<int> CompareAsync(DateTimeOffset x, DateTimeOffset y) => Compare(x, y).AsTask();

    public int Compare(DateTimeOffset x, DateTimeOffset y) => x.CompareTo(y);
}