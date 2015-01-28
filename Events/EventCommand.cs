namespace Kathulhu {

    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Base class for commands that dispatch an event. The base Execute() implementation calls the EventDispatcher to dispatch the event (this EventCommand).
    /// Usage :
    /// 
    /// ConcreteEventCommand cmd = new ConcreteEventCommand();
    /// cmd.Execute();
    /// 
    /// </summary>
    public abstract class EventCommand : BaseEvent, ICommand {

        public virtual void Execute()
        {
            EventDispatcher.Event( this );
        }

    }
}
