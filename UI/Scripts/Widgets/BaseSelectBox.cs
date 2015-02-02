namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System;

    public abstract class BaseSelectBox<T, L> : UIBehaviour where L : BaseList<T>
    {

        public event Action<T> OnSelected;

        /// <summary>
        /// Returns the data held by the selected element's widget
        /// </summary>
        public T Value
        {
            get { return _selectedElement.Data; }
        }

        /// <summary>
        /// Returns the BaseList<T> component used by this SelectBox
        /// </summary>
        public L List
        {
            get { return _listComponent; }
        }

        [SerializeField]
        private GameObject _selectedElementPrefab;//set in the inspector

        [SerializeField]
        private L _listComponent;//set in the inspector

        [SerializeField]
        private RectTransform _selectedElementRect;//set in the inspector

        [SerializeField]
        private bool _listStayVisible = false;

        private BaseListElement<T> _selectedElement;
        //cached components
        private RectTransform _rectTransform;
        private Canvas _parentCanvas;

        protected override void Awake()
        {
            base.Awake();

            _rectTransform = transform as RectTransform;
            _parentCanvas = GetComponentInParent<Canvas>();

            if ( _listComponent != null )
                _listComponent.OnElementClicked += SelectElement;

            if ( _selectedElementPrefab != null )
            {
                GameObject go = Instantiate( _selectedElementPrefab ) as GameObject;
                _selectedElement = go.GetComponent<BaseListElement<T>>();
                if ( _selectedElement == null )
                {
                    Destroy( go );
                    Debug.LogWarning( "Cannot create selected element, BaseListElement<T> component not found on prefab." );
                    return;
                }

                go.transform.SetParent( _selectedElementRect, false );

                Button button = go.GetComponent<Button>();
                if ( button != null )
                    button.onClick.AddListener( () => { SelectedElementClickedHandler(); } );

                //TODO -
                //constrain seleted element widget in rect bounds ?
            }
        }

        void LateUpdate()
        {
            if ( _listStayVisible )
                return;

            //Hide list if user clicked outside of the selectbox
            if ( Input.GetMouseButtonDown( 0 ) )
            {
                Vector2 pos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );

                Camera camera = _parentCanvas == null ? null : _parentCanvas.worldCamera;
                bool clickedOnSelectBox = RectTransformUtility.RectangleContainsScreenPoint( _rectTransform, pos, camera );
                bool clickedOnList = RectTransformUtility.RectangleContainsScreenPoint( _listComponent.transform as RectTransform, pos, camera );

                if ( !clickedOnSelectBox && !clickedOnList )
                    HideList();
            }
        }

        void SelectedElementClickedHandler()
        {
            if ( _listStayVisible )
                return;

            ToggleList();
        }

        /// <summary>
        /// Sets the selected element widget's data.
        /// </summary>
        void SelectElement( BaseListElement<T> element )
        {
            if ( !_listStayVisible )
                HideList();

            _selectedElement.Data = element.Data;

            OnSelectionChanged();

            if ( OnSelected != null )
                OnSelected( Value );
        }

        /// <summary>
        /// Override this method to add behaviour when the selected element's data changes
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            //
        }

        /// <summary>
        /// Toggles the list's visibility
        /// </summary>
        public void ToggleList()
        {
            if ( _listComponent.gameObject.activeInHierarchy )
                HideList();
            else
                ShowList();
        }

        public void HideList()
        {
            _listComponent.gameObject.SetActive( false );
        }

        public void ShowList()
        {
            _listComponent.gameObject.SetActive( true );
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if ( _listComponent != null )
                _listComponent.OnElementClicked -= SelectElement;
        }
    }
}
