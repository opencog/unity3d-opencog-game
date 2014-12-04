Opencog.Interfaces.Custom Editor

* Q: What is the Custom Editor.Editor folder? A: A collection of scripts that must never compile outside of Editor Mode. Read more below.  
* Purpose: Add Editor Functionality. Typically, custom editor functionality should be in a folder called 'Editor' so it doesn't build with the main game. However as some helper functionality is based on Monobehaviors and tied to prefabs, I have it in alternative folders and using #if UNITY_EDITOR to eliminate warnings and get the exact functionality we desire.  
* Note: Some of these classes should be deprecated, but that is a clean up for another time.  

* Structure:
    * Attributes: Old extensions of functions like booleans.  
    * BlockSet: Although these classes are really only used at edit time, they are often Monobehaviors and tied to prefabs, so we want them outside any Editor folders (Which would exclude them from builds and create funky warnings to scare noobs)  
	* Editor: We created a pretty big custom base editor. Not all pieces are (or even should be) in use any more, but until we can do a more thorough cleaning we are leaving them in. They are not tied to monobehaviors and should be built only in the editor- so don't rename this folder!      
    * Effects: Animation that occurs over the interface (such as a fade out, or a pop up)  
    * Setup: A few miscellaneous helper classes with almost no implementations  
