
#if UNITY_EDITOR
using UnityEditor;
using OpenCog.EditorExtensions;



using UnityEngine;
using System.Collections;
using OpenCog;

using OpenCog.Extensions;

[ExecuteInEditMode]
[CustomEditor(typeof(OCAnimationEffect))]
public class OCAnimationEffectEditor : OCEditor<OCAnimationEffect>
{
}
#endif




