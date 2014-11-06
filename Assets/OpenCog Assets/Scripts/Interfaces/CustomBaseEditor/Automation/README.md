# Interfaces.CustomBaseEditor.Automation

* Purpose: Scripts that allow certain complex tasks, like building players, to be done automatically using a script. 

* Notes: Some scripts, like OCAutomatedPlayerBuilder, are understood to still be very important. Others, like OCScriptableObjectAssetFactory, have unknown utility. 

* Refactoring TODO:
    [ ] OCScriptScanner and OCAutomatedScriptScanner look very different but have identical explanations. Is one unnecessary?
	[ ] OCScriptFixer is not implemented
	[ ] Actually, in general, what is running here and what is unnecessary? It seems the AutomatedEditorBuilder may not be important, but the AutomatedPlayerBuilder may be. 