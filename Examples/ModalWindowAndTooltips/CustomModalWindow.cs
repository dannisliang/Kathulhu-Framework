using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Kathulhu;

public class CustomModalWindow : UIModalWindow
{

    [SerializeField]
    private Text statusBarTextComponent;

    protected override void InitModalWindow( ModalWindowSettings settings )
    {
        base.InitModalWindow( settings );

        if ( statusBarTextComponent != null )
        {
            if ( settings.buttons != null && settings.buttons.Length > 1 )
            {
                statusBarTextComponent.text = "This modal window cannot display more than one button!";
            }
            else
            {
                statusBarTextComponent.text = "This is an example of a custom modal window!";
            }
        }
    }

}
