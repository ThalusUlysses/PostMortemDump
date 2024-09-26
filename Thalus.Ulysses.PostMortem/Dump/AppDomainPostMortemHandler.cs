using System.Diagnostics;
using System.Reflection;

namespace Thalus.Ulysses.PostMortem.Dump
{
    /// <summary>
    /// Implemets a AppDomain post mortem handler
    /// </summary>
    public class AppDomainPostMortemHandler
    {
        AppDomain? _appDomain;

        Process _process;

        const int MAX_FIRST_CHANCE_EX_TO_KEEP = 50;
        int _maxFirstHandExceptionsToKeep = MAX_FIRST_CHANCE_EX_TO_KEEP;
        List<ExceptionStorageItem> _firstHandExceptions = new List<ExceptionStorageItem>();
        UnhandledExceptionEventArgs? _unhandledException;
        string _location;

        /// <summary>
        /// Creates an instance of <see cref="AppDomainPostMortemHandler"/> with passed parameters
        /// </summary>
        /// <param name="appDomain">Pass the app domain to listen to event</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AppDomainPostMortemHandler(AppDomain appDomain)
        {
            if (appDomain == null)
            {
                throw new ArgumentNullException(nameof(appDomain), $"The passed parameter for the AppDomain MUST not be null");
            }

            _appDomain = appDomain;
            _process = Process.GetCurrentProcess();
            
            var asm = Assembly.GetExecutingAssembly();
            var asmName = asm.GetName();
            
            _location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), asmName?.Name ?? "UNKNOWN", asmName?.Version?.ToString() ?? "UNKNOWN");
        }
    
        private void OnHandleProcessExit(object? sender, EventArgs e)
        {
            AttachDetachUnhandledException(_appDomain);
            AttachDetachFirstChanceExceptions(_appDomain);
            AttachDetachProcessExit(_appDomain);
        }

        private void OnHandleFirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            var dtUtc = DateTime.UtcNow;

            var exStorageItem = new ExceptionStorageItem
            {
                Exception = e.Exception,
                Utc = dtUtc,
                Local = dtUtc.ToLocalTime(),
                ExceptionStackTrace = new StackTrace(e.Exception, true).ToString()
            };

            if (_firstHandExceptions.Count >= _maxFirstHandExceptionsToKeep)
            {
                _firstHandExceptions.RemoveAt(0);
            }

            _firstHandExceptions.Add(exStorageItem);
        }

        private void OnHandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _unhandledException = e;
            var utcNow = DateTime.UtcNow;

            if (!e.IsTerminating)
            {
                return;
            }

            var exceptionObj = e?.ExceptionObject as Exception;

            var exStorage = new ExceptionStorageItem
            {
                Exception = e?.ExceptionObject as Exception,
                Utc = utcNow,
                Local = utcNow.ToLocalTime(),
                ExceptionStackTrace = exceptionObj != null ? new StackTrace(exceptionObj, true).ToString() : null,
                ExecutionStacktrace = new StackTrace(true).ToString(),
                IsTerminating = e.IsTerminating
            };

            var writer = new PostMortemFileWriter(_location);
            var info = new PostMortemInfo
            {
                UnhandledException = exStorage,
                LastFirstChanceExceptions = _firstHandExceptions,
                Process = new PostMortemProcessInfo
                {
                    ExitCode = Environment.ExitCode,
                    ProcessId = _process.Id,
                    ProcessName = _process.ProcessName,
                    StartTime = _process.StartTime
                },
                Utc = utcNow,
                Local = utcNow.ToLocalTime()
            };

            writer.Write(info);

            AttachDetachUnhandledException(_appDomain);
            AttachDetachFirstChanceExceptions(_appDomain);
            AttachDetachProcessExit(_appDomain);
        }

        /// <summary>
        /// Enable logging of unhandled exception data using <see cref="AppDomain.UnhandledException"/>
        /// </summary>
        /// <returns></returns>
        public AppDomainPostMortemHandler LogUnhandledException()
        {
            AttachDetachUnhandledException(_appDomain, true);
            return this;
        }

        /// <summary>
        /// Enables logging of process exit data using <see cref="AppDomain.ProcessExit"/>
        /// </summary>
        /// <returns></returns>
        public AppDomainPostMortemHandler LogExitData()
        {
            AttachDetachProcessExit(_appDomain, true);
            return this;
        }

        /// <summary>
        /// Enable logging of first chance exceptions using <see cref="AppDomain.FirstChanceException"/>
        /// </summary>
        /// <param name="maxExceptionsToKeep">Pass a max exceptions to be stored for logging. Default is <see cref="MAX_FIRST_CHANCE_EX_TO_KEEP"/></param>
        /// <returns></returns>
        public AppDomainPostMortemHandler LogFirstChanceExceptions(int maxExceptionsToKeep=MAX_FIRST_CHANCE_EX_TO_KEEP)
        {
            _maxFirstHandExceptionsToKeep = maxExceptionsToKeep;
            AttachDetachFirstChanceExceptions(_appDomain, true);
            return this;
        }

        void AttachDetachFirstChanceExceptions(AppDomain? appDomain, bool attach=false)
        {

            if (appDomain == null)
            {
                return;
            }

            appDomain.FirstChanceException -= OnHandleFirstChanceException;

            if (attach)
            {
                appDomain.FirstChanceException += OnHandleFirstChanceException;                
            }
        }

        void AttachDetachUnhandledException(AppDomain? appDomain, bool attach = false)
        {

            if (appDomain == null)
            {
                return;
            }

            appDomain.UnhandledException -= OnHandleUnhandledException;

            if (attach)
            {
                appDomain.UnhandledException += OnHandleUnhandledException;
            }
        }

        void AttachDetachProcessExit(AppDomain? appDomain, bool attach = false)
        {

            if (appDomain == null)
            {
                return;
            }

            appDomain.ProcessExit -= OnHandleProcessExit;

            if (attach)
            {
                appDomain.ProcessExit += OnHandleProcessExit;
            }
        }
    }
}
