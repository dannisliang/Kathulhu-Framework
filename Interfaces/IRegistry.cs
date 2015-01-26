namespace Kathulhu
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface that declares methods for registering and fetching objects in a generic registry.
    /// </summary>
    public interface IRegistry
    {

        /// <summary>
        /// Registers an instance of an object in the registry.
        /// </summary>
        /// <param name="identifier">An identifier we can use to resolve this instance of type T</param>
        void Register( object obj, string identifier );

        /// <summary>
        /// Registers an instance of type T in the registry.
        /// </summary>
        /// <param name="identifier">An identifier we can use to resolve this instance of type T</param>
        void Register<T>( T obj, string identifier );

        /// <summary>
        /// Returns an object in the registry that has the specified identifier.
        /// </summary>
        object Resolve( string identifier );

        /// <summary>
        /// Returns an instance of type T in the registry that has the specified identifier.
        /// </summary>
        T Resolve<T>( string identifier );

        /// <summary>
        /// Returns the first instance of type T in the registry.
        /// </summary>
        T Resolve<T>( );        

        /// <summary>
        /// Returns an enumeration of all objects of type T present in the registry
        /// </summary>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Removes a generic object from the registry
        /// </summary>
        /// <param name="obj">An object to remove from the registry</param>
        /// <returns>true if the object was removed, else false</returns>
        bool Remove<T>( T obj );

        /// <summary>
        /// Removes an object from the registry
        /// </summary>
        /// <param name="obj">An object to remove from the registry</param>
        bool Remove( object obj );

        /// <summary>
        /// Clears all entries in the registry
        /// </summary>
        void Clear();

    }
}
