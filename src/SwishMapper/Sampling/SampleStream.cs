
using System;
using System.IO;

namespace SwishMapper.Sampling
{
    public class SampleStream : IDisposable
    {
        private readonly Action<SampleStream> cleanHook;


        public SampleStream(string filename, Stream stream, Action<SampleStream> cleanHook = null)
        {
            Filename = filename;
            Stream = stream;

            this.cleanHook = cleanHook;
        }


        public string Filename { get; private set; }
        public Stream Stream { get; private set; }


        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();

                if (cleanHook != null)
                {
                    cleanHook(this);
                }
            }
        }
    }
}
