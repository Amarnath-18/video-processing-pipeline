# Docker Compose

This folder contains the production-oriented Docker Compose setup for the solution.

## Files

- `docker-compose.yml`: Builds and runs Postgres, pgAdmin, the one-shot database migrator, `AuthService`, and `VideoService`.
- `compose.https.yml`: Optional override that enables HTTPS for `AuthService` and `VideoService` using mounted `.pfx` certificates.
- `.env.example`: Template for the required runtime variables.

## Usage

1. Copy `.env.example` to `.env`.
2. Replace the placeholder secrets, especially `POSTGRES_PASSWORD`, `PGADMIN_DEFAULT_PASSWORD`, and `JWT_KEY`.
3. Run:

```powershell
docker compose --env-file docker-compose/.env -f docker-compose/docker-compose.yml up --build -d
```

## Access

- AuthService HTTP: `http://localhost:5044`
- VideoService HTTP: `http://localhost:5026`
- Postgres host port: `localhost:5432`
- pgAdmin: `http://localhost:5050`

## HTTPS

HTTPS is optional and requires certificate files on the host.

1. Put `auth-service.pfx` and `video-service.pfx` in a folder on the host.
2. Set `HTTPS_CERTS_PATH`, `AUTH_SERVICE_CERT_PASSWORD`, and `VIDEO_SERVICE_CERT_PASSWORD` in `docker-compose/.env`.
3. Start Compose with the HTTPS override:

```powershell
docker compose --env-file docker-compose/.env -f docker-compose/docker-compose.yml -f docker-compose/compose.https.yml up --build -d
```

After that, the HTTPS endpoints are:

- AuthService HTTPS: `https://localhost:7044`
- VideoService HTTPS: `https://localhost:7026`

## Notes

- Postgres is published to the host on `POSTGRES_PORT` for local access.
- pgAdmin uses `PGADMIN_DEFAULT_EMAIL` and `PGADMIN_DEFAULT_PASSWORD` for the initial login.
- In pgAdmin, connect to the database server with host `postgres`, port `5432`, and the `POSTGRES_*` credentials from `.env`.
- `database-migrator` runs once after Postgres becomes healthy.
- `auth-service` starts only after migrations complete successfully.
