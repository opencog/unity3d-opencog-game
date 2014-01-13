using UnityEngine;
using UnityEditor;
using System.Linq;
using ColoredCubesVolumeData = Cubiquity.ColoredCubesVolumeData;
namespace OCCubiquity
{
    [CustomEditor(typeof(Cubiquity.ColoredCubesVolume))]
    public class OCColoredCubesVolumeInspector : Cubiquity.ColoredCubesVolumeInspector
    {
        #region private data members

      
        private CBScriptableObject scLoadedObject;
        private Color32 ForAllColors = Color.green;

        #endregion

        #region OnEnable and OnInspectorGUI functions
       public void OnEnable()
        {
            base.OnEnable();
            scLoadedObject = ScriptableObject.CreateInstance<CBScriptableObject>();
            scLoadedObject = loadObject();
            if (scLoadedObject != null)
            {
                if (!Object.ReferenceEquals(SubstrateIdToCubiquityColor.data, LoadDataObject()))
                {
                    SubstrateIdToCubiquityColor.data = LoadDataObject();
                }
            }          

        }
        
        public override void OnInspectorGUI()
        {
           
            base.OnInspectorGUI();
            GUILayout.Space(20);
            if (scLoadedObject != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                ForAllColors = EditorGUILayout.ColorField(ForAllColors, GUILayout.Width(200));
                if (GUILayout.Button("Fill All"))
                {
                    scLoadedObject._cbColors = scLoadedObject._cbColors.Select(C => C = ForAllColors).ToList();
                                       

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                for (int x = 0; x < scLoadedObject._cbColors.Count; x++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Color Id :" + scLoadedObject._cbColorIds[x]);
                    scLoadedObject._cbColors[x] = EditorGUILayout.ColorField(scLoadedObject._cbColors[x]);
                    EditorGUILayout.EndHorizontal();
                    CBScriptableObject.paintVolume(scLoadedObject._cbColorIds[x],scLoadedObject._cbColors[x], scLoadedObject);
                }

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(target);
                    EditorUtility.SetDirty(scLoadedObject);
                    EditorUtility.SetDirty(SubstrateIdToCubiquityColor.data);
                   
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// loads ScriptableObject  reference type from Source folder
        /// </summary>
        /// <returns> returns ColoredCubesVolumeData reference type</returns>
        private ColoredCubesVolumeData LoadDataObject()
        {
            object _dataObject = Resources.Load("ColoredCubesVolumeData_Reference", typeof(ColoredCubesVolumeData));
            ColoredCubesVolumeData dataObject = (ColoredCubesVolumeData)_dataObject;
            return dataObject;
        }
       /// <summary>
        /// loads ScriptableObject  reference type from Source folder based on the given name
       /// </summary>
        /// <returns> returns CBScriptableObject reference type </returns>
        private CBScriptableObject loadObject()
        {
            object _colorObject = Resources.Load("Color_Id", typeof(CBScriptableObject));
            CBScriptableObject colorObject = (CBScriptableObject)_colorObject;
            return colorObject;
        }

        #endregion
    }
}
