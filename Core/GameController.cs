namespace Kathulhu {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class GameController : MonoBehaviour 
    {

        public static GameController Instance { get; private set; }

        //---------------------------------------
        //STATIC MEMBERS
        //---------------------------------------

        /// <summary>
        /// A registry for holding references to any object in the game
        /// </summary>
        public static IRegistry Registry
        {
            get { return _registry ?? ( _registry = new GameRegistry() ); }
        }

        /// <summary>
        /// Returns the active scene manager. The active SceneManager is the last SceneManager that was added from a non-additive transition 
        /// (ie. calling an additive scene load doesn't overwrite the active SceneManager)
        /// </summary>
        public static SceneManager ActiveSceneManager
        {
            get { return Instance == null ? null : Instance._activeSceneManager; }
        }

        /// <summary>
        /// Loading a scene from with method will raise the appropriate events to make sure that a SceneManager can execute any loading logic
        /// </summary>
        /// <param name="scene">The name of the scene to load</param>
        public static void LoadScene( string scene, bool additive = false )
        {
            if ( Instance == null )
                return;

            SceneTransitionSettings transition = new SceneTransitionSettings{
                sceneName = scene,
                additive = additive
            };
            Instance.StartCoroutine( Instance.Load( transition ) );
        }


        private static GameRegistry _registry;

        //---------------------------------------
        //INSTANCE MEMBERS
        //---------------------------------------

        public GameObject LoadingScreenPrefab;//set in the inspector

        private List<SceneManager> _sceneManagers;
        private SceneManager _activeSceneManager;

        private LoadSceneStartedEvent loadSceneEvent;
        private LoadSceneProgressUpdateEvent loadSceneProgressUpdateEvent;
        private LoadSceneCompletedEvent loadSceneCompletedEvent;

        void Awake()
        {
            _sceneManagers = new List<SceneManager>();

            loadSceneEvent = new LoadSceneStartedEvent();
            loadSceneProgressUpdateEvent = new LoadSceneProgressUpdateEvent();
            loadSceneCompletedEvent = new LoadSceneCompletedEvent();

            Instance = this;

            tag = "GameController";
            DontDestroyOnLoad( gameObject );
        }

        public void RegisterSceneManager(SceneManager sceneMgr)
        {
            _sceneManagers.Add( sceneMgr );
        }

        public void UnregisterSceneManager( SceneManager sceneMgr )
        {
            _sceneManagers.Remove( sceneMgr );
        }

        IEnumerator Load( SceneTransitionSettings transition )
        {
            //validate transition settings
            if ( string.IsNullOrEmpty( transition.sceneName ) )
                yield break;

            //Raise "Load Scene" Event
            loadSceneEvent.sceneName = transition.sceneName;
            EventDispatcher.Event( loadSceneEvent );

            //Add loading screen and Unload current scenes if scene load is not additive
            LoadingScreen loadingScreen = null;
            if ( !transition.additive )
            {
                //Add loading screen
                if ( LoadingScreenPrefab != null )
                {
                    GameObject loadingScreenInstance = Instantiate( LoadingScreenPrefab, Vector3.zero, Quaternion.identity ) as GameObject;
                    loadingScreen = loadingScreenInstance.GetComponent<LoadingScreen>();
                }

                foreach ( SceneManager manager in _sceneManagers )
                    manager.UnloadScene();
            }

            //Load the scene
            if ( Application.HasProLicense() && transition.useAsync )
            {
                AsyncOperation asyncLoading;
                asyncLoading = transition.additive ? Application.LoadLevelAdditiveAsync(transition.sceneName) : Application.LoadLevelAsync( transition.sceneName );
                while ( !asyncLoading.isDone )
                    yield return null;
            }
            else
            {
                if (transition.additive)
                    Application.LoadLevelAdditive( transition.sceneName );
                else
                    Application.LoadLevel( transition.sceneName );
            }

            //Setup the scene via the scene manager
            SceneManager sceneManager = _sceneManagers.FirstOrDefault( x => x.SceneName == transition.sceneName ); 
            if ( sceneManager != null )
            {
                if ( !transition.additive )
                    _activeSceneManager = sceneManager;

                sceneManager.OnLoadSceneStart();

                Coroutine sceneLoadCoroutine = sceneManager.StartCoroutine( sceneManager.Load() );
                yield return sceneLoadCoroutine;

                sceneManager.OnLoadSceneCompleted();
            }
            else if (!transition.additive) _activeSceneManager = null;

            //Remove loading screen
            if ( loadingScreen != null ) {
                Destroy( loadingScreen.gameObject );
            }

            //Raise "Scene Loaded" Event
            loadSceneCompletedEvent.sceneName = transition.sceneName;
            EventDispatcher.Event( loadSceneCompletedEvent );

        }

        private class SceneTransitionSettings
        {

            public string sceneName;

            public bool useAsync = true;
            public bool additive;

        }


    }

    /// <summary>
    /// Event for notifying subscribers that a scene loading will occur
    /// </summary>
    public class LoadSceneStartedEvent : Event
    {
        public string sceneName;
        public bool additive;
    }

    /// <summary>
    /// Event for notifying subscribers that a scene load has progressed
    /// </summary>
    public class LoadSceneProgressUpdateEvent : Event
    {
        public float progress;
        public string message;
    }

    /// <summary>
    /// Event for notifying subscribers that a scene load was completed
    /// </summary>
    public class LoadSceneCompletedEvent : Event
    {
        public string sceneName;
    }
}
