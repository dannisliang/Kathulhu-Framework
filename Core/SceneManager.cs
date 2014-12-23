namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;

    public class SceneManager : MonoBehaviour
    {

        /// <summary>
        /// The name of the scene this SceneManager should manage
        /// </summary>
        public string SceneName { 
            get; 
            protected set; 
        }

        protected virtual void Awake()
        {
            GameController.Instance.RegisterSceneManager( this );
            SceneName = Application.loadedLevelName;
        }

        /// <summary>
        /// Callback when a scene will be loaded. Called before the LoadScene() coroutine.
        /// </summary>
        public virtual void OnLoadSceneStart()
        {
            //
            Debug.LogWarning("OnLoadSceneStart. scene->" + SceneName);
        }

        /// <summary>
        /// Coroutine for loading the scene. Override to add scene 'setup' logic.
        /// </summary>
        public virtual IEnumerator Load()
        {
            Debug.Log( "Scene load coroutine. scene->" + SceneName );
            yield break;
        }

        /// <summary>
        /// Callback on the completion of the scene loading.
        /// </summary>
        public virtual void OnLoadSceneCompleted() 
        {
            //
            Debug.LogWarning( "OnLoadSceneCompleted. scene->" + SceneName );
        }

        protected virtual void OnDestroy()
        {
            GameController.Instance.UnregisterSceneManager( this );
        }
    }
}
