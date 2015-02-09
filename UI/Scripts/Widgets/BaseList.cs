namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.Linq;

    public abstract class BaseList<T> : UIBehaviour, IList<T>
    {

        public event Action<BaseListElement<T>> OnElementClicked;

        [SerializeField]
        private GameObject listElementPrefab;//set in the inspector

        private List<T> _dataList;
        private List<BaseListElement<T>> _widgetsList;

        protected override void Awake()
        {
            base.Awake();

            _dataList = new List<T>();
            _widgetsList = new List<BaseListElement<T>>();
        }

        /// <summary>
        /// Creates a list element's widget
        /// </summary>
        /// <param name="index">index of the element</param>        
        BaseListElement<T> CreateElement( int index = -1 )
        {
            if ( listElementPrefab == null )
                return null;

            GameObject go = Instantiate( listElementPrefab ) as GameObject;
            BaseListElement<T> component = go.GetComponent<BaseListElement<T>>();
            if ( component == null )
            {
                Destroy( go );
                Debug.LogWarning( "Cannot create list element, BaseListElement<T> component not found on prefab." );
                return null;
            }

            go.transform.SetParent( GetComponentInChildren<ScrollRect>().content, false );

            if ( index >= 0 )
                SetElementIndex( index, go.transform as RectTransform );

            Button button = go.GetComponent<Button>();
            if ( button != null )
                button.onClick.AddListener( () => { ElementClickedHandler( component ); } );

            if ( GetComponentInChildren<ScrollRect>().content.sizeDelta.y < GetComponent<RectTransform>().sizeDelta.y )
            {
                GetComponentInChildren<ScrollRect>().content.sizeDelta = new Vector2( GetComponentInChildren<ScrollRect>().content.sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y );
                GetComponentInChildren<ScrollRect>().verticalScrollbar.value = 0;
            }


            return component;
        }

        /// <summary>
        /// Destroys a list element's widget at the given index.
        /// </summary>        
        void RemoveElement( int index )
        {
            BaseListElement<T> widget = _widgetsList[index];
            if ( widget != null )
            {
                Button button = widget.GetComponent<Button>();
                if ( button != null )
                    button.onClick.RemoveAllListeners();

                widget.transform.SetParent( null );
                Destroy( widget.gameObject );

                _widgetsList.RemoveAt( index );
            }
        }

        /// <summary>
        /// Sets the sibling index of the list element's transform
        /// </summary>
        /// <param name="index">index of the element</param>
        /// <param name="elementTransform">RectTransform of the element</param>
        void SetElementIndex( int index, RectTransform elementTransform )
        {
            elementTransform.SetSiblingIndex( index );
        }

        void ElementClickedHandler( BaseListElement<T> element )
        {
            if ( OnElementClicked != null )
                OnElementClicked( element );
        }

        #region IList<T> members

        public int IndexOf( T item )
        {
            return _dataList.IndexOf( item );
        }

        public void Insert( int index, T item )
        {
            BaseListElement<T> widget = CreateElement( index );
            widget.Data = item;

            _widgetsList.Insert( index, widget );
            _dataList.Insert( index, item );
        }

        public void RemoveAt( int index )
        {
            RemoveElement( index );

            _dataList.RemoveAt( index );
            _widgetsList.RemoveAt( index );
        }

        public T this[int index]
        {
            get
            {
                return _dataList[index];
            }
            set
            {
                _dataList[index] = value;
                _widgetsList[index].Data = value;
            }
        }

        public void Add( T item )
        {
            Insert( Count, item );
        }

        public void Clear()
        {
            while ( _widgetsList.Count > 0 )
                RemoveElement( 0 );

            _dataList.Clear();
            _widgetsList.Clear();
        }

        public bool Contains( T item )
        {
            return _dataList.Contains( item );
        }

        public void CopyTo( T[] array, int arrayIndex )
        {
            _dataList.CopyTo( array, arrayIndex );
        }

        public int Count
        {
            get { return _dataList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove( T item )
        {
            int index = _dataList.IndexOf( item );
            if ( index < 0 )
                return false;

            RemoveAt( index );
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dataList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dataList.GetEnumerator();
        }

        #endregion
    }
}
