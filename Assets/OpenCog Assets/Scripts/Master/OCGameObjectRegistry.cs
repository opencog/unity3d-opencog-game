using System;
using System.Collections.Generic;

namespace OpenCog.Extensions
{
	/// <summary>
	/// OC game object registry. Used to store GameObjects so they can be looked up by GameObject.(Get)InstanceID.
	/// </summary>
	public class OCGameObjectRegistry : OpenCog.OCSingletonScriptableObject<OCGameObjectRegistry>
	{
		private System.Collections.Generic.Dictionary<int, UnityEngine.GameObject> _gameObjectRegistry;
		
		public OCGameObjectRegistry ()
		{
			_gameObjectRegistry = new Dictionary<int, UnityEngine.GameObject>();
		}
		
		public static OCGameObjectRegistry Instance
		{
			get
			{
				return GetInstance<OCGameObjectRegistry>();
			}
		}
		
		public void RegisterGameObject(UnityEngine.GameObject objectToRegister)
		{
			if (objectToRegister != null)
			{ 
				_gameObjectRegistry.Add (objectToRegister.GetInstanceID(), objectToRegister);	
			} 
			else 
			{
				UnityEngine.Debug.Log ("OCGameObjectRegistry::RegisterGameObject: objectToRegister == null");	
			}
		}
		
		public void RemoveGameObject(int instanceID)
		{
			if (_gameObjectRegistry.ContainsKey(instanceID))
			{
				_gameObjectRegistry.Remove (instanceID);
			}
			else
			{
				UnityEngine.Debug.Log ("OCGameObjectRegistry:RemoveGameObject: No gameObject found with instanceID == " + instanceID + "");
			}
		}
		
		public UnityEngine.GameObject RetrieveGameObject(int instanceID)
		{
			if (_gameObjectRegistry.ContainsKey(instanceID))
			{
				return _gameObjectRegistry[instanceID];
			}
			else
			{
				UnityEngine.Debug.Log ("OCGameObjectRegistry:RetrieveGameObject: No gameObject found with instanceID == " + instanceID + "");
				
				return null;
			}
		}
	}
}

