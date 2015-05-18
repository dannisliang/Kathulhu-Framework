namespace Kathulhu
{

    /// <summary>
    /// Interface for command handlers. A CommandHandler is responsible for executing a command.
    /// </summary>
    public interface ICommandHandler
    {

    }

    /// <summary>
    /// Interface for command handlers. A CommandHandler is responsible for executing a command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommandHandler<T> : ICommandHandler where T : ICommand
    {

        /// <summary>
        /// Execute the requested command.
        /// </summary>
        /// <param name="cmd">The command to execute</param>
        void Execute( T cmd );

    }

}