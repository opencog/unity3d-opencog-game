using UnityEngine;
using Behave.Assets;

[System.Serializable]
public class BehaveAsset : ScriptableObject, IBehaveAsset
{
	[HideInInspector]
	public byte[] data;
	
	public byte[] Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}
	
	
	public string Path
	{
		get
		{
			return "";
		}
	}
}