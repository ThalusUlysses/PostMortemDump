namespace Thalus.Ulysses.PostMortem.Dump
{
    /// <summary>
    /// Implememts the post mortem information for exceptions and
    /// process
    /// </summary>
    public class PostMortemInfo
    {
        /// <summary>
        /// Gets or sets a collection of stored first chance exceptions
        /// </summary>
        public IEnumerable<ExceptionStorageItem>? LastFirstChanceExceptions { get; set; }

        /// <summary>
        /// Gets or sets the exception information of the unhandled exception that caused
        /// the termination as <see cref="ExceptionStorageItem"/>
        /// </summary>
        public ExceptionStorageItem? UnhandledException { get; set; }

        /// <summary>
        /// Gets or sets the process termination information as <see cref="PostMortemProcessInfo"/>
        /// </summary>
        /// <remarks>
        /// Please note that the process info might be null, as the handle might already been destroyed
        /// </remarks>
        public PostMortemProcessInfo? Process { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the post mortem information as <see cref="Utc"/>
        /// </summary>
        public DateTime Utc { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the post mortem information as <see cref="Local"/>
        /// </summary>
        public DateTime Local { get; set; }
    }
}
