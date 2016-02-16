using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF6Logger
{
    /// <summary>
    /// Writes the output to the visual studio output window.
    /// </summary>
    public class OutputWindowLogger : IQueryLogger
    {
        public void Write(params string[] content)
        {
            content.ToList().ForEach(data => System.Diagnostics.Trace.WriteLine(data, "Expensive Query log =>"));
        }
    }
}
