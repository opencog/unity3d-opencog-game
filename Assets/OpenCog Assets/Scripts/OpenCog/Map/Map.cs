using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("VoxelEngine/Map")]
public class Map : MonoBehaviour
{
	
	[SerializeField]
	private BlockSet blockSet;
	private List3D<Chunk> chunks = new List3D<Chunk> ();
	private SunLightMap sunLightmap = new SunLightMap ();
	private LightMap lightmap = new LightMap ();
	
	public enum PathDirection
	{
		ForwardWalk,
		ForwardRun,
		ForwardClimb,
		ForwardJump,
		ForwardDrop
	};
	
	public void SetBlockAndRecompute (BlockData block, Vector3i pos)
	{
		SetBlock (block, pos);
		
		Vector3i chunkPos = Chunk.ToChunkPosition (pos);
		Vector3i localPos = Chunk.ToLocalPosition (pos);
		
		SetDirty (chunkPos);
		
		if (localPos.x == 0)
			SetDirty (chunkPos - Vector3i.right);
		if (localPos.y == 0)
			SetDirty (chunkPos - Vector3i.up);
		if (localPos.z == 0)
			SetDirty (chunkPos - Vector3i.forward);
		
		if (localPos.x == Chunk.SIZE_X - 1)
			SetDirty (chunkPos + Vector3i.right);
		if (localPos.y == Chunk.SIZE_Y - 1)
			SetDirty (chunkPos + Vector3i.up);
		if (localPos.z == Chunk.SIZE_Z - 1)
			SetDirty (chunkPos + Vector3i.forward);
		
		SunLightComputer.RecomputeLightAtPosition (this, pos);
		LightComputer.RecomputeLightAtPosition (this, pos);
		
		UpdateMeshColliderAfterBlockChange ();
	}
	
//	private bool IsBlockOnChunkEdge (Vector3i blockPositionGlobal)
//	{
//		int chunkXDim = 16;
//		int chunkZDim = 16;
//		
//		if ((blockPositionGlobal.x == 0) || ((blockPositionGlobal.x + 1) % chunkXDim == 0))
//			return true;
//		else if ((blockPositionGlobal.z == 0) || ((blockPositionGlobal.z + 1) % chunkZDim == 0))
//			return true;
//		else 
//			return false;
//		
//	}
	
	private Vector3i Vector3ToVector3i(Vector3 inputVector)
	{
		int iX, iY, iZ;
		
		iX = (int)Mathf.Round (inputVector.x);
		iY = (int)Mathf.Round (inputVector.y);
		iZ = (int)Mathf.Round (inputVector.z);
		return new Vector3i(iX, iY, iZ);
	}
	
