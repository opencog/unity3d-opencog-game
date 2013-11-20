using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Cubiquity
{
	public class Installation
	{
		public static void ValidateAndFix()
		{
			if( (Application.platform != RuntimePlatform.WindowsEditor) &&
				(Application.platform != RuntimePlatform.WindowsPlayer) )
			{
				Debug.LogError("We're sorry, but Cubiquity for Unity3D is currently only supported on Windows. We hope to support more platfors in the future.");
			}
			
			string fileName = "CubiquityC.dll";
	        string sourcePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Cubiquity");
	        string destPath =  System.IO.Path.Combine(Application.dataPath, "..");
	
	        // Use Path class to manipulate file and directory paths. 
	        string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
	        string destFile = System.IO.Path.Combine(destPath, fileName);
			
			if(System.IO.File.Exists(destFile))
			{
				byte[] sourceChecksum = GetChecksum(sourceFile);
				byte[] destChecksum = GetChecksum(destFile);
				
				bool checksumsMatch = true;
				for(int i = 0; i < sourceChecksum.Length; i++)
				{
					if(sourceChecksum[i] != destChecksum[i])
					{
						checksumsMatch = false;
						break;
					}
				}
				
				if(!checksumsMatch)
				{
					Debug.LogWarning("The file " + fileName + " in the project root folder appears to be the wrong version (or corrupt). " 
						+ "We have a copy in '" + sourcePath + "' and will use this version to overwrite the project root. "
						+ " If for some reason you do not want this automatic fix in the future you can disable it by editing Installation.cs");
					
					try
					{
						// The target file exists (it's just the wrong version) so we set the flag to overwrite.
						System.IO.File.Copy(sourceFile, destFile, true);
					}
					catch(Exception e)
					{
						Debug.LogException(e);
						Debug.LogError("Failed to copy '" + fileName + "'");
					}
						
				}
			}
			else
			{
				Debug.LogWarning("The file " + fileName + " was not found in the project root, and will be copied there automatically. This warning "
					+ "is normal the first time you use Cubiquity in a project but you should be concerned if you continue to see it after that."
					+ " If for some reason you do not want this automatic fix in the future you can disable it by editing Installation.cs");
				
				try
				{
					// The target file doesn't exist so we don't need to set the flag to overwrite.
					System.IO.File.Copy(sourceFile, destFile, false);
				}
				catch(Exception e)
				{
					Debug.LogException(e);
					Debug.LogError("Failed to copy '" + fileName + "'");
				}
			}
			
			if(System.IO.File.Exists(destFile) == false)
			{
				Debug.LogWarning("The Cubiquity DLL was not found on startup, and this problem was not resolved.");
			}
		}
	
		/*public static void ValidateAndFixWithPrompts()
		{
			string fileName = "CubiquityC.dll";
	        string sourcePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Cubiquity");
	        string destPath =  @".";
			
			string canFixMessage = "This can be fixed automatically because we have a copy of the required DLL in the "
				+ sourcePath + " folder. Would you like this file to be copied to the root of the project folder?";
			string fixItMessage = "Yes, please fix this!";
			string dontFixItMessage = "No, I know what I'm doing...";
	
	        // Use Path class to manipulate file and directory paths. 
	        string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
	        string destFile = System.IO.Path.Combine(destPath, fileName);
			
			if(System.IO.File.Exists(destFile))
			{
				byte[] sourceChecksum = GetChecksum(sourceFile);
				byte[] destChecksum = GetChecksum(destFile);
				
				bool checksumsMatch = true;
				for(int i = 0; i < sourceChecksum.Length; i++)
				{
					if(sourceChecksum[i] != destChecksum[i])
					{
						checksumsMatch = false;
						break;
					}
				}
				
				if(!checksumsMatch)
				{
					if(EditorUtility.DisplayDialog("Cubiquity DLL in project root appears to be the wrong version",
						"This project is using the Cubiquity voxel terrain engine but the DLL in the root of the " + 
						"project folder appears to be the wrong version (or corrupt). \n\n" + canFixMessage, fixItMessage, dontFixItMessage))
					{
						// The target file exists (it's just th wrong version) so we set the flag to overwrite.
						System.IO.File.Copy(sourceFile, destFile, true);
					}
				}
			}
			else
			{
				if(EditorUtility.DisplayDialog("Cubiquity DLL not found in project root",
					"This project is using the Cubiquity voxel terrain engine but the required DLL has not been found " + 
					"in the root of the project folder. \n\n" + canFixMessage, fixItMessage, dontFixItMessage))
				{
					// The target file doesn't exist so we don't need to set the flag to overwrite.
					System.IO.File.Copy(sourceFile, destFile, false);
				}
			}
			
			if(System.IO.File.Exists(destFile) == false)
			{
				Debug.LogWarning("The Cubiquity DLL was not found on startup, and this problem was not resolved.");
			}
		}*/
		
		// From http://stackoverflow.com/q/1177607
		private static byte[] GetChecksum(string file)
		{
			using (FileStream stream = File.OpenRead(file))
			{
				SHA256Managed sha = new SHA256Managed();
				return sha.ComputeHash(stream);
			}
		}
	}
}
