# Walory

For some time form 18.06.2025 avalable at [Walory Portal](http://walory.northeurope.cloudapp.azure.com/)

## How to run

### Walory – Backend

Walory is an application designed for managing and analyzing investment data. This guide helps you set up the backend part of the project locally using Docker.

---

#### 🛠️ 1. Install Docker

To run the backend locally, you'll need [Docker Desktop](https://www.docker.com/products/docker-desktop):

👉 [Download Docker Desktop](https://www.docker.com/products/docker-desktop/)

---

#### 📦 2. Clone the repository

```bash
git clone https://github.com/KasmyrA/Walory.git
cd Walory
```

---

#### 🔐 3. Configure environment variables

In the root of the repository:

1. Copy the example environment file:

```bash
cp .env.example .env
```

2. Open `.env` and provide your own values for the variables:

```env
POSTGRES_DB=walorydb
POSTGRES_USER=user
POSTGRES_PASSWORD=your_password
CONNECTION_STRING=Host=db;Port=5432;Database=walorydb;Username=user;Password=your_password
```

⚠️ **Important:** Never commit your `.env` file. It's excluded via `.gitignore`.

---

#### 🚀 4. Run the backend with Docker Compose

Navigate to the root folder and run:

```bash
docker compose -f "Walory Backend/docker-compose.yml" up
```

Or to rebuild everything:

```bash
docker compose -f "Walory Backend/docker-compose.yml" up --build
```

---

#### 📘 5. Open the API in Swagger UI

Once the backend is running, open your browser:

👉 http://localhost:8080/swagger/index.html

---

#### 🗄️ 6. (Optional) Connect to PostgreSQL via pgAdmin or any SQL client

Use the following connection details:

| Setting       | Value                    |
| ------------- | ------------------------ |
| Host          | `localhost`              |
| Port          | `5432`                   |
| Database Name | `walorydb`               |
| Username      | `user`                   |
| Password      | *(your `.env` password)* |

---

### 🔧 Additional Commands

##### Stop the containers

```bash
docker compose -f "Walory Backend/docker-compose.yml" down
```

##### View logs

```bash
docker compose -f "Walory Backend/docker-compose.yml" logs
```

##### Clean up (remove containers and volumes)

```bash
docker compose -f "Walory Backend/docker-compose.yml" down -v
```

---

### 📝 Notes

- Make sure Docker Desktop is running before executing any commands
- The application will be available at `http://localhost:8080`
- Database data is persisted in Docker volumes
- Check the logs if you encounter any issues during startup

# Portal dla kolekcjonerów

Celem projektu jest stworzenie systemu służącego do ewidencjonowania walorów (inaczej przedmiotów
kolekcjonerskich) przez kolekcjonerów. System ma pozwolić na wprowadzanie przez kolekcjonera nowych
walorów do kolekcji, ich edycji, czy też usuwania. Użytkownik ma możliwość podziału kolekcji na kategorie (np. monety, znaczki, etc.). Tworząc nową kategorię walorów można stworzyć model danych
opisujących dany walor np. rocznik, stan zachowania, zakład wytwórczy itp. Dodatkowo należy zapewnić filtrację i sortowanie walorów wg wybranych cech w celu ich przeglądania. Kolekcjonerzy będą
mieli możliwość zapraszania się (po adresie email) do grona znajomych w celu przeglądania swoich
kolekcji. Wówczas możliwe będzie przeglądanie kolejcji osoby zaproszonej i dodawanie komentarzy pod
konretnymi walorami. Jako dodatkową funkcjonalność można zaimplementować komunikator pomiędzy
znajomymi.

Funkcjonalności

1. System posiada mechanizm tworzenia konta oraz logowania.
2. Użytkownik ma możliwość dodawania, edycji oraz usuwania walorów.
3. Użytkownik ma możliwość wyboru istnitejących kategorii albo dodania nowej jeśli taka nie istnieje.
4. Użytkownik dodając nową kategorię ma możliwość określenia modelu danych ją opisującących.
5. Walory są tworzone w ramach kategorii (np. monety, znaczki, biżuteria, etc.).
6. Użytkownik ma możliwość wysłać zaproszenie (po adresie email) do grona znajomych innemu użytkownikowi systemu. Po akceptacji, użytkownicy mają możliwość przeglądania swoich kolekcji.
7. Użytkownik ma możliwość filtracji oraz sortowania walorów.
8. Użytkownik ma możliwość eksportu danych o walorach (np. po filtracji lub sortowaniu).

Założenia niefunkcjonalne

1. Eksport danych o walorach w formacjie PDF lub xlsx.
2. Dane gromadzone w realcyjnej bazie danych.

# 