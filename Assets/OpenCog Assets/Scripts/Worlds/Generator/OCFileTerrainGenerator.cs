using System;
using UnityEngine;
using System.Collections.Generic;
using OpenCog.Map;
using OpenCog;
using OpenCog.BlockSet.BaseBlockSet;
using OpenCog.Utilities.Logging;

public class OCFileTerrainGenerator
{
private string _fullMapPath;
private OpenCog.Map.OCMap _map;
private const string _baseMapFolder = "Assets\\Maps\\Resources";
private Dictionary<System.String, Vector3i> chunkList = new System.Collections.Generic.Dictionary<string, Vector3i>();
	
private Dictionary<int, int> mcToOCBlockDictionary = new Dictionary<int, int>()
	{ {0, -1}
	, {1, 4}
	, {2, 1}
	, {3, 0}
	, {4, 5}
	, {5, 12}
	, {6, 15}
	, {7, 6}
	, {8, 8}
	, {9, 8}
	, {10, 25}
	, {11, 25}
	, {12, 3}
	, {16, 7}
	, {17, 11}
	, {18, 13}
	, {26, 26}		
	, {35, 31}
  , {45, 23}
	, {46, 22}
	, {62, 24}
	, {90, 29}
	};
	
public OCFileTerrainGenerator(OpenCog.Map.OCMap map, string mapName)
{
		System.Console.WriteLine(OCLogSymbol.CREATE + this.GetType().ToString ());

        map.MapName = mapName;

        _map = map;
		
        UnityEngine.Object[] objects = Resources.LoadAll(mapName + "/level");
		
        //Debug.Log("Objects Loaded: ");
        foreach(UnityEngine.Object obj in objects)
        {
			System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + obj.ToString() + "was Loaded.");
        }
		
        string dataPath;

//		if(Application.isEditor)
//		{
//			dataPath = UnityEngine.Application.dataPath + "/OpenCog Assets/Maps/StreamingAssets/";
//		}
//		else
        {
                dataPath = Application.streamingAssetsPath;
        }
		
        System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + "Data Path:" + dataPath);
		
        _fullMapPath = System.IO.Path.Combine(dataPath, mapName);
}
	
