namespace Kathulhu
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System;

    public class UIManager : MonoBehaviour
    {
        private static UIManager Instance { get; set; }

        //---------------------------------------
        //STATIC MEMBERS
        //---------------------------------------

        /// <summary>
        /// Static indexer to access a registered panel by it's name. Ex : UImanager.Panels["PanelName"] returns a panel named "PanelName" or null if the panel is not registered
        /// </summary>
        public static IPanelIndexer Panels { 
            get { return _panels; } 
        }

        private static PanelsIndexer _panels = new PanelsIndexer();

        /// <summary>
        /// Registers a panel to the UIManager. Registered panels can be accessed via the static Panels property.
        /// </summary>
        public static void RegisterPanel( UIPanel panel )
        {
            Debug.Log("RegisterPanel->" + panel);
            if ( !_panels.Contains( panel ) )
                _panels.Add( panel );
        }

        /// <summary>
        /// Removes a panel from the UImanager's list of panels
        /// </summary>
        public static void UnregisterPanel( UIPanel panel )
        {
            Debug.Log( "UnregisterPanel->" + panel );
            _panels.Remove( panel );
        }

        //---------------------------------------
        //INSTANCE MEMBERS
        //---------------------------------------


        [SerializeField]
        private List<UIPanel> _scenepanels = new List<UIPanel>();

        void Awake()
        {
            Instance = this;

            foreach ( var panel in _scenepanels )
                RegisterPanel( panel );
        }

        void OnDestroy()
        {
            foreach ( var panel in _scenepanels )
                UnregisterPanel( panel );
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

        #region MENU ITEMS

        //Add a menu item to find all UIPanel objects in the scene and add them to the UIManager scene panels list
        [MenuItem( "KATHULHU/UI/Register all panels to UIManager" )]
        public static void ScanSceneForPanels()
        {
            UIManager uiMgr = GameObject.FindObjectOfType<UIManager>();
            if ( uiMgr == null )
            {
                Debug.LogWarning( "No UIManager found in scene, creating a UIManager" );
                GameObject go = new GameObject( "UIManager" );
                go.transform.position = Vector3.zero;
                uiMgr = go.AddComponent<UIManager>();
            }

            uiMgr._scenepanels.Clear();

            foreach ( Transform tr in UnityEngine.Object.FindObjectsOfType( typeof( Transform ) ) )
            {
                if ( tr.parent == null )
                {
                    UIPanel[] panels = tr.GetComponentsInChildren<UIPanel>( true );
                    foreach ( var panel in panels)
                        uiMgr._scenepanels.Add( panel );
                }
            }
        }

        #endregion MENU ITEMS

    }

    public interface IPanelIndexer
    {
        UIPanel this[ string name ] { get; }

        List<UIPanel> ToList();
    }
}
