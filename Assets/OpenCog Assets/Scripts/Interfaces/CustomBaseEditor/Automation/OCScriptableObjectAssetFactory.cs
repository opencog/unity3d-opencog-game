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
using AssetDatabase = UnityEditor.AssetDatabase;
using EditorUtility = UnityEditor.EditorUtility;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using ScriptableObject = UnityEngine.ScriptableObject;
using Selection = UnityEditor.Selection;
using Serializable = System.SerializableAttribute;
using System.IO;


//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog
{

/// <summary>
/// The OpenCog OCScriptableObjectAssetFactory.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
    
#endregion
public static class OCScriptableObjectAssetFactory
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

	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAsset<T>() where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();
 
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if(path == "")
		{
			path = "Assets";
		}
		else
		if(Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}
 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
 
		AssetDatabase.CreateAsset(asset, assetPathAndName);
 
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAsset<T>(T asset) where T : ScriptableObject
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if(path == "")
		{
			path = "Assets";
		}
		else
		if(Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

		AssetDatabase.CreateAsset(asset, assetPathAndName);

		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>
	public static void CreateAssets(OCDelegate[] assets)
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if(path == "")
		{
			path = "Assets";
		}
		else
		if(Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
		}

		path += "/OpenCog Assets/OtherAssets/ActionConditions/";

		foreach(OCDelegate asset in assets)
		{
			string basePath = path + asset.Name + " " + typeof(OCDelegate).Name + ".asset";
			OCDelegate existingAsset = (OCDelegate)AssetDatabase.LoadAssetAtPath(basePath, typeof(OCDelegate));

			if(existingAsset != null)
			{
				AssetDatabase.DeleteAsset(basePath);
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(basePath);
			AssetDatabase.CreateAsset(asset, assetPathAndName);
		}


		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = assets[0];
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

    

	//---------------------------------------------------------------------------

  #endregion

	//---------------------------------------------------------------------------

}// class OCScriptableObjectAssetFactory

}// namespace OpenCog




