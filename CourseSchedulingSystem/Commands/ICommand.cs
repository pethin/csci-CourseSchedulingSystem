using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Commands
{
    public interface ICommand
    {
        Task OnExecute();
    }
}
