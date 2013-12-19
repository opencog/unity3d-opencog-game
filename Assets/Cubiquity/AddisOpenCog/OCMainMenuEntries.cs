using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

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

        public static List<Color32> CubeColors = new List<Color32>();
        [SerializeField]
        Cubiquity.ColoredCubesVolumeData data;

        void OnEnable()
        {
            data = new Cubiquity.ColoredCubesVolumeData();
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
                CreateCubeVolume();
            }



        }

        private void CreateCubeVolume()
        {
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
            OCCubeVolume.MCSubstrate(data);
        }

    }
}