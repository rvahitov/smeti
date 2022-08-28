using LanguageExt;
using LanguageExt.TypeClasses;

namespace Smeti.Domain.Common;

public readonly struct OrdTimeSpan : Ord<TimeSpan>
{
    public Task<int> GetHashCodeAsync(TimeSpan x) => x.GetHashCode().AsTask();

    public int GetHashCode(TimeSpan x) => x.GetHashCode();

    public Task<bool> EqualsAsync(TimeSpan x, TimeSpan y) => x.Equals(y).AsTask();

    public bool Equals(TimeSpan x, TimeSpan y) => x.Equals(y);

    public Task<int> CompareAsync(TimeSpan x, TimeSpan y) => x.CompareTo(y).AsTask();

    public int Compare(TimeSpan x, TimeSpan y) => x.CompareTo(y);
}