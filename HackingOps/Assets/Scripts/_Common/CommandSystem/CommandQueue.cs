using HackingOps.Utilities.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HackingOps.Common.CommandSystem
{
    public class CommandQueue
    {
        private static CommandQueue _instance;
        public static CommandQueue Instance => _instance ?? (_instance = new CommandQueue());

        private readonly Queue<ICommand> _commandsToExecute;
        private bool _isRunningCommand;

        public CommandQueue()
        {
            _commandsToExecute = new Queue<ICommand>();
            _isRunningCommand = false;
        }

        public void AddCommand(ICommand commandToEnqueue)
        {
            _commandsToExecute.Enqueue(commandToEnqueue);
            RunNextCommand().WrapErrors();
        }

        private async Task RunNextCommand()
        {
            if (_isRunningCommand)
                return;

            while (_commandsToExecute.Count > 0)
            {
                _isRunningCommand = true;
                ICommand commandToExecute = _commandsToExecute.Dequeue();
                await commandToExecute.Execute();
            }

            _isRunningCommand = false;
        }
    }
}