public void LoadLevel()
{
        int verticalOffset = 85;
		
        //Debug.Log("Loading level at: " + _fullMapPath);
		
        Substrate.AnvilWorld mcWorld = Substrate.AnvilWorld.Create(_fullMapPath);
			
        Substrate.AnvilRegionManager mcAnvilRegionManager = mcWorld.GetRegionManager();
				
        OpenCog.BlockSet.OCBlockSet blockSet = _map.GetBlockSet();
				
        //_map.GetSunLightmap().SetSunHeight(20, 4, 4);

        int createCount = 0;
		
        System.Collections.Generic.Dictionary<int, int> unmappedBlockTypes = new System.Collections.Generic.Dictionary<int, int>();

        //Debug.Log("In LoadLevel, there are " + blockSet.BlockCount + " blocks available.");
		
        foreach(Substrate.AnvilRegion mcAnvilRegion in mcAnvilRegionManager)
        {
                // Loop through x-axis of chunks in this region
                for(int iMCChunkX  = 0; iMCChunkX < mcAnvilRegion.XDim; iMCChunkX++)
                {
                        // Loop through z-axis of chunks in this region.
                        for(int iMCChunkZ = 0; iMCChunkZ < mcAnvilRegion.ZDim; iMCChunkZ++)
                        {
                                // Retrieve the chunk at the current position in our 2D loop...
                                Substrate.ChunkRef mcChunkRef = mcAnvilRegion.GetChunkRef(iMCChunkX, iMCChunkZ);
					
                                if(mcChunkRef != null)
                                {
                                        if(mcChunkRef.IsTerrainPopulated)
                                        {
                                                // Ok...now to stick the blocks in...
							
                                                int iMCChunkY = 0;
							
                                                OCChunk chunk = null;//new OCChunk(_map, new Vector3i(iMCChunkX, iMCChunkY, iMCChunkZ));
                                                OCChunk lastChunk = null;
							
							
                                                Vector3i chunkPos = new Vector3i(mcAnvilRegion.ChunkGlobalX(iMCChunkX), iMCChunkY + verticalOffset, mcAnvilRegion.ChunkGlobalZ(iMCChunkZ));
                                                Vector3i lastChunkPos = Vector3i.zero;
                                                chunk = _map.GetChunkInstance(chunkPos);
							
                                                for(int iMCChunkInternalY = 0; iMCChunkInternalY < mcChunkRef.Blocks.YDim; iMCChunkInternalY++)
                                                {
                                                        if(iMCChunkInternalY / OCChunk.SIZE_Y > iMCChunkY)
                                                        {
                                                                lastChunk = chunk;
                                                                lastChunkPos = chunkPos;
                                                                chunkPos = new Vector3i(mcAnvilRegion.ChunkGlobalX(iMCChunkX), (iMCChunkInternalY + verticalOffset) / OCChunk.SIZE_Y, mcAnvilRegion.ChunkGlobalZ(iMCChunkZ));
                                                                chunk = _map.GetChunkInstance(chunkPos);
                                                        }
								
                                                        for(int iMCChunkInternalX = 0; iMCChunkInternalX < mcChunkRef.Blocks.XDim; iMCChunkInternalX++)
                                                        {
                                                                for(int iMCChunkInternalZ = 0; iMCChunkInternalZ < mcChunkRef.Blocks.ZDim; iMCChunkInternalZ++)
                                                                {
                                                                        int iBlockID = mcChunkRef.Blocks.GetID(iMCChunkInternalX, iMCChunkInternalY, iMCChunkInternalZ);
										
                                                                        if(iBlockID != 0)
                                                                        {
                                                                                Vector3i blockPos = new Vector3i(iMCChunkInternalX, iMCChunkInternalY % OCChunk.SIZE_Y, iMCChunkInternalZ);											
											
                                                                                int ourBlockID = -1;
											
//											switch (iBlockID)
//											{
//											case 3: // Dirt to first grass
//												ourBlockID = 1;
//												break;
//											case 12: // Grass to grass
//												ourBlockID = 1;
//												break;
//											case 13: // Gravel to stone
//												ourBlockID = 4;
//												break;
//											case 1: // Stone to second stone
//												ourBlockID = 5;
//												break;
//											case 16: // Coal ore to fungus
//												ourBlockID = 17;
//												break;
//											case 15: // Iron ore to pumpkin
//												ourBlockID = 20;
//												break;
//											case 9: // Water to water
//												ourBlockID = 8;
//												//Debug.Log ("Creating some water at [" + blockPos.x + ", " + blockPos.y + ", " + blockPos.z + "]");
//												break;
////											case 2:
////												iBlockID = 16;
////												break;
////											case 4:
////												iBlockID = 16;
////												break;
////											case 18:
////												iBlockID = 16;
////												break;
//											default: 
//											{
//												//Debug.Log ("Unmapped BlockID: " + iBlockID);
//												
//												if (!unmappedBlockTypes.ContainsKey (iBlockID))
//												{
//													unmappedBlockTypes.Add (iBlockID, 1);	
//												}
//												else
//												{
//													unmappedBlockTypes[iBlockID] += 1;	
//												}
//												
//												break;
//												}
//											}
											
                                                                                if(mcToOCBlockDictionary.ContainsKey(iBlockID))
                                                                                {
                                                                                        ourBlockID = mcToOCBlockDictionary[iBlockID];
                                                                                } else
                                                                                {
                                                                                        if(!unmappedBlockTypes.ContainsKey(iBlockID))
                                                                                        {
                                                                                                unmappedBlockTypes.Add(iBlockID, 1);	
                                                                                        } else
                                                                                        {
                                                                                                unmappedBlockTypes[iBlockID] += 1;	
                                                                                        }
                                                                                }
											
                                                                                if(ourBlockID != -1)
                                                                                {
                                                                                        OCBlock newBlock = blockSet.GetBlock(ourBlockID);
												
                                                                                        //OCBlockData block = new OpenCog.Map.OCBlockData(newBlock, blockPos);
                                                                                        OCBlockData block = (OCBlockData)OCScriptableObject.CreateInstance<OCBlockData>();
                                                                                        block.Init(newBlock, blockPos);
											
                                                                                        chunk.SetBlock(block, blockPos);
                                                                                        OpenCog.Map.Lighting.OCLightComputer.RecomputeLightAtPosition(_map, blockPos);
												
                                                                                        if(block.block.GetName() == "Battery")
                                                                                        {
                                                                                                GameObject batteryPrefab = OCMap.Instance.BatteryPrefab;
                                                                                                if(batteryPrefab == null)
                                                                                                {
                                                                                                        UnityEngine.Debug.LogWarning(OCLogSymbol.WARN + "OCBuilder::Update, batteryPrefab == null");
                                                                                                } else
                                                                                                {
                                                                                                        GameObject battery = (GameObject)GameObject.Instantiate(batteryPrefab);
                                                                                                        battery.transform.position = blockPos;
                                                                                                        battery.name = "Battery";		
                                                                                                        battery.transform.parent = OCMap.Instance.BatteriesSceneObject.transform;
                                                                                                }
													
                                                                                        }
													
                                                                                        if(block.block.GetName() == "Hearth")
                                                                                        {
                                                                                                GameObject hearthPrefab = OCMap.Instance.HearthPrefab;
                                                                                                if(hearthPrefab == null)
                                                                                                {
																									UnityEngine.Debug.LogWarning(OCLogSymbol.WARN +"OCBuilder::Update, hearthPrefab == null");
                                                                                                } else
                                                                                                {
                                                                                                        GameObject hearth = (GameObject)GameObject.Instantiate(hearthPrefab);
                                                                                                        hearth.transform.position = blockPos;
                                                                                                        hearth.name = "Hearth";		
                                                                                                        hearth.transform.parent = OCMap.Instance.HearthsSceneObject.transform;
                                                                                                }
                                                                                        }
												
                                                                                        createCount += 1;
                                                                                }
                                                                        }
                                                                } // End for (int iMCChunkInternalZ = 0; iMCChunkInternalZ < mcChunkRef.Blocks.ZDim; iMCChunkInternalZ++)
                                                        } // End for (int iMCChunkInternalY = 0; iMCChunkInternalY < mcChunkRef.Blocks.YDim; iMCChunkInternalY++)
								
                                                        string chunkCoord = chunkPos.x + ", " + chunkPos.z;
								
                                                        if(!chunkList.ContainsKey(chunkCoord))
                                                        {
                                                                chunkList.Add(chunkCoord, chunkPos);
                                                        }
								
                                                        if(iMCChunkY < iMCChunkInternalY / OCChunk.SIZE_Y)
                                                        {
                                                                _map.Chunks.AddOrReplace(lastChunk, lastChunkPos);
                                                                _map.UpdateChunkLimits(lastChunkPos);
                                                                _map.SetDirty(lastChunkPos);
                                                                iMCChunkY = iMCChunkInternalY / OCChunk.SIZE_Y;
                                                        }
								
                                                } // End for (int iMCChunkInternalX = 0; iMCChunkInternalX < mcChunkRef.Blocks.XDim; iMCChunkInternalX++)
                                        } // End if (mcChunkRef.IsTerrainPopulated)
                                } // End if (mcChunkRef != null)
                        } // End for (int iMCChunkZ = 0; iMCChunkZ < mcAnvilRegion.ZDim; iMCChunkZ++)
                } // End for (int iMCChunkX  = 0; iMCChunkX < mcAnvilRegion.XDim; iMCChunkX++)
        } // End foreach( Substrate.AnvilRegion mcAnvilRegion in mcAnvilRegionManager )
		
        foreach(Vector3i chunkToLight in chunkList.Values)
        {
                OpenCog.Map.Lighting.OCChunkSunLightComputer.ComputeRays(_map, chunkToLight.x, chunkToLight.z);
                OpenCog.Map.Lighting.OCChunkSunLightComputer.Scatter(_map, null, chunkToLight.x, chunkToLight.z);
        }
		
        foreach(System.Collections.Generic.KeyValuePair<int, int> unmappedBlockData in unmappedBlockTypes)
        {
                UnityEngine.Debug.LogWarning(OCLogSymbol.WARN + "Unmapped BlockID '" + unmappedBlockData.Key + "' found " + unmappedBlockData.Value + " times.");	
        }
		
        Debug.Log(OCLogSymbol.CLEARED + "Loaded level: " + _fullMapPath + ", created " + createCount + " blocks.");
		
        _map.AddColliders();
		
} // End public void LoadLevel()
}