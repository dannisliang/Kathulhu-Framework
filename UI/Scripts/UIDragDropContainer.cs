namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Adapted from the uGUI Examples package ( https://www.assetstore.unity3d.com/en/#!/content/25468 ) 
    /// Originally ; DropMe.cs
    /// </summary>
    public class UIDragDropContainer : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Color highlightColor = Color.yellow;

        [SerializeField]
        protected Image highlightImage;
        [SerializeField]
        protected Image droppedObjectImage;

        protected Color normalColor;

        protected virtual void Awake()
        {
            normalColor = highlightImage != null ? highlightImage.color : Color.white;            
        }        

        public virtual void OnDrop( PointerEventData data )
        {
            if ( highlightImage != null )
                highlightImage.color = normalColor;
        }

        public virtual void OnPointerEnter( PointerEventData data )
        {            
            if ( highlightImage != null )
                highlightImage.color = highlightColor;
        }

        public void OnPointerExit( PointerEventData data )
        {
            if ( highlightImage != null )
                highlightImage.color = normalColor;
        }
    }
}
