#OpenCog.Master

* Purpose: Top level managers, singletons, and loaders which do not fit in any other category; and which need to exert a lot of top-down control or be accessed widely by the whole program. 

* Notes: 
    * Currently the only active resident is GameManager.cs  
    * OCGameObjectRegistry.cs came with a number of Extensions scripts I was sorting, and is not currently in use. 

* Refactoring TODO
    [ ] Determine what to do with OCGameObjectRegistry.cs and whether it should be here, embedded deeper, or simply deleted.