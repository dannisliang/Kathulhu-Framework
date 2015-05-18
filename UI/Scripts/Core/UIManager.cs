namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class UIManager : MonoBehaviour
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
        private List<GameObject> _tooltipPrefabs = new List<GameObject>();

        [SerializeField]
        private Canvas _modalWindowsCanvas;

        private PanelsIndexer _panels = new PanelsIndexer();

        private List<UIModalWindow> _modalWindows = new List<UIModalWindow>();

        private List<UITooltip> _tooltips = new List<UITooltip>();

        private UITooltip _activeTooltip;

        private UIModalWindow _activeModalWindow;


        protected virtual void Awake()
        {
            //Create Canvas for modal windows
            if ( _modalWindowsCanvas == null )
            {
                _modalWindowsCanvas = CreateModalWindowCanvas( _modalWindowsCanvas );
            }

            if ( _modalWindowsCanvas != null )
            {
                //Create UIModalWindow instances
                foreach ( var windowPrefab in _modalWindowPrefabs )
                {
                    if ( windowPrefab == null )
                        continue;

                    if ( windowPrefab.GetComponent<UIModalWindow>() == null )
                        continue;

                    Transform modalWindowTransform = _modalWindowsCanvas.transform.InstantiateChild( windowPrefab );
                    modalWindowTransform.gameObject.SetActive( false );
                    modalWindowTransform.gameObject.name = windowPrefab.name;

                    _modalWindows.Add( modalWindowTransform.GetComponent<UIModalWindow>() );
                }

                //Create tooltip instances
                foreach ( var tooltipPrefab in _tooltipPrefabs )
                {
                    if ( tooltipPrefab == null )
                        continue;

                    Transform tooltipTransform = _modalWindowsCanvas.transform.InstantiateChild( tooltipPrefab );
                    tooltipTransform.gameObject.SetActive( false );
                    tooltipTransform.gameObject.name = tooltipPrefab.name;

                    _tooltips.Add( tooltipTransform.GetComponent<UITooltip>() );
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
            c.sortingOrder = 1;

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

        #region Modal window management members

        /// <summary>
        /// Shows the default modal window with the given parameters
        /// </summary>
        /// <param name="title">The title of the modal window</param>
        /// <param name="message">The message to display in the modal window</param>        
        /// <param name="texture">Optional texture to display along with the text</param>
        public void ShowModalWindow( string title, string message, Texture2D texture = null )
        {
            ShowModalWindow( title, message, null, null, texture );
        }

        /// <summary>
        /// Shows the default modal window with the given parameters
        /// </summary>
        /// <param name="title">The title of the modal window</param>
        /// <param name="message">The message to display in the modal window</param>
        /// <param name="buttons">Array of strings to disply on buttons in the modal window</param>
        /// <param name="callback">Delegate called when closing the modal window</param>
        /// <param name="texture">Optional texture to display along with the text</param>
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

        /// <summary>
        /// Opens the default modal window with the specified settings
        /// </summary>
        /// <param name="settings">The settings for the modal window</param>
        public void ShowModalWindow( ModalWindowSettings settings )
        {
            if ( _modalWindows.Count > 0 )
            {
                ShowModalWindow( settings, _modalWindows[0].name );
            }
        }

        /// <summary>
        /// Opens the modal window specified by name with the specified settings
        /// </summary>
        /// <param name="settings">The settings for the modal window</param>
        /// <param name="windowName">The name of the modal window to use (Prefab name)</param>
        public void ShowModalWindow( ModalWindowSettings settings, string windowName )
        {
            if ( _activeModalWindow == null || !_activeModalWindow.gameObject.activeInHierarchy || settings.overrideActiveModalWindow )//make sure there isn't already a modal window 
            {
                UIModalWindow w = _modalWindows.FirstOrDefault( x => x.name == windowName );
                if ( w != null )
                {
                    CloseActiveModalWindow();
                    ShowModalWindow( settings, w );
                }
                else Debug.LogWarning( "Cannot find modal window with name '" + windowName + "'" );
            }
            else Debug.LogWarning( "There already is an active modal window." );
        }

        private void CloseActiveModalWindow()
        {
            if ( _activeModalWindow != null && _activeModalWindow.gameObject.activeInHierarchy )
            {
                _activeModalWindow.CloseModalWindow();
            }
        }

        private void ShowModalWindow( ModalWindowSettings settings, UIModalWindow window )
        {
            _activeModalWindow = window;
            window.OpenModalWindow( settings );
            HideTooltip();
        }

        #endregion

        #region Tooltip management members

        /// <summary>
        /// Shows the tooltip with the specified message
        /// </summary>
        /// <param name="message">he message to display in the tooltip</param>
        public void ShowTooltip( string message )
        {
            TooltipSettings settings = new TooltipSettings()
            {
                message = message,
            };

            ShowTooltip( settings );
        }

        /// <summary>
        /// Shows a tooltip with the specified settings
        /// </summary>
        /// <param name="settings">The settings for the tooltip</param>
        public void ShowTooltip( TooltipSettings settings )
        {
            ShowTooltip( settings, _tooltips.FirstOrDefault() );
        }

        /// <summary>
        /// Shows a tooltip with the specified settings
        /// </summary>
        /// <param name="settings">The settings for the tooltip</param>
        /// <param name="tooltipName">The name of the tooltip prefab to use</param>
        public void ShowModalWindow( TooltipSettings settings, string tooltipName )
        {
                UITooltip t = _tooltips.FirstOrDefault( x => x.name == tooltipName );
                if ( t != null )
                {
                    ShowTooltip( settings, t );
                }
                else Debug.LogWarning( "Cannot find tooltip with name '" + tooltipName + "'" );
        }

        /// <summary>
        /// Shows a tooltip with the specified settings
        /// </summary>
        /// <param name="settings">The settings for the tooltip</param>
        /// <param name="tooltip">The instance of the tooltip to use</param>
        private void ShowTooltip( TooltipSettings settings, UITooltip tooltip )
        {
            if ( tooltip != null )
            {
                HideTooltip();
                _activeTooltip = tooltip;
                _activeTooltip.ShowTooltip( settings );
            }
        }

        /// <summary>
        /// Hides the tooltip
        /// </summary>
        public void HideTooltip()
        {
            if ( _activeTooltip != null )
            {
                _activeTooltip.HideTooltip();
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
