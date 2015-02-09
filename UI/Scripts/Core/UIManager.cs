namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class UIManager : MonoBehaviour, ICommandAggregator
    {
        #region STATIC MEMBERS

        /// <summary>
        /// Reference to the current UIManager. The current UIManager is the last UIManager that was instantiated.
        /// </summary>
        public static UIManager Current
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Indexer to access a registered panel by it's name. Ex : UImanager.Current.Panels["PanelName"] returns a panel named "PanelName" or null if the panel is not registered
        /// </summary>
        public IPanelIndexer Panels
        {
            get { return _panels; }
        }

        [SerializeField]
        private List<UIPanel> _scenepanels = new List<UIPanel>();

        [SerializeField]
        private List<GameObject> _modalWindowPrefabs = new List<GameObject>();

        [SerializeField]
        private Canvas _modalWindowsCanvas;

        private PanelsIndexer _panels = new PanelsIndexer();

        private Dictionary<Type, ICommand> _commands;

        private List<UIModalWindow> _modalWindows = new List<UIModalWindow>();


        protected virtual void Awake()
        {
            _commands = new Dictionary<Type, ICommand>();

            //Create Canvas for modal windows
            if ( _modalWindowsCanvas == null )
            {
                _modalWindowsCanvas = CreateModalWindowCanvas( _modalWindowsCanvas );
            }

            //Create UIModalWindow instances
            if ( _modalWindowsCanvas != null )
            {
                foreach ( var windowPrefab in _modalWindowPrefabs )
                {
                    if ( windowPrefab == null )
                        continue;

                    if ( windowPrefab.GetComponent<UIModalWindow>() == null )
                        continue;

                    Transform modalWindowTransform = _modalWindowsCanvas.transform.InstantiateChild( windowPrefab );
                    modalWindowTransform.gameObject.SetActive( false );

                    _modalWindows.Add( modalWindowTransform.GetComponent<UIModalWindow>() );
                }
            }

            Current = this;

            foreach ( var panel in _scenepanels )
                RegisterPanel( panel );
        }

        /// <summary>
        /// Override to setup the modal windows canvas the way you want (to add CanvasScaler components or change the sorting order, for example).
        /// Method is not called if the ModalWindows canvas already exists in the scene and is assigned to the UIManager
        /// </summary>
        /// <param name="canvas">The instance of Canvas that ModalWindows will be parented to</param>
        protected virtual Canvas CreateModalWindowCanvas( Canvas canvas )
        {
            GameObject modalCanvas = new GameObject( "ModalWindowsCanvas" );
            modalCanvas.transform.SetParent( transform, false );

            Canvas c = modalCanvas.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.pixelPerfect = true;
            c.sortingOrder = 0;

            modalCanvas.AddComponent<CanvasScaler>();

            modalCanvas.AddComponent<GraphicRaycaster>();

            return c;
        }


        /// <summary>
        /// Registers a panel to this UIManager. Registered panels can be accessed via the Panels property.
        /// </summary>
        public void RegisterPanel( UIPanel panel )
        {
            if ( !_panels.Contains( panel ) )
                _panels.Add( panel );
        }

        /// <summary>
        /// Removes a panel from this UImanager's list of panels
        /// </summary>
        public void UnregisterPanel( UIPanel panel )
        {
            _panels.Remove( panel );
        }

        protected virtual void OnDestroy()
        {
            //
        }

        /// <summary>
        /// Registers all panels found in the scene to this UIManager.
        /// </summary>
        public void ScanForPanels()
        {
            if ( !Application.isPlaying )
                _scenepanels.Clear();

            foreach ( Transform tr in UnityEngine.Object.FindObjectsOfType( typeof( Transform ) ) )
            {
                if ( tr.parent == null )
                {
                    UIPanel[] panels = tr.GetComponentsInChildren<UIPanel>( true );
                    foreach ( var panel in panels )
                    {
                        if ( !Application.isPlaying )
                            _scenepanels.Add( panel );
                        else
                            RegisterPanel( panel );
                    }
                }
            }

        }

        //Class for holding a private list of Panels. Can be queried externally by exposing the IPanelIndexer
        private class PanelsIndexer : List<UIPanel>, IPanelIndexer
        {
            public UIPanel this[string name]
            {
                get { return this.FirstOrDefault( x => x.name == name ); }
            }

            public List<UIPanel> ToList()
            {
                return this.ToList();
            }
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

        #region Modal window management members

        public void ShowModalWindow( string title, string message, Texture2D texture = null )
        {
            ShowModalWindow( title, message, null, null, texture );
        }

        public void ShowModalWindow( string title, string message, string[] buttons, Action<string> callback, Texture2D texture = null )
        {
            ModalWindowSettings settings = new ModalWindowSettings()
            {
                title = title,
                message = message,
                image = texture,

                buttons = buttons,
                handler = callback,
            };

            ShowModalWindow( settings );
        }

        public void ShowModalWindow( ModalWindowSettings settings )
        {
            if ( _modalWindows.Count > 0 )
            {
                _modalWindows[0].OpenModalWindow( settings );
            }
        }

        #endregion

    }

    public interface IPanelIndexer
    {
        UIPanel this[string name] { get; }

        List<UIPanel> ToList();
    }
}
