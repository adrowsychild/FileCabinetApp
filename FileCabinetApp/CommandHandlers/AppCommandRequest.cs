using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// User's request.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command">The name of the command.</param>
        /// <param name="parameters">The command's parameters.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets the name of command.
        /// </summary>
        /// <value>The name of command.</value>
        public string Command { get; }

        /// <summary>
        /// Gets the command's parameters.
        /// </summary>
        /// <value>The command's parameters.</value>
        public string Parameters { get; }
    }
}
