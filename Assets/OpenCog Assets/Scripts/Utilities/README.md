#OpenCog.Utilities

* Purpose: The Utilities Scripts folder holds scripts that may be used either at edit or play time, and which serve some sort of helper, storage, algebraic, or miscellaneous function. 

* Structure:
    * BaseClasses: Classes like "Singleton" or generics which are extended and used all throughout the program and add additional functionality to unity's available classes. 
	* HelperFunctions: Do not contain data themselves and are instead used to perform operations, often on classes Unity already contains and which cannot be extended (ie: Transform) and whose utility are not specific to any one class. 
	* Logging: Errors and Logs
	* Physics: Raytacing and collision detection that doesn't belong on any specific game class... yet.
	* Startup: Configuration files and other yumyums that allow us to change how the game loads and what it does. 
	* Structures: Like base classes but more finalized, these 'structs' (they're still classes, though) consist of things like the Map2D class and contain a lot of data used everywhere in the program. 
	

### What to Store
* Store small structures & types, enumerated types, helper functions, and generics. 
* Typically, do not store Monobehaviors or Scriptable Objects.
* The exception to the above statement is when creating generics and base classes, such as the Singleton pattern which can be used anywhere in almost any project.
* Utilities scripts should feel generic and modular, as if they could be used in almost any project or in any part of our project. Helper functions specific to characters or worlds should be stored with the classes they assist.  

