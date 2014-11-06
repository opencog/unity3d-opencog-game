# Entities

* Purpose: House code that deals with avatars without directly relating to the opencog side (ie: not actions or planners). Character motors, player control, and animations are all kept in this folder.

* Structure:
    * Animations: Displays and controls avatar animations
	* EntityManager: Newly implemented and agile; for placing high level commands we may need from anywhere in the program, such as the ability to load a new character. 
	* Control: Colliders, motors, and other things we will need to control characters and players; as well as Input classes we need to build blocks and the like. 