
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
using UnityEngine;
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

namespace OpenCog
{

/// <summary>
/// The OpenCog OCEmotionalExpression.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCEmotionalExpression : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------
	
	private static string _facialTexturePath = "Assets/Models/smallrobot/Materials/";
	// The texture of facial expressions stored in resources should be named by this prefix plus feeling name
	private static string _facialTexturePrefix = "smallrobot_texture_";

	private static string _facialTextureExt = ".TGA";

	private Dictionary<string, UnityEngine.Texture2D> _emotionTextureMap = new Dictionary<string, UnityEngine.Texture2D>();

	private UnityEngine.Transform _face = null;

	private float _showEmotionThreshold = 0.5f;
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	public float ShowEmotionThreshold
	{
		get { return _showEmotionThreshold;}
		set { _showEmotionThreshold = value;}
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	void Start()
	{
		_face = transform.Find("robotG/mainG/head_GEO");
		if(!_face)
		{
			Debug.LogError("Face of the robot is not found");
		}
	}

	public void showEmotionExpression(Dictionary<string, float> feelingValueMap)
	{ 
		/* TODO [BLOCKED]: uncomment this function after new robot expressions are done
        string dominant_feeling = ""; 
        float dominant_feeling_value = 0; 

        // Find dominant feeling and its value
        lock (feelingValueMap)
        {
            foreach (string feeling in feelingValueMap.Keys)
            {
                float value = feelingValueMap[feeling];

                if (dominant_feeling_value <= value) {
                    dominant_feeling = feeling;
                    dominant_feeling_value = value;
                }
            }   
        }// lock  


        if (dominant_feeling_value < showEmotionThreshold)
            dominant_feeling = "normal"; 

        // Get corresponding facial texture, if fails try to load it from resources
        Texture2D tex = null;
        string textureName = facialTexturePrefix + dominant_feeling + facialTextureExt;
        string textureFullPath = facialTexturePath + textureName;

        if (this.emotionTextureMap.ContainsKey(dominant_feeling))
            tex = this.emotionTextureMap[dominant_feeling];
        else {
            tex = (Texture2D)Resources.LoadAssetAtPath(textureFullPath, typeof(Texture2D));
            if (tex)
            {
                this.emotionTextureMap[dominant_feeling] = tex;
                Debug.Log("Texture for " + dominant_feeling + " loaded.");
            }
        } 
    
        // Set facial texture
        if (tex)
            face.gameObject.renderer.material.mainTexture = tex;
        else
            Debug.LogError("Failed to get texture named: " + textureName);          
		  */
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

}// class OCEmotionalExpression

}// namespace OpenCog




