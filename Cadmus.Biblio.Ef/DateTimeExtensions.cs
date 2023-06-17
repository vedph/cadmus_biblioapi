using System;

namespace Cadmus.Biblio.Ef;

/// <summary>
/// Extensions to <see cref="DateTime"/>.
/// See https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty/70142836#70142836.
/// </summary>
public static class DateTimeExtensions
{
    public static DateTime? SetKindUtc(this DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            return dateTime.Value.SetKindUtc();
        }
        else
        {
            return null;
        }
    }
 
    public static DateTime SetKindUtc(this DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc) { return dateTime; }
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}