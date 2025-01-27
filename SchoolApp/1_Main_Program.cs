using System.Text;
using SchoolApp;

namespace SchoolApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            string[] menuItems = {
            "Управління учнями",
            "Управління вчителями",
            "Управління адміністрацією",
            "Управління розкладом",
            "Управління оцінками",
            "Управління відвідуваністю",
            "Вихід"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║            Система управління школою               ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ ► {menuItems[i].PadRight(45)} ◄  ║");
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║   {menuItems[i].PadRight(45)}    ║");
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  Використовуйте стрілки вгору/вниз для вибору та Enter для підтвердження.");
                Console.ResetColor();

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : menuItems.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex < menuItems.Length - 1) ? selectedIndex + 1 : 0;
                        break;
                    case ConsoleKey.Enter:
                        HandleMenuSelection(selectedIndex);
                        if (selectedIndex == menuItems.Length - 1) return; 
                        break;
                }
            }
        }

        private static void HandleMenuSelection(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:
                    Student_Manager.Menu();
                    break;
                case 1:
                    Teacher_Manager.Menu();
                    break;
                case 2:
                    Admin_Manager.Menu();
                    break;
                case 3:
                    Schedule_Manager.Menu();
                    break;
                case 4:
                    Grade_Manager.Menu();
                    break;
                case 5:
                    Attendance_Manager.Menu();
                    break;
                case 6:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("╔════════════════════════════════════════════════════╗");
                    Console.WriteLine("║                  До побачення!                     ║");
                    Console.WriteLine("╚════════════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Environment.Exit(0);
                    break;
            }
        }
    }  
}