using System;
using System.Collections.Generic;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'remove' command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
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

            if (request.Command.ToLower() == "remove")
            {
                Remove(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Removes the existing record.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Remove(string parameters)
        {
            if (int.TryParse(parameters, out int id))
            {
                if (id < 1 || !FileCabinetService.GetIds().Contains(id))
                {
                    Console.WriteLine($"#{id} record is not found.");
                    return;
                }
                else
                {
                    FileCabinetService.RemoveRecord(id);
                    Console.WriteLine($"Record #{id} is removed.");
                }
            }
        }
    }
}
