namespace Kathulhu
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    [CustomEditor(typeof(SceneManager), true)]
    public class SceneManagerInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            SceneManager sceneManager = target as SceneManager;

            if (GUILayout.Button("Set Scene Name")){
                //set the sceneName this SceneManager will handle
                string[] path = EditorApplication.currentScene.Split( char.Parse( "/" ) );
                string name = path[ path.Length - 1 ];
                name = name.Remove( name.Length - 6 );
                sceneManager.SceneName = name;
            }

            base.OnInspectorGUI();
        }
        
    }
}
