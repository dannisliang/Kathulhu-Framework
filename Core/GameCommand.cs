namespace Kathulhu
{
    using UnityEngine;

    /// <summary>
    /// A base class for time-dependant game commands. Can be used with the CommandScheduler to manage execution queues and blocking commands.
    /// </summary>
    public abstract class GameCommand : ICommand, IUpdatable
    {
        /// <summary>
        /// Whether we should use the command's priority to validate execution. If true, the command should only be executed if no other command 
        /// with UsePriority is running or if this command's priority is greater than the runnning command's priority. UsePriority can also be used
        /// by the command queue to sort game commands by priority
        /// </summary>
        public bool UsePriority
        {
            get { return _usePriority; }
            set
            {
                if ( State == CommandState.Running )
                {
                    Debug.LogWarning( "Cannot change 'UsePriority' value while command is running" );
                    return;
                }

                _usePriority = value;
            }
        }

        /// <summary>
        /// The priority of this command. Used to evaluate if this command's execution can override the current running command or the order of execution in a queue
        /// </summary>
        public int Priority
        {
            get { return _priority; }
            set
            {
                if (State == CommandState.Running)
                {
                    Debug.LogWarning("Cannot change priority while command is running");
                    return;
                }

                _priority = value;
            }
        }

        /// <summary>
        /// The current state of the command.
        /// </summary>
        public CommandState State { get; private set; }

        private bool _usePriority = false;
        private int _priority = 0;

        /// <summary>
        /// Executes the Gamecommand. Cannot execute if the command is in the Running state.
        /// </summary>
        public void Execute()
        {
            if ( State == CommandState.Running )
            {
                Debug.LogWarning("Command is already running.");
                return;
            }

            State = CommandState.Running;

            OnExecute();
        }

        protected virtual void OnExecute() { }
        
        /// <summary>
        /// Aborts the command. Cannot abort if not in Running state.
        /// </summary>
        public void Abort()
        {
            if ( State != CommandState.Running )
            {
                Debug.LogWarning( "Command is not running. Cannot abort." );
                return;
            }

            State = CommandState.Aborted;

            OnAbort();
        }

        protected virtual void OnAbort() { }

        /// <summary>
        /// Completes the command. Cannot complete is not in Running state;
        /// </summary>
        protected void Complete()
        {
            if ( State != CommandState.Running )
            {
                Debug.LogWarning( "Command is not running. Cannot complete." );
                return;
            }

            State = CommandState.Completed;

            OnComplete();
        }

        protected virtual void OnComplete() { }

        public void Update()
        {
            if ( State == CommandState.Running )
            {
                OnUpdate();
            }
        }

        public virtual void OnUpdate() { }
    }

    public enum CommandState
    {
        None,//This is a new command, it has never ben executed yet
        Running,//The command is currently running
        Aborted,//The command's execution has been aborted
        Completed,//The command has completed normally
    }
}
