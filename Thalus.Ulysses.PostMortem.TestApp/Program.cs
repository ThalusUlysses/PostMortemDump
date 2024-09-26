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

   
}
