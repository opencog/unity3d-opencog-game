# Interfaces.Custom Editor.Editor

* Note: These folders should only compile with the editor; Ergo it is necessary to  name this folder 'Editor'
* Purpose: This was originally many extensions to the unity editor to add additional functionality, some which at this point may no longer be necessary, and others which may be very important. Although if approved some folders, like Automation, really should be bumped up a folder, right now this folder helps us contain a lot of code that needs review. 
    * The code has not been updated or maintained in some time, very little of it is in use, and it is unknown if many of the parts are necessary. Since the update to Unity4, a lot of interface design and extensions have gotten easier in unity. 
	* There are still some components which seem to be in use. For example: the ScriptEditor.AnimationEffectEditor still seems to be used by game Actions; or at least has the potential to be used. 
	* Many of these scripts say 'run in editor' and it is difficult to know if any of the automation ones are currently running and doing their jobs without doing some testing (especially as it is not currently known if Unity now handles any of these desired behaviors on its own, without editing)
	
* Structure: 
	
	* Automation: Scripts for automatically perform tasks such as building players. 
	* Base: Classes like OCEditor that make use of attributes and should be instantiable anywhere.
	* Helpers: Functions to add menu items to quickly add functionality to the unity editor, like the ability to find missing scripts. 
	
* Refactoring TODO
    [ ] This is one of the big areas where a lot of examination of the code needs to be done in order to determine where previous implementations cut off, what's necessary, what can be discarded, and what is actually important or should be developed further to add rapid functionality to the game. 