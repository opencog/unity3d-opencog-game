#Game

* Purpose: The User Interface/HUD and inventory available during the game. Certain classes like OCActionsSetup and OCGameSetup do not seem to belong here

* Refactoring TODO
    [ ] A number of these files need to be moved/subsumed into other files at present. For example, OCGameStateManager needs to work with the new GameManager
	[ ] Certain files like OCActionsSetup.cs and OcGameSetup.cs are bloated with unnecessary regions that makes it hard to see their purpose, and do not appear to belong here. 
