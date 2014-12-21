using UnityEngine;

public static class ExtensionsVector3 {

    /// <summary>
    /// Returns whether the Vector3 is approximately equal to another Vector3. The threshold for being approximately equal can be passed as a parameter.
    /// </summary>
    public static bool Approximately( this Vector3 vector, Vector3 other, float threshold = 0.001f )
    {
        return Vector3.SqrMagnitude( vector - other ) < threshold;
    }

}
