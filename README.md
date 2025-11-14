# Listings‑Portal Web App + API

A modern geospatial Web and API for discovering residential **rent** and **sale** listings in the New York City metro area. It exposes a clean, versioned REST surface backed by PostgreSQL + PostGIS and Entity Framework Core 8. A companion **React** front‑end consumes this API to provide an interactive map experience.

![image](assets/demo-main.png)
_React Frontend Interface_

---

## API Key Features

|  Feature                                |  Tech                                                      |
| --------------------------------------- | ---------------------------------------------------------- |
| Geodesic radius search                  | `ST_DWithin` (PostGIS) via `EF.Functions.IsWithinDistance` |
| Pagination & rich DTOs                  | Minimal‑API style controller returning `PagedResponse<T>`  |
| Seed & Faker utilities                  | Bogus Faker generates realistic NYC data                   |
| Testcontainers‑driven integration tests | `postgis/postgis:16‑3.4` image + xUnit                     |
| GitHub Actions CI gate                  | Pull‑request workflow blocks merges on failing tests       |

## CI / CD

- **Branch protection** requires the _CI / Test‑Gate_ workflow to pass before merge.
- Workflow file: `.github/workflows/ci.yml`
- Matrix tested on Ubuntu LTS, .NET 8.
