using DatabaseHelpers;

namespace SchoolApp
{
    static class Attendance_Manager
    {
        public static void Menu()
        {
            string[] menuItems = {
            "Переглянути відвідуваність",
            "Додати запис про відвідуваність",
            "Редагувати запис про відвідуваність",
            "Видалити запис про відвідуваність",
            "Повернутися до головного меню"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║          Управління відвідуваністю                 ║");
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
                        Console.WriteLine($"║ ► {menuItems[i].PadRight(45)}  ◄ ║");
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
                        selectedIndex = selectedIndex > 0 ? selectedIndex - 1 : menuItems.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = selectedIndex < menuItems.Length - 1 ? selectedIndex + 1 : 0;
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
                    ViewAttendance();
                    break;
                case 1:
                    AddAttendance();
                    break;
                case 2:
                    EditAttendance();
                    break;
                case 3:
                    DeleteAttendance();
                    break;
                case 4:
                    return;
            }
        }

        private static void ViewAttendance()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Перегляд відвідуваності                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT a.attendance_id, p.fname, p.name, p.sname, 
                   s.name AS subject_name, a.attendance_date, a.status 
            FROM Attendance a
            JOIN Students st ON a.student_id = st.student_id
            JOIN Person p ON st.person_id = p.person_id
            JOIN Schedules sc ON a.schedule_id = sc.schedule_id
            JOIN Subjects s ON sc.subject_id = s.subject_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Запис #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID запису: {reader["attendance_id"]}");
                        Console.WriteLine($"║ Учень: {reader["fname"]} {reader["name"]} {reader["sname"]}");
                        Console.WriteLine($"║ Предмет: {reader["subject_name"]}");
                        Console.WriteLine($"║ Дата: {reader["attendance_date"]}");
                        Console.WriteLine($"║ Статус: {reader["status"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про відвідуваність.");
                        Console.ResetColor();
                    }
                });

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╠════════════════════════════════════════════════════╣");
            Console.WriteLine("║ Натисніть будь-яку клавішу, щоб продовжити...      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }


        private static void AddAttendance()
        {
            Console.Write("Введіть ID учня: ");
            string studentId = Console.ReadLine();
            Console.Write("Введіть ID розкладу: ");
            string scheduleId = Console.ReadLine();
            Console.Write("Введіть дату (РРРР-ММ-ДД): ");
            string attendanceDate = Console.ReadLine();
            Console.Write("Введіть статус (Present/Absent/Late/Excused): ");
            string status = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Attendance (student_id, schedule_id, attendance_date, status) " +
                "VALUES (@studentId, @scheduleId, @attendanceDate, @status)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@scheduleId", scheduleId);
                    cmd.Parameters.AddWithValue("@attendanceDate", attendanceDate);
                    cmd.Parameters.AddWithValue("@status", status);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Запис про відвідуваність успішно додано! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditAttendance()
        {
            Console.Write("Введіть ID запису про відвідуваність для редагування: ");
            string attendanceId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Новий статус (Present/Absent/Late/Excused): ");
            string status = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(status))
            {
                Database_Helper.ExecuteQuery(
                    "UPDATE Attendance SET status = @status WHERE attendance_id = @attendanceId",
                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@attendanceId", attendanceId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Запис про відвідуваність успішно оновлено! Натисніть будь-яку клавішу, щоб продовжити...");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Оновлення не виконано. Натисніть будь-яку клавішу, щоб продовжити...");
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        private static void DeleteAttendance()
        {
            Console.Write("Введіть ID запису про відвідуваність для видалення: ");
            string attendanceId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "DELETE FROM Attendance WHERE attendance_id = @attendanceId",
                cmd => cmd.Parameters.AddWithValue("@attendanceId", attendanceId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Запис про відвідуваність успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
