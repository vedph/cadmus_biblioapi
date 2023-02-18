using Cadmus.Biblio.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Work's filter.
/// </summary>
/// <seealso cref="PagingOptionsBindingModel" />
public sealed class WorkFilterBindingModel : PagingOptionsBindingModel
{
    /// <summary>
    /// A value indicating whether to match this filter
    /// it is enough to match any of the specified properties. The default
    /// value is false, meaning that all the specified properties must
    /// be matched.
    /// </summary>
    public bool MatchAny { get; set; }

    /// <summary>
    /// The work's type.
    /// </summary>
    [MaxLength(20)]
    public string? Type { get; set; }

    /// <summary>
    /// The author ID to be matched.
    /// </summary>
    public Guid? AuthorId { get; set; }

    /// <summary>
    /// Any portion of the last name to be matched (filtered).
    /// </summary>
    [MaxLength(50)]
    public string? LastName { get; set; }

    /// <summary>
    /// The language to be matched.
    /// </summary>
    [MaxLength(3)]
    [RegularExpression("^[a-z]{3}$")]
    public string? Language { get; set; }

    /// <summary>
    /// Any portion of the title to be matched (filtered).
    /// </summary>
    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// The container ID to be matched.
    /// </summary>
    public Guid? ContainerId { get; set; }

    /// <summary>
    /// A keyword value to be matched.
    /// </summary>
    [MaxLength(50)]
    public string? Keyword { get; set; }

    /// <summary>
    /// The minimum year of publication.
    /// </summary>
    public short? YearPubMin { get; set; }

    /// <summary>
    /// The maximum year of publication.
    /// </summary>
    public short? YearPubMax { get; set; }

    /// <summary>
    /// The citation key to be matched.
    /// </summary>
    [MaxLength(300)]
    public string? Key { get; set; }

    public WorkFilter ToWorkFilter()
    {
        return new WorkFilter
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            IsMatchAnyEnabled = MatchAny,
            Type = Type,
            AuthorId = AuthorId ?? Guid.Empty,
            LastName = LastName,
            Language = Language,
            Title = Title,
            ContainerId = ContainerId ?? Guid.Empty,
            Keyword = Keyword,
            YearPubMin = YearPubMin ?? 0,
            YearPubMax = YearPubMax ?? 0,
            Key = Key
        };
    }
}
