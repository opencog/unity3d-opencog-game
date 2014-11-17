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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenCog.Attributes;
using ProtoBuf;
using UnityEditor;
using UnityEngine;
using Enum = System.Enum;
using Exception = System.Exception;
using Object = UnityEngine.Object;
using Type = System.Type;
using TypeCode = System.TypeCode;


namespace OpenCog
{

namespace Serialization
{

/// <summary>
/// The OpenCog Property Field.  Provides meta-data utility for storing and
/// retrieving arbitrarily typed properties or fields.  Used to expose
/// MonoBehavior script properties or fields (public or serialized) and to
/// synchronize properties for missing MonoBehavior script references in
/// auto-generated Unity Editors.  Basically an adapter between Unity's
/// serialization interface and C#'s serialization interface.  For ease of
/// reference, the terms Unity Type and C# Type (aka CS Type) will be used
/// to differentiate which serialization interface we're dealing with.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
#endregion
public class OCPropertyField
{

	/////////////////////////////////////////////////////////////////////////////

  #region Private Member Data

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The excluded property field public names.  Some Unity properties and
	/// fields cause strange behavior when we expose them.  Best to exclude them.
	/// </summary>
	private static string[] _excludedPropertyFieldPublicNames =
	{
		"Use GUILayout"
	,	"Enabled"
	,	"Active"
	, "Tag"
	,	"Name"
	,	"Hide Flags"
	};

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Accessors and Mutators

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Gets the serialized Unity Type of the property or field.
	/// </summary>
	/// <value>
	/// The serialized Unity Type of the property or field.  Generic on error.
	/// </value>
	public SerializedPropertyType UnityType
	{
		get
		{
			if(UnityPropertyField != null)
			{
				return UnityPropertyField.propertyType;
			}
			else
			{
				return GetUnityTypeFromCSTypeObject(GetValue());
			}
		}
	}

	/// <summary>
	/// Gets the C# Type of the property or field.
	/// </summary>
	/// <value>
	/// The C# Type of the property or field.  Null on error.
	/// </value>
	public Type CSType
	{
		get
		{
			if(MemberInfo != null)
			{
				if(MemberInfo.MemberType == MemberTypes.Field)
				{
					return (MemberInfo as FieldInfo).FieldType;
				}
				else
				if(MemberInfo.MemberType == MemberTypes.Property)
				{
					return (MemberInfo as PropertyInfo).PropertyType;
				}
				else
				{
					return null;
				}
			}
			else
			if(UnityPropertyField != null)
			{
				return GetCSTypeFromUnityTypePropertyField(UnityPropertyField);
			}
			else
			{
				return null;
			}
		}
	}

	/// <summary>
	/// Gets the public name without the leading m, underscore, and/or lowercase
	/// character.  Also splits apart multiple words with spaces.
	/// </summary>
	/// <value>
	/// A string representing the public name.  Null on error.
	/// </value>
	public string PublicName
	{
		get
		{
			return ObjectNames.NicifyVariableName(this.PrivateName);
		}
	}

	/// <summary>
	/// Gets the private name with the leading m, underscore, and/or lowercase
	/// character.  Should be identical to the declared name in code.
	/// </summary>
	/// <value>
	/// A string representing the private name.  Null on error.
	/// </value>
	public string PrivateName
	{
		get
		{
			if(MemberInfo != null)
			{
				return MemberInfo.Name;
			}
			else
			if(UnityPropertyField != null)
			{
				return UnityPropertyField.name;
			}
			else
			{
				return null;
			}
		}
	}

	/// <summary>
	/// Gets or sets the property' or field's instance.  When we call the get or
	/// set method on this OCPropertyField, we access or mutate the corresponding
	/// property or field on this instance of an object.
	/// </summary>
	/// <value>
	/// The property' or field's instance.
	/// </value>
	public object Instance
	{
		get;
		set;
	}

	/// <summary>
	/// Gets or sets Unity's version of the serialized property.  Will be null
	/// for C# Type properties or fields that we've exposed separately.
	/// </summary>
	/// <value>
	/// The serialized Unity Type property field.
	/// </value>
	public SerializedProperty UnityPropertyField
	{
		get;
		set;
	}

