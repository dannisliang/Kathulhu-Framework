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

        private RectTransform _rect;
        private LayoutElement _layoutElement;

        protected override void Awake()
        {
            base.Awake();

            _rect = GetComponent<RectTransform>();
            _layoutElement = GetComponent<LayoutElement>();
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
            Vector2 mousePos = Input.mousePosition;

            Vector2 halfsizedelta = _rect.sizeDelta * 0.5f;

            if ( mousePos.x + _rect.sizeDelta.x > Screen.width )
                mousePos.x -= halfsizedelta.x;
            else
                mousePos.x += halfsizedelta.x;

            if ( mousePos.y + _rect.sizeDelta.y > Screen.height )
                mousePos.y -= halfsizedelta.y;
            else
                mousePos.y += halfsizedelta.y;

            _rect.position = mousePos;
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
