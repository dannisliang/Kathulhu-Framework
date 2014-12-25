namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;


    /// <summary>
    /// Abstract scene manager component that registers to the GameController and receives callbacks for the scene loading process.
    /// Override the appropriate methods in the concrete class to add scene loading logic.
    /// </summary>
    public abstract class SceneManager : MonoBehaviour
    {

        [SerializeField]
        private string sceneName;//set in the inspector

        /// <summary>
        /// The name of the scene this SceneManager should manage
        /// </summary>
        public string SceneName { 
            get; 
            protected set; 
        }

        /// <summary>
        /// A scene specific registry for objects that should not live outside of this scene. 
        /// </summary>
        public virtual IRegistry SceneRegistry
        {
            get { return _registry; }
        }


        private IRegistry _registry;

        protected virtual void Awake()
        {
            _registry = new GameRegistry();

            GameController.Instance.RegisterSceneManager( this );
            SceneName = sceneName;
        }

        /// <summary>
        /// Callback when a scene will be loaded. Called before the LoadScene() coroutine.
        /// </summary>
        public virtual void OnLoadSceneStart()
        {
            //
        }

        /// <summary>
        /// Coroutine for loading the scene. Override to add scene 'setup' logic.
        /// </summary>
        public virtual IEnumerator Load()
        {
            yield break;
        }

        /// <summary>
        /// Callback on the completion of the scene loading.
        /// </summary>
        public virtual void OnLoadSceneCompleted() 
        {
            //
        }

        /// <summary>
        /// Callback when the application loads another scene (non-additive) and this one should be unloaded
        /// </summary>
        public virtual void UnloadScene()
        {
            //
        }

        protected virtual void OnDestroy()
        {
            GameController.Instance.UnregisterSceneManager( this );
        }
    }
}
