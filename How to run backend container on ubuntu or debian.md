# Running the Walory Application on Linux

## Prerequisites

1. **Docker Installed**
   
   ```bash
   # Ubuntu/Debian
   sudo apt update
   sudo apt install docker.io 
   sudo systemctl start docker
   sudo systemctl enable docker
   
   # (Optional) Add user to the docker group
   sudo usermod -aG docker $USER
   # After adding to the group, log out and log back in
   ```

2. **Verify Installation**
   
   ```bash
   docker --version
   ```

## Running the Application

> **Note**: Use `sudo` where required if you haven't added your user to the Docker group.

### Start the Database

```bash
docker run -d --name walory-db \
  -e POSTGRES_USER=user \
  -e POSTGRES_PASSWORD=Zaq12wsx \
  -e POSTGRES_DB=walory \
  -p 5432:5432 \
  postgres:15
```

> **PostgreSQL must be running for the backend to function properly.**

### Start the Backend

```bash
sudo docker run -d --name walory-app \
  --link walory-db:postgres \
  -e "ConnectionStrings__Default=Host=postgres;Port=5432;Database=walory;Username=user;Password=Zaq12wsx" \
  -p 8080:8080 \
  kasmyr/walory
```

## Basic Docker Commands

### Check Container Status

```bash
docker ps        # Running containers
docker ps -a     # All containers
```

### Manage the Container

```bash
docker stop kasmyr/walory       # Stop container
docker start kasmyr/walory      # Start stopped container
docker restart kasmyr/walory    # Restart container
docker rm kasmyr/walory         # Remove container
docker rm -f kasmyr/walory      # Force remove container
```

## Access the Application

Once running, access the app at:

- **http://localhost:8080/** – Main application
- **http://localhost:8080/swagger** – Swagger UI for API exploration
- **http://SERVER_IP:8080** – For remote server access