	public bool IsPathOpen (Transform characterTransform, float characterHeight, PathDirection intendedDirection)
	{
		bool bPathIsOpen = false;
		
		Vector3i vCharForward = Vector3ToVector3i(characterTransform.forward);
		
		//Debug.Log ("vFeetPosition = [" + vFeetPosition.x + ", " + vFeetPosition.y + ", " + vFeetPosition.z + "]");
		//Debug.Log ("vFeetForwardPosition = [" + vFeetForwardPosition.x + ", " + vFeetForwardPosition.y + ", " + vFeetForwardPosition.z + "]");
		
		Vector3 vFeet = new Vector3 (characterTransform.position.x, characterTransform.position.y, characterTransform.position.z);
				
		vFeet.y -= (characterHeight / 2);
				
		Vector3 vFeetForward = characterTransform.forward + vFeet;
		
		Vector3i viStandingOn = Vector3ToVector3i(vFeet);
		//Debug.Log ("Standing on world block: [" + viStandingOn.x + ", " + viStandingOn.y + ", " + viStandingOn.z + "]");
		
		Vector3i viStandingOnForward = Vector3ToVector3i(vFeetForward);
		//Debug.Log ("Forward of standing on world block: [" + viStandingOnForward.x + ", " + viStandingOnForward.y + ", " + viStandingOnForward.z + "]");
				
		Vector3i viLowerBody = new Vector3i (viStandingOn.x, viStandingOn.y, viStandingOn.z);
		viLowerBody += new Vector3i (0, 1, 0);
		//Debug.Log ("Lower body inhabits world block: [" + viLowerBody.x + ", " + viLowerBody.y + ", " + viLowerBody.z + "]");
		
		Vector3i viUpperBody = new Vector3i (viLowerBody.x, viLowerBody.y, viLowerBody.z);
		viUpperBody += new Vector3i (0, 1, 0);
		//Debug.Log ("Upper body inhabits world block: [" + viUpperBody.x + ", " + viUpperBody.y + ", " + viUpperBody.z + "]");
		
		// Prepare some block vectors to use later.
		Vector3i viOneAboveHead = viUpperBody + Vector3i.up; // The block direct above the upper body
		Vector3i viTwoAboveHead = viOneAboveHead + Vector3i.up; // The block two above the upper body
		
		Vector3i viForwardOneUnder = viStandingOnForward; // The block in front, one down
		Vector3i viForwardKneeHigh = viStandingOnForward + Vector3i.up; // The block in front of the lower body
		Vector3i viForwardChestHigh = viForwardKneeHigh + Vector3i.up; // The block in front of the upper body
		Vector3i viForwardOneAboveHead = viForwardChestHigh + Vector3i.up; // The block one above the block in front of the upper body
		Vector3i viForwardTwoAboveHead = viForwardOneAboveHead + Vector3i.up; // The block two above the block in front of the upper body
		
		Vector3i viTwoForwardKneeHigh = viForwardKneeHigh + vCharForward; // The block two steps ahead, in front of the lower body
		Vector3i viTwoForwardChestHigh = viForwardChestHigh + vCharForward; // The block two steps ahead, in front of the upper body
		
		Vector3i viThreeForwardKneeHigh = viTwoForwardKneeHigh + vCharForward; // The block three steps ahead, in front of the lower body
		Vector3i viThreeForwardChestHigh = viTwoForwardChestHigh + vCharForward; // The block three steps ahead, in front of the upper body
		
		Vector3i viTwoForwardOneUnder = viStandingOnForward + vCharForward; // The block two steps ahead, one down
		Vector3i viThreeForwardOneUnder = viTwoForwardOneUnder + vCharForward; // The block three steps ahead, one down
		
		//Debug.Log ("Forward knee high: [" + viForwardKneeHigh.x + ", " + viForwardKneeHigh.y + ", " + viForwardKneeHigh.z + "]");
		//Debug.Log ("Forward chest high: [" + viForwardChestHigh.x + ", " + viForwardChestHigh.y + ", " + viForwardChestHigh.z + "]");
		//Debug.Log ("Forward one under: [" + viForwardOneUnder.x + ", " + viForwardOneUnder.y + ", " + viForwardOneUnder.z + "]");
		
		
		//Debug.Log ("Forward lower block is: [" + viForwardKneeHigh.x + ", " + viForwardKneeHigh.y + ", " + viForwardKneeHigh.z + "]");
		//Debug.Log ("Forward upper block is: [" + viForwardChestHigh.x + ", " + viForwardChestHigh.y + ", " + viForwardChestHigh.z + "]");
		
		switch (intendedDirection) {
		case PathDirection.ForwardWalk:
			// Requires two clear blocks in front
			if (GetBlock (viForwardKneeHigh).IsEmpty () && GetBlock (viForwardChestHigh).IsEmpty ())
				// And one block under in front
				if (GetBlock (viForwardOneUnder).IsSolid ())
					bPathIsOpen = true;	
				break;
		case PathDirection.ForwardRun:
			// Requires two clear blocks for the next 3 forwards
			if (GetBlock (viForwardKneeHigh).IsEmpty() && GetBlock (viForwardChestHigh).IsEmpty())
				if (GetBlock (viTwoForwardKneeHigh).IsEmpty() && GetBlock (viTwoForwardChestHigh).IsEmpty())
					if (GetBlock (viThreeForwardKneeHigh).IsEmpty() && GetBlock (viThreeForwardChestHigh).IsEmpty())
						if (GetBlock (viForwardOneUnder).IsSolid () && GetBlock (viTwoForwardOneUnder).IsSolid () && GetBlock (viThreeForwardOneUnder).IsSolid ())
							bPathIsOpen = true;
			break;
		case PathDirection.ForwardClimb:
			// Requires a solid block lower front
			if (GetBlock (viForwardKneeHigh).IsSolid())
				// And two empty blocks above that
				if (GetBlock (viForwardChestHigh).IsEmpty () && GetBlock (viForwardOneAboveHead).IsEmpty ())
					bPathIsOpen = true;
			break;
		case PathDirection.ForwardDrop:
			// Requires 3 empty block in front, chest high, knee high and 1 underground
			if (GetBlock (viForwardKneeHigh).IsEmpty() && GetBlock (viForwardChestHigh).IsEmpty () && GetBlock (viForwardOneUnder).IsEmpty ())
				bPathIsOpen = true;
			break;
		case PathDirection.ForwardJump:
			// Requires two empty blocks above, and two empty blocks in front of those
			if (GetBlock (viOneAboveHead).IsEmpty () && GetBlock (viTwoAboveHead).IsEmpty () && GetBlock (viForwardOneAboveHead).IsEmpty () && GetBlock (viForwardTwoAboveHead).IsEmpty ())
				bPathIsOpen = true;
			break;
		default:
			Debug.Log ("Undefined PathDirection in IsPathOpen(basePosition, intendedDirection)");
			break;
		}
		
		//Debug.Log ("Test for PathDirection=" + intendedDirection.ToString () + " yields " + bPathIsOpen);
		
		return bPathIsOpen;
	}
	