	/// <summary>
	/// Gets or sets the C# Type property or field info.  Will be null when
	/// we're wrapping a SerializedProperty for which we don't know the
	/// MemberInfo or Instance (e.g., _script).
	/// </summary>
	/// <value>
	/// The C# Type property or field member info.
	/// </value>
	public MemberInfo MemberInfo
	{
		get;
		set;
	}

	/// <summary>
	/// Gets the standard OpenCog Binding Flags.  Defines the property fields
	/// that we will seek to access or mutate.
	/// </summary>
	/// <value>
	/// The standard binding flags.
	/// </value>
	public static BindingFlags OCBindingFlags
	{
		get
		{
			return
				BindingFlags.Instance | BindingFlags.Public;
		}
	}

	/// <summary>
	/// Gets the excluded property or field public names.  Some Unity properties
	/// and fields cause strange behavior when we expose them.  Best to exclude
	/// them.
	/// </summary>
	/// <value>
	/// The excluded property or field public names.
	/// </value>
	public static string[] ExcludedPropertyFieldPublicNames
	{
		get
		{
			return _excludedPropertyFieldPublicNames;
		}
	}

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Public Constructors

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.SerializationExtensions.OCPropertyField"/> class.
	/// </summary>
	public OCPropertyField()
	{
		Instance = null;
		MemberInfo = null;
		UnityPropertyField = null;
	}

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.SerializationExtensions.OCPropertyField"/> class.
	/// </summary>
	/// <param name='instance'>
	/// The object instance accessed or mutated by this property field.
	/// </param>
	/// <param name='info'>
	/// The C# Type propery or field info.  Must be
	/// System.Reflection.PropertyInfo or System.Reflection.FieldInfo.
	/// </param>
	public OCPropertyField
	(
		object instance
	, MemberInfo info
	)
	{
		Instance = instance;
		MemberInfo = info;
		UnityPropertyField = null;
	}

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.SerializationExtensions.OCPropertyField"/> class.
	/// </summary>
	/// <param name='instance'>
	/// The object instance accessed or mutated by this property field.
	/// </param>
	/// <param name='unityPropertyField'>
	/// The serialized Unity Type property field.
	/// </param>
	public OCPropertyField
	(
		object instance
	, SerializedProperty unityPropertyField
	)
	{
		Instance = instance;
		UnityPropertyField = unityPropertyField;

		MemberInfo[] memberInfos = null;

		if(Instance != null)
		{
			memberInfos =
				Instance
			. GetType()
			. GetMember(unityPropertyField.name, OCBindingFlags);

			if(memberInfos != null && memberInfos.Count() > 0)
				MemberInfo = memberInfos[0];
			else
				MemberInfo = null;
		}
		else
		{
			MemberInfo = null;
		}
	}

