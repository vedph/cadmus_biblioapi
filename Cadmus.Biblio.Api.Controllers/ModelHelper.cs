using Cadmus.Biblio.Core;
using System;

namespace Cadmus.Biblio.Api.Controllers
{
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
    }
}
