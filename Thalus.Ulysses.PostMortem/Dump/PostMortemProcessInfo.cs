using System.Diagnostics;

namespace Thalus.Ulysses.PostMortem.Dump
{
    /// <summary>
    /// Implements the post mortem information of a terminating process
    /// </summary>
    public class PostMortemProcessInfo
    {
        /// <summary>
        /// Gets or sets the process name of from <see cref="Process.GetCurrentProcess"/>
        /// </summary>
        public string? ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the process id form <see cref="Process.GetCurrentProcess"/>
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the exit code from <see cref="Process.GetCurrentProcess"/> 
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Gets or set the process start time from <see cref="Process.GetCurrentProcess"/>
        /// </summary>
        public DateTime StartTime { get; set; }
    }
}
