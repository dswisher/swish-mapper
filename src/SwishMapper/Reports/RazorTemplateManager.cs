
using System;
using System.IO;
using System.Reflection;

using RazorEngine.Templating;

namespace SwishMapper.Reports
{
    public class RazorTemplateManager : ITemplateManager
    {
        private readonly Type rootType;
        private readonly Assembly assembly;

        public RazorTemplateManager()
        {
            rootType = GetType();
            assembly = rootType.Assembly;
        }


        public ITemplateSource Resolve(ITemplateKey key)
        {
            var name = $"Templates.{key.Name}.cshtml";

            using (var stream = assembly.GetManifestResourceStream(rootType, name))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Could not load embedded resource '{rootType.Namespace}.{name}'.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return new LoadedTemplateSource(reader.ReadToEnd());
                }
            }
        }


        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            return new NameOnlyTemplateKey(name, resolveType, context);
        }


        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            throw new NotSupportedException("Adding templates dynamically is not supported. This Manager only supports embedded resources.");
        }
    }
}
