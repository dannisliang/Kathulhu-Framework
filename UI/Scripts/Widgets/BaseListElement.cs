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
        /// Method to request that the list element updates itself to display the data. Override to define how this BaseListElement component can display the element's data.
        /// </summary>
        public virtual void UpdateElement()
        {
            //
        }

    }
}
