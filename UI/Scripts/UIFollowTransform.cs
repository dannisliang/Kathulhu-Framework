namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Add this component or extend it to make a UI element "follow" a given target Transform on the screen.
    /// </summary>
    public class UIFollowTransform : UIBehaviour
    {

        //The transform this UI object should follow
        public Transform targetTransform;

        //The camera used to render this UI object
        public Camera uiObjectCamera;

        //The world camera used to render the target transform
        public Camera worldCamera;

        //The offset to apply on the targetTransform's position for this UI Object to follow
        public Vector3 offset;

        private RectTransform _targetRect;
        private RectTransform _thisRect;

        protected override void Start()
        {
            base.Start();

            //cache the RectTransform of this UI object
            _thisRect = transform as RectTransform;

            //Find the parent Canvas and get it's RectTransform and, if needed, it's camera
            Canvas c = gameObject.GetComponentInParent<Canvas>();
            if ( c != null )
            {

                _targetRect = c.transform as RectTransform;

                if ( uiObjectCamera == null )
                    uiObjectCamera = c.worldCamera;
            }

            //find the world camera
            if ( worldCamera == null )
                worldCamera = Camera.main;
        }

        protected virtual void LateUpdate()
        {

            if ( targetTransform == null )
                return;

            if ( worldCamera == null )
                return;

            if ( _targetRect == null )
                return;

            if ( _thisRect == null )
                return;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint( worldCamera, targetTransform.position + offset );

            Vector3 worldPoint;
            if ( RectTransformUtility.ScreenPointToWorldPointInRectangle( _targetRect, screenPoint, uiObjectCamera, out worldPoint ) )
            {
                _thisRect.position = worldPoint;
            }
        }

    }
}