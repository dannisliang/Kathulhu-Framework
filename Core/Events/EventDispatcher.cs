namespace Kathulhu
{
    using System.Collections.Generic;


    /// <summary>
    /// Dispatcher class. Handles subscriptions, unsubscriptions and the dispatch of events.
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        private delegate void EventDelegate( IEvent e );

        private Dictionary<System.Type, EventDelegate> dispatchDelegates = new Dictionary<System.Type, EventDelegate>();
        private Dictionary<System.Delegate, EventDelegate> subscriberDelegates = new Dictionary<System.Delegate, EventDelegate>();

        public void Subscribe<T>( EventHandler<T> handler ) where T : IEvent
        {
            if ( handler == null ) return;
            if ( subscriberDelegates.ContainsKey( handler ) ) return;

            // Create a new non-generic delegate which calls our generic one
            EventDelegate internalDelegate = ( e ) => handler( ( T )e );
            subscriberDelegates[handler] = internalDelegate;

            //Add the non-generic delegate to the dispatch delegates
            EventDelegate tempDel;
            if ( dispatchDelegates.TryGetValue( typeof( T ), out tempDel ) )
                dispatchDelegates[typeof( T )] = tempDel += internalDelegate;
            else
                dispatchDelegates[typeof( T )] = internalDelegate;
        }

        public void Unsubscribe<T>( EventHandler<T> handler ) where T : IEvent
        {
            if ( handler == null ) return;

            //Get the non-generic subscribed delegate
            EventDelegate internalDelegate;
            if ( subscriberDelegates.TryGetValue( handler, out internalDelegate ) )
            {
                //Get the corresponding multicast delegate and remove the non-generic delegate from it
                EventDelegate tempDel;
                if ( dispatchDelegates.TryGetValue( typeof( T ), out tempDel ) )
                {
                    tempDel -= internalDelegate;
                    if ( tempDel == null )
                        dispatchDelegates.Remove( typeof( T ) );
                    else
                        dispatchDelegates[typeof( T )] = tempDel;
                }
                //Remove the non-generic subscribed delegate
                subscriberDelegates.Remove( handler );
            }
        }

        public void Publish<T>( T @event ) where T : IEvent
        {
            EventDelegate del;
            if ( dispatchDelegates.TryGetValue( @event.GetType(), out del ) )
                del.Invoke( @event );
        }
    }
}