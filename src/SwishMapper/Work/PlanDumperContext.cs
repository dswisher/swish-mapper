
using System;
using System.IO;

namespace SwishMapper.Work
{
    public class PlanDumperContext : IDisposable
    {
        private readonly TextWriter writer;
        private readonly string indent;

        public PlanDumperContext(TextWriter writer)
            : this(writer, 1)
        {
        }


        private PlanDumperContext(PlanDumperContext parent)
            : this(parent.writer, parent.Depth + 1)
        {
        }


        private PlanDumperContext(TextWriter writer, int depth)
        {
            this.writer = writer;

            Depth = depth;

            indent = new string(' ', Depth * 4);
        }


        public int Depth { get; private set; }


        public PlanDumperContext Push()
        {
            return new PlanDumperContext(this);
        }


        public void WriteHeader(IModelMerger worker)
        {
            Write("{0}", worker.GetType().Name);
        }


        public void WriteHeader(IModelUpdater worker)
        {
            Write("{0}", worker.GetType().Name);
        }


        public void WriteHeader<T>(IWorker<T> worker)
        {
            Write("{0}<{1}>", worker.GetType().Name, typeof(T).Name);
        }


        public void WriteHeader<T>(IWorker<T> worker, string format, params object[] args)
        {
            var message = string.Format(format, args);

            Write("{0}<{1}> {2}", worker.GetType().Name, typeof(T).Name, message);
        }


        public void WriteHeader(object worker, string format, params object[] args)
        {
            var message = string.Format(format, args);

            Write("{0} {1}", worker.GetType().Name, message);
        }


        public void Write(string format, params object[] args)
        {
            writer.WriteLine(indent + format, args);
        }


        public void Dispose()
        {
            // Nothing to actually do here, the using pattern just makes the code "cleaner"
        }
    }
}
