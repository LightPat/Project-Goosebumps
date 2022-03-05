# Project-Goosebumps

## Main Premise
Story-Driven Fantasy First Person Sandbox with cool swords, guns, and magic

## Currently Working on
Unity Netcode for gameobjects implementation

## Assets
I'm just using stuff off cgtrader right now

## Classes (abilities basically)
### Electromaster/Railgun
* Based off Mikoto Misaka, controls and sees EM waves
* Can shoot arcs of electricity to a target, as long as that target is conductive. This should be assigned in the stats script
### Teleporter
* Based off Koroko Shirai, can teleport self and objects/players around it
* Assign if an object is teleportable in the stats script
### Lord of Time
* Can slow down or speed up time for a short period
* Useful for melee weapons that can parry bullets
### Psychic
* Can take over and control other player’s or enemy’s minds and control them for a duration.
* Also is able to throw objects using telekinesis
* Able to communicate via telepathy

## Weapon Ideas
### Scar Rifle
* Not actually a scar L, just the main rifle model for the game since the scar looks sick
### Shifting Umbrella
* The umbrella shifts between a steel canopy that can block bullets and magic, and a typical umbrella canopy that can slow the user’s descent in the air.
* The umbrella operates using magic, so it is not a melee weapon, but it is a wand
### Railgun
* Shoots a steel projectile at ridiculously high speeds. Everything it runs into leaves a huge particle effect in its wake.
### Shadow Knives (pair, for parrying like in ranger’s apprentice)
* Based on King Arthur’s knife carnwennan.
* Gives the user the ability to go invisible and not make noise for a period of time
* Hitting an opponent in the back triggers an execution animation and deals critical damage
### Excalibur
* Based off Saber’s excalibur in Fate UBW
* After chaining lots of hits together, it gives its user the ability to unleash a devastating blast of holy light. This light blinds all players in the same room as it, like a really powerful flashbang. And deals huge damage to the target.


## Code Write Up/Plan OUTDATED
### Players/Enemies
#### Stats
May need to make this an abstract class, but it contains the health value of players and enemies right now
#### Controller
Abstract parent class for all types of controllers for AI enemies and players
##### PlayerController : Controller
Handles how the player’s input interacts with the environment<br/>
WASD movement<br/>
Rotation using the mouse<br/>
Jumping with spacebar<br/>
Interacting with items using “e”<br/>
Pickup items with the tag “inventoryitem”<br/>
Call the interact() method on items with the “interactable” tag<br/>
Should attack be in here or in weapon?<br/><br/>
Should switching weapons with 1,2,3 be in here or in inventory?
##### EnemyController : Controller
Handles how an enemy moves and thinks<br/>
The name of this script may change depending on the different types of enemies I implement, that’s why all of them will inherit from the Controller class
#### Inventory/Weapons
##### Inventory
Loadout : list of 3? Weapons the player has equipped<br/>
Items : list of all the gameobjects the player has picked up
##### Weapon
The abstract parent class for all weapons<br/>
Contains basic information about the weapon like base damage, headshot multiplier, full auto, etc.<br/>
The point of this is so that we can call GetComponent<Weapon> and get every type of weapon’s attack functions
##### Gun : Weapon
Handles bullet lines, gun animations, hit detection when attacking, and fire rate logic
##### Sword : Weapon
Handles sword animations, hit detection when attacking, and swing rate logic
##### Magic : Weapon
Handles magic animations, hit detection when attacking, and fire rate logic
