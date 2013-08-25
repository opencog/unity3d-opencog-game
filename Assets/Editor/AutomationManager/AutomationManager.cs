using UnityEditor;

/// <summary>
/// Automation manager for player building and unit testing.
/// Example commandline usage:
/// "C:\Program Files (x86)\Unity\Editor\Unity.exe" -batchMode -quit -nographics -projectPath C:\project -executeMethod AutomationManager.BuildAll
/// </summary>
public class AutomationManager
{
	[MenuItem ("Build/BuildAll")]
	static void BuildAll()
	{
		BuildStandaloneLinuxPlayer();
		BuildStandaloneLinux64Player();
		BuildStandaloneWindowsPlayer();
		BuildStandaloneWindows64Player();
	}
	
	[MenuItem ("Build/BuildStandaloneLinux64Player")]
	static void BuildStandaloneLinux64Player()
	{
		string[] scenes = { "Assets/Scenes/GameScenes/MainGameScene.unity" };
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneLinux64);
		BuildPipeline.BuildPlayer(scenes
								  , "Players/Unity3DGameWorldPlayer_Linux64"
								  , BuildTarget.StandaloneLinux64, BuildOptions.None );
	}
	
	[MenuItem ("Build/BuildStandaloneLinuxPlayer")]
	static void BuildStandaloneLinuxPlayer()
	{
		string[] scenes = { "Assets/Scenes/GameScenes/MainGameScene.unity" };
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneLinux);
		BuildPipeline.BuildPlayer(scenes
								  , "Players/Unity3DGameWorldPlayer_Linux"
								  , BuildTarget.StandaloneLinux, BuildOptions.None );
	}
	
	[MenuItem ("Build/BuildStandaloneWindowsPlayer")]
	static void BuildStandaloneWindowsPlayer()
	{
		string[] scenes = { "Assets/Scenes/GameScenes/MainGameScene.unity" };
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
		BuildPipeline.BuildPlayer(scenes
								  , "Players/Unity3DGameWorldPlayer_Windows.exe"
								  , BuildTarget.StandaloneWindows, BuildOptions.None );
	}
	
	[MenuItem ("Build/BuildStandaloneWindows64Player")]
	static void BuildStandaloneWindows64Player()
	{
		string[] scenes = { "Assets/Scenes/GameScenes/MainGameScene.unity" };
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
		BuildPipeline.BuildPlayer(scenes
								  , "Players/Unity3DGameWorldPlayer_Windows64.exe"
								  , BuildTarget.StandaloneWindows64, BuildOptions.None );
	}
}