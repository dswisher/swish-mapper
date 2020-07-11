
namespace SwishMapper.Models
{
    /// <summary>
    /// The options when running the app
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// The full path to the project file.
        /// <summary>
        public string ProjectFilePath { get; set; }

        /// <summary>
        /// The full path to the directory where reports will be written.
        /// <summary>
        public string ReportDir { get; set; }

        /// <summary>
        /// The full path to the directory where intermediate and other temporary files are written.
        /// <summary>
        public string TempDir { get; set; }
    }
}
