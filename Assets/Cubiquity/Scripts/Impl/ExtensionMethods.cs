using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public static class ExtensionMethods
	{
		// This is a convieniance function because we found we were often calling 'AddComponent' followed by 'GetComponent'.
		// This wraps it into a single line of code, which returns the component if it exists or creates it if it doesn't exist.
	    public static ComponentType GetOrAddComponent<ComponentType>(this GameObject gameObject) where ComponentType : Component
	    {
	        ComponentType component = gameObject.GetComponent<ComponentType>();
			if(component == null)
			{
				component = gameObject.AddComponent<ComponentType>();
			}
			return component;
	    }
		
		// Convieniance method to set the layer recursivly on both the game object and all its children
		public static void SetLayerRecursively(this GameObject gameObject, int layer)
		{
			gameObject.transform.SetLayerRecursively(layer);
		}
		
		// Based on http://answers.unity3d.com/questions/168084/change-layer-of-child.html, though actually
		// I've made this private as I don't think it makes sense to set the layer on the transform. This
		// is just called by the similarly name extension method on the game object.
		private static void SetLayerRecursively(this Transform trans, int layer)
		{
			trans.gameObject.layer = layer;
			foreach (Transform child in trans)
			{
				child.SetLayerRecursively(layer);
			}
		}
	}
}