	public void SetDirty (Vector3i chunkPos)
	{
		Chunk chunk = GetChunk (chunkPos);
		if (chunk != null)
			chunk.GetChunkRendererInstance ().SetDirty ();
	}
	
	public void AddCollidersSync ()
	{
		Transform[] objects = GetComponentsInChildren<Transform> ();
 
		for (int i = 0; i < objects.Length; i++) {
			if (objects [i].gameObject.renderer) {
				Debug.Log ("We found us a " + objects [i].gameObject.GetType ().ToString ());
				
				MeshFilter myFilter = objects [i].gameObject.GetComponent<MeshFilter> ();
				MeshCollider myCollider = objects [i].gameObject.AddComponent<MeshCollider> ();
				
				myCollider.sharedMesh = null;
				
				myCollider.sharedMesh = myFilter.mesh;
				
				myCollider.enabled = true;
				
				Debug.Log ("i: " + objects [i].name);
				Debug.Log ("Center: " + myCollider.bounds.center.ToString ());
				Debug.Log ("Size: [" + myCollider.bounds.size.x + ", " + myCollider.bounds.size.y + ", " + myCollider.bounds.size.z + "]");
			}
		}
	}
	
	public void UpdateMeshColliderAfterBlockChange ()
	{
		StartCoroutine (StartUpdateMeshColliderAfterBlockChange ());	
	}
	
	IEnumerator StartUpdateMeshColliderAfterBlockChange ()
	{
		Transform[] objects = GetComponentsInChildren<Transform> ();
		
		yield return null;
		
		for (int i = objects.Length -1; i >= 0; i--) {
			if (objects [i].gameObject.renderer) {
				MeshFilter myFilter = objects [i].gameObject.GetComponent<MeshFilter> ();
				MeshCollider myCollider = objects [i].gameObject.GetComponent<MeshCollider> ();

				if (myCollider != null) {

					myCollider.sharedMesh = null;
				
					myCollider.sharedMesh = myFilter.mesh;
				
					//Debug.Log ("Reapplied mesh for " + objects[i].gameObject.GetType ().ToString ());
				}
				
//				Debug.Log ("i: " + objects [i].name);
//				Debug.Log ("Center: " + myCollider.bounds.center.ToString());
//				Debug.Log ("Size: [" + myCollider.bounds.size.x + ", " + myCollider.bounds.size.y + ", " + myCollider.bounds.size.z + "]");
			}
		}
	}
	
