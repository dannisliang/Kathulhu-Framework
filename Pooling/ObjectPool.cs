namespace Kathulhu
{
    using UnityEngine;
    using System.Collections.Generic;


    /// <summary>
    /// An object pool for a distinct Prefab.
    /// </summary>
    [System.Serializable]
    public class ObjectPool
    {

        public bool IsInitialized { get; private set; }


        /// <summary>
        /// Reference to the prefab used by this object pool.
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// How many instance of the prefab should be instantiated when this object pool is initialized.
        /// </summary>
        public int amountOfObjectsToPreload;

        /// <summary>
        /// The Transform pool objects should be parented to.
        /// </summary>
        public Transform ParentTransform;


        /// <summary>
        /// The list of available objects.
        /// </summary>
        private List<GameObject> _pooledObjects;



        /// <summary>
        /// Setup the object pool and preload required instances of the prefab.
        /// </summary>
        /// <param name="parent">The transform to use as the parent of pooled objects</param>
        public void Initialize( GameObject prefab, Transform parent )
        {
            Prefab = prefab;
            ParentTransform = parent;

            _pooledObjects = new List<GameObject>();

            for ( int x = 0; x < amountOfObjectsToPreload; x++ )
                CreatePooledInstance();

            IsInitialized = true;
        }

        /// <summary>
        /// Creates an instance of the prefab and adds it to the pool.
        /// </summary>
        public void CreatePooledInstance()
        {
            GameObject poolObject = Object.Instantiate( Prefab ) as GameObject;
            poolObject.name = Prefab.name;

            _pooledObjects.Add( poolObject );

            Deactivate( poolObject );
        }


        /// <summary>
        /// Deactivates a GameObject and returns it to the pool.
        /// </summary>
        /// <param name="poolObject">An instance of this pool's prefab that we want to deactivate</param>
        public void Deactivate( GameObject poolObject )
        {
            if ( poolObject == null )
                return;

            poolObject.transform.parent = ParentTransform;
            poolObject.SetActive( false );
        }


        /// <summary>
        /// Spawn an object from the pool at the specified position and rotation.
        /// </summary>
        public GameObject Spawn( Vector3 pos, Quaternion rot )
        {
            if ( _pooledObjects == null )
                return null;

            if ( _pooledObjects.Count > 0 )
            {
                GameObject spawnedObject = _pooledObjects[0];
                _pooledObjects.RemoveAt( 0 );

                if ( spawnedObject == null )
                {
                    Debug.LogError("Object pool has a null reference of prefab " + Prefab.name );
                    return null;
                }

                spawnedObject.transform.position = pos;
                spawnedObject.transform.rotation = rot;
                spawnedObject.SetActive( true );

                return spawnedObject;
            }
            else
            {
                CreatePooledInstance();
                return Spawn( pos, rot );
            }

        }

    }
}