# Shikimori Crawler

## Generate migration

- Execute `cd ./data`
- Execute `dotnet ef --startup-project ../app migrations add <MigrationName> -v`

## Docker

- Cleanup `docker rm -f $(docker ps -a -q) || true && docker rmi -f $(docker images -q) || true`
- Run postgres for dev `docker-compose -f ./docker-compose.dev.yml up -d`
