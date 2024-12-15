# RegisterMe

A web application managing cat exhibitions and registering cats into them.

The project was generated using
the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 8.0.2.

App can be found at [RegisterMe](https://logmeincats.azurewebsites.net)

---

## Table of Contents

- [Getting Started](#getting-started)
    - [Build](#build)
    - [Run in Docker](#run-in-docker)
- [Access](#access)
- [Testing](#testing)
- [Authors](#authors)
- [Version History](#version-history)
- [License](#license)

---

## Getting Started

### Development setup

#### Navigate to the project root directory:

```bash
cd RegisterMe/RegisterMe
```

#### Build

Run `dotnet build -tl` to build the solution.

#### Run in docker

1. Create a `.env.docker` file based on `.env.example`.  
   This file should include all necessary environment variables for the Docker setup.

2. Run the following command to build and run the application using Docker:

Then run:

```bash
sh compose.sh
```

---

## Access

After the application is successfully running, you can access:

- **Frontend:** [https://localhost](https://localhost)
- **Swagger API Documentation:** [https://localhost:444](https://localhost:444)

### Default Credentials

#### User:

- **Username:** `525025@muni.cz`
- **Password:** `Admin123*`

#### Admin:

- **Username:** `Value of EMAIL__MAIL attribute (from .env.docker)`
- **Password:** `Admin123*`

## Testing

To run the test suite, execute the following command:

```bash
dotnet test
```

---

## Authors

- **Ondřej Man**
    - [University Profile](https://is.muni.cz/auth/osoba/525025)

---

## Version History

### v0.1

- Initial release.

---

## License

This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.
