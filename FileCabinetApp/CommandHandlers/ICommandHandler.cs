using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's commands.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets the next handler.
        /// </summary>
        /// <param name="handler">Handler to set.</param>
        /// <returns>Handler back.</returns>
        ICommandHandler SetNext(ICommandHandler handler);

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request to handle.</param>
        /// <returns>The object back, or null otherwise.</returns>
        object Handle(AppCommandRequest request);
    }
}
