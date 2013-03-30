OpenCog-Unity3D-Game
====================

A New OpenCog Unity3D Game embodiment framework.

Additional Installation Steps:
Set MonoDevelop Policy to OpenCog.
Copy MonoDevelop File Templates to Addins folder.


Best Practices:

Avoid Branching Assets.  There should always only ever be one version of any asset.  If you absolutely have to branch a prefab, scene, or mesh, etc., follow this process  that it is very clear which is the right version:  The "wrong" branch should have a name such as "lake_MainScene," so that it's clear that this isn't the asset that everyone should use.  Branching prefabs requires a specific process to make it safe, described below.
How to Branch Prefabs:  Before you branch a prefab, duplicate it and rename the duplicate to a special name unique to you (such as "lake_MainScene").  Then, after you've branched, tested, and merged your changes, delete the duplicate.  If an error occurs, use the duplicate as a backup.  When there's no other way, remember to coordinate with anyone that might be branching that same asset.
Clone a second copy of the project for testing.  After changes, this second copy, the clean copy, should be updated and tested.  No-one should make any changes to their clean copies.  This is especially useful to catch missing assets.
Use MCEdit or similar tools for level editing.  Unity is not the perfect level editor, and we should only build tools for the things that MCEdit and similar tools can't do.  For example, we'll eventually build a tool to instantiate a prefab or set of prefabs from an XML file, allowing us to effectively create maps with game-specific objects without using separate scenes.
Use named empty game objects as scene folders.  These folders should use the identity transform.  If a prefab's transform is not specifically used to position an object, it should be at the origin.  This will allow us to organize files in our scenes without running into problems with local and world space, etc.
Use prefabs for everything.  The only game objects in the scene that should not be prefabs are the folders.  Even unique objects that are used only once should be prefabs.  This makes it easier to make changes that don't requirethe scene to change.  In general, when given the choice of whether to make a new prefab or a new instance that deviates from an old prefab, choose to make a new prefab for that new instance.  
Don't use direct-references or link instances to instances whenever possible.  Try to make links automatic by having the instance find the target instance by its tag.  Linking to prefabs from instances is ok.
