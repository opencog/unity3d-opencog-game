
/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente			
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using OpenCog.Utilities.Logging;
using OpenCog.Master;

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using UnityEngine;
using System.Linq;
using OpenCog.Map;
using OpenCog.Actions;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Builder
{

/// <summary>
/// The OpenCog OCBuilder.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCBuilder : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
		
	[UnityEngine.SerializeField]
	private UnityEngine.GameObject
		_cursor;

	private UnityEngine.Transform _cameraTrans;

	private CharacterController _characterCollider;

	private OpenCog.Map.OCMap _map;

	private OpenCog.BlockSet.BaseBlockSet.OCBlock _selectedBlock;

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------

	public OpenCog.BlockSet.BaseBlockSet.OCBlock SelectedBlock
	{
		get { return _selectedBlock;}
		set { _selectedBlock = value;}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Called when the script instance is being loaded.
	/// </summary>
	public void Awake()
	{
		Initialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is awake.");
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		if(UnityEngine.Screen.showCursor)
		{
			return;
		}
		
		if(UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftControl))
		{
			Vector3i? point = GetCursor(false);
			if(point.HasValue)
			{
				byte sun = _map.GetSunLightmap().GetLight(point.Value);
				byte light = _map.GetLightmap().GetLight(point.Value);
				Debug.Log(OCLogSymbol.DETAILEDINFO + "Sun " + " " + sun + "  Light " + light);
			}
		}
		
		if(UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.RightControl))
		{
			Vector3i? point = GetCursor(true);
			if(point.HasValue)
			{
				byte sun = _map.GetSunLightmap().GetLight(point.Value);
				byte light = _map.GetLightmap().GetLight(point.Value);
				Debug.Log(OCLogSymbol.DETAILEDINFO + "Sun " + sun + "  Light " + light);
			}
		}
		
		if(UnityEngine.Input.GetMouseButtonDown(0))
		{
			Vector3i? point = GetCursor(true);
			if(point.HasValue)
			{
//				Vector3i above = point.Value;
//				above.y += 1;
//				OCBlockData blockAbove = _map.GetBlock(above);
//				OCBlockData blockData = _map.GetBlock (point.Value);
//				if(blockAbove.IsEmpty() && !blockData.IsEmpty())
//				{
//					_map.SetBlockAndRecompute(blockData, above);
//					_map.SetBlockAndRecompute(OCBlockData.CreateInstance<OCBlockData>().Init(null, point.Value), point.Value);
//				}
//				else

				Debug.Log(OCLogSymbol.DEBUG + "DeleteSelectedVoxel called from CreateBlockEffect");
				GameManager.world.voxels.DeleteSelectedVoxel(point.Value);
			}
		}
		
		if(UnityEngine.Input.GetMouseButtonDown(1))
		{
			Vector3i? point = GetCursor(false);
			if(point.HasValue)
			{
				bool empty = !BlockCharacterCollision.GetContactBlockCharacter(point.Value, transform.position, gameObject.GetComponent<CharacterController>()).HasValue;
				if(empty)
				{
					Debug.Log(OCLogSymbol.DEBUG + "AddSelectedVoxel called from OCBuilder");
					GameManager.world.voxels.AddSelectedVoxel(point.Value, -transform.forward, _selectedBlock);
					
						
					OCGoalController[] goalControllers = (OCGoalController[])GameObject.FindObjectsOfType(typeof(OCGoalController));

					foreach(OCGoalController goalController in goalControllers)
					{
						if(goalController.GoalBlockType == _selectedBlock)
						{
							Vector3 sourcePos = goalController.gameObject.transform.position;
							Vector3 oldDistanceVec = ((Vector3)goalController.GoalBlockPos) - sourcePos;
							Vector3 newDistanceVec = point.Value - sourcePos;
							if(newDistanceVec.sqrMagnitude < oldDistanceVec.sqrMagnitude)
							{
								goalController.GoalBlockPos = point.Value;	
								goalController.MoveTargetsIfNecessary();
							}
						}
					}	
				}
			}
		}
		
		Vector3i? cursor = GetCursor(true);
		_cursor.SetActive(cursor.HasValue);
		if(cursor.HasValue)
		{
			_cursor.transform.position = cursor.Value;
		}
		//System.Console.WriteLine(OCLogSymbol.DETAILEDINFO +gameObject.name + " is updated.");	
	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is reset.");	
	}

	/// <summary>
	/// Raises the enable event when OCBuilder is loaded.
	/// </summary>
	public void OnEnable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when OCBuilder goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCBuilder is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		System.Console.WriteLine(OCLogSymbol.DETAILEDINFO + gameObject.name + " is about to be destroyed.");
	}



	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	private void Initialize()
	{
		_cameraTrans = transform.GetComponentInChildren<UnityEngine.Camera>().transform;
		_characterCollider = GetComponent<CharacterController>();
		_map = OpenCog.Map.OCMap.Instance;//(OpenCog.Map.OCMap)UnityEngine.GameObject.FindObjectOfType(typeof(OpenCog.Map.OCMap));
		_cursor = (UnityEngine.GameObject)UnityEngine.GameObject.Instantiate(_cursor);
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{
	}

	private Vector3i? GetCursor(bool inside)
	{
		UnityEngine.Ray ray = new UnityEngine.Ray(_cameraTrans.position, _cameraTrans.forward);
		UnityEngine.Vector3? point = MapRayIntersection.Intersection(_map, ray, 10);
		if(point.HasValue)
		{
			UnityEngine.Vector3 pos = point.Value;
			if(inside)
			{
				pos += ray.direction * 0.01f;
			}
			if(!inside)
			{
				pos -= ray.direction * 0.01f;
			}
			int posX = UnityEngine.Mathf.RoundToInt(pos.x);
			int posY = UnityEngine.Mathf.RoundToInt(pos.y);
			int posZ = UnityEngine.Mathf.RoundToInt(pos.z);
			return new Vector3i(posX, posY, posZ);
		}
		return null;
	}
	
	
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCBuilder"/> class.  
	/// Generally, intitialization should occur in the Start or Awake
	/// functions, not here.
	/// </summary>
	public OCBuilder()
	{
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCBuilder

}// namespace OpenCog




