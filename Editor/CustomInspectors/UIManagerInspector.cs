namespace Kathulhu
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;

    [CustomEditor(typeof(UIManager), true)]
    public class UIManagerInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            UIManager uiMgr = target as UIManager;
            
            if (GUILayout.Button("Register all panels"))
                uiMgr.ScanForPanels();

            base.OnInspectorGUI();
        }
        
    }
}