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
        /// <param tooltip="id">id of a block</param>
        /// <param tooltip="data">a reference type of ColoredCubesVolumeData to access SetVoxel </param>
        /// <param tooltip="cubeColor"> the colored to be painted</param>
        /// <param tooltip="cbobject"> a referece type that contains a set of 3D points from which the coordinate of a block mapped based on id</param>
        public static void paintVolume(int id, Color32  cubeColor,CBScriptableObject cbobject)
        {
           
            if (SubstrateIdToCubiquityColor.data!= null)
            {
                foreach (Point p in cbobject._cb3DpointsPlusId)
                {
                    
                        if (p.id == id)
                        {
                            SubstrateIdToCubiquityColor.data.SetVoxel(p.x, p.y, p.z, (Cubiquity.QuantizedColor)cubeColor);
                        }
                    
                }
            }
        }
       
    }
}
