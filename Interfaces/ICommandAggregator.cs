namespace Kathulhu
{
    /// <summary>
    /// Defines an object that aggregates instances of 'CmdType' type commands. Should only hold one instance of a specific type.
    /// </summary>
    public interface ICommandAggregator<CmdType> where CmdType : ICommand
    {

        /// <summary>
        /// Registers a command type, making it possible to execute via the ExecuteCommand<T>() method.
        /// </summary>
        void RegisterCommand( CmdType command );

        /// <summary>
        /// Removes a command of the given type.
        /// </summary>
        /// <typeparam name="T">The type of ICommand to remove</typeparam>
        void RemoveCommand<T>() where T : CmdType;

        /// <summary>
        /// Executes an ICommand of type T, given that it has been registered prior to calling this method.
        /// </summary>
        /// <typeparam name="T">The type of ICommand to execute</typeparam>
        void ExecuteCommand<T>() where T : CmdType;

        /// <summary>
        /// Executes an ICommand of type T, with the given parameters, given that the command has been registered prior to calling this method.
        /// </summary>
        /// <typeparam name="T">The type of ICommand to execute</typeparam>
        /// <param name="args">Array of object parameters for the command's execution</param>
        void ExecuteCommand<T>( params object[] args ) where T : CmdType;

    }

    /// <summary>
    /// Defines an object to which ICommand objects can be registered to and executed.
    /// </summary>
    public interface ICommandAggregator : ICommandAggregator<ICommand>
    {

    }



}