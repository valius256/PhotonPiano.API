# PhotonPiano.API

This repository contains the PhotonPiano.API project, built using .NET. Below is a guide to help you get started and follow best practices while working with this codebase.

---

## Table of Contents
- [Setup](#setup)
- [Pulling from Staging and Main](#pulling-from-staging-and-main)
- [Database Migrations](#database-migrations)
- [Development Practices](#development-practices)
- [Contributing](#contributing)
- [License](#license)

---

## Setup
1. Clone the repository to your local machine:
   ```bash
   git clone <repository-url>
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Build the solution:
   ```bash
   dotnet build
   ```
4. Update your local `appsettings.json` file to match your environment requirements.

---

## Pulling from Staging and Main
Before pulling code from the `Staging` or `Main` branches, ensure the following:

1. Open the `appsettings.json` file in the `PhotonPiano.API` project.
2. Locate the `EnableMigration` setting.
3. Set the value of `EnableMigration` to `false` if:
   - You intend to update the database locally.
   - You are planning to create or modify migrations.

Failing to do this may lead to unintended database migrations or updates.

---

## Database Migrations
When working with database migrations, follow these steps:

1. Ensure `EnableMigration` is set to `false` in `appsettings.json`.
2. Create a new migration:
   ```bash
   dotnet ef migrations add <MigrationName>
   ```
3. Apply the migration to your local database:
   ```bash
   dotnet ef database update
   ```
4. Test the migration thoroughly before committing the changes.

---

## Development Practices
- Follow the repository's coding guidelines.
- Write unit tests for new features and bug fixes.
- Document any significant changes or configurations.
- Keep `Staging` and `Main` branches clean and functional.

---

## Contributing
We welcome contributions! Please follow these steps:
1. Fork the repository.
2. Create a feature branch.
3. Commit your changes with clear and descriptive messages.
4. Submit a pull request to `Staging` for review.

---

