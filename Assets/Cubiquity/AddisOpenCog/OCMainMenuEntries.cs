using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace OCCubiquity
{
    public class OCMainMenuEntries : EditorWindow
    {
        [MenuItem("GameObject/Create Other/Cubiquity Volume")]
        static void Init()
        {
            OCMainMenuEntries window = (OCMainMenuEntries)EditorWindow.GetWindow<OCMainMenuEntries>();
            window.position = new Rect(100, 100, 500, 500);
        }

        Cubiquity.ColoredCubesVolumeData data;
        CBScriptableObject scLoadedObject;
        bool isCreated = false;
        Color32 ForAllColors = Color.green;

        void OnEnable()
        {
            data = ScriptableObject.CreateInstance<Cubiquity.ColoredCubesVolumeData>();
            scLoadedObject = ScriptableObject.CreateInstance<CBScriptableObject>();
            scLoadedObject = loadObject();
            if (scLoadedObject != null)
            {
                if (!Object.ReferenceEquals(data, LoadDataObject()))
                {
                    data = LoadDataObject();
                }
            }
        }
        void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Map Name"), GUILayout.Width(80));
            OCCubeVolume.MapName = GUILayout.TextField(OCCubeVolume.MapName, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Load Map", GUILayout.Width(200)))
            {
                if (scLoadedObject != null)
                {
                    scLoadedObject = null;
                }
                CreateCubeVolume();
                CreateAssets<CBScriptableObject>();
            }

            if (scLoadedObject != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                for (int x = 0; x < scLoadedObject._cbColors.Count; x++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Color Id :" + scLoadedObject._cbColorIds[x]);
                    scLoadedObject._cbColors[x] = EditorGUILayout.ColorField(scLoadedObject._cbColors[x]);
                    EditorGUILayout.EndHorizontal();
                    scLoadedObject.paintVolume(scLoadedObject._cbColorIds[x], data, scLoadedObject._cbColors[x], scLoadedObject);
                }
            }   
        }
        /// <summary>
        /// creates Cubiquity Region
        /// </summary>
        private void CreateCubeVolume()
        {
            isCreated = true;
            OCCubeVolume OCObject = ScriptableObject.CreateInstance<OCCubeVolume>();

            int regionwidth = OCCubeVolume.GetRegions();
            if (regionwidth > 3)
            {
                regionwidth = 3;
            }
            int width = regionwidth * 32 * 16;
            int height = 256;
            int depth = regionwidth * 32 * 16;
            data = Cubiquity.ColoredCubesVolumeData.CreateEmptyVolumeData(new Cubiquity.Region(0, 0, 0, width - 1, height - 1, depth - 1));
            Cubiquity.ColoredCubesVolume.CreateGameObject(data);
            OCObject.MCSubstrate(data);
            scLoadedObject = SubstrateIdToCubiquityColor.CBObject;
            CreateAssetsData();
        }

        /// <summary>
        /// creates .asset objects for Colors
        /// </summary>
        private void CreateAssets<T>() where T : CBScriptableObject
        {
            T scObject = ScriptableObject.CreateInstance<T>();
            scObject._cbColors = scLoadedObject._cbColors;
            scObject._cbColorIds = scLoadedObject._cbColorIds;
            scObject._cb3DpointsPlusId = scLoadedObject._cb3DpointsPlusId;
            AssetDatabase.CreateAsset(scObject, "Assets/Cubiquity/Editor/Resources/Color_Id.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = scObject;
        }
        /// <summary>
        /// saves the object reference to the ColoredCubesVolumeData
        /// os as to use on Editor reloading or reopening and restarting the Scene.
        /// </summary>
        private void CreateAssetsData()
        {
            Cubiquity.ColoredCubesVolumeData data_ = ScriptableObject.CreateInstance<Cubiquity.ColoredCubesVolumeData>();
            data_ = data;
            AssetDatabase.CreateAsset(data_, "Assets/Cubiquity/Editor/Resources/ColoredCubesVolumeData_Reference.asset");
            AssetDatabase.SaveAssets();
        }
        /// <summary>
        /// loading ColoredCubesVolumeData reference type from Resources director.
        /// </summary>
        /// <returns>ColoredCubeVolumeData referece type</returns>
        private Cubiquity.ColoredCubesVolumeData LoadDataObject()
        {
            object _dataObject = Resources.Load("ColoredCubesVolumeData_Reference", typeof(Cubiquity.ColoredCubesVolumeData));
            Cubiquity.ColoredCubesVolumeData dataObject = (Cubiquity.ColoredCubesVolumeData)_dataObject;
            return dataObject;
        }
        /// <summary>
        ///  loading CBScriptableObject reference type from Resources director.
        /// </summary>
        /// <returns>CBScriptableObject reference type</returns>
        private CBScriptableObject loadObject()
        {
            object _cbObject = Resources.Load("Color_Id", typeof(CBScriptableObject));
            CBScriptableObject cbObject = (CBScriptableObject)_cbObject;
            return cbObject;
        }
        /// <summary>
        /// On disable window Editor, save the changes.
        /// </summary>
        void OnDisable()
        {
            if ((isCreated == true) || (scLoadedObject != null))
                CreateAssets<CBScriptableObject>();
        }

    }
}