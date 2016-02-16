using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF6Logger
{
    /// <summary>
    /// Implement this logger for any custom targets where queries should be logged.
    /// </summary>
    public interface IQueryLogger
    {
        void Write(params string[] content);
    }
}
