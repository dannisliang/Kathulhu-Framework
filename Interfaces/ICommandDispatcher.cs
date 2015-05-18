namespace Kathulhu
{
    /// <summary>
    /// Interface for command dispatchers. A command dispatcher is responsible for dispatching commands to the appropriate handler for execution. 
    /// </summary>
    public interface ICommandDispatcher
    {

        /// <summary>
        /// Registers a command handler to the dispatcher.
        /// </summary>
        /// <typeparam name="T">The type of command this command handler handles</typeparam>
        /// <param name="handler">The handler to register</param>
        void RegisterHandler<T>( ICommandHandler<T> handler ) where T : ICommand;

        /// <summary>
        /// Unregisters a command handler from the dispatcher.
        /// </summary>
        /// <param name="handler">The handler to register</param>
        void UnregisterHandler<T>( ICommandHandler<T> handler ) where T : ICommand;

        /// <summary>
        /// Dispatches the command to the appropriate handler for execution
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">A command to execute</param>
        void Execute<T>( T cmd ) where T : ICommand;
        
    }
}