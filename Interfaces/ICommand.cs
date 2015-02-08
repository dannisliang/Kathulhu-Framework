namespace Kathulhu
{

    /// <summary>
    /// Interface for commands. Allows objects to 'Execute'
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute a command.
        /// </summary>
        void Execute();

        /// <summary>
        /// Execute a command with the given parameters
        /// </summary>
        /// <param name="args">Parameters for the command's execution</param>
        void Execute( params object[] args );
    }

}
