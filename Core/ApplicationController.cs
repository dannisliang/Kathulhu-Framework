namespace Kathulhu
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class ApplicationController : MonoBehaviour, ICommandAggregator
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
        /// Loading a scene with this method will raise the appropriate events to make sure that a SceneManager can execute any loading logic
        /// </summary>
        /// <param name="scene">The name of the scene to load</param>        
        public static void LoadScene( string scene )
        {
            LoadScene( scene, false );
        }

        /// <summary>       
        /// Loading a scene with this method will raise the appropriate events to make sure that a SceneManager can execute any loading logic
        /// </summary>
        /// <param name="scene">The name of the scene to load</param>
        /// <param name="additive">Whether we should load this scene additively</param>
        public static void LoadScene( string scene, bool additive )
        {
            LoadScene( scene, additive, !additive );
        }


        /// <summary>       
        /// Loading a scene with this method will raise the appropriate events to make sure that a SceneManager can execute any loading logic
        /// </summary>
        /// <param name="scene">The name of the scene to load</param>
        /// <param name="additive">Whether we should load this scene additively</param>
        /// <param name="useLoadingScreen">Whether the loading screen should be displayed or not</param>
        public static void LoadScene( string scene, bool additive, bool useLoadingScreen )
        {
            LoadScene( scene, additive, null, useLoadingScreen );
        }

        /// <summary>       
        /// Loading a scene from with method will raise the appropriate events to make sure that a SceneManager can execute any loading logic
        /// </summary>
        /// <param name="scene">The name of the scene to load</param>
        /// <param name="additive">Whether we should load this scene additively</param>
        /// <param name="additionalScenes">Additional scenes to load additively</param>
        /// /// <param name="useLoadingScreen">Whether the loading screen should be displayed or not</param>
        public static void LoadScene( string scene, bool additive, string[] additionalScenes, bool useLoadingScreen = true )
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
                additive = additive,
                useLoadingScreen = useLoadingScreen,
            };

            Instance.StartCoroutine( Instance.Load( transition ) );
        }

        //---------------------------------------
        //INSTANCE MEMBERS
        //---------------------------------------

        public GameObject LoadingScreenPrefab;//set in the inspector

        private List<SceneManager> _sceneManagers;
        private SceneManager _activeSceneManager;

        private Dictionary<Type, ICommand> _commands;
        private List<IUpdatable> _updatables;
        private List<IUpdatable> _updatablesToUnregister;

        private SceneTransitionBeginEvent beginTransitionEvent;
        private SceneTransitionCompleteEvent completeTransitionEvent;

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
                _commands = new Dictionary<Type, ICommand>();
                _updatables = new List<IUpdatable>();
                _updatablesToUnregister = new List<IUpdatable>();

                beginTransitionEvent = new SceneTransitionBeginEvent();
                completeTransitionEvent = new SceneTransitionCompleteEvent();

                Instance = this;

                tag = "GameController";
                DontDestroyOnLoad( gameObject );

                Initialize();
            }
        }

        /// <summary>
        /// Override to setup the Application
        /// </summary>
        protected virtual void Initialize()
        {
            
        }

        public void RegisterSceneManager( SceneManager sceneMgr )
        {
            _sceneManagers.Add( sceneMgr );
        }

        public void UnregisterSceneManager( SceneManager sceneMgr )
        {
            _sceneManagers.Remove( sceneMgr );
        }

        #region ICommandAggregator members

        public void RegisterCommand( ICommand command )
        {
            Type t = command.GetType();
            if ( !_commands.ContainsKey( t ) || _commands[t] == null )
            {
                _commands[t] = command;
            }
        }

        public void RemoveCommand<T>() where T : ICommand
        {
            _commands.Remove( typeof( T ) );
        }

        public void ExecuteCommand<T>() where T : ICommand
        {
            Type t = typeof( T );
            if ( _commands.ContainsKey( t ) )
                _commands[t].Execute();
        }

        public void ExecuteCommand<T>( params object[] args ) where T : ICommand
        {
            Type t = typeof( T );
            if ( _commands.ContainsKey( t ) )
                _commands[t].Execute( args );
        }

        #endregion

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

        public void UnregisterUpdatable( IUpdatable obj )
        {
            _updatablesToUnregister.Add( obj );            
        }

        void Update()
        {
            //Update updatables
            foreach ( var item in _updatables )
                item.Update();

            //Unregister updatables
            foreach ( var item in _updatablesToUnregister ) 
                _updatables.Remove( item );
            _updatablesToUnregister.Clear();
        }


        IEnumerator Load( SceneTransitionSettings transition )
        {
            //validate transition settings
            if ( transition == null ) yield break;
            if ( transition.scenes == null || transition.scenes.Count < 1 ) yield break;
            if ( string.IsNullOrEmpty( transition.scenes[0] ) ) yield break;

            //BEGIN THE TRANSITION

            //Raise "Load Scene" Event
            beginTransitionEvent.sceneName = transition.scenes[0];
            EventDispatcher.Event( beginTransitionEvent );

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
            completeTransitionEvent.sceneName = transition.scenes[0];
            EventDispatcher.Event( completeTransitionEvent );

        }

        /// <summary>
        /// Settings for a scene transition. Holds a list of scenes to lot as well as information on how to load these scenes.
        /// </summary>
        private class SceneTransitionSettings
        {
            public List<string> scenes;

            public bool additive;

            public bool useAsync = true;
            public bool useLoadingScreen = true;

        }

    }


    /// <summary>
    /// Event for notifying subscribers that a scene loading will occur
    /// </summary>
    public class SceneTransitionBeginEvent : BaseEvent
    {
        public string sceneName;
        public bool additive;
    }

    /// <summary>
    /// Event for notifying subscribers that a scene load has progressed
    /// </summary>
    public class SceneTransitionLoadingProgressUpdateEvent : BaseEvent
    {
        public float progress;
        public string message;
    }

    /// <summary>
    /// Event for notifying subscribers that a scene load was completed
    /// </summary>
    public class SceneTransitionCompleteEvent : BaseEvent
    {
        public string sceneName;
    }
}
