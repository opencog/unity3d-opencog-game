
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
using UnityEngine;
using System.Collections;
using ProtoBuf;
using UnityEditor;

namespace OpenCog
{

namespace Automation
{

/// <summary>
/// The OpenCog Automated Player Builder.  Builds standalone players and allows for unit testing.
/// Example commandline usage:
/// "C:\Program Files (x86)\Unity\Editor\Unity.exe" -batchMode -quit -nographics -projectPath C:\project -executeMethod OCAutomatedPlayerBuilder.BuildAll
/// </summary>
				#region Class Attributes

				#endregion 
public class OCAutomatedPlayerBuilder //@TODO: coordinate with David and rename to OCAutomatedPlayerBuild
{

/////////////////////////////////////////////////////////////////////////////		
  						#region Public Member Functions
/////////////////////////////////////////////////////////////////////////////

						#if UNITY_PRO_LICENSE
						[MenuItem ("Build/BuildAll")]
						#endif
static void BuildAll()
{
  BuildStandaloneLinux32Player();
  BuildStandaloneLinux64Player();
  BuildStandaloneWindows32Player();
  BuildStandaloneWindows64Player();
				
  BuildStandaloneLinux64TestPlayer();			
}
	
						#if UNITY_PRO_LICENSE
						[MenuItem ("Build/BuildStandaloneLinux64Player")]
						#endif
static void BuildStandaloneLinux64Player()
{			
  string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity"};//, "Assets/Scenes/MainMenu/MainMenu.unity", "Assets/Scenes/BlockSetViewer/BlockSetViewer.unity" };
  EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneLinux64);
  BuildPipeline.BuildPlayer(scenes
								  , "../Players/Unity3DGameWorldPlayer_Linux64"
								  , BuildTarget.StandaloneLinux64, BuildOptions.None);
}
						
						#if UNITY_PRO_LICENSE
						[MenuItem ("Build/BuildStandaloneLinux64TestPlayer")]
						#endif
static void BuildStandaloneLinux64TestPlayer()
{
  PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "TEST_AND_EXIT");
				
  string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity" };
  EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneLinux64);
  BuildPipeline.BuildPlayer(scenes
								  , "../Players/Unity3DGameWorldTestPlayer_Linux64"
								  , BuildTarget.StandaloneLinux64, BuildOptions.None);
}		

						#if UNITY_PRO_LICENSE
						[MenuItem ("Build/BuildStandaloneLinux32Player")]
						#endif
static void BuildStandaloneLinux32Player()
{
  string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity"};//, "Assets/Scenes/MainMenu/MainMenu.unity", "Assets/Scenes/BlockSetViewer/BlockSetViewer.unity" };
  EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneLinux);
  BuildPipeline.BuildPlayer(scenes
								  , "../Players/Unity3DGameWorldPlayer_Linux32"
								  , BuildTarget.StandaloneLinux, BuildOptions.None);
}
	
						#if UNITY_PRO_LICENSE
						[MenuItem ("Build/BuildStandaloneWindows32Player")]
						#endif
static void BuildStandaloneWindows32Player()
{
  string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity"};//, "Assets/Scenes/MainMenu/MainMenu.unity", "Assets/Scenes/BlockSetViewer/BlockSetViewer.unity" };
  EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
  BuildPipeline.BuildPlayer(scenes
								  , "../Players/Unity3DGameWorldPlayer_Windows32.exe"
								  , BuildTarget.StandaloneWindows, BuildOptions.None);
}
						#if UNITY_PRO_LICENSE
						[MenuItem ("Build/BuildStandaloneWindows64Player")]
						#endif
static void BuildStandaloneWindows64Player()
{	
  string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity"};//, "Assets/Scenes/MainMenu/MainMenu.unity", "Assets/Scenes/BlockSetViewer/BlockSetViewer.unity" };
  EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
  BuildPipeline.BuildPlayer(scenes
								  , "../Players/Unity3DGameWorldPlayer_Windows64.exe"
								  , BuildTarget.StandaloneWindows64, BuildOptions.None);
}
						
/////////////////////////////////////////////////////////////////////////////
  						#endregion
/////////////////////////////////////////////////////////////////////////////


}// class OCAutomatedPlayerBuilder

}// namespace Automation

}// namespace OpenCog



