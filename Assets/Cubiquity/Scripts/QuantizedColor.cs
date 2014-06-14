using UnityEngine;
using System.Collections;

namespace Cubiquity
{	
	/**
	 * Stores an *approximate* color value with a limited bit-depth.
	 *
	 * The QuantizedColor structure is used to represent the color of the cubes in a colored cubes volume. It provides similar
	 * functionality to the standard Unity color classes (Color and Color32) but stores the colors with reduced precision.
	 * This means that if you write a value into one of the color components and then read it back, then the value which 
	 * you get may not be exactly the same as the value you wrote.
	 *
	 * We deliberately implement this unusual behavior because it makes it more likely that adjacent voxels will actually
	 * have the same quantized  color value. This has a couple of benefits for our system:
	 *
	 * There are a couple of reasons why it is desirable for the QuantizedColor class to exhibit this unusual behavior.
	 * 
	 * 1. It reduces the size of each voxel in memory, which can become significant when a volume can contain hundreds
	 * of millions of voxels.
	 * 
	 * 2. It makes it more likely that adjacent voxels will have the same color value. This improves compression of the voxel
	 * data and also improves rendering performance as adjacent voxels with the same color can be combined.
	 *
	 * The effects of quantization may be observed if you try to create smooth gradients, but other than they should
	 * generally not be visible. Precision is sufficient for most purposes and the quantization artifacts are further hidden
	 * by applying noise, lighting, and other special effects.
	 *
	 * The code below shows some ways of creating and initializing a QuantizedColor:
	 *
	 * ...
	 *
	 * Because the QuantizedColor is a structure (rather than a class) you can actually skip the initialization by the 'new'
	 * operator and just get straight to assigning the values:
	 *
	 * ...
	 * 
	 * It is important to remember that a QuantizedColor is passed by value rather than by reference. As such, you should
	 * not use the code below to set a voxel value:
	 *
	 * ...
	 *
	 * But you should do something like the following instead:
	 *
	 * ...
	 */
	public struct QuantizedColor
	{
		private static int MaxInOutValue = byte.MaxValue;
		
		private static int RedMSB = 31;
		private static int RedLSB = 27;		
		private static int GreenMSB = 26;
		private static int GreenLSB = 21;
		private static int BlueMSB = 20;
		private static int BlueLSB = 16;
		private static int AlphaMSB = 15;
		private static int AlphaLSB = 12;
		
		private static int NoOfRedBits = RedMSB - RedLSB + 1;
		private static int NoOfGreenBits = GreenMSB - GreenLSB + 1;
		private static int NoOfBlueBits = BlueMSB - BlueLSB + 1;
		private static int NoOfAlphaBits = AlphaMSB - AlphaLSB + 1;
		
		private static int RedScaleFactor = MaxInOutValue / ((0x01 << NoOfRedBits) - 1);
		private static int GreenScaleFactor = MaxInOutValue / ((0x01 << NoOfGreenBits) - 1);
		private static int BlueScaleFactor = MaxInOutValue / ((0x01 << NoOfBlueBits) - 1);
		private static int AlphaScaleFactor = MaxInOutValue / ((0x01 << NoOfAlphaBits) - 1);
		
	    public uint color;
		
		public QuantizedColor(byte red, byte green, byte blue, byte alpha)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
			this.alpha = alpha;
		}
		
		public static explicit operator QuantizedColor(Color color)
		{
			QuantizedColor quantizedColor = new QuantizedColor();
			quantizedColor.red = (byte)(color.r * 255.0f);
			quantizedColor.green = (byte)(color.g * 255.0f);
			quantizedColor.blue = (byte)(color.b * 255.0f);
			quantizedColor.alpha = (byte)(color.a * 255.0f);
			return quantizedColor;
		}
		
		public static explicit operator QuantizedColor(Color32 color32)
		{
			QuantizedColor quantizedColor = new QuantizedColor();
			quantizedColor.red = color32.r;
			quantizedColor.green = color32.g;
			quantizedColor.blue = color32.b;
			quantizedColor.alpha = color32.a;
			return quantizedColor;
		}
		
		public static explicit operator Color32(QuantizedColor quantizedColor)
		{
			Color32 color32 = new Color32();
			color32.r = quantizedColor.red;
			color32.g = quantizedColor.green;
			color32.b = quantizedColor.blue;
			color32.a = quantizedColor.alpha;
			return color32;
		}
	
	    public byte red
	    {
	        get
			{
				return (byte)(getBits(RedMSB, RedLSB) * RedScaleFactor);
			}
			set
			{
				setBits(RedMSB, RedLSB, (byte)(value / RedScaleFactor));
			}
	    }
		
		public byte green
	    {
	        get
			{
				return (byte)(getBits(GreenMSB, GreenLSB) * GreenScaleFactor);
			}
			set
			{
				setBits(GreenMSB, GreenLSB, (byte)(value / GreenScaleFactor));
			}
	    }
		
		public byte blue
	    {
	        get
			{
				return (byte)(getBits(BlueMSB, BlueLSB) * BlueScaleFactor);
			}
			set
			{
				setBits(BlueMSB, BlueLSB, (byte)(value / BlueScaleFactor));
			}
	    }
		
		public byte alpha
	    {
	        get
			{
				return (byte)(getBits(AlphaMSB, AlphaLSB) * AlphaScaleFactor);
			}
			set
			{
				setBits(AlphaMSB, AlphaLSB, (byte)(value / AlphaScaleFactor));
			}
	    }
		
		uint getBits(int MSB, int LSB)
		{
			int noOfBitsToGet = (MSB - LSB) + 1;

			// Build a mask containing all '0's except for the least significant bits (which are '1's).
			uint mask = uint.MaxValue; //Set to all '1's
			mask = mask << noOfBitsToGet; // Insert the required number of '0's for the lower bits
			mask = ~mask; // And invert

			// Move the desired bits into the LSBs and mask them off
			uint result = (color >> LSB) & mask;

			return result;
		}
		
		void setBits(int MSB, int LSB, uint bitsToSet)
		{
			int noOfBitsToSet = (MSB - LSB) + 1;

			uint mask = uint.MaxValue; //Set to all '1's
			mask = mask << noOfBitsToSet; // Insert the required number of '0's for the lower bits
			mask = ~mask; // And invert
			mask = mask << LSB;

			bitsToSet = (bitsToSet << LSB) & mask;

			color = (color & ~mask) | bitsToSet;
		}
	}
}
