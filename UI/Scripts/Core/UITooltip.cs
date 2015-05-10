namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;

    public class UITooltip : UIBehaviour
    {
        [SerializeField]
        protected Text textComponent;

        protected bool followCursor = true;

        private RectTransform _rect;
        private LayoutElement _layoutElement;
        private Canvas _parentCanvas;
        private RectTransform _parentCanvasRectTransform;

        protected override void Awake()
        {
            base.Awake();

            _rect = GetComponent<RectTransform>();
            _layoutElement = GetComponent<LayoutElement>();
        }

        protected override void Start()
        {
            base.Start();

            _parentCanvas = GetComponentInParent<Canvas>();
            if ( _parentCanvas != null )
                _parentCanvasRectTransform = _parentCanvas.transform as RectTransform;
        }

        /// <summary>
        /// Show the tooltip with the specified settings
        /// </summary>
        /// <param name="settings">The tooltip settings</param>
        public void ShowTooltip( TooltipSettings settings )
        {
            if ( settings != null )
            {
                gameObject.SetActive( true );

                OnShowTooltip( settings );
            }
        }

        /// <summary>
        /// Override to add additional behaviour when the tooltip is shown
        /// </summary>
        /// <param name="settings">The tooltip settings</param>
        protected virtual void OnShowTooltip( TooltipSettings settings )
        {
            textComponent.text = settings.message;

            if ( _layoutElement != null )
                _layoutElement.preferredWidth = settings.preferredWidth;
        }

        void Update()
        {
            if ( _parentCanvas == null )
                return;

            if ( _parentCanvasRectTransform == null )
                return;

            if ( followCursor )
            {
                Vector2 localPositionInCanvas = Vector2.zero;
                Vector2 pivot = Vector2.zero;

                switch ( _parentCanvas.renderMode )
                {
                    case RenderMode.ScreenSpaceCamera:
                        RectTransformUtility.ScreenPointToLocalPointInRectangle( _parentCanvasRectTransform, Input.mousePosition, _parentCanvas.worldCamera, out localPositionInCanvas );
                        break;
                    case RenderMode.ScreenSpaceOverlay:
                        RectTransformUtility.ScreenPointToLocalPointInRectangle( _parentCanvasRectTransform, Input.mousePosition, null, out localPositionInCanvas );
                        break;
                    case RenderMode.WorldSpace:
                        break;
                    default:
                        break;
                }

                if ( localPositionInCanvas.x + _rect.sizeDelta.x > ( _parentCanvasRectTransform.sizeDelta.x / 2 ) )
                    pivot.x = 1;
                if ( localPositionInCanvas.y + _rect.sizeDelta.y > ( _parentCanvasRectTransform.sizeDelta.y / 2 ) )
                    pivot.y = 1;

                _rect.pivot = pivot;
                _rect.localPosition = localPositionInCanvas;
            }
        }

        public void HideTooltip()
        {
            gameObject.SetActive( false );
        }

    }

    public class TooltipSettings
    {
        public string message;
        public int preferredWidth = 200;
    }
}
