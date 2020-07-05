
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwishMapper.Sampler
{
    public class Options
    {
        private static readonly Dictionary<string, Flag> Flags = new Dictionary<string, Flag>();

        private readonly List<string> inputFiles = new List<string>();

        static Options()
        {
            Flags.Add("--output", new Flag("outfile", (o, s) => o.OutputFile = s, "Override the default output file name."));
            Flags.Add("--zip-mask", new Flag("regex", (o, s) => o.ZipMask = s, "Filter to pick out samples within a zip file."));
        }


        public IEnumerable<string> InputFiles { get { return inputFiles; } }
        public string OutputFile { get; private set; }
        public string ZipMask { get; private set; }


        public static Options Parse(string[] args)
        {
            var options = new Options
            {
                OutputFile = "my-samples.json",
                ZipMask = "^.*$"
            };

            string expecting = null;
            foreach (var s in args)
            {
                if (expecting != null)
                {
                    Flags[expecting].Setter(options, s);

                    expecting = null;
                }
                else if (s.StartsWith("-"))
                {
                    if (Flags.ContainsKey(s))
                    {
                        expecting = s;
                    }
                    else
                    {
                        throw new OptionsException($"Unexpected option: '{s}'.");
                    }
                }
                else
                {
                    options.inputFiles.Add(s);
                }
            }

            return options;
        }


        public static string Usage()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine();
            builder.AppendLine("Usage: dotnet run -- [options] input-files");
            builder.AppendLine("  where:");
            builder.AppendLine("    input-files is a list of one or more input files to scan");
            builder.AppendLine("    [options] can be one or more of:");

            foreach (var pair in Flags.OrderBy(x => x.Key))
            {
                var head = $"{pair.Key} {pair.Value.ArgName}";

                builder.AppendLine($"        {head,25}  -> {pair.Value.Description}");
            }

            return builder.ToString();
        }


        private class Flag
        {
            public Flag(string argName, Action<Options, string> setter, string description)
            {
                ArgName = argName;
                Setter = setter;
                Description = description;
            }

            public Action<Options, string> Setter { get; private set; }
            public string ArgName { get; private set; }
            public string Description { get; private set; }
        }
    }
}
