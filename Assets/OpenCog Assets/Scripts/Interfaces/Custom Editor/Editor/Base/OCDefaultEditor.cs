#if UNITY_EDITOR

using UnityEditor;


using UnityEngine;
using System.Collections;
using OpenCog;
using OpenCog.EditorExtensions;

[ExecuteInEditMode]
[CustomEditor(typeof(MonoBehaviour))]
public class OCDefaultEditor : OCEditor<MonoBehaviour>
{
}

#endif




