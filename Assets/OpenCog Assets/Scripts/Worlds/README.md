# Worlds

* Purpose: This folder is for block, blockset, mesh, rendering, and data pertaining to the creation of the game environment, particularly of the voxel engine. 

* Notes: There are a number of files scattered elsewhere in the hierarchy that pertain to maps which will have to be edited when Cubiquity is implemented. These include Entitles.Control.OCBuilder.cs, collision detection scripts in Utility, 

* Structure:
    * Block: Holds the types of blocks and their meshes (water/glass/cactus) 
	* BlockSet: Data classes for the blockset, for holding different types of block for rendering
	* Data: The Chunk, Block, and Column data that represents the map
	* Effects: Animations, translations, and other useful alterations that operate on the blocks of the world.
	* Generator: Randomly create terrain!
	* Rendering: actually display the map on the screen
	
	*OCMap.cs: The main manager for the map

* Refactoring TODO:
    [ ] Establish if any refactoring is to be done.... Most likely not, as we are going to be gutting this area and pushing in Cubiquity. 
	    * But if there WAS I would definitely be creating a MapManager that was seperate from a lot of the data and functionality inside Map. 