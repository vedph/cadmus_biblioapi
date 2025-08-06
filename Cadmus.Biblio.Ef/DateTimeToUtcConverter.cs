using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq.Expressions;

namespace Cadmus.Biblio.Ef;

public sealed class DateTimeToUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeToUtcConverter() : base(Serialize, Deserialize, null) { }

    static readonly Expression<Func<DateTime, DateTime>> Deserialize = x =>
        x.Kind == DateTimeKind.Unspecified
        ? DateTime.SpecifyKind(x, DateTimeKind.Utc) : x;

    static readonly Expression<Func<DateTime, DateTime>> Serialize = x => x;
}
