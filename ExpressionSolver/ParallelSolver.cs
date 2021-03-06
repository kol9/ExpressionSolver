using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExpressionParserLib.Parser;
using ParserResult =
    System.Tuple<double, ExpressionParserLib.Model.IExpression,
        System.Collections.Generic.List<ExpressionParserLib.Exceptions.ParsingException>>;

namespace ExpressionSolver
{
    internal class ParallelSolver : IDisposable
    {
        private readonly ExpressionParser _parser;
        private readonly Queue<Task> _tasks;
        private readonly Thread _worker;

        public ParallelSolver()
        {
            _parser = new ExpressionParser();
            _tasks = new Queue<Task>();
            _worker = new Thread(Run);
            _worker.Start();
        }

        public ParserResult SendAndEval(string s)
        {
            var result = new Result();
            lock (_tasks)
            {
                _tasks.Enqueue(new Task((() => { result.Eval(s, _parser); })));
                Monitor.PulseAll(_tasks);
            }

            return result.GetRes();
        }

        private class Result
        {
            private ParserResult _res;

            internal Result()
            {
                _res = null;
            }

            internal void Eval(string s, ExpressionParser parser)
            {
                lock (this)
                {
                    var (expression, errors) = parser.Parse(s);
                    _res = new ParserResult(expression.Evaluate(), expression, errors);
                    Monitor.Pulse(this);
                }
            }

            internal ParserResult GetRes()
            {
                lock (this)
                {
                    while (_res == null)
                    {
                        Monitor.Wait(this);
                    }
                }

                return _res;
            }
        }

        private void Eval()
        {
            Task task;
            lock (_tasks)
            {
                while (_tasks.Count == 0)
                {
                    Monitor.Wait(_tasks);
                }

                task = _tasks.Dequeue();
            }

            task.Start();
        }

        private void Run()
        {
            try
            {
                while ((Thread.CurrentThread.ThreadState & ThreadState.WaitSleepJoin) == 0)
                {
                    Eval();
                }
            }
            catch (ThreadInterruptedException)
            {
            }
            finally
            {
                Thread.CurrentThread.Interrupt();
            }
        }

        public void Dispose()
        {
            _worker.Interrupt();
            try
            {
                _worker.Join();
            }
            catch (ThreadInterruptedException)
            {
            }
        }
    }
}