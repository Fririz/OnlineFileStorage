# OnlineFileStorage

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Docker](https://img.shields.io/badge/Docker-Compose-blue)
![License](https://img.shields.io/badge/License-MIT-green)

A scalable, microservices-based file storage system built with **.NET 9**.
Supports file upload/download, distributed logging, and full-text search.

## Tech Stack

**Backend & Architecture:**
* **Framework:** ASP.NET Core (.NET 9)
* **Communication:** gRPC (Inter-service), RabbitMQ (Event Bus)
* **Database:** PostgreSQL (Metadata), Redis (Caching)
* **Storage:** MinIO (S3 compatible object storage)

**Observability & Logging:**
* **Logging:** Serilog
* **Monitoring Stack:** Elasticsearch, Kibana (ELK)

## Architecture

![Architecture Schema](./docs/Untitled-2025-08-04-0138.png)

The solution consists of the following microservices:

| Service | Description | Port |
| :--- | :--- | :--- |
| **Gateway API** | Entry point, Ocelot, Auth | `:6002` |
| **FileApiService** | Manages file metadata | `:8081` |
| **IdentityService** | JWT Authentication & User management | `:8080` |
| **FileStorageService** | Manages file storage(minIO) | `:8082` |

## 🛠 Getting Started

### Prerequisites
* Docker & Docker Compose
* .NET 9 SDK (for local development)
* Npm with vue

### Run with Docker (Recommended)

To start the entire infrastructure (Databases, MinIO, RabbitMQ, ELK) and services:

```bash
docker-compose up -d
