using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF6Logger
{
    /// <summary>
    /// Writes the queries to the files system.
    /// </summary>
    public class FileLogger : IQueryLogger
    {
        private readonly string _logFile;

        public FileLogger(string file)
        {
            _logFile = file;
        }

        public void Write(params string[] content)
        {
            File.AppendAllLines(
                _logFile, content
                );
        }
    }
}
