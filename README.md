# Camagru
## Description
42Network project about building a web application
[See project's PDF](/en.subject.pdf)

## Prerequisites
Make sure you have the following installed on your machine:
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- Docker Desktop([Mac](https://docs.docker.com/desktop/install/mac-install/), [Windows](https://docs.docker.com/desktop/install/windows-install/), [Linux](https://docs.docker.com/desktop/install/linux-install/)) Or [Docker](https://docs.docker.com/engine/install/) *with [docker compose](https://docs.docker.com/compose/install/)
- [MongoDB Compass](https://www.mongodb.com/try/download/compass)

## Getting Started
1. Clone the repository:
```bash
git clone https://github.com/MinsuKin/Camagru
```
2. Navigate to the project directory:
```bash
cd Camagru
```
3. Run MongoDB in a Docker container:
	- Provide value to .env variables
```bash
cp .env.example .env
```
- In file .env: Edit the values of MONGOINIT_DB_* to your preferences

```bash
docker-compose up -d
```
This command will start a MongoDB container in the background. Make sure to customize the docker-compose.yml file based on your MongoDB configuration.

4. Restore dependencies and run the project:
```bash
dotnet restore
dotnet run
```
Open your browser and navigate to http://localhost:5000 to access the application.

## MongoDB Connection
By default, the application will connect to MongoDB at mongodb://localhost:27017. If you need to change the connection string, update the configuration in the appsettings.json file.

## Database Initialization
The application may require initial data or schema setup. Check the project documentation or source code for any specific initialization steps.

## Stopping the Environment
When you're done working on the project, stop the Docker containers:

```bash
docker-compose down
```

# Ressources
Styling of the website made following a [Tutorial](https://www.youtube.com/watch?v=oYRda7UtuhA)