This following document will show an administrator how to release new spells for the game.  This guide assumings you already have a proofreading copy of the spell created by being approved someone in the spell approver role, so actual spell description / text should already be complete and vetted for any problems.

*** Spell ***

1.  Check the name of the spell.  Check for spelling and grammatical errors.  Remove any leading or trailing whitespaces.  Spells should follow proper English grammar on title capitalization.  If in any doubt you can do a quick check of this here if needed:  https://capitalizemytitle.com/ (use Default settings).  Due to the lack of proper foreign keys, it is critical to get the name of the spell correct the first time as attempting to re-publish a spell with a different name, even if it's only off by a single letter in the wrong case, will break the game.

2.  Make sure that Mana cost is set to 7.  ALL spells should have this value set by default, but some older spells may use differing values before the number 7 was standardized.  (Note that 7 is only a base cost and that mana costs will be adjusted later on based on victim and attacker levels.)

3.  Similarly, Transformation points added by a successful casting of this spell should be set to 10.

4.  Similarly, Target's willpower decrease when hit by this spell should be set to 4.5 .

5.  Make sure the spell is learned at a valid region or location.  A valid location should look something like "comicstore_videogames" and NOT the value that players actually see in game.  Make sure the dropdown above is set appropriately.  If you need to look up the precise name of a location or region, click the Help button and scroll to the bottom of the page to get a datatable view of all locations.  Make sure there are no trailing or leading whitespaces.

*** Form ***

6.  Make sure the name of the form follows the same title capitalization rules as the name of the spell.  As with the name of the spell, this is very important to get right the first time.

7.  Make sure the box for "How many transformation energy points are needed to fully transform a target into this form" is always set to 100.  Some older spells allowed this to be set by the author but now this value is standardized.

8.  Make sure the gender of the form is set correctly.  Some writers skip this box and forget to set it to Female.

*** Item (when applicable) ***

9.  Make sure the name of the item follows proper title capitalization rules as mentioned for the spell and form name.

10.  If the Mobility type set back up in the form box is 'Animal' then make sure 'Pet' type is selected in the "What type of item is this?" dropdown.

11.  If a value is set for the "(Optional)  Target animate form of this item/pet's transformation curse" box, make sure it confirms to the proper database form name which should look something like "form_Ticklish_Half-Dragon_Lilah_Lace".

*** Form OR Item ***

12.  Under the "Give this Form/Item bonuses" box, make sure the form follows proper balancing rules:
	- animate forms should have a net stat amount of 0 and an absolute value of no more than 20*
	- items should have a net stat amount of 10 and an absolute value of no more than 20*
	- pets should have a net stat amount of 20 and an absolute value of no more than 36*
		*absolute values are the sum of all stats when made to be positive values, ie (10 - 5) is the same as (10 + 5) with an absolute value of 15.
	- Make sure any of the old stats have been completely removed.  The only exception to this is forms that are meant to be entirely immobile such as the enchanted tree which should have an action point movement discount of -999.
	- If the stats aren't balanced, feel free to adjust them to fit the rules the best you can while attempting to keep as close to the spirit of the spell as you can.  Most writers don't really care about this and many will skip this process entirely; in this scenario the stats are entirely up to you.

13. Download the graphic linked to in the "Is there an artist with whom you have arranged to do this artwork?" box.  Make sure the image is 600x600 in resolution and is either a .png, .jpg, or in rare cases a .gif format.  Upload it to the appropriate directory using FTP of your choice (I use Filezilla).  Only admins have permissions to do this.

14.  Set the filename of the graphic you downloaded into the "Image URL:" box at the bottom of the page.  Check the "Is Player Learnable" box if this is a spell that anyone can learn which is nearly all of user submitted spells

15.  Save the contribution and re-open it without using the browser back button.  Assert that the image now appears at the bottom of the page.  Don't worry about the thumbnail for now, those are generated in large batches periodically.

*** Publishing the Spell *

16.  Great!  Verify one last time that the spell, form, and item names look okay.  In this order click Publish Item, Publish Form, and Publish Spell.  

17.  Copy the text suggested at the top of the page after publishing the spell and paste this into the news, cleaning up any formatting problems and adding any hyperlinks to authors / artists that provide their own or have provided previous ones in the past.  Add what form the spell transforms the victim into with the format of "This spell turns its victim into [FORM NAME]!"  If the form mobility type is not obvious, add something like ", a pet!" to the end instead.

18  Click "Release Spell".  This will make it so players can now find it via searching in the correct location.

19.  Click "Mark as Live."  This will allow the spell to show up in the author's bio page, be no longer listed in the "Pending Approved Contributions" page, and be filterable on the Contributions table by the Live column by changing that value to be a green Y instead of a red N.

20.  Make sure the news listing is set to Live when you desire it to be so, and update the News date that shows up in the header to be the current day (full month name, day of month without 0 padding, and leave off year.)  You can do this via the "Change game date" link on the admin page.


All done!  This process is a lot faster that it sounds in this document but can still take 3-5 minutes to do, and processing a bunch becomes wearinesome quickly.  Keep an eye open for older spells in particular as those are most likely to have out of date standards regarding balancing rules.  As you can see there are a lot, LOT of opportunities for streamlining here but the code regarding this process is some of the oldest and most delicate code in the project.