namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Base class to initialize a scene with custom logic and automatically load a scene after initialization
    /// </summary>
    public class ApplicationInitializer : MonoBehaviour
    {

        [SerializeField]
        private string _nextScene;

        [SerializeField]
        private string[] _additionalScenesToLoad;

        [SerializeField]
        private bool _additive = false;

        [SerializeField]
        private bool _useLoadingScreen = true;

        void Start()
        {
            Initialize();
            LoadNextScene();
        }

        /// <summary>
        /// Override to Initialize the application with custom logic
        /// </summary>
        protected virtual void Initialize()
        {
            //
        }

        /// <summary>
        /// Loads the next scene automatically
        /// </summary>
        void LoadNextScene()
        {
            if ( string.IsNullOrEmpty( _nextScene ) )
                return;

            ApplicationController.LoadScene( _nextScene, _additive, _additionalScenesToLoad, _useLoadingScreen );
        }
    }
}


