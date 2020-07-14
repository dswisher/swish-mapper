
using System.Collections.Generic;

namespace SwishMapper.Models.Project
{
    /// <summary>
    /// The definition (details) needed to populate a model with sample data.
    /// </summary>
    public class SampleProjectModelPopulator : ProjectModelPopulator
    {
        private readonly List<SampleInputFile> files = new List<SampleInputFile>();

        /// <summary>
        /// The identifier for this sample set.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The files (which may include wildcards) to scan for samples.
        /// </summary>
        public List<SampleInputFile> Files { get { return files; } }

        /// <summary>
        /// If Files contains zip files, apply this mask (regex) to the contents.
        /// </summary>
        public string ZipMask { get; set; }
    }
}
