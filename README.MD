# UPS

## Uruchomienie projektu

### Tryb "produkcyjny"
docker-compose -f docker-compose.yml up --build -d

### Tryb developerski
docker-compose -f docker-compose.dev.yml up --build -d
(sama baza, backend i frontend należy uruchomić lokalnie)
(w adminerze credentiale do bazy to base/postgres/example)
