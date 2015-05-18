namespace Kathulhu
{
    /// <summary>
    /// Delegate for handling events.
    /// </summary>
    /// <typeparam name="T">A type of IEvent this handler can handle</typeparam>
    /// <param name="event">An instance of the event</param>
    public delegate void EventHandler<T>( T @event ) where T : IEvent;

    /// <summary>
    /// Event dispatchers manage event subscriptions and publications.
    /// </summary>
    public interface IEventDispatcher<EventType> where EventType : IEvent
    {

        /// <summary>
        /// Subscribe to an event. Sets a delegate function to handle an event.
        /// </summary>
        /// <typeparam name="T">The type of IEvent this delegate subscribes to</typeparam>
        /// <param name="handler">The delegate that handles the event</param>
        void Subscribe<T>( EventHandler<T> handler ) where T : EventType;

        /// <summary>
        /// Unsubscribe from an event. Removes a delegate from the publication list for an event.
        /// </summary>
        /// <typeparam name="T">The type of event to unsubscribe from</typeparam>
        /// <param name="handler">The delegate to unsubscribe</param>
        void Unsubscribe<T>( EventHandler<T> handler ) where T : EventType;

        /// <summary>
        /// Publish an event.
        /// </summary>
        /// <typeparam name="T">The type of event to publish</typeparam>
        /// <param name="event">The event object</param>
        void Publish<T>( T @event ) where T : EventType;
    }

    public interface IEventDispatcher : IEventDispatcher<IEvent>
    {

    }
}