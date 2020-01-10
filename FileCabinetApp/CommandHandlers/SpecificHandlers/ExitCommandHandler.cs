using System;
using System.Collections.Generic;
using System.Text;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'exit' command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private Action<bool> state;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="state">Action to change the running state of the service.</param>
        public ExitCommandHandler(Action<bool> state)
        {
            this.state = state;
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request to handle.</param>
        /// <returns>The object back, or null otherwise.</returns>
        public override object Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException($"Request is null.");
            }

            if (request.Command.ToLower() == "exit")
            {
                this.Exit();
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void Exit()
        {
            Console.WriteLine("Exiting an application...");
            this.state.Invoke(false);
        }
    }
}