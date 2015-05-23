using UnityEngine;
using System.Collections.Generic;
using Kathulhu;
using System.Linq;
using System;

public class CommandDispatcher : ICommandDispatcher
{

    private Dictionary<System.Type, ICommandHandler> _handlers = new Dictionary<System.Type, ICommandHandler>();

    public void RegisterHandler<T>( ICommandHandler<T> handler ) where T : ICommand
    {
        Type t = typeof( T );
        if ( _handlers.ContainsKey( t ) )
        {
            Debug.LogWarning( "Cannot register command handler, a handler is already assigned to command type " + t.ToString() );
            return;
        }

        _handlers.Add( t, handler );
    }

    public void UnregisterHandler<T>( ICommandHandler<T> handler ) where T : ICommand
    {
        Type t = typeof( T );
        if ( _handlers.ContainsKey( t ) && _handlers[t] == handler )
        {
            _handlers.Remove( t );
        }


    }

    public void Execute<T>( T cmd ) where T : ICommand
    {
        Type t = typeof(T);
        if (_handlers.ContainsKey(t)){
            ICommandHandler<T> genericHandler = _handlers[t] as ICommandHandler<T>;
            genericHandler.Execute( cmd );
        }
        else Debug.LogWarning( "No handler found for command type " + t.ToString() );
    }

}

