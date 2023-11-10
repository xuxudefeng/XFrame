using SimplePatchToolCore;
using SimplePatchToolSecurity;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SimplePatchToolUnity
{
	public class PatcherEditor : EditorWindow
	{
		private const string PROJECT_PATH_HOLDER = "Library/SPT_ProjectPath.txt";

		private string projectRootPath;
		private Vector2 scrollPosition;

		private ProjectManager project;
		private bool? projectExists = null;

		[MenuItem( "工具/补丁工具" )]
		private static void Initialize()
		{
			PatcherEditor window = GetWindow<PatcherEditor>();
			window.titleContent = new GUIContent("补丁工具");
			window.minSize = new Vector2( 300f, 310f );

			window.Show();
		}

		private void OnEnable()
		{
			projectRootPath = File.Exists( PROJECT_PATH_HOLDER ) ? File.ReadAllText( PROJECT_PATH_HOLDER ) : "";
			CheckProjectExists();
		}

		private void OnDisable()
		{
			EditorApplication.update -= OnUpdate;
			File.WriteAllText( PROJECT_PATH_HOLDER, projectRootPath ?? "" );
		}

		private void OnGUI()
		{
			scrollPosition = GUILayout.BeginScrollView( scrollPosition );

			GUILayout.BeginVertical();
			GUILayout.Space( 5f );

			EditorGUI.BeginChangeCheck();
			projectRootPath = PathField( "项目目录: ", projectRootPath, true );
			if( EditorGUI.EndChangeCheck() )
				CheckProjectExists();

			GUILayout.Space( 5f );

			GUI.enabled = ( project == null || !project.IsRunning ) && projectExists.HasValue && !projectExists.Value;

			if( GUILayout.Button( "创建项目", GUILayout.Height( 30 ) ) )
			{
				project = new ProjectManager( projectRootPath );
				project.CreateProject();

				ProjectInfo projectInfo = project.LoadProjectInfo();
				projectInfo.IgnoredPaths.Add( "*output_log.txt" );
				project.SaveProjectInfo( projectInfo );

				EditorApplication.update -= OnUpdate;
				EditorApplication.update += OnUpdate;

				CheckProjectExists();

				EditorUtility.DisplayDialog( "自我修补程序", "如果这是一个自我修补应用程序（即此应用程序将自行更新），您需要生成一个自我修补程序。", "知道了！");
			}

			GUI.enabled = ( project == null || !project.IsRunning ) && projectExists.HasValue && projectExists.Value;

			if( GUILayout.Button( "生成补丁", GUILayout.Height( 30 ) ) )
			{
				project = new ProjectManager( projectRootPath );
				if( project.GeneratePatch() )
				{
					Debug.Log("<b>操作开始</b>");

					EditorApplication.update -= OnUpdate;
					EditorApplication.update += OnUpdate;
				}
				else
					Debug.LogWarning("<b>无法开始操作。 也许它已经在运行?</b>");
			}

			DrawHorizontalLine();

			if( GUILayout.Button("更新下载链接", GUILayout.Height( 30 ) ) )
			{
				project = new ProjectManager( projectRootPath );
				project.UpdateDownloadLinks();

				EditorApplication.update -= OnUpdate;
				EditorApplication.update += OnUpdate;
			}

			DrawHorizontalLine();

			if( GUILayout.Button("签名XMLs", GUILayout.Height( 30 ) ) )
			{
				ProjectManager project = new ProjectManager( projectRootPath );
				SecurityUtils.SignXMLsWithKeysInDirectory( project.GetXMLFiles( true, true ), project.utilitiesPath );

				EditorUtility.DisplayDialog("安全", "不要与未知方共享您的私钥！", "知道了！");
				Debug.Log("<b>操作成功...</b>");
			}

			if( GUILayout.Button("验证签名XMLs", GUILayout.Height( 30 ) ) )
			{
				string[] invalidXmls;

				ProjectManager project = new ProjectManager( projectRootPath );
				if( !SecurityUtils.VerifyXMLsWithKeysInDirectory( project.GetXMLFiles( true, true ), project.utilitiesPath, out invalidXmls ) )
				{
					Debug.Log("<b>无法验证以下 XML：</b>");
					for( int i = 0; i < invalidXmls.Length; i++ )
						Debug.Log( invalidXmls[i] );
				}
				else
					Debug.Log("<b>所有 XML 都经过验证...</b>");
			}

			GUI.enabled = true;

			DrawHorizontalLine();

			if( GUILayout.Button( "帮助", GUILayout.Height( 25 ) ) )
				Application.OpenURL( "https://github.com/yasirkula/UnitySimplePatchTool/wiki" );

			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		private void OnUpdate()
		{
			if( project == null )
			{
				EditorApplication.update -= OnUpdate;
				return;
			}

            string log = project.FetchLog();
			bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", log, 1);
			//while (log != null)
   //         {
   //             Debug.Log(log);
				
			//	log = project.FetchLog();
   //         }

			if ( !project.IsRunning )
			{
				if( project.Result == PatchResult.Failed )
					Debug.Log("<b>操作失败...</b>");
				else
					Debug.Log("<b>操作成功...</b>");

				project = null;
				EditorUtility.ClearProgressBar();
				EditorApplication.update -= OnUpdate;
			}
		}

		private string PathField( string label, string path, bool isDirectory )
		{
			GUILayout.BeginHorizontal();
			path = EditorGUILayout.TextField( label, path );
			if( GUILayout.Button( "选择路径", GUILayout.Width( 80f ) ) )
			{
				string selectedPath = isDirectory ? EditorUtility.OpenFolderPanel( "Choose a directory", "", "" ) : EditorUtility.OpenFilePanel( "Choose a file", "", "" );
				if( !string.IsNullOrEmpty( selectedPath ) )
					path = selectedPath;

				GUIUtility.keyboardControl = 0; // Remove focus from active text field
			}
			GUILayout.EndHorizontal();

			return path;
		}

		private void CheckProjectExists()
		{
			projectRootPath = projectRootPath == null ? "" : projectRootPath.Trim();

			if( string.IsNullOrEmpty( projectRootPath ) )
				projectExists = null;
			else
			{
				DirectoryInfo projectDir = new DirectoryInfo( projectRootPath );
				if( !projectDir.Exists )
					projectExists = false;
				else
					projectExists = projectDir.GetFiles( PatchParameters.PROJECT_SETTINGS_FILENAME ).Length > 0;
			}
		}

		private void DrawHorizontalLine()
		{
			GUILayout.Space( 5 );
			GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 1 ) );
			GUILayout.Space( 5 );
		}
	}
}