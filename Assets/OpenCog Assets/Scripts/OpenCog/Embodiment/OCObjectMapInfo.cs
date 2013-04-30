
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

#region Usings, Namespaces, and Pragmas
using System.Collections;
using System.Collections.Generic;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Embodiment
{

/// <summary>
/// The OpenCog OCObjectMapInfo.
/// </summary>
#region Class Attributes
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCObjectMapInfo : OCMonoBehaviour
	{

		//---------------------------------------------------------------------------

	#region Private Member Data

		//---------------------------------------------------------------------------
		
		/// <summary>
		/// An example variable.  Don't fall into the trap of making all variables
		/// public (I know Unity encourages you to do this).  Instead, make use of
		/// public properties whenever possible.
		/// </summary>
		private int m_ExampleVar;
		
		 #region Constants
		private static readonly float POSITION_DISTANCE_THRESHOLD = 0.05f;
		private static readonly float ROTATION_DELTA = 0.0001f;
		private static readonly float DEFAULT_AVATAR_LENGTH = 1f;
		private static readonly float DEFAULT_AVATAR_WIDTH = 1f;
		private static readonly float DEFAULT_AVATAR_HEIGHT = 3f;


        #endregion
		
		private string m_id;
		private string m_name;
		private string m_type;
		private UnityEngine.Vector3 m_position; // Position of object
		private Vector3Wrapper m_positionWrapper;
		private Rotation m_rotation; // Rotation of object
		private UnityEngine.Vector3 m_velocity; // Velocity of an object, if it is moving.
		private Vector3Wrapper m_velocityWrapper;
		private float m_length, m_width, m_height; // Size of an object.
		private float m_weight; // weight of an object
		private UnityEngine.Vector3 m_startMovePos; // the lastest time start to move position
		private List<OpenCog.Serialization.OCPropertyField> m_properties = new List<OpenCog.Serialization.OCPropertyField> ();
		private VISIBLE_STATUS m_visibility = VISIBLE_STATUS.VISIBLE; // Set the visibility of an object to visible by default.

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Accessors and Mutators

		//---------------------------------------------------------------------------
		
		public string Id {
			get { return m_id; }
			set { m_id = value; }
		}

		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		public string Type {
			get { return m_type; }
			set { m_type = value; }
		}

		// Will be called implicitly when serialized by protobuf-net.
		// Do not invoke this explicitly.
		public Vector3Wrapper PositionWrapper {
			get { return m_positionWrapper; }
			set { Position = value.ToVector3 (); }
		}

		public UnityEngine.Vector3 Position {
			get { return m_position; }
			set {
				m_position = value;
				m_positionWrapper = new Vector3Wrapper (m_position);
			}
		}

		// Will be called implicitly when serialized by protobuf-net.
		// Do not invoke this explicitly.
		public Vector3Wrapper VelocityWrapper {
			get { return m_velocityWrapper; }
			set { Velocity = value.ToVector3 ();  }
		}

		public UnityEngine.Vector3 Velocity {
			get { return m_velocity; }
			set {
				m_velocity = value;
				velocityWrapper = new Vector3Wrapper (m_velocity);
			}
		}

		public Rotation Rotation {
			get { return m_rotation; }
			set { m_rotation = value; }
		}

		public float Length {
			get { return m_length; }
			set { m_length = value; }
		}

		public float Width {
			get { return m_width; }
			set { m_width = value; }
		}

		public float Height {
			get { return m_height; }
			set { m_height = value; }
		}

		public VISIBLE_STATUS Visibility {
			get { return this.visibility; }
			set { this.visibility = value; }
		}

		public List<OpenCog.Serialization.OCPropertyField> Properties {
			get { return properties; }
			set { properties = value; }
		}

		public float Weight {
			get { return weight; }
			set { weight = value; }
		}

		public UnityEngine.Vector3 Size {
			get { return new UnityEngine.Vector3 ((float)this.m_length, (float)this.m_width, (float)this.m_height); }
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Public Member Functions

		//---------------------------------------------------------------------------

		/// <summary>
		/// Called when the script instance is being loaded.
		/// </summary>
		public void Awake ()
		{
			Initialize ();
			OCLogger.Fine (gameObject.name + " is awake.");
		}

		/// <summary>
		/// Use this for initialization
		/// </summary>
		public void Start ()
		{
			OCLogger.Fine (gameObject.name + " is started.");
		}

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		public void Update ()
		{
			OCLogger.Fine (gameObject.name + " is updated.");	
		}
		
		/// <summary>
		/// Reset this instance to its default values.
		/// </summary>
		public void Reset ()
		{
			Uninitialize ();
			Initialize ();
			OCLogger.Fine (gameObject.name + " is reset.");	
		}

		/// <summary>
		/// Raises the enable event when OCObjectMapInfo is loaded.
		/// </summary>
		public void OnEnable ()
		{
			OCLogger.Fine (gameObject.name + " is enabled.");
		}

		/// <summary>
		/// Raises the disable event when OCObjectMapInfo goes out of scope.
		/// </summary>
		public void OnDisable ()
		{
			OCLogger.Fine (gameObject.name + " is disabled.");
		}

		/// <summary>
		/// Raises the destroy event when OCObjectMapInfo is about to be destroyed.
		/// </summary>
		public void OnDestroy ()
		{
			Uninitialize ();
			OCLogger.Fine (gameObject.name + " is about to be destroyed.");
		}
		
		public OpenCog.Serialization.OCPropertyField CheckPropertyExist (string keyStr)
		{
			foreach (OpenCog.Serialization.OCPropertyField ocp in properties) {
				if (ocp.key == keyStr)
					return ocp;
			}
			return null;
		}

		public void AddProperty (string keyStr, string valueStr, System.Type type)
		{
			// Check if property existing
			OpenCog.Serialization.OCPropertyField ocp = CheckPropertyExist (keyStr);
			if (ocp != null) {
				properties.Remove (ocp);
			}
			properties.Add (new OpenCog.Serialization.OCPropertyField (keyStr, valueStr, type));
		}

		public void RemoveProperty (string keyStr)
		{
			// Check if property existing
			OpenCog.Serialization.OCPropertyField ocp = CheckPropertyExist (keyStr);
			if (ocp != null) {
				properties.Remove (ocp);
			}
		}

		public void UpdateProperty (string keyStr, string valueStr, System.Type type)
		{
			AddProperty (keyStr, valueStr, type);
		}

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Private Member Functions

		//---------------------------------------------------------------------------
	
		/// <summary>
		/// Initializes this instance.  Set default values here.
		/// </summary>
		private void Initialize ()
		{

		}
	
		/// <summary>
		/// Uninitializes this instance.  Cleanup refernces here.
		/// </summary>
		private void Uninitialize ()
		{
		}
			
		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	#region Other Members

		//---------------------------------------------------------------------------		

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenCog.OCObjectMapInfo"/> class.  
		/// Generally, intitialization should occur in the Start or Awake
		/// functions, not here.
		/// </summary>
		public OCObjectMapInfo ()
		{
		}
		
		public OCObjectMapInfo (UnityEngine.GameObject go)
		{
			// Get id of a game object
			this.m_id = go.GetInstanceID ().ToString ();
			// Get name
			this.m_name = go.name;
			// TODO: By default, we are using object type.
			this.m_type = EmbodimentXMLTags.ORDINARY_OBJECT_TYPE;

			// Convert from unity coordinate to OAC coordinate.
			this.Position = VectorUtil.ConvertToOpenCogCoord (go.transform.position);
			// Get rotation
			this.Rotation = new Rotation (go.transform.rotation);
			// Calculate the velocity later
			this.Velocity = Vector3.zero;

			// Get size
			if (go.collider != null) {
				// Get size information from collider.
				this.width = go.collider.bounds.size.z;
				this.height = go.collider.bounds.size.y;
				this.length = go.collider.bounds.size.x;
			} else {
				Debug.LogWarning ("No collider for gameobject " + go.name + ", assuming a point.");
				// Set default value of the size.
				this.width = 0.1f;
				this.height = 0.1f;
				this.length = 0.1f;
			}

			if (go.tag == "OCA") {
				// This is an OC avatar, we will use the brain id instead of unity id.
				OCConnector connector = go.GetComponent<OCConnector> () as OCConnector;
				if (connector != null)
					this.m_id = connector.BrainId;
				this.m_type = EmbodimentXMLTags.PET_OBJECT_TYPE;

			} else if (go.tag == "Player") {
				// This is a human player avatar.
				this.m_type = EmbodimentXMLTags.AVATAR_OBJECT_TYPE;
				this.length = OCObjectMapInfo.DEFAULT_AVATAR_LENGTH;
				this.width = OCObjectMapInfo.DEFAULT_AVATAR_WIDTH;
				this.height = OCObjectMapInfo.DEFAULT_AVATAR_HEIGHT;
			}

			// Get weight
			if (go.rigidbody != null) {
				this.weight = go.rigidbody.mass;
			} else {
				this.weight = 0.0f;
			}

			// Get a property manager instance
			OCPropertyManager manager = go.GetComponent<OCPropertyManager> () as OCPropertyManager;
			if (manager != null) {
				// Copy all OC properties from the manager, if any.
				foreach (OpenCog.Serialization.OCPropertyField ocp in manager.propertyList) {
					this.AddProperty (ocp.Key, ocp.value, ocp.valueType);
				}
			}

			this.AddProperty ("visibility-status", "visible", PropertyType.STRING);
			this.AddProperty ("detector", "true", PropertyType.BOOL);
			
			string goName = go.name;
			if (go.name.Contains ("("))
				goName = go.name.Remove (go.name.IndexOf ('('));

			this.AddProperty ("class", goName, PropertyType.STRING);
		}
		
		public static OCObjectMapInfo CreateTerrainMapInfo (Chunk chunk, uint x, uint y, uint z, BlockData blockData)
		{
			// Construct a unique block name.
			string blockName = "CHUNK_" + chunk.X + "_" + chunk.Y + "_" + chunk.Z +
                               "_BLOCK_" + x + "_" + y + "_" + z;
			OCObjectMapInfo mapinfo = new OCObjectMapInfo ();
			mapinfo.Height = 1;
			mapinfo.Width = 1;
			mapinfo.Length = 1;
			mapinfo.Type = EmbodimentXMLTags.STRUCTURE_OBJECT_TYPE;
			mapinfo.Id = blockName;
			mapinfo.Name = blockName;
			Vector3 pos = new Vector3 (chunk.X * chunk.Width + x, chunk.Z * chunk.Depth + z, chunk.Y * chunk.Height + y);
			pos = VectorUtil.ConvertToCentralCoord (pos, mapinfo.Size);
			mapinfo.Velocity = Vector3.zero;
			mapinfo.Position = pos;
			Rotation rot = new Rotation (0, 0, 0);
			mapinfo.Rotation = rot;

			// Add block properties
			mapinfo.AddProperty ("class", "block", PropertyType.STRING);
			mapinfo.AddProperty ("visibility-status", "visible", PropertyType.STRING);
			mapinfo.AddProperty ("detector", "true", PropertyType.BOOL);
			mapinfo.AddProperty (EmbodimentXMLTags.MATERIAL_ATTRIBUTE, blockData.GetType().ToString(), PropertyType.STRING);
			//mapinfo.AddProperty("color_name", "green", PropertyType.STRING);
			return mapinfo;
		}
		
		/// <summary>
		/// Since protobuf-net requires a data contract declaration on fields of 
		/// class that are to be serialized, we need to wrap the built-in types of
		/// Unity3D with manual data contract declaration.
		/// </summary>
	    public class Vector3Wrapper
		{
			public float x;
			public float y;
			public float z;

			// Constructor from a Unity3D Vector3
			public Vector3Wrapper (UnityEngine.Vector3 vec)
			{
				x = vec.x;
				y = vec.y;
				z = vec.z;
			}

			// Dummy constructor
			public Vector3Wrapper ()
			{
				x = 0.0f;
				y = 0.0f;
				z = 0.0f;
			}

			public UnityEngine.Vector3 ToVector3 ()
			{
				return new UnityEngine.Vector3 (x, y, z);
			}
		}

		public enum VISIBLE_STATUS
		{
			VISIBLE = 0,
			INVISIBLE = 1,
			UNKNOWN = 2
		};

		//---------------------------------------------------------------------------

	#endregion

		//---------------------------------------------------------------------------

	}// class OCObjectMapInfo

}// namespace OpenCog




