namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;


    /// <summary>
    /// Adapted from the uGUI Examples package ( https://www.assetstore.unity3d.com/en/#!/content/25468 ) 
    /// /// Originally ; DragMe.cs
    /// </summary>
    public class UIDraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Override this to set the Image component to use to get the Sprite for the icon shown during drag
        /// </summary>
        public virtual Image DragIconSourceImage
        {
            get { return _iconSource; }
        }

        public bool dragOnSurfaces = true;

        private GameObject _icon;
        private RectTransform _plane;

        private Image _iconSource;

        protected virtual void Awake()
        {
            _iconSource = GetComponent<Image>();
        }

        public void OnBeginDrag( PointerEventData eventData )
        {
            var canvas = GetComponentInParent<Canvas>();
            if ( canvas == null )
                return;

            // Create an icon of the object we want to drag
            _icon = new GameObject( "icon" );

            _icon.transform.SetParent( canvas.transform, false );
            _icon.transform.SetAsLastSibling();

            var image = _icon.AddComponent<Image>();

            // The icon should be ignored by the event system.
            CanvasGroup group = _icon.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;

            if ( DragIconSourceImage != null )
            {
                image.sprite = DragIconSourceImage.sprite;
                image.SetNativeSize();
            }

            if ( dragOnSurfaces )
                _plane = transform as RectTransform;
            else
                _plane = canvas.transform as RectTransform;

            SetDraggedPosition( eventData );
        }

        public void OnDrag( PointerEventData data )
        {
            if ( _icon != null )
                SetDraggedPosition( data );
        }

        private void SetDraggedPosition( PointerEventData data )
        {
            if ( dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null )
                _plane = data.pointerEnter.transform as RectTransform;

            var rt = _icon.GetComponent<RectTransform>();
            Vector3 globalMousePos;
            if ( RectTransformUtility.ScreenPointToWorldPointInRectangle( _plane, data.position, data.pressEventCamera, out globalMousePos ) )
            {
                rt.position = globalMousePos;
                rt.rotation = _plane.rotation;
            }
        }

        public void OnEndDrag( PointerEventData eventData )
        {
            if ( _icon != null )
                Destroy( _icon );
        }

    }
}
