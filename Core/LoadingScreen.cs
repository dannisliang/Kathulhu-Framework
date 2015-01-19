namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;

    public class LoadingScreen : MonoBehaviour
    {

        public Image loadingBarFilledImage;//set in the inspector
        public Text message;//set in the inspector

        protected virtual void Awake()
        {
            DontDestroyOnLoad( gameObject );

            EventDispatcher.Subscribe<LoadingProgressUpdateEvent>( OnProgressChanged );
        }

        private void OnProgressChanged( Event e )
        {
            LoadingProgressUpdateEvent evt = e as LoadingProgressUpdateEvent;
            OnProgressChange( evt );
        }

        /// <summary>
        /// Override this method to update the loading screen with the current progress
        /// </summary>
        /// <param name="evt">The event holding the current scene loading progress</param>
        protected virtual void OnProgressChange( LoadingProgressUpdateEvent evt ) 
        {
            if ( loadingBarFilledImage != null )
                loadingBarFilledImage.fillAmount = evt.progress;

            if (message != null)
                message.text = evt.message;
        }

        protected virtual void OnDestroy()
        {
            EventDispatcher.Unsubscribe<LoadingProgressUpdateEvent>( OnProgressChanged );
        }
    }
}
