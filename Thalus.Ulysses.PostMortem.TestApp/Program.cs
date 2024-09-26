using Newtonsoft.Json;
using System.Diagnostics;
using Thalus.Ulysses.PostMortem.Dump;

namespace Thalus.Ulysses.PostMortem.TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var appDOmainListener = new AppDomainPostMortemHandler(AppDomain.CurrentDomain);
            appDOmainListener.LogUnhandledException();
            appDOmainListener.LogFirstChanceExceptions();
            appDOmainListener.LogExitData();

            for (int i = 0; i < 55; i++)
            {
                FirstHandException();
            }

            throw new Exception("Final");
        }

        static void FirstHandException()
        {
            try
            {
                throw new Exception("BlaBlaBla");
            }
            catch (Exception)
            {
            }
        }
    }

    public class LastResortAppDomainHandler
    {
        AppDomain _appDomain;
        string _lastResortPath;

        public LastResortAppDomainHandler(AppDomain appDomain, string lastResortPath)
        {
            if (appDomain == null)
            {
                appDomain = AppDomain.CurrentDomain;
            }

            if (string.IsNullOrEmpty(lastResortPath))
            {
                throw new ArgumentNullException(nameof(lastResortPath), $"Path not provided to log last resort issues for {nameof(LastResortAppDomainHandler)}");
            }
            _lastResortPath = lastResortPath;
            _appDomain = appDomain;
        }

        public LastResortAppDomainHandler LogUnhandledException()
        {
            AttachDetachUnhandledExceptionHandler(_appDomain, true);
            return this;
        }

        public LastResortAppDomainHandler LogExitData()
        {
            AttachDetachExitHandler(_appDomain, true);
            return this;
        }

        private void AttachDetachUnhandledExceptionHandler(AppDomain appDomain, bool attach = false)
        {
            if (appDomain == null)
            {
                return;
            }

            appDomain.UnhandledException -= AppDomain_UnhandledException;

            if (attach)
            {
                appDomain.UnhandledException += AppDomain_UnhandledException;
            }
        }


        private void AttachDetachExitHandler(AppDomain appDomain, bool attach = false)
        {
            if (appDomain == null)
            {
                return;
            }

            appDomain.ProcessExit -= AppDomain_ProcessExit;

            if (attach)
            {
                appDomain.ProcessExit += AppDomain_ProcessExit;
            }
        }

        private void AppDomain_ProcessExit(object sender, EventArgs e)
        {
            if (sender == this)
            {
                return;
            }
            var dt = DateTime.UtcNow;

            var dict = new Dictionary<string, object>
            {
                {"EventArgs",e },
                {"ProcessTerminating",true },
                {"Sender",sender},
                { "Utc", dt},
                { "Local", dt.ToLocalTime()}
            };

            var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            var pth = Path.Combine(_lastResortPath, $"ProcessExitPostMortemFrom_{dt.Ticks}.json");
            File.WriteAllText(pth, json);
        }

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (sender == this)
            {
                return;
            }
            var dt = DateTime.UtcNow;

            var st = new StackTrace(e.ExceptionObject as Exception, true);
            var exSt = new StackTrace(true);

            var dict = new Dictionary<string, object>
            {
                {"Exception",e.ExceptionObject },
                {"ProcessTerminating",e.IsTerminating },
                {"Sender",sender},
                { "Utc", dt},
                { "Local", dt.ToLocalTime()},
                { "ExceptionStackTrace", st.ToString()},
                { "AppDomainStackTract", exSt.ToString()},
            };

            var json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            var pth = Path.Combine(_lastResortPath, $"UnhandledExceptionPostMortemFrom_{dt.Ticks}.json");
            File.WriteAllText(pth, json);
        }
    }
}
