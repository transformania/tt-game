# Welcome to Transformania Time! #
This rather unofficial document is meant to help any new developers get up to speed with what's going on in the source code.

First off, here is the stack of technologies being used:

* ASP.Net MVC5
* C# as backend code
* Entity Framework 6
* Javascript for frontend code (as if there's any alternative)
* Git for source control
* SignalR for chat / realtime notifications
* Simple Membership for authentication

# Good tools to have #

Below are some tools I keep in my developer environment used daily:

* Visual Studio 2013 Express (free download.  I might consider moving to a newer version in the future, possibly Visual Studio Community)
* SQL Server Management Studio 2012 (provides a nice way to access the database in greater detail.  Visual Studio also has some limited built in tools for this)
* 7-Zip or another similar file archiving program (to extract the SQL backups)
* Sourcetree (GUI for Git; command line via Bash also works fine if you are comfortable with it)

# Files you'll want #

https://www.dropbox.com/sh/iifq0ht8z7ucu00/AAC-mCl8Ce_5Kj2gXyLt9vnma?dl=0 
The link above takes you to a Dropbox folder that contains files that can be downloaded and used by any TT dev.  It contains information such as as .SQL backup of the database which will allow you to seed your local database with some saved live data, populating your local database with forms / spells / item information that will be useful in development or to get an idea of how the database is arranged.

# Creating a blank local test database #
Being able to build the project and run it on your local machine isn't very useful if you don't have a test database to work with, so you'll probably want to import the .SQL backup of the database. Here's how to get started!

  1. Download a copy of a database dump from dropbox and unzip the .7z file, creating a .SQL file.
  1. Copy tfgame\MachineKey.sample.config to tfgame\MachineKey.config
  1. Copy tfgame\ConnectionStrings.config to tfgame\ConnectionStrings.config and modify as needed to suit your SQL Server version.
  1. Start up SQL Server Management Studio. Connect to your LocalDB.
      * If SQL Server 2012 LocalDB, use (localdb)\v11.0 with Windows Authentication
      * If SQL Server 2014 LocalDB, use (localdb)\MSSQLLocalDB with Windows Authentication
  1. If you already have Databases\Stats in your LocalDB instance, delete it.
      * Note: This can also be done from Visual Studio, but I have found it to be less reliable than doing from SSMS.
  1. Verify that tfgame\App_Data has no .mdf or .ldf files.
  1. In Visual Studio, build and run the project. Register a new user, and then stop debugging.
  1. Back in SSMS, open the extracted .SQL file. This will take some period of time, because this is a large file.
  1. Click on Execute on the toolbar, and the .SQL file will get imported into your local database. This will probably take a few minutes.
  1. You should now be able to go back to Visual Studio, start the game, and log in!

# Random things to know #

 TT uses (mostly) Entity Framework Code First to interact with an SQL database.  Entity Framework is meant to be a tool to help abstract away some SQL by writing queries in C#, automatically converting them to SQL queries and executing them behind the scenes.  Unfortunately Entity Framework has some drawbacks, such as:

  - If the database schema changes, you will have to update it by executing 'Update-Database' in the Package Manager Cosole window.  Otherwise you'll get a 'context has changed' exception when trying to launch the application
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