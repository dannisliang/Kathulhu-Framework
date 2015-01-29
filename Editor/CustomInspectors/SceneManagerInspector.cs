namespace Kathulhu
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    [CustomEditor( typeof( SceneManager ), true )]
    public class SceneManagerInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            SceneManager sceneManager = target as SceneManager;

            if ( GUILayout.Button( "Set Scene Name" ) )
            {
                if ( !string.IsNullOrEmpty( EditorApplication.currentScene ) )
                {
                    //set the sceneName this SceneManager will handle
                    string[] path = EditorApplication.currentScene.Split( char.Parse( "/" ) );
                    string name = path[path.Length - 1];
                    name = name.Remove( name.Length - 6 );
                    sceneManager.SceneName = name;
                }
                else { Debug.LogWarning( "Can't find current scene name. Save the current scene before setting the name." ); }
            }

            base.OnInspectorGUI();
        }

    }
}