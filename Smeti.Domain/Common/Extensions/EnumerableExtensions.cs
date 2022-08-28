using LanguageExt;

namespace Smeti.Domain.Common.Extensions;

public static class EnumerableExtensions
{
    public static Either<Lst<TLeft>, Lst<TRight>> Traverse<TLeft, TRight>(
        this IEnumerable<Either<TLeft, TRight>> eitherSeq
    )
    {
        var lefts = List.empty<TLeft>();
        var rights = List.empty<TRight>();
        foreach(var e in eitherSeq)
        {
            Prelude.match(
                e,
                right => rights = rights.Add(right),
                left => lefts = lefts.Add(left)
            );
        }

        return lefts.IsEmpty
                   ? rights
                   : lefts;
    }
}