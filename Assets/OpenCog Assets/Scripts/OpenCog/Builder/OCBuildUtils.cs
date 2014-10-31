
/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente			
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#region Usings, Namespaces, and Pragmas

using System.Collections;
using OpenCog.Attributes;
using OpenCog.Extensions;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;
using UnityEngine;
using OpenCog.BlockSet.BaseBlockSet;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Builder
{

/// <summary>
/// The OpenCog OCBuildUtils.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCBuildUtils 
{


	public static UnityEngine.Color GetSmoothVertexLight(OpenCog.Map.OCMap map, Vector3i pos, UnityEngine.Vector3 vertex, OpenCog.BlockSet.BaseBlockSet.OCCubeBlock.CubeFace face) {
		// pos - позиция блока 
		// vertex позиция вершины относительно блока т.е. от -0.5 до 0.5
		int dx = (int)Mathf.Sign( vertex.x );
		int dy = (int)Mathf.Sign( vertex.y );
		int dz = (int)Mathf.Sign( vertex.z );
		
		Vector3i a, b, c, d;
		if(face == OCCubeBlock.CubeFace.Left || face == OCCubeBlock.CubeFace.Right) { // X
			a = pos + new Vector3i(dx, 0,  0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(dx, 0,  dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else 
		if(face == OCCubeBlock.CubeFace.Bottom || face == OCCubeBlock.CubeFace.Top) { // Y
			a = pos + new Vector3i(0,  dy, 0);
			b = pos + new Vector3i(dx, dy, 0);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		} else { // Z
			a = pos + new Vector3i(0,  0,  dz);
			b = pos + new Vector3i(dx, 0,  dz);
			c = pos + new Vector3i(0,  dy, dz);
			d = pos + new Vector3i(dx, dy, dz);
		}
		
		if(map.GetBlock(b).IsAlpha() || map.GetBlock(c).IsAlpha()) {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			Color c4 = GetBlockLight(map, d);
			return (c1 + c2 + c3 + c4)/4f;
		} else {
			Color c1 = GetBlockLight(map, a);
			Color c2 = GetBlockLight(map, b);
			Color c3 = GetBlockLight(map, c);
			return (c1 + c2 + c3)/3f;
		}
	}
	
	public static UnityEngine.Color GetBlockLight(OpenCog.Map.OCMap map, Vector3i pos) {
		Vector3i chunkPos = OpenCog.Map.OCChunk.ToChunkPosition(pos);
		Vector3i localPos = OpenCog.Map.OCChunk.ToLocalPosition(pos);
		float light = (float) map.GetLightmap().GetLight( chunkPos, localPos ) / OpenCog.Map.Lighting.OCSunLightComputer.MAX_LIGHT;
		float sun = (float) map.GetSunLightmap().GetLight( chunkPos, localPos, pos.y ) / OpenCog.Map.Lighting.OCSunLightComputer.MAX_LIGHT;
		return new UnityEngine.Color(light, light, light, sun);
	}


	
	private static UnityEngine.Color ComputeSmoothLight(OpenCog.Map.OCMap map, Vector3i a, Vector3i b, Vector3i c, Vector3i d) {
		if(map.GetBlock(b).IsAlpha() || map.GetBlock(c).IsAlpha()) {
			UnityEngine.Color c1 = GetBlockLight(map, a);
			UnityEngine.Color c2 = GetBlockLight(map, b);
			UnityEngine.Color c3 = GetBlockLight(map, c);
			UnityEngine.Color c4 = GetBlockLight(map, d);
			return (c1 + c2 + c3 + c4)/4f;
		} else {
			UnityEngine.Color c1 = GetBlockLight(map, a);
			UnityEngine.Color c2 = GetBlockLight(map, b);
			UnityEngine.Color c3 = GetBlockLight(map, c);
			return (c1 + c2 + c3)/3f;
		}
	}
			

}// class OCBuildUtils

}// namespace OpenCog




