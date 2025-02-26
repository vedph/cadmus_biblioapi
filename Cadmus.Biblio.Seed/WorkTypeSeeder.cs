﻿using Cadmus.Biblio.Core;
using System;

namespace Cadmus.Biblio.Seed;

public sealed class WorkTypeSeeder : IBiblioSeeder
{
    public static string[] TypeIds =
        new[] { "book", "journal", "procs", "article", "ancient" };

    public void Seed(IBiblioRepository repository, int count)
    {
        ArgumentNullException.ThrowIfNull(repository);

        foreach (WorkType type in new[]
        {
            new WorkType{ Id = "book", Name = "Book" },
            new WorkType{ Id = "journal", Name = "Journal" },
            new WorkType{ Id = "procs", Name = "Proceedings" },
            new WorkType{ Id = "article", Name = "Article" },
            new WorkType{ Id = "ancient", Name = "Ancient Work" },
        })
        {
            repository.AddWorkType(type);
        }
    }
}
