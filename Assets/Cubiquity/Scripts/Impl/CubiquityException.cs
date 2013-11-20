using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public class CubiquityException: System.Exception
	{
	   public CubiquityException()
	   {
	   }
	
	   public CubiquityException(string message)
			:base(message)
	   {
	   }
		
		public CubiquityException(string message, CubiquityException innerException)
			:base(message, innerException)
	   {
	   }
	}
}
