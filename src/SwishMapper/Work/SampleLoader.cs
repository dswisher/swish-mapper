
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using SwishMapper.Models;
using SwishMapper.Models.Data;

namespace SwishMapper.Work
{
    public class SampleLoader : IModelProducer
    {
        private readonly ILogger logger;

        public SampleLoader(ILogger<SampleLoader> logger)
        {
            this.logger = logger;
        }


        public string Path { get; set; }
        public SampleWriter Writer { get; set; }


        public async Task<DataModel> RunAsync()
        {
            // Have the writer do its thing, making sure we have an up-to-date sample file.
            await Writer.RunAsync();

            // Load the sample file
            SampleJson content;
            using (var stream = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                content = await JsonSerializer.DeserializeAsync<SampleJson>(stream);
            }

            // Create a model from the sample file
            // TODO - xyzzy - implement SampleLoader RunAsync
            var model = new DataModel();

            logger.LogWarning("SampleLoader - still a WIP!");

            return model;
        }


        public void Dump(PlanDumperContext context)
        {
            context.WriteHeader(this);

            using (var childContext = context.Push())
            {
                Writer.Dump(childContext);
            }
        }
    }
}
