
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
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using OpenCog.Attributes;
using OpenCog.Extensions;
using Activator = System.Activator;
using Delegate = System.Delegate;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using SerializableAttribute = System.SerializableAttribute;
using Type = System.Type;
using SerializeField = UnityEngine.SerializeField;
using MenuItem = UnityEditor.MenuItem;
using ScriptableObject = UnityEngine.ScriptableObject;
using OpenCog.Actions;
using System.Collections.Generic;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCSerializeDelegate.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
    
#endregion
public class OCDelegate : OCScriptableObject, ISerializable
{

	//---------------------------------------------------------------------------

  #region Private Member Data

	//---------------------------------------------------------------------------

	[SerializeField]
	private string _Name = "";

	private Delegate _Delegate;
            
	//---------------------------------------------------------------------------

  #endregion

	//---------------------------------------------------------------------------

  #region Accessors and Mutators

	//---------------------------------------------------------------------------

	public string Name
	{
		get {return _Name;}
		set {_Name = value;}
	}

	public Delegate Delegate
	{
		get { return _Delegate;}
	}
            
	//---------------------------------------------------------------------------

  #endregion

	//---------------------------------------------------------------------------

  #region Public Member Functions

	//---------------------------------------------------------------------------

  public OCDelegate Initialize(Delegate del)
	{
		_Delegate = del;
		_Name = del.Method.Name;
		return this;
	}

	public OCDelegate Initialize(SerializationInfo info, StreamingContext context)
	{
		Type delType = (Type)info.GetValue("delegateType", typeof(Type));

		//If it's a "simple" delegate we just read it straight off
		if(info.GetBoolean("isSerializable"))
		{
			_Delegate = (Delegate)info.GetValue("delegate", delType);
			_Name = _Delegate.Method.Name;
		}

        //otherwise, we need to read its anonymous class
		else
		{
			MethodInfo method = (MethodInfo)info.GetValue("method", typeof(MethodInfo));

			OCAnonymousClassWrapper w = 
                (OCAnonymousClassWrapper)info.GetValue("class", typeof(OCAnonymousClassWrapper));

			_Delegate = Delegate.CreateDelegate(delType, w.obj, method);
			_Name = _Delegate.Method.Name;
		}

		return this;
	}

//	[MenuItem("Assets/Create/OpenCog/Delegates")]
	public static void CreateAssets()
	{
		List<OCDelegate> delegates = new List<OCDelegate>();

		MethodInfo[] delegateMethods = typeof(OCAction).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

		foreach(MethodInfo dmi in delegateMethods)
		{
			ParameterInfo[] paramInfo = dmi.GetParameters();
			if(paramInfo != null && paramInfo.Length == 2 && paramInfo[0].ParameterType == typeof(OCAction))
			{
				OCDelegate del = ScriptableObject.CreateInstance<OCDelegate>()
					.Initialize(Delegate.CreateDelegate(typeof(OCAction.OCActionCondition), dmi));
				delegates.Add(del);
			}
		}

		//OCScriptableObjectAssetFactory.CreateAsset<OCDelegate>();

		OCScriptableObjectAssetFactory.CreateAssets(delegates.ToArray());
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("delegateType", _Delegate.GetType());

		//If it's an "simple" delegate we can serialize it directly
		if((_Delegate.Target == null ||
            _Delegate.Method.DeclaringType.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0) &&
            _Delegate != null)
		{
			info.AddValue("isSerializable", true);
			info.AddValue("delegate", _Delegate);
		}

    //otherwise, serialize anonymous class
		else
		{
			info.AddValue("isSerializable", false);
			info.AddValue("method", _Delegate.Method);
			info.AddValue("class", 
                new OCAnonymousClassWrapper(_Delegate.Method.DeclaringType, _Delegate.Target));
		}
	}

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

	[Serializable]
  class OCAnonymousClassWrapper : ISerializable
	{
		internal OCAnonymousClassWrapper(Type bclass, object bobject)
		{
			this.type = bclass;
			this.obj = bobject;
		}

		internal OCAnonymousClassWrapper(SerializationInfo info, StreamingContext context)
		{
			Type classType = (Type)info.GetValue("classType", typeof(Type));
			obj = Activator.CreateInstance(classType);

			foreach(FieldInfo field in classType.GetFields())
			{
				//If the field is a delegate
				if(typeof(Delegate).IsAssignableFrom(field.FieldType))
				{
					field.SetValue(obj,
                        ((OCDelegate)info.GetValue
				(field.Name, typeof(OCDelegate)))
                            .Delegate);
				}
        //If the field is an anonymous class
				else
				if(!field.FieldType.IsSerializable)
				{
					field.SetValue(obj,
                        ((OCAnonymousClassWrapper)info.GetValue
				(field.Name, typeof(OCAnonymousClassWrapper)))
                            .obj);
				}
        //otherwise
				else
				{
					field.SetValue(obj, info.GetValue(field.Name, field.FieldType));
				}
			}
		}

		void ISerializable.GetObjectData
		(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("classType", type);

			foreach(FieldInfo field in type.GetFields())
			{
				//See corresponding comments above
				if(typeof(Delegate).IsAssignableFrom(field.FieldType))
				{
					info.AddValue(field.Name, OCScriptableObject.CreateInstance<OCDelegate>().Initialize
					((Delegate)field.GetValue(obj)));
				}
				else
				if(!field.FieldType.IsSerializable)
				{
					info.AddValue(field.Name, new OCAnonymousClassWrapper
				(field.FieldType, field.GetValue(obj)));
				}
				else
				{
					info.AddValue(field.Name, field.GetValue(obj));
				}
			}
		}

		public Type type;

		public object obj;
	}

	//---------------------------------------------------------------------------

  #endregion

	//---------------------------------------------------------------------------

}// class OCSerializeDelegate

}// namespace OpenCog




