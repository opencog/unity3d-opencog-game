using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Cubiquity
{
	/**
	 * Represents the combination of materials which a given voxel is composed of.
	 * 
	 * The use of MaterialSets is one of the more unusual and powerful aspects of the Cubiquity voxel engine,
	 * and it is important to their use in order to effectivly modify the TerrainVolume through scripts. For
	 * simplicity we begin by considering how other voxel engines represent materials and then show how the
	 * Cubiquity approach is a logical extension of this.
	 * 
	 * Most voxel terrain engines (including Cubiquity) are based on the Marching Cubes algorithm, in which the
	 * shape of the terrain is defined by an underlying density field. A basic understanding of this algorithm is
	 * useful to get maximum benefit from the system, so you may want to consult the links below before going
	 * further:
	 * 
	 * * Marching cubes links go here...
	 * 
	 * Some voxel terrain engines extend this principle by storing a material identifier (an integer) with each 
	 * voxel, in addition to the required density value. This material identifier is used at runtime to decide
	 * which texture should be applied to different parts of the terrain. However, such a system has a couple of
	 * limitations:
	 * 
	 * * Transitions between adjacent materials are typically quite sharp and visually unappealing.
	 * * Level of detail is difficult because such a representation is not easily filterable.
	 * 
	 * Cubiquity address these shortcomings by adopting a slightly different system. For each voxel we discard
	 * both the density and the material identfier, and instead store a set of material weights. These encode the
	 * contribution of each material to the voxel, and the density value (required for Marching Cubes) is not
	 * stored explicitly but is instead computed on-the-fly as the sum of the these weights.
	 * 
	 * An example should help to make this clearer. Let's say we wish to create a single 'rock' voxel, where rock
	 * has a material identifer of '3' (your actual configuration may differ). Furthermore, lets say our Marching
	 * Cubes implementation uses 8-bit unsigned values for the density. This means the range of density values is
	 * 0-255, and any voxels with a density over the halfway point of 127 will be considered to be solid. A
	 * traditional voxel engine would reresent this as follows:
	 * 
	 * * Image goes here...
	 * 
	 * Whereas Cubiquity will represent it like this:
	 * 
	 * * Image goes here...
	 * 
	 * Materials weights do not have to be just 0 or 255, they can be any value within this range. Let's say that our
	 * voxel is actually on the boundary between rock and another material type such as 'snow'. Most voxel engines cannot
	 * expresses this, but if we give snow an identifer of '1' (for example) then Cubiquity can represent it as follows:
	 * 
	 * * Image goes here...
	 * 
	 * We cannot exactly split 255 in half so actually the rock has a slightly greater contribution in this example,
	 * but this won't be visible. The sum of the material values is 255 so this voxel is still considered to be
	 * completly full.
	 * 
	 * It is often easiest to work with voxels which are completly full or completly empty, but this is not required
	 * and will actually lead to the mesh having a jagged apearance. Partially full voxels can be used to create much
	 * smoother surfaces. To understand why, it can be helpful to consider the problem in 2D:
	 * 
	 * * Images here
	 * 
	 * These images represent the 2D equivalent of a density field and an attempt to extract a contour from them. As
	 * you can see, the contour in the second image is consideraby smoother.
	 * 
	 * Coming back to our 3D voxel representation, a natural question is 'what happens if the sum of the material
	 * weights exceeds 255?'. This cannot occur in a traditional voxel engine because the stored density value would
	 * be limited by it's 8-bit type, but in Cubiquity it would be *possible* to set all material weight to '255' and
	 * thereby cause the summed value to be significantly greater than this. Basically, you should avoid doing this
	 * because it will not behave in an intuitive way.
	 * 
	 * Practical tips for procedurally generating MaterialSet values
	 * 
	 * Some of the concepts outlined above may seem confusing, and they do take some getting used to when writing
	 * code to procedurally generate volumes. You may find it useful to loosly follow the following process when
	 * devising your noise functions for procudural generation.
	 * 
	 * 1) Start by only using a single material weight, and only set it to either '0' or '255'. Your mesh will have
	 * a jagged appearance but at this point you are only interested in defining the general shape of your world.
	 * 
	 * 2) Move to using full range of values (from 0 to 255) but continue to only use a single material weight. During
	 * this stage you are trying to perfect the shape of your world and ensure that you have smooth surfaces where
	 * you want them.
	 * 
	 * 3) Begin working with multiple materials, but only write to a single material for any given voxel. That is,
	 * you might decide that a given voxel is 'rock' and it's neighbour is 'snow', but at this point no voxel is a 
	 * combination of the two. However, the actual value which you write can continue to be thr continuous (0 to 255)
	 * value which you derived in the previous step.
	 * 
	 * 4) (Optional) You may finally decide that some voxel should be composed of multiple materials. From the
	 * previous steps you already know what the sum should be, so you just need to decide how to distribute the values
	 * to give this sum.
	 * 
	 * One additional point to keep in mind is that you may find it easier to work with floating point values with a
	 * threshold centered at zero, rather than a unsigned byte with a theshold at 127. You are welcome to do this and
	 * simply shift/scale/clamp the floating point values just before you write them to the voxel data. This approach 
	 * may have some performance impact when generating, but it is more intuitive so might be a good starting point 
	 * at least.
	 * 
	 * Do not that this is just a suggestion, and you may want to adopt a different workflow. It's worked for us though,
	 * and it's generally good to break the process down into such a series of smaller steps.
	 *
	 */
	public struct MaterialSet
	{		
		public ByteArray weights;
	}
}
