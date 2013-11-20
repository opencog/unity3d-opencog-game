Cubiquity For Unity3D
=====================

Introduction
------------
Cubiquity is a free voxel terrain asset for Unity Free and Pro. It uses the power of native code to create environments which can be dynamically modified in both the Unity editor and also in-game, enabling dynamic digging, building, and destruction. It is currently in active development, and you can follow its progress in this Git repository.


.. image:: http://i.imgur.com/jGP69pV.jpg
   :target: http://youtu.be/6z0jd-J8QMQ
*Our tank demo shows how a voxel environment can be destroyed in real-time* `(download) <http://www.volumesoffun.com/downloads/Cubiquity/CubiquityUnity3DTest3.zip>`_

Key Features (mostly work in progress!)
---------------------------------------
- Supports two kinds of voxel environment – cubic and smooth

  - Build your world from millions of tiny colored cubes to create detailed 3D worlds. 
  
  - Create realistic terrains with powerful sculpting tools (coming soon).
  
- Advanced Graphical capabilities

  - Flexible material system supports arbitrary blending between multiple materials (coming soon).  
  
  - Level-of-detail meshes ensure fast operation across a range of hardware.
  
  - Supports Unity's Surface Shader system to allow easy customisation of appearance.
  
  - Works with real-time shadows and other dynamic lighting solutions.
  
- Physics

  - Mesh data can be passed into Unity's physics engine for unified collision detection and handling.
  
  - Collision detection can also be performed directly against the voxel data.
  
  - Custom raycast functionality for picking voxels.
  
- Create worlds easily

  - Import voxel terrains from external sources such as heightmaps.
  
  - Direct access to the voxel data allows you to procedurally generate worlds from code.
  
  - Editing tools available both in-editor and in-game (coming soon).
  
- Solid engineering

  - Cubiquity is powered by the PolyVox voxel library, which has been in development for over seven years and is used by several games.
  
  - Package includes all C# scripts allowing you to customize it to your needs.
  
  - Integrates seamlessly with many other packages from the asset store.
  
- Licensing options

  - Available free for non-commercial and evaluation use (see LICENSE.txt).
  
  - A commercial license will be available in the future for a fee.
  
More details
------------
For many years we have been actively developing the `PolyVox voxel terrain library <http://www.volumesoffun.com/polyvox-about/>`_ which has been used `by several games and engines <http://www.volumesoffun.com/polyvox-projects/>`_. PolyVox is an extremely powerful and flexible library, but requires an advanced knowledge of C++ and graphics programming to use effectively. Therefore we have recently introduced Cubiquity as a higher-level and easier to use interface to this functionality.

Cubiquity is independent of any particular game engine, but this Git repository provides the integration layer which connects Cubiquity to the Unity3D engine. It is this combination of the Cubiquity library and the integration layer which we will be making available for download from our website and also through the Unity3D asset store.

Follow Us
---------
- `Our main blog <http://www.volumesoffun.com/blog/>`_
- `Our Twitter feed <http://www.twitter.com/volumesoffun>`_
- `Our Youtube channel <http://www.youtube.com/user/VolumesOfFun>`_