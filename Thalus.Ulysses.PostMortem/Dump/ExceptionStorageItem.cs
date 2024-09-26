using System.Diagnostics;

namespace Thalus.Ulysses.PostMortem.Dump
{
    /// <summary>
    /// Implements exception storage information e.g. like Stacktrace
    /// or is terminating
    /// </summary>
    public class ExceptionStorageItem
    {
        /// <summary>
        /// Gets or sets the exception that caused the storage item as <see cref="Exception"/>
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the Stack Trace of the execution context. 
        /// </summary>
        /// <remarks>
        /// A final exception that is terminating the process might have a different
        /// execution context and therefore a different stack trace
        /// </remarks>
        public string? ExecutionStacktrace { get; set; }

        /// <summary>
        /// Gets or sets the Stack Trace of the exception stored in <see cref="Exception"/>
        /// as <see cref="string"/>
        /// </summary>
        public string? ExceptionStackTrace { get; set; }

        /// <summary>
        /// Gets or sets the flag if the exceptions is suppose to terminate the process
        /// it was thrown in
        /// </summary>
        public bool IsTerminating { get; set; }

        /// <summary>
        /// Gets or sets the Timestamp when the exception occured as <see cref="Utc"/>
        /// </summary>
        public DateTime Utc { get; set; }

        /// <summary>
        /// Gets or sets the Timestamp when the exception occured as <see cref="Local"/>
        /// </summary>
        public DateTime Local { get; set; }        
    }
}
