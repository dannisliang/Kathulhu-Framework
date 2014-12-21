using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ExtensionsGameObject {

    /// <summary>
    /// GetComponentInChildren that can find components in inactive objects
    /// </summary>    
    public static T GetComponentInChildren<T>( this GameObject root, bool searchInactive ) where T : Component
    {
        T component = root.GetComponent<T>();
        if ( component == null )
        {
            Transform t = root.transform;
            int childcount = t.childCount;

            for ( int i = 0; i < childcount; ++i )
            {
                component = t.GetChild( i ).gameObject.GetComponentInChildren<T>( searchInactive );
                if ( null != component )
                    break;
            }
        }
        return component;
    }

    /// <summary>
    /// Returns the first component on target gameObject that implement an interface of type T
    /// </summary>
    public static T GetInterface<T>( this GameObject inObj ) where T : class
    {
        if ( !typeof( T ).IsInterface )
        {
            Debug.LogError( typeof( T ).ToString() + ": is not an actual interface!" );

            return null;
        }

        return inObj.GetComponents<Component>().OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Returns components on target gameObject that implement an interface of type T
    /// </summary>
    public static IEnumerable<T> GetInterfaces<T>( this GameObject inObj ) where T : class
    {
        if ( !typeof( T ).IsInterface )
        {
            Debug.LogError( typeof( T ).ToString() + ": is not an actual interface!" );

            return Enumerable.Empty<T>();
        }

        return inObj.GetComponents<Component>().OfType<T>();
    }

}
