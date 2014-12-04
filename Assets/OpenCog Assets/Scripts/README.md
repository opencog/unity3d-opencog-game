# Scripts

* Purpose: Top level housing for all OpenCog scripts. Should not internally house exterior libraries. 

* Structure:
    * Connections: Having to do with networking and Opencog elements, such as the planner
	* Entities: Display and control in-game characters and the invisible player avatar.
	* Gameplay: Underpopulated folder that can contain narrative, scripting, and non-open-cog-specific gameplay. 
	* Interfaces: Editor and GUI interfaces both for extending the functionality of Unity and providing HUDs and UIs in game. 
	* Master: Top-Level Manager folder for containing the GameManager and any loaders, object managers, or other Singletons that should stand alone at the top. GameManager is the parent of EntityManager, located under Entities.
	* Utilities: Helpful scripts, algorithms, classes, and containers for making other tasks easier
	* Worlds: Everything to do with the voxel map and the children and attributes thereof. 