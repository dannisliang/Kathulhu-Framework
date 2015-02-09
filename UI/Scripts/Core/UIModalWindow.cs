namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;
    using System;

    public class UIModalWindow : UIBehaviour
    {
        [SerializeField]
        private Text titleText;

        [SerializeField]
        private Text messageText;

        [SerializeField]
        private Image image;

        [SerializeField]
        private List<GameObject> buttons;

        /// <summary>
        /// Event raised when the user closes the window. String parameter is the closing button text value;
        /// </summary>
        public event Action<string> OnCloseModalWindow;

        protected Canvas canvas;

        private Action<string> _handler;

        protected override void Awake()
        {
            base.Awake();

            canvas = GetComponentInParent<Canvas>();
        }

        /// <summary>
        /// Initializes the modal window with the given settings and opens it
        /// </summary>
        /// <param name="settings">The modal window settings</param>
        public void OpenModalWindow( ModalWindowSettings settings )
        {
            if ( settings != null )
            {
                gameObject.SetActive( true );

                InitModalWindow( settings );
            }
        }

        protected virtual void InitModalWindow( ModalWindowSettings settings )
        {
            //set the title
            if ( titleText != null )
                titleText.text = settings.title;

            //set the message
            if ( messageText != null )
                messageText.text = settings.message;

            //set the image            
            if ( image != null )
            {
                image.gameObject.SetActive( true );

                if ( settings.image == null )
                {
                    image.sprite = null;
                    image.gameObject.SetActive( false );
                }
                else
                {
                    image.sprite = Sprite.Create( settings.image, new Rect( 0, 0, settings.image.width, settings.image.height ), new Vector2( 0.5f, 0.5f ) );
                    image.gameObject.SetActive( true );
                }
            }

            //set the buttons
            if ( buttons != null )
            {
                foreach ( var item in buttons )
                    item.gameObject.SetActive( false );

                if ( settings.buttons != null && settings.buttons.Length > 0 )
                {
                    for ( int i = 0; i < Mathf.Min( settings.buttons.Length, buttons.Count ); i++ )
                    {
                        buttons[i].gameObject.SetActive( true );
                        buttons[i].name = settings.buttons[i];
                        buttons[i].GetComponentInChildren<Text>().text = settings.buttons[i];
                    }
                }
                else if ( buttons.Count > 0 )
                {
                    buttons[0].gameObject.SetActive( true );
                    buttons[0].name = "Close";
                    buttons[0].GetComponentInChildren<Text>().text = "Close";
                }
            }

            //set the modal window handler
            _handler = settings.handler;
        }

        /// <summary>
        /// Handler for the modal window buttons click events
        /// </summary>
        /// <param name="btn">The button that was clicked</param>
        public void ButtonClickHandler( GameObject btn )
        {
            gameObject.SetActive( false );

            if ( _handler != null )
                _handler( btn.name );

            _handler = null;
        }

        /// <summary>
        /// Closes the modal window
        /// </summary>
        public void CloseModalWindow()
        {
            gameObject.SetActive( false );

            if ( _handler != null )
                _handler( "" );

            _handler = null;
        }

    }

    /// <summary>
    /// Class for holding various parameters for the modal window.
    /// </summary>
    public class ModalWindowSettings
    {

        public string title;
        public string message;
        public Texture2D image;

        public string[] buttons;

        public Action<string> handler;
    }
}
