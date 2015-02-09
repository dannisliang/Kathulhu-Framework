using UnityEngine;

public static class ExtensionsTransform
{

    /// <summary>
    /// Instantiates an object and sets it's parent
    /// </summary>    
    public static Transform InstantiateChild( this Transform parent, Object original, bool worldPositionStays = false )
    {
        GameObject go = GameObject.Instantiate( original ) as GameObject;
        Transform child = go.transform;
        child.SetParent( parent, worldPositionStays );
        return child;
    }

    /// <summary>
    /// Instantiates an object with a position and rotation and sets it's parent
    /// </summary>    
    public static Transform InstantiateChild( this Transform parent, Object original, Vector3 pos, Quaternion rot )
    {
        Transform child = GameObject.Instantiate( original ) as Transform;
        child.position = pos;
        child.rotation = rot;
        child.SetParent( parent, true );
        return child;
    }

}
