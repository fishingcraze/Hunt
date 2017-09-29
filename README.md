# Hunt

### Description
Hunt is a virtual scavenger hunt app where players can join a game, select a team and solve hints to acquire treasure. The team with the most points wins.


### Supported Platforms

Platform | Build | Deploy
--- | --- | ---
iOS 10.0 and up | [![Build status](https://build.mobile.azure.com/v0.1/apps/e282924c-141c-4f66-8575-093291d8b5c1/branches/master/badge)](https://mobile.azure.com) | deploy
Android 5.0 and up | [![Build status](https://build.mobile.azure.com/v0.1/apps/61439d5f-9b86-461b-8128-738730c45b6b/branches/master/badge)](https://mobile.azure.com) | deploy



### Overview
The purpose of Hunt is to facilitate a virtual scavenger hunt between multiple teams. Players on each team will have to solve a riddle or hint and photograph the object. If the photograph contains the correct object, the team will be awarded the full amount of points for that treasure.

The first team to acquire all the treasures will win the game. If no team acquires all the treasure, the team with the most points will win when the time runs out. Alternatively, the Coordinator of the game can end the game early, in which case the team with the most points will win.


### Rules
  
* Roles
  * User can be involved in only one game at a time as either the Coordinator or a Player
    * Coordintors are responsible for
      * creating the game by configuring:
        * the amount of teams
        * the amount of players per teams
        * the amount of time the game lasts (from 5 - 180min)
      * adding treasure to the game
      * starting the game
      * ending the game early, if needed
    * Players are responsible for
      * joining games via 6 character entry code or scanning the QR code
      * choosing a team
      * acquriing treasure once the game has started
  * Treasure
    * There must be at least 2 treasures added to the game before it can be started
    * As a Coordinator, when adding treasure, the hint should not give away the object - the intent is to make the players sovle a riddle or puzzle and then take a phtotograph of the answer.
    

### Front-end Patterns
* Each ContentPage derives from BaseContentPage<T> where T is a BaseViewModel and serves as the BindingContext.
* All outbound requests are routed as new tasks through TaskRunner.RunProtected to handle common failure scenarios such as device network connectivity changes, back-end server outages, etc.
* Game document writes are further routed through SaveGameSafe proxy method to handle version conflicts resiliently with minimal code.
* Most images/icons are SVG files embedded in the Common project assembly once and rendered at runtime via the custom SvgImage control. Fill color and vector scale can be specified per instance.
* Animations are made possible with the Lottie Animation library from the folks at Airbnb. Animations are also vector and stored in a small .json file.
* Almost all UI code is shared, include the custom HUD and Toast elements.

### Back-end Patterns
* Games are saved as documents in DocumentDB. Games contain the teams, players, treasures and acquired treasures in a single document. Whenever the game is updated, players of the game are notified via silent push notification which triggers an update.
* All images are stored in blob storage and passed to the Vision APi via the blob URL.


### Technologies Utilized
* #### Front-end
  * Xamarin Forms
  * Mobile Center
    * Build
    * Test
    * Analytics
    * Push
    * Crashes
    * Distribution
  * 3rd Party SDKs
    * SkiaSharp
    * Lottie
    * PullToRefreshPage
    * ImageCircle
    * CrossMedia
    * CrossConnectivity
    * ZXing
    * XFGloss
    
* #### Back-end
  * C# Functions
  * Cosmos / Document DB
  * Blob Storage
  * Application Insights
  * Mobile Center Push via REST
  * Cognitive Services
    * Vision API
    * Content Moderator API
  * Service Bus
  * ARM Templates


### Presentations and Hackathons

Hunt was designed and is intended specifically for use with audiences at presentations, hackathons, community meetups, on-site meetings and speaking engagements.

* Mock data
  * When creating a new game, the mobile app has several options for seeding data into an empty game. The following options are available and allow the game to be put into different states depending on the goal of the demo.
    * User can choose to be the Coordinator or a Player
      * if Coordinator is chosen, the user can add additional treasure and manully start the game
      * if Player is chosed, the user is put on House Lannister and the game will be started automatically
    * Games can be seeded with players that join teams - about half the game slots will fill with random players
    * Games can be seeded with pre-configured Treasure (3)
    * If both players and treasures are seeeded, the team the player joins can be seeded with 2 acquired treasure
* Azure Deployment
  * To make it easy for new developers to stand up their own personal Hunt backend, ARM templates are used so that by clicking a button and selecting a few options (like subscription, resource group and region), developers can deploy their own pre-configured instance of all services needed by Hunt to function.

[![Deploy to Azure](https://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)
