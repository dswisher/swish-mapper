
namespace SwishMapper.Models
{
    /// <summary>
    /// A single equivalence between an item in a data sink and the corresponding item in
    /// a data source.
    /// </summary>
    public class MappingEntry
    {
        /// <summary>
        /// The item in the data sink.
        /// <summary>
        public string SinkItem { get; set; }

        /// <summary>
        /// The item in the data source from which the sink item is populated.
        /// <summary>
        public string SourceItem { get; set; }
    }
}
