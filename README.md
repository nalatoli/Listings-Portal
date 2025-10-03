# Listings‑Portal API

A modern geospatial Web API for discovering residential **rent** and **sale** listings in the New York City metro area.  It exposes a clean, versioned REST surface backed by PostgreSQL + PostGIS and Entity Framework Core 8.  A companion **React + Vite** front‑end (work in progress) consumes this API to provide an interactive map experience.

---

## ✨ Key Features

|  Feature                                |  Tech                                                      |
| --------------------------------------- | ---------------------------------------------------------- |
| Geodesic radius search                  | `ST_DWithin` (PostGIS) via `EF.Functions.IsWithinDistance` |
| Pagination & rich DTOs                  | Minimal‑API style controller returning `PagedResponse<T>`  |
| Seed & Faker utilities                  | Bogus Faker generates realistic NYC data                   |
| Testcontainers‑driven integration tests | `postgis/postgis:16‑3.4` image + xUnit                     |
| GitHub Actions CI gate                  | Pull‑request workflow blocks merges on failing tests       |

## 🛠️ CI / CD

- **Branch protection** requires the *CI / Test‑Gate* workflow to pass before merge.
- Workflow file: `.github/workflows/ci.yml`
- Matrix tested on Ubuntu LTS, .NET 8.
