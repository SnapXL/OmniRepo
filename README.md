# OmniRepo

OmniRepo is a web-based server for hosting and managing package repositories for Debian/Ubuntu (apt/deb), RedHat/CentOS (rpm/yum), and generic package files.

The server supports:

- Generation and hosting of RPM, DEB, and generic repositories
- Package upload, deletion, copying, and promotion between environments
- PGP signing key creation and management
- Version management
- User read and write access control
- REST API
<!-- - CLI for CI integration -->

## Getting Started

The recommended way to run OmniRepo is with Docker. Persistent files such as the database, cache, PGP keys, and package files are stored in the `omnirepo-data` directory.

Install [Podman](https://podman.io/) or [Docker](https://docker.io).

Start the server:

```bash
docker run -d --name omnirepo \
  -p 5333:5333 \
  -v $(pwd)/omnirepo-data:/app/data \
  ghcr.io/SnapXL/OmniRepo:latest
```

## Build

Run `dotnet build -tl` to build the solution.

## Run

To run the web application:

```bash
cd ./src/Web
dotnet watch run
```

Navigate to http://localhost:5333. The application will automatically reload if you change any of the source files.

## Test

The solution contains unit, integration, functional, and acceptance tests.

To run the unit, integration, and functional tests (excluding acceptance tests):
```bash
dotnet test --filter "FullyQualifiedName!~AcceptanceTests"
```

To run the acceptance tests, first start the application:

```bash
cd ./src/Web
dotnet run
```

Then, in a new console, run the tests:
```bash
cd ./src/Web
dotnet test
```