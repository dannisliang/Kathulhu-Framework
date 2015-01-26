namespace Kathulhu
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System;


    /// <summary>
    /// A concrete registry to hold references to objects and access them via an identifier.
    /// </summary>
    public class GameRegistry : IRegistry
    {

        private List<GameRegistryElement> _registry = new List<GameRegistryElement>();

        public void Register( object obj, string identifier )
        {
            Type t = obj.GetType();
            Register( t, obj, identifier );
        }

        public void Register<T>( T obj, string identifier )
        {
            Type t = typeof( T );
            Register( t, obj, identifier );
        }

        private void Register( Type t, object obj, string identifier )
        {
            if ( obj == null )
                return;

            GameRegistryElement element = _registry.FirstOrDefault( x => x.Type == t && x.Identifier == identifier );
            if ( element == null )
                _registry.Add( new GameRegistryElement() { Identifier = identifier, Type = t, Instance = obj } );
            else
                element.Instance = obj;
        }

        public object Resolve( string identifier )
        {
            GameRegistryElement element = _registry.FirstOrDefault( x => x.Identifier == identifier );
            if ( element != null )
                return element.Instance;

            return null;
        }

        public T Resolve<T>( string identifier )
        {
            Type t = typeof( T );
            GameRegistryElement element = _registry.FirstOrDefault( x => x.Type == t && x.Identifier == identifier);
            if ( element != null )
                return ( T ) element.Instance;

            return default( T );
        }

        public T Resolve<T>()
        {
            Type t = typeof( T );
            GameRegistryElement element = _registry.FirstOrDefault(x => x.Type == t);
            if (element != null)
                return (T)element.Instance;

            return default(T);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            Type t = typeof( T );
            foreach ( GameRegistryElement element in _registry.Where( p => p.Type == t ) )
                yield return ( T ) element.Instance;
        }

        public bool Remove<T>( T obj )
        {
            return Remove( obj );
        }

        public bool Remove( object obj )
        {
            GameRegistryElement element = _registry.FirstOrDefault( x => x.Instance == obj );
            if ( element != null )
                return _registry.Remove( element );

            return false;
        }

        public void Clear()
        {
            _registry.Clear();
        }

    }

    public class GameRegistryElement
    {
        public string Identifier { get; set; }

        public Type Type { get; set; }

        public object Instance { get; set; }

    }


}
