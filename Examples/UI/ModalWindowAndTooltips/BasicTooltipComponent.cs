using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Kathulhu;

public class BasicTooltipComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public string msg = "tooltip message";

    public void OnPointerEnter( PointerEventData eventData )
    {
        UIManager.Current.ShowTooltip( msg );
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        UIManager.Current.HideTooltip();
    }
}
