# Welcome to Transformania Time! #
This rather unofficial document is meant to help any new developers get up to speed with what's going on in the source code.

First off, here is the stack of technologies being used:

* ASP.Net MVC5
* C# as backend code
* Entity Framework 6
* SQL Server / LocalDB
* Javascript for frontend code (as if there's any alternative)
* Git for source control
* SignalR for chat / realtime notifications

# Good tools to have #

Below are some tools I keep in my developer environment used daily:

* Visual Studio 2015 Community
* SQL Server Management Studio 2012 (provides a nice way to access the database in greater detail.  Visual Studio also has some limited built in tools for this)
* 7-Zip or another similar file archiving program (to extract the SQL backups)
* Sourcetree (GUI for Git; command line via Bash also works fine if you are comfortable with it)

# Setting up your development environment #

Thanks to Tempest, TT now has an automated build system which uses Cake, a Make/Rake like build system built on the Roslyn compiler. To set up your development environment you will need to run the build script which will 

* Build TT
* Create your DB
* Seed your DB with initial data
* Run unit tests

You will need to relax the default PowerShell scripting policy slightly to run the build script. From a PowerShell prompt, execute the following (you only need to do this once)

```
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned
```

To run the default build, kick off the following from a command prompt within the TT directory

```
Powershell .\build.ps1
```

By default, the build will attempt to use SQL Server 2012 LocalDB however if you are using SQL Server 2014 you will want to use

```
Powershell .\build.ps1 -dbType "localdb_v2"
```

If you are running SQL Express (any version) and have localhost set as an alias you can use 

```
Powershell .\build.ps1 -dbType "server"
```

## Migrating Database ##

When schema changes have been made, you can use the build system to update your development database simply by running the default build. If you only specifically want to run migrations and nothing else, use 

```
Powershell .\build.ps1 -target "Migrate"
```

Again, you can use the `-dbType` arguement suitable for your environment.

## Re-creating Database ##

To re-create your DB from scratch you can do the following

`Powershell .\build.ps1 -target "Recreate-DB"`

This will drop your existing database, recreate from migrations and apply the seed data.

# Random things to know #

 TT uses (mostly) Entity Framework Code First to interact with an SQL database.  Entity Framework is meant to be a tool to help abstract away some SQL by writing queries in C#, automatically converting them to SQL queries and executing them behind the scenes.  Unfortunately Entity Framework has some drawbacks, such as:

  - If the database schema changes, you will have to update it by executing `Update-Database` in the Package Manager Cosole window.  Otherwise you'll get a 'context has changed' exception when trying to launch the application
  - If you drop a property in a table, you'll have to do some funky stuff in SQL Server to permit this to happen (Arrhae can fill in details here...)
  - A stored procedure is used to retrieve the Buffs (bonuses from form / gear / effects) for a player.  The script to alter this is found in the Schema folder.
  - If you want to do a join between two tables (such as the GetPlayerFormViewModel() method under PlayerProcedures) you have to have them in the same contex

  This repo has a few different projects in it, having evolved from a simple server that hosted the singleplayer Transformania Time game (the gameshow one) to also including some experiments with AngularJS (Fashion Wars) and HTML5 canvas (Bombie).  These have nothing to do with the core multiplayer TT game so they should be left alone for the time being unless you're just curious.  (As June 24, a different repo was created and all future code in those projects will go there.  TT gameshow code will stay here.)

# Guts / Project Organization  #

There are few conventions I've tried to follow to keep organized.  The big ones are:

## Used Controllers: ##

- PvPController has most of the core game methods such as moving, cleansing, attacking, etc.
- PvPAdminController has administrator / moderator type methods used to keep the game running without having to directly alter the database.  
- NPCController has methods for interacting with non-player characters, namely interacting with merchants and Rusty as well as some mind control stuff
- SettingsController deals with various customization aspects, allowing player to write bio, RP classified ads, respond to polls, etc.
- InfoController deals mainly with static information and pages
- ItemController deals with interaction with inventory items.  A lot more methods should be refactored into here from PvPController.
- TransformaniaTimeController is for non TT-multiplayer stuff such as singleplayer gameshow, Fashion Wars, and Bombie.  Pleave leave this alone
- AccountController deals with account registrations, login, etc.  This is all code straight from Simple Membership
- ContributionController deals with contributed content pages and actions.
- CovenantController deals with anything covenant related: joining, leaving, donating Arpeyjis, using furniture, etc


## Unused Controllers: ##

- HomeController (template code, should be removed)
- DuelController (was meant to be used for some realtime 1v1 or group based dueling using SignalR that unfortunately never materialized... yet)
- TFWorldController (unused code from an early prototype of a multiplayer game similar to this)

## Procedures: ##

- Try to keep any calls that directly alter a database in the Procedures folder.  (Exceptions to this are Contribution and PvPAdmin controllers, where code is generally single-use enough that it's okay.)


## Chat: ##

- Everything in realtime (chat, realtime notifications, etc) is kept in the Chat folder.  ChatHub deals with chat and NoticeHub deals with notices, as the name implies.  DuelHub has some semi-abandoned dueling code

## Images: ##

- Images are (generally speaking) divided into animate forms (portraits), item forms, (itemPortraits), and animal forms (animalPortraits).  Every image (except a few such as GIFS) also have thumbnail versions to be loaded when the page does not call for a larger view.  This sizes on a hell of a lot of bandwidth as Arrhae has shown with processed IIS logs.