using Cadmus.Biblio.Core;
using System;

namespace Cadmus.Biblio.Api.Controllers;

internal static class ModelHelper
{
    public static WorkAuthor GetAuthor(WorkAuthorBindingModel model)
    {
        if (model == null) return null;

        return new WorkAuthor
        {
            Id = model.Id ?? Guid.Empty,
            First = model.First,
            Last = model.Last,
            Suffix = model.Suffix,
            Role = model.Role
        };
    }

    public static Keyword GetKeyword(KeywordBindingModel model)
    {
        if (model == null) return null;

        return new Keyword
        {
            Language = model.Language,
            Value = model.Value
        };
    }

    public static WorkFilter GetWorkFilter(WorkFilterBindingModel model)
    {
        if (model == null) return null;

        return new WorkFilter
        {
            PageNumber = model.PageNumber,
            PageSize = model.PageSize,
            IsMatchAnyEnabled = model.MatchAny,
            Type = model.Type,
            AuthorId = model.AuthorId ?? Guid.Empty,
            LastName = model.LastName,
            Language = model.Language,
            Title = model.Title,
            ContainerId = model.ContainerId ?? Guid.Empty,
            Keyword = model.Keyword,
            YearPubMin = model.YearPubMin ?? 0,
            YearPubMax = model.YearPubMax ?? 0,
            Key = model.Key
        };
    }
}
