using UnityEditor;

/// <summary>
/// Automation manager for player building and unit testing.
/// Example commandline usage:
/// "C:\Program Files (x86)\Unity\Editor\Unity.exe" -batchMode -quit -nographics -projectPath C:\project -executeMethod AutomationManager.BuildAll
/// </summary>
public class OCAutomatedPlayerBuilder
{
		[MenuItem ("Build/BuildAll")]
		static void BuildAll ()
		{
				BuildStandaloneLinux32Player ();
				BuildStandaloneLinux64Player ();
				BuildStandaloneWindows32Player ();
				BuildStandaloneWindows64Player ();
		}
	
		[MenuItem ("Build/BuildStandaloneLinux64Player")]
		static void BuildStandaloneLinux64Player ()
		{
				string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity" };
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneLinux64);
				BuildPipeline.BuildPlayer (scenes
								  , "Players/Unity3DGameWorldPlayer_Linux64"
								  , BuildTarget.StandaloneLinux64, BuildOptions.None);
		}
	
		[MenuItem ("Build/BuildStandaloneLinux32Player")]
		static void BuildStandaloneLinux32Player ()
		{
				string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity" };
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneLinux);
				BuildPipeline.BuildPlayer (scenes
								  , "Players/Unity3DGameWorldPlayer_Linux32"
								  , BuildTarget.StandaloneLinux, BuildOptions.None);
		}
	
		[MenuItem ("Build/BuildStandaloneWindows32Player")]
		static void BuildStandaloneWindows32Player ()
		{
				string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity" };
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneWindows);
				BuildPipeline.BuildPlayer (scenes
								  , "Players/Unity3DGameWorldPlayer_Windows32.exe"
								  , BuildTarget.StandaloneWindows, BuildOptions.None);
		}
	
		[MenuItem ("Build/BuildStandaloneWindows64Player")]
		static void BuildStandaloneWindows64Player ()
		{
				string[] scenes = { "Assets/OpenCog Assets/Scenes/Game/Game.unity" };
				EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneWindows64);
				BuildPipeline.BuildPlayer (scenes
								  , "Players/Unity3DGameWorldPlayer_Windows64.exe"
								  , BuildTarget.StandaloneWindows64, BuildOptions.None);
		}
}
