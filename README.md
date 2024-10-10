# RegisterMe

The project was generated using
the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 8.0.2.

App can be found at [RegisterMe](https://logmeincats.azurewebsites.net)

## Getting Started

### Development setup

#### Go to the project root

```bash
cd RegisterMe/RegisterMe
```

#### Build

Run `dotnet build -tl` to build the solution.

#### Run in docker

Firstly create `.env.docker` according to `.env.example`

```bash
sh compose.sh
```

## Access

After successfully running the application, you may access:\
frontend at https://localhost
swagger at https://localhost:444.

Default user credentials:\
username: OndrejMan1@gmail.com
password: Admin123*

Default admin credentials:\
username: registracekocek@gmail.com
password: Admin123*

## Test

The solution contains unit and functional tests.

To run the tests:

```bash
dotnet test
```

## Authors

Ondřej Man [@IS](https://is.muni.cz/auth/osoba/525025)

## Version History

* 0.1
    * Initial Release

## License

This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.

## Description

The application is developed in ASP.NET Core MVC using EntityFramework for ORM.
Its frontend is designed with Bootstrap for responsiveness and aesthetics.
The backend communicates with PostgreSQL, and the code is hosted on GitHub.
The application features an API with Swagger support for documentation and interaction.
API endpoints are validated using Fluent Validation and processed through Mediator.
Price calculations are handled by RuleEngine, and payments are integrated via Stripe.

## Project summary:

- Ability to register for exhibitions, including:
    - Input of personal information.
    - Input of cat details, including EMS validation.
    - Information about the cat’s parents, if it is not a household or experimantal cat.
    - Exhibition participation parameters, such as days of participation, group registration, and cage selection (new
      own, new rented, previously used own, previously used rented).
- Administrative functions for exhibition management.
- Option for users to view their registrations.
- Compliance with GDPR rules for data protection.

## Useful Links for Documentations and Manuals:

- [Moravia Cat Club - Exhibitions](http://www.moraviacatclub.eu/vystavy.html)
- [DataTables i18n Plugins](https://datatables.net/plug-ins/i18n/)
- [SCHK PDF 2018](https://www.schk.cz/files/V-2018.pdf)
