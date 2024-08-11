# Welcome to Transformania Time

This rather unofficial document is meant to help any new developers get up to speed with what's going on in the source code.

## UPGRADE IN PROGRESS

Please be aware that we are in the process of migrating from .NET Framework 4.8 to .NET 8.

__.NET 8 is now required in addition to .NET Framework 4.8__

This is a significant undertaking and many of the technologies that Transformania Time is built on have evolved, been discontinued 
or are no-longer required. As a result, there will be a lot of change taking place around how to run and develop the code locally 
as well as, down the line, changes to how parts of the system work.

Tempest will try and keep this README up to date with any major impacts on contributors so please keep your forks up to date!

## Technologies
Here is the stack of technologies being used:

* ASP.Net MVC5
  * Migration to ASP.NET Core MVC 8 is underway but not complete
* C# as backend code
* Entity Framework 6
* SQL Server / LocalDB
* Javascript for frontend code (as if there's any alternative)
* Git for source control
* SignalR for chat / realtime notifications
* T4MVC for strong typing in MVC
  * This will be retired once migration to .NET 8 is complete

## Interested in contributing code to Transformania Time?

Please read through CONTRIBUTING.md if you would like to contribute code to the project. We happily accept merge requests!

## Good tools to have

Below are some tools I keep in my developer environment used daily:

* [Visual Studio 2022 Community](https://www.visualstudio.com/downloads/)
* [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) (provides a nice way to access the database in greater detail.  Visual Studio also has some limited built in tools for this)
* [7-Zip](http://www.7-zip.org) or another similar file archiving program (to extract the SQL backups)
* [Sourcetree](https://www.sourcetreeapp.com) (GUI for Git; command line via Bash also works fine if you are comfortable with it)
* The [AutoT4MVC](https://marketplace.visualstudio.com/items?itemName=BennorMcCarthy.AutoT4MVC) extension for Visual Studio to regenerate T4MVC's files automatically when MVC controllers are changed

## Setting up your development environment

### Database Configuration

As TT uses SQL Server as a database, there are a number of ways to set this up and connect locally. 

By default, TT is configured to use LocalDB which is available as part of [SQL Server Express editions](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) 
except SQL Server Express Core. LocalDB is a minimal version of the SQL Server engine which covers the vast majority of requirements for local development.

TT Can be run against a SQL Server instance (usually Express) running on the local machine, a remote machine (if correctly configured) or inside a Docker container. 
A later section will demonstrate how to set up your environment in this way.

### Running Builds

The easiest way to get your environment setup is to simply run the `build.ps1` or `build.sh` scripts in the root folder which will
* Install any required tools and packages
* Build TT
* Create your DB
* Seed your DB with initial data
* Run unit tests

You will need to relax the default PowerShell scripting policy slightly to run the build script. From a PowerShell prompt, execute the following (you only need to do this once)

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

To run the default build, kick off the following from PowerShell within the TT directory

```powershell
./build.ps1
```

### Running the Game

Get the game started, you'll need to create a few configuration files, although you only need to do this once. 

__All of the following are for `src/TT.Web`__

1. Copy the `AppSettings.sample.config` file and make a new file called `AppSettings.config` with the contents.
2. Copy the `ConnectionStrings.sample.config` file and make a new file called `ConnectionStrings.config` with the contents
3. Copy the `MachineKey.sample.config` file and make a new file called `MachineKey.config` with the contents
4. Copy the `Rewrite.sample.config` file and make a new file called `Rewrite.config` with the contents
5. Open the `TT.sln` file in your IDE of choice (Most likely Visual Studio) and run `TT.Web` with the IIS Express configuration
6. Open http://localhost:52223/
7. Login with the username `developer` and password `password`

### Custom Configuration

If you want to use a database connection that isn't for LocalDB, you'll need to follow 2 steps:

1. In `src/TT.Server`, create a new file called `localsettings.json` with the following code. You'll need to replace `SERVER_HOSTNAME`, `SERVER_USER_NAME` and `SERVER_PASSWORD` 
as appropriate.
```json
{
  "ConnectionStrings": {
    "StatsWebConnection": "Data Source={SERVER_HOSTNAME}; Initial Catalog=Stats; User ID={SERVER_USER_NAME}; Password={SERVER_PASSWORD}; TrustServerCertificate=True;"
  }
}
```
2. In `src/TT.Web/ConnectionStrings.config`. Change the connection string to the same one as you used in the previous step.

## TT.Console Utility
A utility console app lives alongside the rest of the code which facilitates working with the DB during dev and other functions such as 
local turn updates.

It can be run from the root using 
```
dotnet run --project src\TT.Console --
```
Running without any arguments will give you usage information on how to interact with it. There are also `console.ps` and `console.sh` scripts for shortcuts.

### Database Command
```
database - Performs operations on the database
└── Performs operations on the database
    └── dotnet run -- database status|up|migrate|rollback|recreate
        ├── [-c, --config-file <configfile>]
        ├── [-s, --seed-data <seeddatapath>]
        ├── [-d, --db-name <database>]
        ├── [-Y, --skip-confirmation]
        └── [-r, --rollback-steps <rollbacksteps>]


                                   Usage   Description
────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
                              subcommand   The operation to perform on the database
        [-c, --config-file <configfile>]   Path to JSON file containing connection string to be used
        [-s, --seed-data <seeddatapath>]   Seeds database with files in the provided path and performs migrations
              [-d, --db-name <database>]   The name of the target database
               [-Y, --skip-confirmation]   Skip confirmation prompts such as when dropping/recreating the database
  [-r, --rollback-steps <rollbacksteps>]   Number of steps to rollback when used with rollback sub-command
```

This command is used for checking the status of your local dev DB, migrating your DB, rolling back migrations or recreating it entirely.

The additional arguments the command take allow overriding various assumed defaults such as where to find the connection string and seed data, 
the name of the DB, whether to skip confirmation on recreating and how many steps to roll back on a rollback operation.

#### Check the status of your DB
```
console.ps1` database status
```
This will 
- Check the connection string works
- That pre-seed data is applied
- That all migrations are applied
- That all seed data is applied

A table detailing the status of each item above will be displayed

#### Standing up a new DB from scratch
```
console.ps1` database up
```
This will
- Check it can connect to the DB with the connection string
- Create the DB if it doesn't already exist
- Pre-seed any data required prior to migrations
- Run the migrations
- Apply the seed data

#### Applying a Migration
```
console.ps1` database migrate
```
This will run any migrations not yet applied to the target DB. 

#### Rolling back a Migration
```
console.ps1` database rollback
```
This will rollback the last migration applied ___IF___ it can be rolled back. Not all migrations will support this, it depends what they change and how they are written.

The `-r` flag or `--rollback-steps` argument can be used to specify the number of steps to rollback.

#### Recreating the DB
```
console.ps1` database recreate
```
This will __DROP__ the existing target DB and recreate it from nothing. It is the same as `database up` except in addition it will first destroy the target DB.

The `-Y` flag or `--skip-confirmation` argument can be used to skip the confirmation prompt.

### Update Turn Command
```
update-turn - Performs the turn update against a target server
└── Performs the turn update against a target server
    └── dotnet run -- update-turn
        └── [-t, --target-server <targetserver>]


                                 Usage   Description
─────────────────────────────────────────────────────────────────────────────────────────────
  [-t, --target-server <targetserver>]   The server, including port, to run the turn update
```
```
console.ps1` update-turn
```
This will run all the end end-of-turn actions and advance onto the next turn. 

The `-t` flag or `--target-server` argument can be used to proivde an alternative base URL to call the turn update on.

## Guts / Project Organization

There are few conventions I've tried to follow to keep organized.  The big ones are:

### Used Controllers

* PvPController has most of the core game methods such as moving, cleansing, attacking, etc.
* PvPAdminController has administrator / moderator type methods used to keep the game running without having to directly alter the database.
* NPCController has methods for interacting with non-player characters, namely interacting with merchants and Rusty as well as some mind control stuff
* SettingsController deals with various customization aspects, allowing player to write bio, RP classified ads, respond to polls, etc.
* InfoController deals mainly with static information and pages
* ItemController deals with interaction with inventory items.  A lot more methods should be refactored into here from PvPController.
* TransformaniaTimeController is for non TT-multiplayer stuff such as singleplayer gameshow, Fashion Wars, and Bombie.  Pleave leave this alone
* AccountController deals with account registrations, login, etc.  This is all code straight from Simple Membership
* ContributionController deals with contributed content pages and actions.
* CovenantController deals with anything covenant related: joining, leaving, donating Arpeyjis, using furniture, etc

### Unused Controllers

* HomeController (template code, should be removed)
* DuelController (was meant to be used for some realtime 1v1 or group based dueling using SignalR that unfortunately never materialized... yet)
* TFWorldController (unused code from an early prototype of a multiplayer game similar to this)

### Procedures

* Try to keep any calls that directly alter a database in the Procedures folder.  (Exceptions to this are Contribution and PvPAdmin controllers, where code is generally single-use enough that it's okay.)

### Chat

* Everything in realtime (chat, realtime notifications, etc) is kept in the Chat folder.  ChatHub deals with chat and NoticeHub deals with notices, as the name implies.  DuelHub has some semi-abandoned dueling code

### Images

* Images are (generally speaking) divided into animate forms (portraits), item forms, (itemPortraits), and animal forms (animalPortraits).  Every image (except a few such as GIFS) also have thumbnail versions to be loaded when the page does not call for a larger view.  This sizes on a hell of a lot of bandwidth as Arrhae has shown with processed IIS logs.