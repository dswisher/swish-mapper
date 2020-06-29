
using System;
using System.IO;
using System.Xml;

namespace SwishMapper.Tests.TestHelpers
{
    public class CustomXmlResolver : XmlResolver
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            // For now, we only know how to respond with a stream.
            if (ofObjectToReturn != typeof(Stream))
            {
                Console.WriteLine("-> Caller does not want a stream! They want: {0}", ofObjectToReturn.Name);
                return null;
            }

            // If we see anything other than a blank role, point it out.
            if (!string.IsNullOrEmpty(role))
            {
                Console.WriteLine("-> non-blank role: '{0}'", role);
            }

            // Console.WriteLine("CustomXmlResolver, GetEntity: absoluteUri='{0}', role='{1}', typeToReturn='{2}'",
            //         absoluteUri, role, ofObjectToReturn);

            var filename = Path.GetFileName(absoluteUri.LocalPath);

            // Filter out some stuff...
            if (filename == "EN")
            {
                return null;
            }

            // TODO - is there a better way to locate the test files?
            var directory = new DirectoryInfo("../../../TestHelpers/dtd");

            // Do we have a copy of the file? If not, issue a message and return.
            var file = new FileInfo(Path.Join(directory.FullName, filename));
            if (!file.Exists)
            {
                Console.WriteLine("-> Could not locate file '{0}'. Requested URI: '{1}'.", filename, absoluteUri);
                return null;
            }

            // Open the desired file and return a stream.
            // TODO - verify the caller is closing the file!
            return file.Open(FileMode.Open, FileAccess.Read);
        }
    }
}
