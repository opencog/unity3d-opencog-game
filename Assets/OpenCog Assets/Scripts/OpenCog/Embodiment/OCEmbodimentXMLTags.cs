
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
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog EmbodimentXMLTags.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCEmbodimentXMLTags : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		

			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	 #region XMLELEMENTS
        public static string FROM_PROXY_ROOT_ELEMENT = "embodiment-msg";
        public static string ENTITY_ELEMENT = "entity";
        public static string POSITION_ELEMENT = "position";
        public static string VELOCITY_ELEMENT = "velocity";
        public static string ROTATION_ELEMENT = "rotation";
        public static string PARAMETER_ELEMENT = "param";
        public static string VECTOR_ELEMENT = "vector";
        public static string ACTION_PLAN_ELEMENT = "oc:action-plan";
        public static string ACTION_ELEMENT = "action";
        public static string ACTION_AVAILABILITY_ELEMENT = "action-availability";
        public static string MAP_INFO_ELEMENT = "map-info";
        public static string TERRAIN_INFO_ELEMENT = "terrain-info";
        public static string BLIP_ELEMENT = "blip";
        public static string AVATAR_SIGNAL_ELEMENT = "avatar-signal";
        public static string OBJECT_SIGNAL_ELEMENT = "object-signal";
        public static string AGENT_SIGNAL_ELEMENT = "agent-signal";
        public static string INSTRUCTION_ELEMENT = "instruction";
        public static string FEELING_ELEMENT = "feeling";
        public static string EMOTIONAL_FEELING_ELEMENT = "oc:emotional-feeling";
        public static string PSI_DEMAND_ELEMENT = "oc:psi-demand"; 
        public static string DEMAND_ELEMENT = "demand";
		public static string SINGLE_ACTION_COMMAND_ELEMENT = "single-action-command";
        #endregion

        #region XMLATTRIBUTES
        public static string ENTITY_ID_ATTRIBUTE = "entity-id";
        public static string DEMAND_ATTRIBUTE = "demand"; 
        public static string NAME_ATTRIBUTE = "name";
        public static string TIMESTAMP_ATTRIBUTE = "timestamp";
        public static string DETECTOR_ATTRIBUTE = "detector";
        public static string ID_ATTRIBUTE = "id";
        public static string TYPE_ATTRIBUTE = "type";
        public static string VALUE_ATTRIBUTE = "value";
        public static string ACTION_PLAN_ID_ATTRIBUTE = "plan-id";
        public static string SEQUENCE_ATTRIBUTE = "sequence";
        public static string AVATAR_ID_ATTRIBUTE = "avatar-id";
        public static string X_ATTRIBUTE = "x";
        public static string Y_ATTRIBUTE = "y";
        public static string Z_ATTRIBUTE = "z";
        public static string PITCH_ATTRIBUTE = "pitch";
        public static string ROLL_ATTRIBUTE = "roll";
        public static string YAW_ATTRIBUTE = "yaw";
        public static string OWNER_ID_ATTRIBUTE = "owner-id";
        public static string OWNER_NAME_ATTRIBUTE = "owner-name";
        public static string OBJECT_ID_ATTRIBUTE = "object-id";
        public static string STATUS_ATTRIBUTE = "status";
        public static string REMOVE_ATTRIBUTE = "remove";
        public static string LENGTH_ATTRIBUTE = "length";
        public static string WIDTH_ATTRIBUTE = "width";
        public static string HEIGHT_ATTRIBUTE = "height";
        public static string EDIBLE_ATTRIBUTE = "edible";
        public static string DRINKABLE_ATTRIBUTE = "drinkable";
        public static string PET_HOME_ATTRIBUTE = "petHome";
        public static string FOOD_BOWL_ATTRIBUTE = "foodBowl";
        public static string WATER_BOWL_ATTRIBUTE = "waterBowl";
		public static string MATERIAL_ATTRIBUTE = "material";

        public static string PRIORITY_ATTRIBUTE = "priority";
        public static string GLOBAL_POS_X_ATTRIBUTE = "global-position-x";
        public static string GLOBAL_POS_Y_ATTRIBUTE = "global-position-y";
        #endregion

        // xml object types 
        public static string PET_OBJECT_TYPE = "pet";
        public static string AVATAR_OBJECT_TYPE = "avatar";
        public static string ORDINARY_OBJECT_TYPE = "object";
		public static string STRUCTURE_OBJECT_TYPE = "structure";
		public static string BLOCK_ENTITY_TYPE = "block-entity";
        public static string UNKNOWN_OBJECT_TYPE = "unknown";

        // known parameter names
        public static string POSITION_PARAMETER_NAME = "position";
        public static string ROTATE_PARAMETER_NAME = "rotate";

        // xml action status (values for <avatar-signal>'s status attribute)  
        public static string DONE_ACTION_STATUS = "done";
        public static string ERROR_ACTION_STATUS = "error";

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class EmbodimentXMLTags

}// namespace OpenCog




