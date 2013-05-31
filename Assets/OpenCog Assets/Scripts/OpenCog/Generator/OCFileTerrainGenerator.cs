using System;
using UnityEngine;
using System.Collections;

public class OCFileTerrainGenerator
{
	private string _fullMapPath;
	private OpenCog.Map.OCMap _map;
	private const string _baseMapFolder = "Assets\\Maps";
	
	public OCFileTerrainGenerator (OpenCog.Map.OCMap map, string mapName)
	{
		map.MapName = mapName;

		_map = map;
		_fullMapPath = System.IO.Path.Combine (_baseMapFolder, mapName);
	}
	
	public void LoadLevel()
	{
		int verticalOffset = 85;
		
		Debug.Log ("About to load level folder: " + _fullMapPath + ".");
		
		Substrate.AnvilWorld mcWorld = Substrate.AnvilWorld.Create (_fullMapPath);
			
		Substrate.AnvilRegionManager mcAnvilRegionManager = mcWorld.GetRegionManager();
				
		OpenCog.BlockSet.OCBlockSet blockSet = _map.GetBlockSet();
				
		_map.GetSunLightmap().SetSunHeight(3, 3, 3);

		int createCount = 0;

		Debug.Log("In LoadLevel, there are " + blockSet.BlockCount + " blocks available.");
		
		foreach( Substrate.AnvilRegion mcAnvilRegion in mcAnvilRegionManager )
		{
			// Loop through x-axis of chunks in this region
			for (int iMCChunkX  = 0; iMCChunkX < mcAnvilRegion.XDim; iMCChunkX++)
			{
				// Loop through z-axis of chunks in this region.
				for (int iMCChunkZ = 0; iMCChunkZ < mcAnvilRegion.ZDim; iMCChunkZ++)
				{
					// Retrieve the chunk at the current position in our 2D loop...
					Substrate.ChunkRef mcChunkRef = mcAnvilRegion.GetChunkRef (iMCChunkX, iMCChunkZ);
					
					if (mcChunkRef != null)
					{
						if (mcChunkRef.IsTerrainPopulated)
						{
							// Ok...now to stick the blocks in...
							
							for (int iMCChunkInternalX = 0; iMCChunkInternalX < mcChunkRef.Blocks.XDim; iMCChunkInternalX++)
							{
								for (int iMCChunkInternalY = 0; iMCChunkInternalY < mcChunkRef.Blocks.YDim; iMCChunkInternalY++)
								{
									for (int iMCChunkInternalZ = 0; iMCChunkInternalZ < mcChunkRef.Blocks.ZDim; iMCChunkInternalZ++)
									{
										int iBlockID = mcChunkRef.Blocks.GetID (iMCChunkInternalX, iMCChunkInternalY, iMCChunkInternalZ);
										
										Vector3i blockPos = new Vector3i(iMCChunkInternalX, iMCChunkInternalY + verticalOffset, iMCChunkInternalZ);
										
										if (iBlockID != 0)
										{
											switch (iBlockID)
											{
											case 3: // Dirt to first grass
												iBlockID = 1;
												break;
											case 12: // Grass to grass
												iBlockID = 1;
												break;
											case 13: // Gravel to stone
												iBlockID = 4;
												break;
											case 1: // Stone to second stone
												iBlockID = 5;
												break;
											case 16: // Coal ore to fungus
												iBlockID = 17;
												break;
											case 15: // Iron ore to pumpkin
												iBlockID = 20;
												break;
											case 9: // Water to water
												iBlockID = 8;
												//Debug.Log ("Creating some water at [" + blockPos.x + ", " + blockPos.y + ", " + blockPos.z + "]");
												break;
											default:
												Debug.Log ("Unmapped BlockID: " + iBlockID);
												break;
											}
											
											OpenCog.BlockSet.BaseBlockSet.OCBlock newBlock = blockSet.GetBlock(iBlockID);

											_map.SetBlock (new OpenCog.Map.OCBlockData(newBlock, blockPos), blockPos);
											
											Vector3i chunkPos = OpenCog.Map.OCChunk.ToChunkPosition(blockPos);
				
											_map.SetDirty (chunkPos);

											createCount += 1;
										}
									} // End for (int iMCChunkInternalZ = 0; iMCChunkInternalZ < mcChunkRef.Blocks.ZDim; iMCChunkInternalZ++)
								} // End for (int iMCChunkInternalY = 0; iMCChunkInternalY < mcChunkRef.Blocks.YDim; iMCChunkInternalY++)
							} // End for (int iMCChunkInternalX = 0; iMCChunkInternalX < mcChunkRef.Blocks.XDim; iMCChunkInternalX++)
						} // End if (mcChunkRef.IsTerrainPopulated)
					} // End if (mcChunkRef != null)
				} // End for (int iMCChunkZ = 0; iMCChunkZ < mcAnvilRegion.ZDim; iMCChunkZ++)
			} // End for (int iMCChunkX  = 0; iMCChunkX < mcAnvilRegion.XDim; iMCChunkX++)
		} // End foreach( Substrate.AnvilRegion mcAnvilRegion in mcAnvilRegionManager )
		
		Debug.Log ("Loaded level: " + _fullMapPath + ", created " + createCount + " blocks.");
		
		_map.AddColliders ();
		
	} // End public void LoadLevel()
}