# rm-dotnet-api
A basic web api that uses JWT authentication

## Overview
This project is a very basic web api that supports two functions, _Register_ and _Login_. The _Register_ function creates a new user in the database and the _Login_ function returns a JWT token that can be used to authenticate future requests.

## Setup

### JWT Key
The secret key must be configured using the dotnet user-secrets functionality.

_dotnet user-secrets set "Jwt:Key" "[SECRET-KEY]"_

Note: This requires the project file contain <UserSecretsId>[UNIQUE-IDENTIFIER]</UserSecretsId>.  This is already setup for this project, but can be changed.

### Database
The default database is setup as a SQLite database. This can be changed, but make sure you update the connection string in appsettings.json.

Code first migrations are setup. If you want to modify the migrations for your project (which you will), you will want to have Entity Framework Tools installed.  The command _dotnet tool install --global dotnet-ef_ should install the tools globally for you so you can run _dotnet ef migrations add <migration_name>_ if you want. The code automatically runs the migrations on startup.  This may not be ideal if you're using a database first approach.

Keys for the tables are set to GUIDs. This is a personal preference and can be changed to whatever you want.

### Identity
The project uses a custom _opaque_ token provider. The token simply consists of a unique identifer and an expiration date.  The same user can have multiple tokens to support logging into different devices without logging out of the original device. 

## Running the project
The project can be run using the dotnet CLI.

_dotnet run_

### HTTP Client
The http client will be available at http://localhost:5152 (configurable in the launch settings)
When running in debug mode, the Swagger UI will be available at http://localhost:5152/swagger

### HTTPS Client
To configure the client to use HTTPS, you will need to create a self-signed certificate and configure the project to use it.

1. Create a self-signed certificate using the following command: _dotnet dev-certs https --trust_
2. Run the project using the dotnet CLI: _dotnet run --launch-profile https_

When running in debug mode, the Swagger UI will be available at https://localhost:7015/swagger

## Other Notes
Warnings are set to be treated as errors. This helps keep the codebase clean from the outset, but it can be changed in the csproj file.