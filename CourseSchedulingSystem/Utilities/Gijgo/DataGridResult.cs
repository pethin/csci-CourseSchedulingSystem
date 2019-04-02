using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Utilities.Gijgo
{
    public class DataGridResult<T> where T : IRecord
    {
        public List<T> Records { get; set; }
        public int Total { get; set; }
    }
}
