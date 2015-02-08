namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;


    /// <summary>
    /// Abstract scene manager component that registers to the GameController and receives callbacks for the scene loading process.
    /// Override the appropriate methods in the concrete class to add scene loading logic.
    /// </summary>
    public abstract class SceneManager : MonoBehaviour, ICommandAggregator
    {

        /// <summary>
        /// The name of the scene this SceneManager should manage
        /// </summary>
        public string SceneName;//set in the inspector

        /// <summary>
        /// A scene specific registry for objects that should not live outside of this scene. 
        /// </summary>
        public virtual IRegistry SceneRegistry
        {
            get { return _registry; }
        }


        private IRegistry _registry;
        private Dictionary<Type, ICommand> _commands;

        protected virtual void Awake()
        {
            _registry = new GameRegistry();
            _commands = new Dictionary<Type, ICommand>();

            GameController.Instance.RegisterSceneManager( this );
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

    }
}
