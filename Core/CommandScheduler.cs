namespace Kathulhu{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Scheduler for managing a queue of game commands and executing them
    /// </summary>
    public class CommandScheduler : MonoBehaviour 
    {

        // A list of commands with no priority restriction. These commands will be executed concurrently, receive updates every frame and be removed once aborted or completed.
        private List<GameCommand> _concurrentCommands = new List<GameCommand>();

        //A list of game commands that will be executed one after the other. Sorted by priority during insertion. Cannot insert at indice 0
        private List<GameCommand> _queue = new List<GameCommand>();

        //A priority task that can only be aborted by executing a game command with higher priority
        private GameCommand _current;
	    
	    void Update () {

            //Remove non-running commands
            _concurrentCommands.RemoveAll( x => x.State != CommandState.Running );

            //Update current command
            if ( _current != null && _current.State == CommandState.Running )
                _current.Update();

            //Update queue
            if ( _queue.Count > 0 )
            {
                if (_queue[0].State == CommandState.Running)
                    _queue[0].Update();
                else
                {
                    _queue.RemoveAt( 0 );
                    if ( _queue.Count > 0 )
                        _queue[0].Execute();
                }
            }

            //Update running commands
            foreach ( var cmd in _concurrentCommands )
                cmd.Update();
            
	    }

        /// <summary>
        /// Executes the GameCommand and adds it to the list of commands to update. If the GameCommand has UsePriority set to true, it will only be executed if the current priority command is null
        /// or has a lower priority.
        /// </summary>
        public void ExecuteCommand( GameCommand cmd )
        {
            if ( cmd == null )
                return;

            if ( cmd.State == CommandState.Running )
            {
                Debug.LogWarning( "Cannot execute a running command" );
                return;
            }

            if ( cmd.UsePriority )//Validate execution
            {
                if ( _current == null || _current.State != CommandState.Running)//No current running command, run this command
                {
                    _current = cmd;
                    cmd.Execute();
                }
                else if ( cmd.Priority > _current.Priority )//Current command has lower priority. Abort current command, run this command
                {
                    _current.Abort();
                    _current = cmd;
                    cmd.Execute();
                }
                else//Current command has higher priority, abort this command
                    cmd.Abort();

                return;
            }

            //No priority validation, execute this command
            _concurrentCommands.Add( cmd );
            cmd.Execute();
        }

        /// <summary>
        /// Adds the GameCommand to a queue. The command will be executed and receive Update when appropriate. If the GameCommand has UsePriority set to true, it's priority will be used to 
        /// insert the command at the correct index in the queue.
        /// </summary>
        public void EnqueueCommand( GameCommand cmd )
        {
            if ( cmd == null )
                return;

            if ( cmd.State == CommandState.Running )
            {
                Debug.LogWarning("Cannot enqueue a running command");
                return;
            }

            //Queue is empty, execute this command
            if ( _queue.Count == 0 )
            {
                _queue.Add( cmd );
                cmd.Execute();
                return;
            }

            if ( cmd.UsePriority )//Insert at relevant position in queue
            {
                int index = Mathf.Max( _queue.FindIndex( x => !x.UsePriority || x.Priority < cmd.Priority ), 1 );
                _queue.Insert( index, cmd );
            }
            else//Append to queue
                _queue.Add( cmd );                
        }
    }
}
