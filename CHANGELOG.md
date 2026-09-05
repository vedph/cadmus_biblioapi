# History

## 9.0.2

- 2026-09-05:
  - unified versions.
  - updated packages.

## 9.0.0

- 2025-11-23: ⚠️ upgraded to NET 10.

## 8.1.0

- 2025-08-06:
  - ⚠️ changed language field size to varchar 50 characters (was char 3). This unties the language from the ISO 639 code, which is not always suitable for all languages. Note that this is not a breaking change per se, but it will require you to change the data type in existing databases (extending it, so that no data is lost).
  - updated packages.
- 2025-08-02: updated packages.
- 2025-07-24: updated packages.

## 8.0.3

- 2025-07-16: updated packages.

## 8.0.2

- 2025-06-03: updated packages.

## 8.0.1

- 2025-05-06:
  - updated packages.
  - updated preview profiles (migration V3).

## 8.0.0

- 2025-03-19: updated packages (bumped Cadmus API to v11).

## 7.0.3

- 2025-02-14: updated packages.

## 7.0.2

- 2025-01-27:
  - updated packages.
  - fix to container binding model (missing default ctor).
- 2025-01-04: updated packages.
- 2024-12-06: updated packages.
- 2024-30-11: updated packages.
- 2024-11-20: updated packages.

## 7.0.0

- 2024-11-18:
  - ⚠️ upgraded to .NET 9.
  - refactored infrastructure to drop the legacy `Startup` class.

## 6.0.2

- 2024-01-01:
  - updated packages.
  - added ARM support to Docker scripts.
- 2024-06-25: updated packages.

## 6.0.1

- 2024-06-08: updated packages.
- 2024-05-24: updated packages.
- 2024-04-13: updated packages.
- 2024-01-31: updated packages.
- 2023-11-21: updated packages.

## 6.0.0

- 2023-11-09: ⚠️ upgraded to .NET 8.

## 5.1.2

- 2023-09-04: updated packages.

## 5.1.1

- 2023-08-29:
  - updated packages.
  - added type for ancient work to predefined work types in seeder.

## 5.1.0

- 2023-07-29: added links to container and work.
- 2023-07-28: added `datation` and `datationValue` to work/container.
- 2023-07-20: refactored [logging](https://myrmex.github.io/overview/cadmus/dev/history/b-logging).

## 5.0.1

- 2023-07-01: updated packages.
- 2023-06-21: updated packages.

## 5.0.0

- 2023-06-17: moved to PostgreSQL.

## 4.0.1

- 2023-06-02: updated packages.

## 4.0.0

- 2023-05-23: updated packages (breaking change in general parts introducing [AssertedCompositeId](https://github.com/vedph/cadmus-bricks-shell/blob/master/projects/myrmidon/cadmus-refs-asserted-ids/README.md#asserted-composite-id)).
- 2023-05-16: updated packages.

## 3.1.5

- 2023-05-16: updated packages.

## 3.1.4

- 2023-02-20: fix to model copy in saving authors (`EfHelper`).

## 3.1.3

- 2023-02-20: fix to author-work/author-container save.

## 3.1.2

- 2023-02-20:
  - work key builder: add number only if it's a container.
  - work key builder: fix to auto suffix for existing keys.

## 3.1.1

- 2023-02-18:
  - added optional `yearPub2` to work/container. This is a breaking change in the schema; the new field being nullable, you just have to add it to tables `work` and `container`.

## 3.0.3

- 2023-02-17:
  - updated packages.
  - enabed nullable for all the projects.
  - fix to work authors save.
- 2023-02-02: migrated to new components factory. This is a breaking change for backend components, please see [this page](https://myrmex.github.io/overview/cadmus/dev/history/#2023-02-01---backend-infrastructure-upgrade). Anyway, in the end you just have to update your libraries and a single namespace reference. Benefits include:
  - more streamlined component instantiation.
  - more functionality in components factory, including DI.
  - dropped third party dependencies.
  - adopted standard MS technologies for DI.

- 2023-01-24:
  - updated packages.
  - added demo API.
- 2023-01-16: updated packages.

## 2.1.8

- 2023-01-10:
  - fix to work controller AddWork (container not set).
  - updated packages.
  - reformatted code.
- 2023-01-09: refactored CLI tool infrastructure.

## 2.1.7

- 2022-12-21: updated Pomelo EF MySql to 7.0.0-silver.1 (prerelease).
- 2022-11-10: upgraded to NET 7 advancing version to 3.0.0 (NET Framework still to 6 while waiting for the release of [Pomelo EF](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/pull/1717)).
- 2022-10-10: updated packages for new `IRepositoryProvider`.

## 2.1.6

- 2022-09-29: optional HTTPS.

## 2.1.5

- 2022-09-24:
  - fixes to EF repository.
  - added preview infrastructure to demo API (just to avoid annoying console logs).
- 2022-09-19: configurable allowed origins in startup.
- 2022-09-15: updated packages.
- 2022-07-14: updated packages.

## 2.1.2

- 2022-06-11: updated packages.

## 2.1.1

- 2022-05-18: updated packages.

## 2.1.0

- 2022-04-29: updated Cadmus packages (NET 6.0)

## 2.0.2

- 2022-03-09: updated packages.

## 2.0.1

- 2021-12-22: updated packages. Docker version: 2.0.1.
- 2021-11-09: upgraded to NET 6. This requires Pomelo RC.
