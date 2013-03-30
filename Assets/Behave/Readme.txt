Behave readme
=================
The information in this readme file only applies to people upgrading their project from a pre 1.2 version of Behave.

Installing Behave
-----------------
Four simple steps to stuff your Unity project full of Behave awesome:

 1. Remove any existing Behave folder from the project.
 2. Remove any compiled Behave libraries (the [LibraryName]Build assets).
 3. Fix all compiler errors (comment out any code referencing Behave).
 4. Import the Behave package.
 5. If your Behave assets show the default inspector when selected,
    with a "MonoBehaviour (missing)" value in it's script field:
       Select the BehaveAsset script for that field, select something
       else and then select your Behave asset again.


Note on adapting assets
-----------------------
Existing Behave assets will automatically load into 1.2 and update.
However, already built ones will have to be rebuilt with 1.2 before
they can be used, since Behave 1.2 includes runtime API changes.