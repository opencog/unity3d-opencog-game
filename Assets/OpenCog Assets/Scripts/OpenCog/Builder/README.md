# OpenCog.Builder

* Purpose: Handles several different types of classes associated with building maps and blocks

Note: 
* **OCBuilder:** Something of a top level *manager* for the game and environment. Will process *input* for placing/removing blocks. Does not communicate directly with Chunks; communicates with *map*
* **OCChunkBuilder:** Will create the voxel chunks themselves using **map** data, and is built by Map.OCChunkRenderer
* **Others:** Responsible for creating the meshes of all the different block types from **vertices**
