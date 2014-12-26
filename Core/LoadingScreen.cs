namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;

    public class LoadingScreen : MonoBehaviour
    {

        public Image loadingBarFilledImage;//set in the inspector

        protected virtual void Awake()
        {
            DontDestroyOnLoad( gameObject );

            EventDispatcher.Subscribe<LoadSceneProgressUpdateEvent>( OnProgressChanged );
        }

        private void OnProgressChanged( Event e )
        {
            LoadSceneProgressUpdateEvent evt = e as LoadSceneProgressUpdateEvent;
            OnProgressChange( evt );
        }

        protected virtual void OnProgressChange( LoadSceneProgressUpdateEvent evt ) 
        {
            if ( loadingBarFilledImage != null )
            {
                loadingBarFilledImage.fillAmount = evt.progress;
            }
        }

        protected virtual void OnDestroy()
        {
            EventDispatcher.Unsubscribe<LoadSceneProgressUpdateEvent>( OnProgressChanged );
        }
    }
}
