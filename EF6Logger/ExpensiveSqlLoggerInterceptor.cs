using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF6Logger
{
    public class ExpensiveSqlLoggerInterceptor : DbCommandInterceptor
    {
        private readonly IQueryLogger _queryLogger;
        private readonly int _executionMillisecondThreshold;
        private readonly bool _includeStackTrace;

        public ExpensiveSqlLoggerInterceptor(IQueryLogger logger, int executionMillisecondThreshold, bool enableStackTrace = true)
        {
            _queryLogger = logger;
            _executionMillisecondThreshold = executionMillisecondThreshold;
            _includeStackTrace = enableStackTrace;
        }

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Executing(interceptionContext);
            base.ReaderExecuting(command, interceptionContext);
        }

        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            Executed(command, interceptionContext);
            base.ReaderExecuted(command, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Executing(interceptionContext);
            base.NonQueryExecuting(command, interceptionContext);
        }

        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Executed(command, interceptionContext);
            base.NonQueryExecuted(command, interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Executing(interceptionContext);
            base.ScalarExecuting(command, interceptionContext);
        }

        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Executed(command, interceptionContext);
            base.ScalarExecuted(command, interceptionContext);
        }

        private void Executing<T>(DbCommandInterceptionContext<T> interceptionContext)
        {
            var timer = new Stopwatch();
            interceptionContext.UserState = timer;
            timer.Start();
        }

        private void Executed<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {
            var timer = (Stopwatch)interceptionContext.UserState;
            timer.Stop();

            if (interceptionContext.Exception != null)
            {
                _queryLogger.Write("FAILED COMMAND",
                    interceptionContext.Exception.Message,
                    command.CommandText,
                    _includeStackTrace ? Environment.StackTrace : string.Empty,
                    string.Empty,
                    string.Empty);
            }
            else if (timer.ElapsedMilliseconds >= _executionMillisecondThreshold)
            {
#if NET45                
                _queryLogger.Write(
                    string.Format("SLOW COMMAND ({0} ms)",timer.ElapsedMilliseconds),
                    command.CommandText,
                    _includeStackTrace ? Environment.StackTrace : string.Empty,
                    string.Empty,
                    string.Empty
                    );
#endif
#if NET46ORLATER                
                _queryLogger.Write(
                    $"SLOW COMMAND ({timer.ElapsedMilliseconds}ms)",
                    command.CommandText,
                    _includeStackTrace ? Environment.StackTrace : string.Empty,
                    string.Empty,
                    string.Empty
                    );
#endif
            }
        }
    }
}