	public void AddColliders ()
	{
 
		StartCoroutine (StartAddColliders ());
	}
 
	IEnumerator StartAddColliders ()
	{
		Transform[] objects = GetComponentsInChildren<Transform> ();
		
		yield return null;
 
		for (int i = objects.Length -1; i >= 0; i--) {
			if (objects [i].gameObject.renderer) {
				//Debug.Log("We found us a " + objects[i].gameObject.GetType ().ToString ());
				
				MeshFilter myFilter = objects [i].gameObject.GetComponent<MeshFilter> ();
				MeshCollider myCollider = objects [i].gameObject.AddComponent<MeshCollider> ();
				
				myCollider.sharedMesh = null;
				
				myCollider.sharedMesh = myFilter.mesh;
				
				//Debug.Log ("i: " + objects [i].name);
				//Debug.Log ("Center: " + myCollider.bounds.center.ToString());
				//Debug.Log ("Size: [" + myCollider.bounds.size.x + ", " + myCollider.bounds.size.y + ", " + myCollider.bounds.size.z + "]");
				
			}
		}
	}
	
	public void SetBlock (Block block, Vector3i pos)
	{
		SetBlock (new BlockData (block), pos);
	}

	public void SetBlock (Block block, int x, int y, int z)
	{
		SetBlock (new BlockData (block), x, y, z);
	}
	
	public void SetBlock (BlockData block, Vector3i pos)
	{
		SetBlock (block, pos.x, pos.y, pos.z);
	}

	public void SetBlock (BlockData block, int x, int y, int z)
	{
		Chunk chunk = GetChunkInstance (Chunk.ToChunkPosition (x, y, z));
		if (chunk != null)
			chunk.SetBlock (block, Chunk.ToLocalPosition (x, y, z));
	}
	
	public BlockData GetBlock (Vector3i pos)
	{
		return GetBlock (pos.x, pos.y, pos.z);
	}

	public BlockData GetBlock (int x, int y, int z)
	{
		Chunk chunk = GetChunk (Chunk.ToChunkPosition (x, y, z));
		if (chunk == null)
			return default(BlockData);
		return chunk.GetBlock (Chunk.ToLocalPosition (x, y, z));
	}
	
	public int GetMaxY (int x, int z)
	{
		Vector3i chunkPos = Chunk.ToChunkPosition (x, 0, z);
		chunkPos.y = chunks.GetMax ().y;
		Vector3i localPos = Chunk.ToLocalPosition (x, 0, z);
		
		for (; chunkPos.y >= 0; chunkPos.y--) {
			localPos.y = Chunk.SIZE_Y - 1;
			for (; localPos.y >= 0; localPos.y--) {
				Chunk chunk = chunks.SafeGet (chunkPos);
				if (chunk == null)
					break;
				BlockData block = chunk.GetBlock (localPos);
				if (!block.IsEmpty ())
					return Chunk.ToWorldPosition (chunkPos, localPos).y;
			}
		}
		
		return 0;
	}
	
	private Chunk GetChunkInstance (Vector3i chunkPos)
	{
		if (chunkPos.y < 0)
			return null;
		Chunk chunk = GetChunk (chunkPos);
		if (chunk == null) {
			chunk = new Chunk (this, chunkPos);
			chunks.AddOrReplace (chunk, chunkPos);
		}
		return chunk;
	}

	public Chunk GetChunk (Vector3i chunkPos)
	{
		return chunks.SafeGet (chunkPos);
	}
	
	public List3D<Chunk> GetChunks ()
	{
		return chunks;
	}
	
	public SunLightMap GetSunLightmap ()
	{
		return sunLightmap;
	}
	
	public LightMap GetLightmap ()
	{
		return lightmap;
	}
	
	public void SetBlockSet (BlockSet blockSet)
	{
		this.blockSet = blockSet;
	}

	public BlockSet GetBlockSet ()
	{
		return blockSet;
	}
	
}