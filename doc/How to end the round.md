# How to end the round

Welcome!  So it is the last turn of the game and there are no more updates to the ground.  Now it's time to put the game into chaos mode will new features get added, bugs squashed, stories/quests written, etc etc.

Here is what to do when ending the round:

1. Assign end-of-the-round badges.  Admins can go to [this link](https://www.transformaniatime.com/PvPAdmin/AssignLeadersBadges) to run this automatically.  Note that this will only work when the game is on the very last turn of the round (at the time of writing this, turn 5000/5000) and NOT in chaos mode.  Please make sure to only run this once.

2 The PvP, XP, and Item leaderboard now all autosave on the last turn of the round and there is no need to do anything with them anymore.

3. Update the JSON file Scripts/pastRounds.json with the latest round information.  The winner of the era is the first-place winner of a certain achievement which differs each round.  Use the winner's full name including nickname and generation title, ie "Bob 'I Won!' Smith IV".

4. [Save the achievement leaderboard](https://www.transformaniatime.com/leaderboard/playerstatsleaders).  To do this you should expand every achievement to see the full top 10 of EACH category.  Once this is done, copy all the HTML out from the `<div class="row">...</div>` below the `<h1>Statistics Leaders</h1>` markup.  Save this data as a new file under Views/PvP/RoundLeaderboards/Statistics and then Run Custom Tool on T4MVC.tt in TT.Web.

5. Everything should be saved now.  If you have access to the FTP live server, drop this files in the appropriate locations and check that everything is saved correctly.

6. Write in game news that the round has ended and that the game is entering chaos mode.  Congratulate the winner of the era.

7. Set everyone to SuperProtection mode: https://www.transformaniatime.com/pvpadmin/seteveryonetosp

1. Time for chaos!  Go to [this link](https://www.transformaniatime.com/PvPAdmin/ChangeWorldStats) and check Chaos Mode.  Roll back the turn number back to 0.  The server should instantly register this change and begin chaos mode.
