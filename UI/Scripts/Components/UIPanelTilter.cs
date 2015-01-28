namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Code adapted from the script TiltWindow.cs from Unity's UI Samples
    ///     https://www.assetstore.unity3d.com/#!/content/25468
    /// </summary>
    public class UIPanelTilter : MonoBehaviour
    {

        public Vector2 tiltRange = new Vector2( 0, 0 );
        public bool resetRotationOnDisable = true;

        private RectTransform _rectTransform;
        private Quaternion _startRotation;
        private Vector2 _rot = Vector2.zero;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startRotation = _rectTransform.localRotation;
        }

        void Update()
        {
                Vector3 pos = Input.mousePosition;

                float halfWidth = Screen.width * 0.5f;
                float halfHeight = Screen.height * 0.5f;
                float x = Mathf.Clamp( ( pos.x - halfWidth ) / halfWidth, -1f, 1f );
                float y = Mathf.Clamp( ( pos.y - halfHeight ) / halfHeight, -1f, 1f );
                _rot = Vector2.Lerp( _rot, new Vector2( x, y ), Time.deltaTime * 5f );

                _rectTransform.localRotation = _startRotation * Quaternion.Euler( -_rot.y * tiltRange.y, _rot.x * tiltRange.x, 0f );
            }


        void OnDisable()
        {
            if (resetRotationOnDisable)
                _rectTransform.localRotation = _startRotation;
        }
    }
}