using UnityEngine;
using System;
using System.Collections.Generic;
namespace OCCubiquity
{   
    public class CBScriptableObject : ScriptableObject
    {
        [SerializeField]
        public SortedDictionary<int, Color32> _cbColorType;  
        [SerializeField]
        public  List<Color32> _cbColors;
        [SerializeField]
        public  List<int> _cbColorIds;
        [SerializeField]
        public List<Point> _cb3DpointsPlusId;
        /// <summary>
        /// Manually paint every block of Cubiquity. 
        /// </summary>
        /// <param name="id">id of a block</param>
        /// <param name="data">a reference type of ColoredCubesVolumeData to access SetVoxel </param>
        /// <param name="cubeColor"> the colored to be painted</param>
        /// <param name="cbobject"> a referece type that contains a set of 3D points from which the coordinate of a block mapped based on id</param>
        public void paintVolume(int id, Cubiquity.ColoredCubesVolumeData data, Color32 cubeColor,CBScriptableObject cbobject)
        {
            if (data != null)
            {
                foreach (Point p in cbobject._cb3DpointsPlusId)
                {
                    if (p.id == id)
                    {
                        data.SetVoxel(p.x, p.y, p.z, (Cubiquity.QuantizedColor)cubeColor); 
                    }
                }
            }
        }
    }
}
