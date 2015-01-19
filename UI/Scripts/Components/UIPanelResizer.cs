namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;


    /// <summary>
    /// Code adapted from the script ResizePanel.cs from Unity's UI Samples
    ///     https://www.assetstore.unity3d.com/#!/content/25468
    /// </summary>
    public class UIPanelResizer : MonoBehaviour, IPointerDownHandler, IDragHandler
    {

        public Vector2 minSize = new Vector2( 100, 100 );
        public Vector2 maxSize = new Vector2( 400, 400 );

        private RectTransform _rectTransform;
        private Vector2 _localOrigin;
        private Vector2 _sizeOrigin;

        void Awake()
        {
            UIPanel panel = GetComponentInParent<UIPanel>();
            if ( panel != null )
                _rectTransform = panel.transform.GetComponent<RectTransform>();
        }

        public void OnPointerDown( PointerEventData eventData )
        {            
            _sizeOrigin = _rectTransform.sizeDelta;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( _rectTransform, eventData.position, eventData.pressEventCamera, out _localOrigin );
        }

        public void OnDrag( PointerEventData eventData )
        {
            if ( _rectTransform == null )
                return;

            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( _rectTransform, eventData.position, eventData.pressEventCamera, out localPosition );
            Vector3 offset = localPosition - _localOrigin;

            Vector2 sizeDelta = _sizeOrigin + new Vector2( offset.x, -offset.y );
            sizeDelta = new Vector2(
                Mathf.Clamp( sizeDelta.x, minSize.x, maxSize.x ),
                Mathf.Clamp( sizeDelta.y, minSize.y, maxSize.y )
            );

            _rectTransform.sizeDelta = sizeDelta;
        }
    }
}
