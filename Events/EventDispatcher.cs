namespace Kathulhu
{
    using System.Collections.Generic;


    /// <summary>
    /// Dispatcher class. Handles subscriptions, unsubscriptions and the dispatch of events.
    /// </summary>
    public class EventDispatcher
    {

        private delegate void EventDelegate( BaseEvent e );
        public delegate void GenericEventDelegate<T>( T e ) where T : BaseEvent;

        private static Dictionary<System.Type, EventDelegate> dispatchDelegates = new Dictionary<System.Type, EventDelegate>();
        private static Dictionary<System.Delegate, EventDelegate> subscriberDelegates = new Dictionary<System.Delegate, EventDelegate>();        


        /// <summary>
        /// Subscribes to an event.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to</typeparam>
        /// <param name="del">The delegate to add to the event's callbacks</param>
        public static void Subscribe<T>( GenericEventDelegate<T> del ) where T : BaseEvent
        {
            if ( del == null ) return;
            if ( subscriberDelegates.ContainsKey( del ) ) return;

            // Create a new non-generic delegate which calls our generic one
            EventDelegate internalDelegate = ( e ) => del( ( T ) e );
            subscriberDelegates[ del ] = internalDelegate;

            //Add the non-generic delegate to the dispatch delegates
            EventDelegate tempDel;
            if ( dispatchDelegates.TryGetValue( typeof( T ), out tempDel ) )
                dispatchDelegates[ typeof( T ) ] = tempDel += internalDelegate;
            else
                dispatchDelegates[ typeof( T ) ] = internalDelegate;
        }


        /// <summary>
        /// Unsubscribes from an event.
        /// </summary>
        /// <typeparam name="T">The event type to unsubscribe from</typeparam>
        /// <param name="del">The delegate to remove</param>
        public static void Unsubscribe<T>( GenericEventDelegate<T> del ) where T : BaseEvent
        {
            if ( del == null ) return;

            //Get the non-generic subscribed delegate
            EventDelegate internalDelegate;
            if ( subscriberDelegates.TryGetValue( del, out internalDelegate ) )
            {
                //Get the corresponding multicast delegate and remove the non-generic delegate from it
                EventDelegate tempDel;
                if ( dispatchDelegates.TryGetValue( typeof( T ), out tempDel ) )
                {
                    tempDel -= internalDelegate;
                    if ( tempDel == null )
                        dispatchDelegates.Remove( typeof( T ) );
                    else
                        dispatchDelegates[ typeof( T ) ] = tempDel;
                }
                //Remove the non-generic subscribed delegate
                subscriberDelegates.Remove( del );
            }
        }

        /// <summary>
        /// Raises an event and notifies all subscribers.
        /// </summary>
        /// <param name="evt">The event parameter</param>
        public static void Event( BaseEvent evt )
        {
            EventDelegate del;
            if ( dispatchDelegates.TryGetValue( evt.GetType(), out del ) )
                del.Invoke( evt );
        }
    }
}