	/// <summary>
	/// Initializes a new instance of the
	/// <see cref="OpenCog.SerializationExtensions.OCPropertyField"/> class.
	/// </summary>
	/// <param name='propertyField'>
	/// The OpenCog Property field we'd like to copy.
	/// </param>
	public OCPropertyField(OCPropertyField propertyField)
	{
		Instance = propertyField.Instance;
		MemberInfo = propertyField.MemberInfo;
		UnityPropertyField = propertyField.UnityPropertyField;
	}

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Public Member Functions

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Gets the property' or field's value.
	/// </summary>
	/// <returns>
	/// The property' or field's value.  Null on error.
	/// </returns>
	public object GetValue()
	{
		if(MemberInfo != null)
		{
			if(MemberInfo.MemberType == MemberTypes.Field)
			{
				return (MemberInfo as FieldInfo).GetValue(Instance);
			}
			else
			if(MemberInfo.MemberType == MemberTypes.Property)
			{
				PropertyInfo info = (PropertyInfo)MemberInfo;
				ParameterInfo[] parameters = info.GetIndexParameters();

				if(parameters == null)
				{
					return info.GetValue(Instance, null);
				}
				else
				{
					return info.GetValue(Instance, parameters);
				}
			}
			else
			{
				return null;
			}
		}
		else
		if(UnityPropertyField != null)
		{
			return UnityPropertyField.objectReferenceValue;
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Sets the property' or field's value.
	/// </summary>
	/// <param name='value'>
	/// The property' or field's value.  Throws on error.
	/// </param>
	public void SetValue(object value)
	{
		if(MemberInfo != null)
		{
			if(MemberInfo.MemberType == MemberTypes.Field)
			{
				(MemberInfo as FieldInfo).SetValue(Instance, value);
			}
			else
			if(MemberInfo.MemberType == MemberTypes.Property)
			{
				(MemberInfo as PropertyInfo).SetValue(Instance, value, null);
			}
			else
			{
				//@TODO: Make an OCException class
				//throw new Exception
				Debug.Log
				("In OCPropertyField.SetValue," +
					"member is neither Field nor Property");
			}
		}
		else
		if(UnityPropertyField != null)
		{
			UnityPropertyField.objectReferenceValue = (Object)value;
		}
		else
		{
			//@TODO: Make an OCException class
			//throw new Exception
			Debug.Log
			("In OCPropertyField.SetValue," +
				"no member info or Unity property field.");
		}
	}

	/// <summary>
	/// Gets the attribute of type T attached to the Property Field.
	/// </summary>
	/// <returns>
	/// The attribute of type T.
	/// </returns>
	/// <param name='propertyField'>
	/// The Property Field.
	/// </param>
	/// <typeparam name='T'>
	/// The type of attribute to find.
	/// </typeparam>
	public T GetAttribute<T>()
	{
		//If there is no field, Unity might find non-fields it wants to display
		//	(like script name).
		if(MemberInfo == null)
		{
			return default(T);
		}
     
		//Findes all attributes of type T attached to the field
		T[] attributes = MemberInfo.GetCustomAttributes(typeof(T), true) as T[];
     
		if(attributes == null || attributes.Length == 0)
		{
			return default(T);
		}
  
		//returns the first instance of the attribute
		return attributes[0];
	}

	public bool IsVisibleForEnumCondition(OCEnumPropertyToggleAttribute enumCondition)
	{
		// If there is no enumCondition, it is allowed to be visible, there is nothing to hide it.
		if(enumCondition == null)
		{
			//Debug.Log("The '[EnumPropertyToggle]' is not attributed to this " + PublicName + " property field.");
			return true;
		}
     
		//Gets the Property or Field for the enum
		OCPropertyField enumPropertyField = null;

		GetPropertyOrField
		(	out enumPropertyField
		, enumCondition.EnumField
		, Instance
		, null
		);
     
		if(enumPropertyField == null)
		{
			//If the field in boolCondition.BooleanField doesn't exist.
			Debug.LogError("The '[EnumPropertyToggle(" + enumCondition.EnumField + ", " + enumCondition.EnumValue + ")]' failed. The field '" + enumCondition.EnumField + "' does not exisit in '" + Instance + "'.");
			return true;
		}
  
		if(enumPropertyField.UnityType != SerializedPropertyType.Enum)
		{
			//If the wanted field is not a enum
			Debug.LogError("The '[EnumPropertyToggle(" + enumCondition.EnumField + ", " + enumCondition.EnumValue + ")]' failed. The field '" + enumCondition.EnumField + "' is not a enum in '" + Instance + "'.");
			return true;
		}

		//Tests if the fields current value is equal to the value it should be to be shown
		if(Enum.Equals(enumPropertyField.GetValue(), enumCondition.EnumValue))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
  
	public bool IsVisibleForBoolCondition(OCBoolPropertyToggleAttribute boolCondition)
	{
		// If there is no boolCondition, it is allowed to be visible, there is nothing to hide it.
		if(boolCondition == null)
		{
			//Debug.Log("The '[BoolPropertyToggle]' is not attributed to this " + PublicName + " property field.");
			return true;
		}

		//Gets the Property or Field for the boolean
		OCPropertyField boolPropertyField = null;

		OCPropertyField.GetPropertyOrField
		(	out boolPropertyField
		, boolCondition.BooleanField
		, Instance
		, null
		);

		if(boolPropertyField == null)
		{
			//If the field in boolCondition.BooleanField doesn't exist.
			Debug.LogError("The '[BoolPropertyToggle(" + boolCondition.BooleanField + ", " + boolCondition.EqualsValue + ")]' failed. The field '" + boolCondition.BooleanField + "' does not exisit in '" + Instance + "'.");
			return true;
		}

		if(boolPropertyField.CSType != typeof(bool))
		{
			//If the wanted field is not a bool
			Debug.LogError("The '[BoolPropertyToggle(" + boolCondition.BooleanField + ", " + boolCondition.EqualsValue + ")]' failed. The field '" + boolCondition.BooleanField + "' is not a type of bool in '" + Instance + "'.");
			return true;
		}
     
		//Finds the current value
		bool currentValue = (bool)boolPropertyField.GetValue();
     
		//Tests if the current Value is equal to the value it should be to be shown
		if(currentValue == boolCondition.EqualsValue)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Translates from a C# Type object to a serialized Unity Type.
	/// </summary>
	/// <returns>
	/// True if there exists a serialized Unity Type that corresponds to the
	/// C# Type object.
	/// </returns>
	/// <param name='obj'>
	/// The C# Type object.
	/// </param>
	/// <param name='unityType'>
	/// If set to <c>true</c>, the serialized Unity Type that corresponds to the
	/// C# Type.
	/// </param>
	public static bool GetUnityTypeFromCSTypeObject
	(
		object obj
	, out SerializedPropertyType unityType
	)
	{
		if(obj == null)
		{
			unityType = SerializedPropertyType.Generic;
			return false;
		}

		unityType = GetUnityTypeFromCSTypeObject(obj);

		if(unityType == SerializedPropertyType.Generic)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	/// <summary>
	/// Translates from a serialized Unity Type property field to a C# Type.
	/// </summary>
	/// <returns>
	/// True if there exists a C# Type that corresponds to the serialized
	/// Unity Type property field.
	/// </returns>
	/// <param name='unityPropertyField'>
	/// The Unity Type property field.
	/// </param>
	/// <param name='csType'>
	/// If set to <c>true</c>, the C# Type that corresponds to the serialized
	/// Unity Type property field.
	/// </param>
	public static bool GetCSTypeFromUnityTypePropertyField
	(
		SerializedProperty unityPropertyField
	, out Type csType
	)
	{
		if(unityPropertyField == null)
		{
			csType = null;
			return false;
		}

		csType = GetCSTypeFromUnityTypePropertyField(unityPropertyField);

		if(csType == null)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	/// <summary>
	/// Gets all properties and fields from a given C# Type object and/or Unity
	/// Type property field.  Returns true if we found any.
	/// </summary>
	/// <returns>
	/// All properties and fields exposed (does not overwrite existing list).
	/// </returns>
	/// <param name='obj'>
	/// The optional C# Type object.
	/// </param>
	/// <param name='unityPropertyField'>
	/// The optional Unity Type property field.
	/// </param>
	/// <param name='allPropertiesAndFields'>
	/// If set to <c>true</c>, all the properties and fields.
	/// </param>
	public static List<OCPropertyField> GetAllPropertiesAndFields
	( object obj = null
	, SerializedProperty unityPropertyField = null
	, OCExposePropertyFieldsAttribute.OCExposure exposure = OCExposePropertyFieldsAttribute.OCExposure.None
	)
	{
		List<OCPropertyField> allPropertyFields = new List<OCPropertyField>();
		Type type = obj.GetType();

		if
		(
			 type == null
		&& unityPropertyField == null
		)
		{
			return null;
		}

		Stack<OCPropertyField> candidates = new Stack<OCPropertyField>();

		if(unityPropertyField != null)
		{
			while(unityPropertyField.NextVisible(true))
			{
				if(unityPropertyField.editable && !unityPropertyField.DuplicateCommand())
				{
					string publicName =
						ObjectNames.NicifyVariableName(unityPropertyField.name);
		
					if(ExcludedPropertyFieldPublicNames.Contains(publicName))
					{
						continue;
					}
		
					candidates.Push
					(
						new OCPropertyField(obj, unityPropertyField.Copy())
					);
				}
			}

			unityPropertyField.Reset();
		}

		if(type != null)
		{
//			Debug.Log("We found a type!");
			MemberInfo[] memberInfos = type.GetMembers(OCBindingFlags);

			foreach(MemberInfo info in memberInfos)
			{
				if(
						(		info.MemberType == MemberTypes.Field
						&& !(info as FieldInfo).IsSpecialName
						)
					||
						(		info.MemberType == MemberTypes.Property
						&& !(info as PropertyInfo).IsSpecialName
						&&	(info as PropertyInfo).CanRead
						&&	(info as PropertyInfo).CanWrite
						)
					)
				{
//					Debug.Log("We found a good member info! :" + info.Name);

					string publicName = ObjectNames.NicifyVariableName(info.Name);
		
					if(ExcludedPropertyFieldPublicNames.Contains(publicName))
					{
						continue;
					}
//					Debug.Log("We found a good candidate! :" + info.Name);
					candidates.Push(new OCPropertyField(obj, info));
				}
			}
		}

		while(candidates.Count > 0)
		{
			OCPropertyField candidate = candidates.Pop();

			OCPropertyField match =
				allPropertyFields.Find(p => p.PublicName == candidate.PublicName);

			if(match != null && match != default(OCPropertyField) && (match.UnityPropertyField != null || match.Instance != null))
			{
				match = candidate;
			}
			else
			{
				allPropertyFields.Insert(0, candidate);
			}
		}

		if(allPropertyFields.Count > 0)
		{
			return allPropertyFields;
		}
		else
		{
			return null;
		}
	}

	public static bool GetPropertyOrField
	(
		out OCPropertyField propertyField
	, string privateName
	, object obj = null
	, SerializedProperty unityPropertyField = null
	)
	{
		if
		(
			 obj == null
		&& unityPropertyField == null
		)
		{
			propertyField = null;
			return false;
		}

		if(obj != null)
		{
			MemberInfo[] memberInfos =
				obj.GetType().GetMember(privateName, OCBindingFlags);

			foreach(MemberInfo info in memberInfos)
			{
				if(
						(		info.MemberType == MemberTypes.Field
						&& !(info as FieldInfo).IsSpecialName
						)
					||
						(		info.MemberType == MemberTypes.Property
						&& !(info as PropertyInfo).IsSpecialName
						&&	(info as PropertyInfo).CanRead
						&&	(info as PropertyInfo).CanWrite
						)
					)
				{
					string publicName = ObjectNames.NicifyVariableName(info.Name);
		
					if(ExcludedPropertyFieldPublicNames.Contains(publicName))
					{
						continue;
					}

					propertyField = new OCPropertyField(obj, info);
					return true;
				}
			}
		}

		if(unityPropertyField != null)
		{
//			List<OCPropertyField> candidatePropertyFields =
//				new List<OCPropertyField>();

			while(unityPropertyField.NextVisible(true))
			{
				if(unityPropertyField.editable
				&& !unityPropertyField.DuplicateCommand())
				{
					string publicName =
						ObjectNames.NicifyVariableName(unityPropertyField.name);
		
					if(ExcludedPropertyFieldPublicNames.Contains(publicName))
					{
						continue;
					}
		
					if(unityPropertyField.name == privateName)
					{
						propertyField =
							new OCPropertyField(obj, unityPropertyField.Copy());

						unityPropertyField.Reset();

						return true;
					}
				}
			}

			unityPropertyField.Reset();
		}

		propertyField = null;
		return false;
	}

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

  #region Private Member Functions

	/////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Translates from a C# Type object to a serialized Unity Type.
	/// </summary>
	/// <returns>
	/// The serialized Unity Type that corresponds to the C# Type object.
	/// Generic on error.
	/// </returns>
	/// <param name='object'>
	/// The C# Type object.
	/// </param>
	private static SerializedPropertyType GetUnityTypeFromCSTypeObject
	(
		object obj
	)
	{
		if(obj == null)
		{
			return SerializedPropertyType.Generic;
		}

		Type csType = obj.GetType();

		// classic switch statements are efficient for plain old types
		switch(Type.GetTypeCode(csType))
		{
		case TypeCode.Boolean:
			return SerializedPropertyType.Boolean;

		case TypeCode.Decimal:
		case TypeCode.Double:
		case TypeCode.Single:
			return SerializedPropertyType.Float;

		case TypeCode.Int16:
		case TypeCode.Int32:
		case TypeCode.Int64:
		case TypeCode.UInt16:
		case TypeCode.UInt32:
		case TypeCode.UInt64:
			return SerializedPropertyType.Integer;

		case TypeCode.String:
			return SerializedPropertyType.String;

		case TypeCode.Char:
			return SerializedPropertyType.Character;

		case TypeCode.Object:
			{
				// use our own nested type switch here
				SerializedPropertyType unityType = SerializedPropertyType.Generic;

				OCTypeSwitch.Do
				(
					obj
				, OCTypeSwitch.Case<AnimationCurve>
					(
						() => unityType = SerializedPropertyType.AnimationCurve
					)
				, OCTypeSwitch.Case<Bounds>
					(
						() => unityType = SerializedPropertyType.Bounds
					)
				, OCTypeSwitch.Case<Color>
					(
						() => unityType = SerializedPropertyType.Color
					)
				, OCTypeSwitch.Case<Gradient>
					(
						() => unityType = SerializedPropertyType.Gradient
					)
				, OCTypeSwitch.Case<LayerMask>
					(
						() => unityType = SerializedPropertyType.LayerMask
					)
				, OCTypeSwitch.Case<Rect>
					(
						() => unityType = SerializedPropertyType.Rect
					)
				, OCTypeSwitch.Case<Vector2>
					(
						() => unityType = SerializedPropertyType.Vector2
					)
				, OCTypeSwitch.Case<Vector3>
					(
						() => unityType = SerializedPropertyType.Vector3
					)
				, OCTypeSwitch.Default
					(
						() => unityType = SerializedPropertyType.ObjectReference
					)
				);

				if(csType.IsEnum)
				{
					unityType = SerializedPropertyType.Enum;
				}

				return unityType;
			}

		case TypeCode.Byte:
		case TypeCode.SByte:
		case TypeCode.DateTime:
		case TypeCode.DBNull:
		case TypeCode.Empty:
		default:
			return SerializedPropertyType.Generic;

		}

//		return SerializedPropertyType.Generic;
	}

	/// <summary>
	/// Translates from a serialized Unity Type property field to a C# Type.
	/// </summary>
	/// <returns>
	/// The C# Type that corresponds to the serialized Unity Type property field.
	/// </returns>
	/// <param name='unityPropertyField'>
	/// The serialized Unity Type property field.
	/// </param>
	private static Type GetCSTypeFromUnityTypePropertyField
	(
		SerializedProperty unityPropertyField
	)
	{
		if(unityPropertyField == null)
		{
			return null;
		}

		SerializedPropertyType unityType = unityPropertyField.propertyType;

		switch(unityType)
		{
		case SerializedPropertyType.AnimationCurve:
			return typeof(AnimationCurve);

		case SerializedPropertyType.Boolean:
			return typeof(bool);

		case SerializedPropertyType.Bounds:
			return typeof(Bounds);

		case SerializedPropertyType.Character:
			return typeof(char);

		case SerializedPropertyType.Color:
			return typeof(Color);

		case SerializedPropertyType.Enum:
			return typeof(Enum);

		case SerializedPropertyType.Float:
			return typeof(float);

		case SerializedPropertyType.Gradient:
			return typeof(Gradient);

		case SerializedPropertyType.Integer:
			return typeof(int);

		case SerializedPropertyType.LayerMask:
			return typeof(LayerMask);

		case SerializedPropertyType.ObjectReference:
			return typeof(Object);

		case SerializedPropertyType.Rect:
			return typeof(Rect);

		case SerializedPropertyType.String:
			return typeof(string);

		case SerializedPropertyType.Vector2:
			return typeof(Vector2);

		case SerializedPropertyType.Vector3:
			return typeof(Vector3);

		default:
			return null;
		}
	}

	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

	#region Other Members

	/////////////////////////////////////////////////////////////////////////////



	/////////////////////////////////////////////////////////////////////////////

  #endregion

	/////////////////////////////////////////////////////////////////////////////

}// class OCPropertyField

}// namespace Serialization

}// namespace OpenCog



