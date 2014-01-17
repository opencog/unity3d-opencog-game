using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;
namespace OCCubiquity
{
  

    [CustomEditor(typeof(OCSubstrateMap))]
    public class OCSubstrateMapEditor : Editor
    {
        #region private data members
        private SerializedObject _serializeObject;
        private SerializedProperty _serializedProperty;
        private CBScriptableObject scLoadedObject;        
        private GUIContent checkMapName;
        private GUIContent MapName;
        private bool IsGUIEnabled = false;
        private OCSubstrateMapAttributes substrateMapAttribute;
        private string tabs = "\t\t\t\t\t\t\t\t\t";
        private string OnEmpty = "Map Name is empty";
        private string OnNotMatch = "Incorrect Map Name";
        private string OnMatch = "Correct Map Name!!";
        private string _mapName = "MapName";
                                     

        #endregion 

        #region draw Inspector GUI and OnEnable functions
       
        void OnEnable()
        {            
            _serializeObject = new SerializedObject(target);
            _serializedProperty = _serializeObject.FindProperty(_mapName);
            scLoadedObject = ScriptableObject.CreateInstance<CBScriptableObject>();
            MapName = new GUIContent( ObjectNames.NicifyVariableName(_mapName));           
            Type type = typeof(OCSubstrateMap);
            FieldInfo  fieldInfo = type.GetField(_mapName);
            substrateMapAttribute = (OCSubstrateMapAttributes)Attribute.GetCustomAttribute(fieldInfo, typeof(OCSubstrateMapAttributes));
       }

       

        public override void OnInspectorGUI()
        {
             MapName.tooltip = substrateMapAttribute.Tooltip;
            _serializeObject.Update();            
            GUILayout.Space(20);           
           _serializedProperty.stringValue = EditorGUILayout.TextField(MapName, _serializedProperty.stringValue);     
  
           if (_serializedProperty.stringValue.Length!=0)
           {
               OCColoredCubeVolume.MapName = _serializedProperty.stringValue;
               checkMapName = new GUIContent(OnNotMatch);
               IsGUIEnabled = false;
               if (Directory.Exists(OCColoredCubeVolume.Dir))
               {
                   checkMapName = new GUIContent(OnMatch);
                   IsGUIEnabled = true;
               }               
           }
           else if (_serializedProperty.stringValue.Length == 0)
           {
               checkMapName = new GUIContent(OnEmpty);
               IsGUIEnabled = false;
           }

           GUI.enabled = IsGUIEnabled;
           GUILayout.Space(5);
           EditorGUILayout.LabelField(tabs+""+ checkMapName.text,EditorStyles.boldLabel);
            GUILayout.Space(15);
            if (GUILayout.Button("Load Map", GUILayout.Width(150)))
            {
                CreateCubeVolume();
                CreateObjectData();
                CreateAssets();
            }
            _serializeObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
        #endregion 

   

        #region Creates Cubiquity volume
        /// <summary>
        /// Creates Cubiquity region by getting existing Substrate Region dimentions
        /// </summary>
        private void CreateCubeVolume()
        {
            OCColoredCubeVolume OCObject = new OCColoredCubeVolume();
            int regionwidth = OCColoredCubeVolume.GetRegions();
            if (regionwidth > 3)
            {
                regionwidth = 3;
            }
            int width = regionwidth * 32 * 16;
            int height = 256;
            int depth = regionwidth * 32 * 16;
            SubstrateIdToCubiquityColor.data = Cubiquity.ColoredCubesVolumeData.CreateEmptyVolumeData(new Cubiquity.Region(0, 0, 0, width - 1, height - 1, depth - 1));
            Cubiquity.ColoredCubesVolume.CreateGameObject(SubstrateIdToCubiquityColor.data);
            OCObject.MCSubstrate(SubstrateIdToCubiquityColor.data);
            scLoadedObject = SubstrateIdToCubiquityColor.CBObject;
        }       

        #endregion

        #region Creates ScriptableObject for later use
        /// <summary>
        /// Creates CBScriptableObject reference type of ScriptableObject
        /// </summary>
        public void CreateAssets()
        {

            CBScriptableObject scObject = ScriptableObject.CreateInstance<CBScriptableObject>();
            scObject = scLoadedObject;
            AssetDatabase.CreateAsset(scObject, "Assets/Cubiquity/Editor/Resources/Color_Id.asset");
            AssetDatabase.SaveAssets();

        }
        /// <summary>
        /// Creates ColoredCubesVolumeData reference type of ScriptableObject
        /// </summary>
        public void CreateObjectData()
        {
            Cubiquity.ColoredCubesVolumeData data_ = ScriptableObject.CreateInstance<Cubiquity.ColoredCubesVolumeData>();
            data_ =SubstrateIdToCubiquityColor.data;
            AssetDatabase.CreateAsset(data_, "Assets/Cubiquity/Editor/Resources/ColoredCubesVolumeData_Reference.asset");
            AssetDatabase.SaveAssets();
        }


        #endregion
    }
}