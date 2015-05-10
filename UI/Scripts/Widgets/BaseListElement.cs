namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;

    public class BaseListElement<T> : UIBehaviour
    {


        /// <summary>
        /// Property holding the generic data contained by this list element.
        /// </summary>
        public T Data
        {
            get
            {
                return _elementData;
            }
            set
            {
                _elementData = value;
                UpdateElement();
            }
        }

        private T _elementData;

        /// <summary>
        /// Property to set whether this element is selected or not
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                SelectElement( _isSelected );
            }
        }

        private bool _isSelected;

        /// <summary>
        /// Method to request that the list element updates itself to display the data. Override to define how this BaseListElement component can display the element's data.
        /// </summary>
        public virtual void UpdateElement()
        {
            //
        }

        /// <summary>
        /// Override to update this UI element's graphics depending on it's selected state
        /// </summary>
        /// <param name="selected">Whether this element is selected or not</param>
        protected virtual void SelectElement( bool selected )
        {
            //
        }

    }
}
