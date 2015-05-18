namespace Kathulhu
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class ApplicationController : MonoBehaviour, ICommandHandler<SceneTransition>
    {
        public static ApplicationController Instance { get; private set; }

        //---------------------------------------
        //STATIC MEMBERS
        //---------------------------------------

        /// <summary>
        /// Returns the active scene manager. The active SceneManager is the last SceneManager that was added from a non-additive transition 
        /// (ie. calling an additive scene load doesn't overwrite the active SceneManager)
        /// </summary>
        public static SceneManager ActiveSceneManager
        {
            get { return Instance == null ? null : Instance._activeSceneManager; }
        }

        /// <summary>
        /// Returns the current scene transition. Returns null if there is no scene transition being processed at this time.
        /// </summary>
        public static SceneTransition CurrentTransition
        {
            get { return Instance == null ? null : Instance._currentTransition; }
        }

        /// <summary>
        /// Returns the CommandDispatcher for the application
        /// </summary>
        public static ICommandDispatcher Commands
        {
            get { return Instance == null ? null : Instance._commandDispatcher; }
        }

        /// <summary>
        /// Returns the CommandDispatcher for the application
        /// </summary>
        public static IEventDispatcher Events
        {
            get { return Instance == null ? null : Instance._eventDispatcher; }
        }

        public static void ExecuteCommand<T>( T cmd ) where T : ICommand
        {
            if ( Instance != null )
            {
                Instance._commandDispatcher.Execute<T>( cmd );
            }
        }

        //---------------------------------------
        //INSTANCE MEMBERS
        //---------------------------------------

        public GameObject LoadingScreenPrefab;//set in the inspector

        private ICommandDispatcher _commandDispatcher;
        private IEventDispatcher _eventDispatcher;

        private List<SceneManager> _sceneManagers;
        private SceneManager _activeSceneManager;

        private List<IUpdatable> _updatables;
        private List<IUpdatable> _updatablesToUnregister;

        private SceneTransition _currentTransition;

        void Awake()
        {
            if ( Instance != null )
            {
                Debug.LogError( "Cannot instantiate two GameController components. This GameController will be destroyed." );
                Destroy( this );
            }
            else
            {
                _sceneManagers = new List<SceneManager>();
                _updatables = new List<IUpdatable>();
                _updatablesToUnregister = new List<IUpdatable>();
                Instance = this;

                Initialize();

                DontDestroyOnLoad( gameObject );
            }
        }

        /// <summary>
        /// Override to setup the Application
        /// </summary>
        protected virtual void Initialize()
        {
            _commandDispatcher = new CommandDispatcher();
            _eventDispatcher = new EventDispatcher();
            //_eventDispatcher = new EventDispatcher();

            Commands.RegisterHandler<SceneTransition>( this );
        }

        /// <summary>
        /// Registers a SceneManager to the ApplicationController.
        /// </summary>
        /// <param name="sceneMgr">A SceneManager object</param>
        internal void RegisterSceneManager( SceneManager sceneMgr )
        {
            _sceneManagers.Add( sceneMgr );
        }

        /// <summary>
        /// Unregisters a SceneManager from ApplicationController.
        /// </summary>
        /// <param name="sceneMgr">A SceneManager object</param>
        internal void UnregisterSceneManager( SceneManager sceneMgr )
        {
            _sceneManagers.Remove( sceneMgr );
        }

        /// <summary>
        /// Registers an IUpdatable object to the ApplicationController.
        /// </summary>
        /// <param name="obj">An IUpdatable object</param>
        public void RegisterUpdatable( IUpdatable obj )
        {
            if ( obj is MonoBehaviour )
            {
                Debug.LogWarning( "RegisterUpdatable ignored because IUpdatable object is a MonoBehaviour." );
                return;
            }

            if ( !_updatables.Contains( obj ) )
                _updatables.Add( obj );
        }

        /// <summary>
        /// Unregisters an IUpdatable object from the ApplicationController.
        /// </summary>
        /// <param name="obj">An IUpdatable object</param>
        public void UnregisterUpdatable( IUpdatable obj )
        {
            _updatablesToUnregister.Add( obj );
        }

        void Update()
        {
            //Update IUpdatable objects
            foreach ( var item in _updatables )
                item.DoUpdate();

            //Unregister IUpdatable
            foreach ( var item in _updatablesToUnregister )
                _updatables.Remove( item );
            _updatablesToUnregister.Clear();
        }

        void ICommandHandler<SceneTransition>.Execute( SceneTransition cmd )
        {
            StartCoroutine( Load( cmd ) );
        }

        IEnumerator Load( SceneTransition transition )
        {
            //validate transition settings
            if ( transition == null ) yield break;
            if ( transition.scenes == null || transition.scenes.Count < 1 ) yield break;
            if ( string.IsNullOrEmpty( transition.scenes[0] ) ) yield break;

            //BEGIN THE TRANSITION

            //Clone transition settings and make them available to external objects
            _currentTransition = new SceneTransition()
            {
                scenes = transition.scenes,
                additive = transition.additive,
                useAsync = transition.useAsync,
                useLoadingScreen = transition.useLoadingScreen,
                parameter = transition.parameter,
            };

            //Raise "Load Scene" Event
            SceneTransitionStarted beginTransitionEvent = new SceneTransitionStarted()
            {
                sceneName = transition.scenes[0]
            };
            _eventDispatcher.Publish<SceneTransitionStarted>( beginTransitionEvent );

            //Add loading screen                        
            LoadingScreen loadingScreen = null;
            if ( transition.useLoadingScreen )
            {
                //Add loading screen
                if ( LoadingScreenPrefab != null )
                {
                    GameObject loadingScreenInstance = Instantiate( LoadingScreenPrefab, Vector3.zero, Quaternion.identity ) as GameObject;
                    loadingScreen = loadingScreenInstance.GetComponent<LoadingScreen>();
                }
            }

            //Unload current scenes if scene load is not additive
            if ( !transition.additive )
            {
                //Wait one frame to make sure any current transition has time to 'yield break' before we unload it
                yield return null;

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
                    if ( ( i > 0 ) || transition.additive )
                        asyncLoading = Application.LoadLevelAdditiveAsync( currentScene );
                    else
                        asyncLoading = Application.LoadLevelAsync( currentScene );

                    while ( !asyncLoading.isDone )
                        yield return null;
                }
                else
                {
                    if ( ( i > 0 ) || transition.additive )
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
            if ( loadingScreen != null )
            {
                Destroy( loadingScreen.gameObject );
            }

            //Raise "Scene Loaded" Event
            SceneTransitionCompleted completeTransitionEvent = new SceneTransitionCompleted()
            {
                sceneName = transition.scenes[0]
            };
            _eventDispatcher.Publish<SceneTransitionCompleted>( completeTransitionEvent );

            //remove current transition settings
            _currentTransition = null;
        }

    }

    /// <summary>
    /// Command to initiate a scene transition. Holds a list of scenes to load as well as information on how to load these scenes.
    /// </summary>
    public class SceneTransition : ICommand
    {
        public List<string> scenes;

        public bool additive;

        public bool useAsync = true;
        public bool useLoadingScreen = true;

        public object parameter;
    }


    /// <summary>
    /// Event for notifying subscribers that a scene loading will occur
    /// </summary>
    public class SceneTransitionStarted : IEvent
    {
        public string sceneName;
        public bool additive;
    }

    /// <summary>
    /// Event for notifying subscribers that a scene load has progressed
    /// </summary>
    public class SceneTransitionProgressUpdate : IEvent
    {
        public float progress;
        public string message;
    }

    /// <summary>
    /// Event for notifying subscribers that a scene load was completed
    /// </summary>
    public class SceneTransitionCompleted : IEvent
    {
        public string sceneName;
    }
}
