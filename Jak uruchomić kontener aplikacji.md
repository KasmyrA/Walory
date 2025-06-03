# Instrukcja uruchomienia aplikacji Walory w systemie Linux

## Wymagania wstępne

1. **Zainstalowany Docker**
   
   ```bash
   # Ubuntu/Debian
   sudo apt update
   sudo apt install docker.io 
   sudo systemctl start docker
   sudo systemctl enable docker
   
   # Dodanie użytkownika do grupy docker (opcjonalne)
   sudo usermod -aG docker $USER
   # Po dodaniu do grupy, wyloguj się i zaloguj ponownie
   ```

2. **Sprawdzenie instalacji**
   
   ```bash
   docker --version
   ```

## Uruchomienie aplikacji

Pamiętaj aby korzystać z sudo

### Uruchomienie bazy danych

```bash
# Pobranie i uruchomienie PostgreSQL
sudo docker run -d --name walory-db -e POSTGRES_USER=user -e POSTGRES_PASSWORD=Zaq12wsx -e POSTGRES_DB=walory -p 5432:5432 postgres:15
```

pamiętaj że PostgreSQL musi działać aby backend działał poprawnie

### Uruchomienie backendu

```bash
# Pobranie i uruchomienie najnowszego obrazu
sudo docker run -d --name walory-app --link walory-db:postgres   -e "ConnectionStrings__Default=Host=postgres;Port=5432;Database=walory;Username=user;Password=Zaq12wsx"   -p 8080:8080   kasmyr/walory
```

## Podstawowe komendy zarządzania

### Sprawdzenie statusu kontenerów

```bash
# Lista uruchomionych kontenerów
docker ps

# Lista wszystkich kontenerów
docker ps -a
```

### Zarządzanie kontenerem

```bash
# Zatrzymanie kontenera
docker stop kasmyr/walory

# Uruchomienie zatrzymanego kontenera
docker start kasmyr/walory

# Restart kontenera
docker restart kasmyr/walory

# Usunięcie kontenera
docker rm kasmyr/walory

# Usunięcie kontenera (wymuszenie)
docker rm -f kasmyr/walory
```

## Dostęp do aplikacji

Po uruchomieniu kontenera aplikacja będzie dostępna pod adresem:

- **http://localhost:8080/** (jeśli używasz portu 8080)
- [**http://localhost:8080/swagger**](http://localhost:8080/swagger) (jeśli chcesz wejść do swaggera i poprzeglądać endpointy)
- **http://IP_SERWERA:8080** (w przypadku zdalnego serwera)
