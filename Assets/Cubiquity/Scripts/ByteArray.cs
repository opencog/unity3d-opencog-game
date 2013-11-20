using UnityEngine;

using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Cubiquity
{
	public struct ByteArray
	{
		private ulong data;
		
		public int Length
		{
			get
			{
				return 8;
			}
		}
		
		public byte this[uint i]
		{
			get
			{
				if(i >= Length)
				{
					throw new ArgumentOutOfRangeException("Index out of range");
				}
				
				return (byte)(getEightBitsAt(i * 8));
			}
			set
			{
				if(i >= Length)
				{
					throw new ArgumentOutOfRangeException("Index out of range");
				}
				
				setEightBitsAt(i * 8, value);
			}
		}
		
		private ulong getEightBitsAt(uint offset)
		{
			ulong mask = 0x000000FF;
			ulong result = data;
			result >>= (int)offset;
			result &= mask;
			return result;
		}
		
		private void setEightBitsAt(uint offset, ulong val)
		{
			ulong mask = 0x000000FF;
			mask <<= (int)offset;
			
			data = (uint)(data & (uint)(~mask));
			
			val <<= (int)offset;
			val &= mask;
			
			data |= val;
		}
	}
}