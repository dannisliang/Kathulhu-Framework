using UnityEngine;
using System.Collections;
using Kathulhu;

public class ModalWindowExampleUIManager : UIManager
{
    [SerializeField]
    private Texture2D _sampleimage;

    public void ShowModalWindowOne()
    {
        this.ShowModalWindow( "Simple Message", "Ia! Ia! Kathulhu ftagh'n! Ph'nglui mglw'nfah Cthulhu R'lyeh wgah'nagl fhtagn!" );
    }

    public void ShowModalWindowTwo()
    {
        this.ShowModalWindow( "Message with image", "Ia! Ia! Kathulhu ftagh'n! Ph'nglui mglw'nfah Cthulhu R'lyeh wgah'nagl fhtagn!", _sampleimage );
    }

    public void ShowModalWindowThree()
    {
        this.ShowModalWindow( "Message with buttons", "Ia! Ia! Kathulhu ftagh'n! Ph'nglui mglw'nfah Cthulhu R'lyeh wgah'nagl fhtagn!", new string[] { "Yes", "No" }, modalButtonsHandler, _sampleimage );
    }

    void modalButtonsHandler( string s )
    {
        if ( string.IsNullOrEmpty( s ) )
            Debug.Log( "Modal Window was closed forcibly or by pressing the 'X' button" );
        else
            Debug.Log( "Modal Window was closed by pressing the '" + s + "' button" );
    }


    public void ShowCustomModalWindow()
    {
        ModalWindowSettings settings = new ModalWindowSettings()
        {
            title = "Custom message with one button",
            message = "Ia! Ia! Kathulhu ftagh'n! Ph'nglui mglw'nfah Cthulhu R'lyeh wgah'nagl fhtagn!",
            buttons = new string[] { "A button" },
            handler = modalButtonsHandler,
        };

        this.ShowModalWindow( settings, "CustomModalWindow" );
    }

    public void ShowCustomModalWindowWithTooManyButtons()
    {
        ModalWindowSettings settings = new ModalWindowSettings()
        {
            overrideActiveModalWindow = true,
            title = "Custom message with one button",
            message = "Ia! Ia! Kathulhu ftagh'n! Ph'nglui mglw'nfah Cthulhu R'lyeh wgah'nagl fhtagn!",
            buttons = new string[] { "Yes", "No" },
            handler = modalButtonsHandler,
        };

        this.ShowModalWindow( settings, "CustomModalWindow" );
    }
}
