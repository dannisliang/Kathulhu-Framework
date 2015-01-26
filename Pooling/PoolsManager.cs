namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Global pooling manager. Holds, initializes and manages multiple GameObject pools.
    /// </summary>
    public class PoolsManager : MonoBehaviour
    {

        public static PoolsManager Instance { get; private set; }

        /// <summary>
        /// The list of object pools. Each pool manages a single prefab's instances.
        /// </summary>
        public List<ObjectPool> ObjectPools;


        /// <summary>
        /// Returns a pool that handles the specified prefab
        /// </summary>
        /// <param name="prefabName">The name of the prefab we're fetching the pool for</param>
        public ObjectPool GetObjectPool( string prefabName )
        {
            return ObjectPools.FirstOrDefault( x => x.Prefab.name == prefabName );
        }

        /// <summary>
        /// Adds an object pool for the specified prefab if it doesn't exist already
        /// </summary>
        /// <param name="prefab">The prefab we want to pool</param>
        public void AddPrefab( GameObject prefab )
        {
            if ( prefab == null )
                throw new MissingReferenceException( "Cannot create an object pool from a null reference." );

            ObjectPool pool = GetObjectPool( prefab.name );
            if ( pool == null )
            {
                pool = new ObjectPool();
                pool.Initialize( prefab, transform );
                ObjectPools.Add( pool );
            }
        }

        /// <summary>
        /// Deactivates a gameObject and returns it to it's pool. If no pool is found for this gameObject, it is destroyed.
        /// </summary>
        public void Deactivate( GameObject go )
        {
            ObjectPool pool = GetObjectPool( go.name );
            if ( pool != null )
            {
                pool.Deactivate( go );
            }
            else
            {
                gameObject.SetActive(false);
                Destroy(go);
            }
        }


        /// <summary>
        /// Spawns an instance of the pooled gameobject.
        /// </summary>
        public GameObject Spawn( string prefabName )
        {
            return Spawn( prefabName, Vector3.zero, Quaternion.identity );
        }

        /// <summary>
        /// Spawns an instance of the pooled gameobject at the specified position and rotation.
        /// </summary>
        public GameObject Spawn( string prefabName, Vector3 pos, Quaternion rot )
        {
            ObjectPool pool = GetObjectPool( prefabName );
            if ( pool != null )
                return pool.Spawn( Vector3.zero, Quaternion.identity );

            return null;
        }


        void Awake()
        {
            Instance = this;

            if ( ObjectPools == null )
                ObjectPools = new List<ObjectPool>();

            for ( int i = 0; i < ObjectPools.Count; i++ )
            {
                if ( ObjectPools[ i ].Prefab == null )
                    continue;

                if ( !ObjectPools[ i ].IsInitialized )
                    ObjectPools[ i ].Initialize( ObjectPools[ i ].Prefab, transform );
            }
        }

    }
}
