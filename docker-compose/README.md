# Docker Compose

This folder contains the production-oriented Docker Compose setup for the solution.

## Files

- `compose.yml`: Builds and runs Postgres, the one-shot database migrator, `AuthService`, and `VideoService`.
- `.env.example`: Template for the required runtime variables.

## Usage

1. Copy `.env.example` to `.env`.
2. Replace the placeholder secrets, especially `POSTGRES_PASSWORD` and `JWT_KEY`.
3. Run:

```powershell
docker compose --env-file docker-compose/.env -f docker-compose/compose.yml up --build -d
```

## Notes

- Postgres is kept private to the Docker network and is not published to the host.
- `database-migrator` runs once after Postgres becomes healthy.
- `auth-service` starts only after migrations complete successfully.
