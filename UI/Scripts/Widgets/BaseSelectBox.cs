namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;

    public abstract class BaseSelectBox<T, L> : UIBehaviour where L : BaseList<T>
    {

        public T Value
        {
            get { return _selectedElement.Data; }
        }

        [SerializeField]
        private GameObject _selectedElementPrefab;//set in the inspector

        [SerializeField]
        private L _listComponent;//set in the inspector

        [SerializeField]
        private RectTransform _selectedElementRect;//set in the inspector

        private BaseListElement<T> _selectedElement;

        protected override void Awake()
        {
            base.Awake();

            if ( _listComponent != null )
                _listComponent.OnElementClicked += SelectElement;

            if ( _selectedElementPrefab != null )
            {
                GameObject go = Instantiate(_selectedElementPrefab) as GameObject;
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

        void SelectedElementClickedHandler()
        {
            ToggleList();
        }        

        /// <summary>
        /// Sets the selected element widget's data.
        /// </summary>
        void SelectElement( BaseListElement<T> element )
        {
            _listComponent.gameObject.SetActive( false );
            _selectedElement.Data = element.Data;

            OnSelectionChanged();
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
            _listComponent.gameObject.SetActive( !_listComponent.gameObject.activeInHierarchy );
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if ( _listComponent != null )
                _listComponent.OnElementClicked -= SelectElement;
        }
    }
}
