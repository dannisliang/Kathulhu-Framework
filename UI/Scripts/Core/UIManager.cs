namespace Kathulhu
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class UIManager : MonoBehaviour
    {
        #region STATIC MEMBERS

        /// <summary>
        /// Reference to the current UIManager. The current UIManager is the last UIManager that was instantiated.
        /// </summary>
        public static UIManager Current { 
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

        private PanelsIndexer _panels = new PanelsIndexer();

        protected virtual void Awake()
        {
            Current = this;

            foreach ( var panel in _scenepanels )
                RegisterPanel( panel );
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
            public UIPanel this[ string name ]
            {
                get { return this.FirstOrDefault( x => x.name == name ); }
            }

            public List<UIPanel> ToList()
            {
                return this.ToList();
            }
        }

    }

    public interface IPanelIndexer
    {
        UIPanel this[ string name ] { get; }

        List<UIPanel> ToList();
    }
}
