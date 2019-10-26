using ConsoleUI.Pages;
using Services;

namespace ConsoleUI
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var pm = new PageManager();
            var main = new MainPage(new InfoService());
            var update = new UpdatePage(new UpdateService());
            pm.RegisterPage(main);
            pm.RegisterPage(update);
            pm.Init();
            while (!pm.Render())
            {
                //
            }
        }
    }
}