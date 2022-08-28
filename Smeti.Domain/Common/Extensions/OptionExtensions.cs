using LanguageExt;

namespace Smeti.Domain.Common.Extensions;

public static class OptionExtensions
{
    public static Option<object> Box<T>(this Option<T> option) => option.Map(v => (object) v!);
}