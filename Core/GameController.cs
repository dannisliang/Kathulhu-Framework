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
        /// <param name="additive">Whether we should load this scene additively</param>
        public static void LoadScene( string scene, bool additive = false )
        {
            LoadScene( scene, additive, null );
        }

        /// <summary>       
        /// Loading a scene from with method will raise the appropriate events to make sure that a SceneManager can execute any loading logic
        /// </summary>
        /// <param name="scene">The name of the scene to load</param>
        /// <param name="additive">Whether we should load this scene additively</param>
        /// <param name="additionalScenes">Additional scenes to load additively</param>
        public static void LoadScene( string scene, bool additive, string[] additionalScenes )
        {
            if ( Instance == null )
                return;

            List<string> scenes = new List<string>() { scene };
            if ( additionalScenes != null && additionalScenes.Length > 0 )
            {
                foreach ( var item in additionalScenes )
                    scenes.Add( item );
            }            

            SceneTransitionSettings transition = new SceneTransitionSettings
            {
                scenes = scenes,
                additive = additive
            };

            Instance.StartCoroutine( Instance.Load( transition ) );
        }

        /// <summary>
        /// Executes a GameCommand via the GameController instance's CommandScheduler
        /// </summary>
        /// <param name="cmd">A GameCommand to execute</param>
        public static void Execute( GameCommand cmd )
        {
            if ( Instance == null )
                return;

            Instance.ExecuteCommand( cmd );
        }

        /// <summary>
        /// Enqueue a GameCommand in the GameController instance's CommandScheduler.
        /// </summary>
        /// <param name="cmd">A GameCommand to add to the execution queue</param>
        public static void Enqeue( GameCommand cmd )
        {
            if ( Instance == null )
                return;

            Instance.EnqueueCommand( cmd );
        }


        private static GameRegistry _registry;

        //---------------------------------------
        //INSTANCE MEMBERS
        //---------------------------------------

        public GameObject LoadingScreenPrefab;//set in the inspector

        private CommandScheduler _scheduler;

        private List<SceneManager> _sceneManagers;
        private SceneManager _activeSceneManager;

        private SceneTransitionBeginEvent beginTransitionEvent;        
        private SceneTransitionCompleteEvent completeTransitionEvent;

        void Awake()
        {
            if ( Instance != null )
            {
                Debug.LogError("Cannot instantiate two GameController components. This GameController will be destroyed.");
                Destroy( this );
            }
            else
            {
                _sceneManagers = new List<SceneManager>();

                beginTransitionEvent = new SceneTransitionBeginEvent();
                completeTransitionEvent = new SceneTransitionCompleteEvent();

                Instance = this;

                tag = "GameController";
                DontDestroyOnLoad( gameObject );

                _scheduler = GetComponent<CommandScheduler>();             
            }
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
            if ( transition == null ) yield break;
            if ( transition.scenes == null || transition.scenes.Count < 1 ) yield break;
            if ( string.IsNullOrEmpty( transition.scenes[0] ) ) yield break;

            //BEGIN THE TRANSITION

            if ( !transition.additive && _scheduler != null )
                _scheduler.Clear();

            //Raise "Load Scene" Event
            beginTransitionEvent.sceneName = transition.scenes[0];
            EventDispatcher.Event( beginTransitionEvent );
            
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

            //LOAD ALL THE SCENES
            string currentScene;
            for ( int i = 0; i < transition.scenes.Count; i++ )
            {
                currentScene = transition.scenes[i];                

                //Load the scene
                if ( Application.HasProLicense() && transition.useAsync )
                {
                    AsyncOperation asyncLoading;
                    if ((i > 0) || transition.additive)
                        asyncLoading = Application.LoadLevelAdditiveAsync( currentScene );
                    else
                        asyncLoading = Application.LoadLevelAsync( currentScene );

                    while ( !asyncLoading.isDone )
                        yield return null;
                }
                else
                {
                    if ( (i>0) || transition.additive )
                        Application.LoadLevelAdditive( currentScene );
                    else
                        Application.LoadLevel( currentScene );

                    yield return null;//necesary to make sure SceneManagers can register themselves to the GameController before we continue
                }

                //Setup the scene via the scene manager
                SceneManager sceneManager = _sceneManagers.FirstOrDefault( x => x.SceneName == currentScene );
                if ( sceneManager != null )
                {
                    if ( i == 0 && !transition.additive )
                        _activeSceneManager = sceneManager;

                    sceneManager.OnLoadSceneStart();

                    Coroutine sceneLoadCoroutine = sceneManager.StartCoroutine( sceneManager.Load() );
                    yield return sceneLoadCoroutine;

                    sceneManager.OnLoadSceneCompleted();
                }
                else if ( i == 0 && !transition.additive ) _activeSceneManager = null;
            }

            //END THE TRANSITION

            //Remove loading screen
            if ( loadingScreen != null ) {
                Destroy( loadingScreen.gameObject );
            }

            //Raise "Scene Loaded" Event
            completeTransitionEvent.sceneName = transition.scenes[0];
            EventDispatcher.Event( completeTransitionEvent );

        }
        
        public void ExecuteCommand( GameCommand cmd )
        {
            if ( _scheduler == null )
                _scheduler = gameObject.AddComponent<CommandScheduler>();

            _scheduler.ExecuteCommand( cmd );
        }

        public void EnqueueCommand( GameCommand cmd )
        {
            if ( _scheduler == null )
                _scheduler = gameObject.AddComponent<CommandScheduler>();

            _scheduler.EnqueueCommand( cmd );
        }

        private class SceneTransitionSettings
        {
            public List<string> scenes;

            public bool useAsync = true;
            public bool additive;
        }


    }

    /// <summary>
    /// Event for notifying subscribers that a scene loading will occur
    /// </summary>
    public class SceneTransitionBeginEvent : Event
    {
        public string sceneName;
        public bool additive;
    }

    /// <summary>
    /// Event for notifying subscribers that a scene load has progressed
    /// </summary>
    public class LoadingProgressUpdateEvent : Event
    {
        public float progress;
        public string message;
    }

    /// <summary>
    /// Event for notifying subscribers that a scene load was completed
    /// </summary>
    public class SceneTransitionCompleteEvent : Event
    {
        public string sceneName;
    }
}
