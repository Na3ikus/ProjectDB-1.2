using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolApp
{
    static class Menu_Helper
    {
        public static void ShowMenu(string title, Dictionary<string, Action> options)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"╔══════════════════════════════════════╗");
                Console.WriteLine($"║        {title.PadRight(30)}║");
                Console.WriteLine($"╚══════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                int i = 1;
                foreach (var option in options)
                {
                    Console.WriteLine($"{i}. {option.Key}");
                    i++;
                }
                Console.WriteLine("0. Назад");
                Console.ResetColor();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Виберіть опцію: ");
                Console.ResetColor();

                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= options.Count)
                {
                    if (choice == 0) break;
                    options[options.Keys.ToArray()[choice - 1]].Invoke();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Невірний вибір. Натисніть будь-яку клавішу, щоб спробувати ще раз...");
                    Console.ResetColor();
                    Console.ReadKey();
                }
            }
        }
    }
}
