# Interfaces

* Purpose: To house every script that is primarily defined to create a user interface, including:
    1. Adding functionality to the Unity Editor, often on behalf of a corresponding data class somewhere else in the program.
	2. Creating an in-game, runtime GUI for editing data during Play mode. 
	
* Organizational Notes
    * A script can be considered as belonging to the Interfaces folder when it performs utility or helping functions, imports or exports, or creates GUI elements; when it contains/manages little object data, and when- in the case of editor scripts- has an element like "Run in Edit Mode" or "Add Menu Item"
	
* Structure
    * There are not enough interfaces in this game to truly warrant separate Editor and Game UI folders. Editors and in-game interfaces are kept in folders at the top level. The CustomBaseEditor details the attributes of an extended Unity Editor that may no longer be necessary, but which needs to be examined further. 
	
	* Control: Contains OCUnitTests.cs; which at present does not have a  much needed view seperate from its functionality
	* Console: In-Game Console for summoning forth the robot embodiment and performing other tasks. Most of the functionality is not implemented. 
	* Custom Editor: Has a lot of extensions to the basic unity editor that may no longer be needed. Refactoring needs to be done to decide if a lot of the extensions are even necessary any longer, and to cherry pick out portions that can stand alone and are still in use (such as perhaps the actions editor)
	* Game: The In-Game HUD, including the inventory. 
	* Menus: The main menu, pause menu, etc.
	
* Refactoring TO DO:
    [ ] Are many of the tools under the Custom Editor still necessary? 
	[ ] A number of sub folder could use some cleaning, documentation, or exceptions established. Examples:
	    [ ] Console contains its own resources. Good/Bad?
		[ ] Many of the console commands are not fully implemented, and operate in deceptive and slightly 'hacky' ways that may confuse anyone new to the program (ie: Load command, I'm looking at you...)
		[ ] Nothing under Custom Editor.BlockSet is appropriately commented.
		[ ] Game contains a lot of classes that seem like they do not belong. 
	[ ] Write a view for OCUnitTests.cs and move the UnitTests themselves elsewhere (ie: OpenCog.Master)
		
		