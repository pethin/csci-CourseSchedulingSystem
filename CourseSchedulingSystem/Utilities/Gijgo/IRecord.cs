using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Utilities.Gijgo
{
    public interface IRecord
    {
        RecordCrudLinks Links { get; set; }
    }
}
