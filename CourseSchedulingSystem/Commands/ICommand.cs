using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Commands
{
    /// <summary>
    /// Classes that implement this interface in this namespace will be added as a sub-command to the application.
    /// </summary>
    internal interface ICommand
    {
        /// <summary>
        /// Called when command is executed.
        /// </summary>
        /// <returns></returns>
        Task OnExecute();
    }
}
