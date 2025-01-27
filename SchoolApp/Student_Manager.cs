using DatabaseHelpers;

namespace SchoolApp
{
    static class Student_Manager
    {
        public static void Menu()
        {
            string[] menuItems = {
            "Список усіх учнів",
            "Додати нового учня",
            "Редагувати учня",
            "Видалити учня",
            "Повернутися до головного меню"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║              Управління учнями                     ║");
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
                    ListStudents();
                    break;
                case 1:
                    AddStudent();
                    break;
                case 2:
                    EditStudent();
                    break;
                case 3:
                    DeleteStudent();
                    break;
                case 4:
                    return;
            }
        }

        private static void ListStudents()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║                Список усіх учнів                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT s.student_id, p.fname, p.name, p.sname, s.class_id 
            FROM Students s
            JOIN Person p ON s.person_id = p.person_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Учень #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID учня: {reader["student_id"]}");
                        Console.WriteLine($"║ Ім'я: {reader["fname"]} {reader["name"]} {reader["sname"]}");
                        Console.WriteLine($"║ ID класу: {reader["class_id"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про учнів.");
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


        private static void AddStudent()
        {
            Console.Write("Введіть ім'я: ");
            string fname = Console.ReadLine();
            Console.Write("Введіть прізвище: ");
            string name = Console.ReadLine();
            Console.Write("Введіть по батькові: ");
            string sname = Console.ReadLine();
            Console.Write("Введіть дату народження (РРРР-ММ-ДД): ");
            string birthDate = Console.ReadLine();
            Console.Write("Введіть стать (0/1): ");
            string sex = Console.ReadLine();
            Console.Write("Введіть ID класу: ");
            string classId = Console.ReadLine();

            try
            {
                Database_Helper.ExecuteQuery(
                    @"INSERT INTO Person (fname, name, sname, when_born, sex) 
              VALUES (@fname, @name, @sname, @birthDate, @sex);
              INSERT INTO Students (person_id, class_id) 
              VALUES (LAST_INSERT_ID(), @classId)",
                    cmd =>
                    {
                        cmd.Parameters.AddWithValue("@fname", fname);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@sname", sname);
                        cmd.Parameters.AddWithValue("@birthDate", birthDate);
                        cmd.Parameters.AddWithValue("@sex", sex);
                        cmd.Parameters.AddWithValue("@classId", classId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Учень успішно доданий! Натисніть будь-яку клавішу, щоб продовжити...");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка при додаванні учня: {ex.Message}");
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        private static void EditStudent()
        {
            Console.Write("Введіть ID учня для редагування: ");
            string studentId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Нове ім'я: ");
            string fname = Console.ReadLine();
            Console.Write("Нове прізвище: ");
            string name = Console.ReadLine();
            Console.Write("Нове по батькові: ");
            string sname = Console.ReadLine();
            Console.Write("Новий ID класу: ");
            string classId = Console.ReadLine();

            var personUpdates = new List<string>();
            var studentUpdates = new List<string>();

            if (!string.IsNullOrWhiteSpace(fname)) personUpdates.Add("fname = @fname");
            if (!string.IsNullOrWhiteSpace(name)) personUpdates.Add("name = @name");
            if (!string.IsNullOrWhiteSpace(sname)) personUpdates.Add("sname = @sname");
            if (!string.IsNullOrWhiteSpace(classId)) studentUpdates.Add("class_id = @classId");

            if (personUpdates.Count > 0 || studentUpdates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $@"{(personUpdates.Count > 0 ?
                        $"UPDATE Person p JOIN Students s ON p.person_id = s.person_id SET {string.Join(", ", personUpdates)} WHERE s.student_id = @studentId;" : "")}
                {(studentUpdates.Count > 0 ?
                        $"UPDATE Students SET {string.Join(", ", studentUpdates)} WHERE student_id = @studentId;" : "")}",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(fname)) cmd.Parameters.AddWithValue("@fname", fname);
                        if (!string.IsNullOrWhiteSpace(name)) cmd.Parameters.AddWithValue("@name", name);
                        if (!string.IsNullOrWhiteSpace(sname)) cmd.Parameters.AddWithValue("@sname", sname);
                        if (!string.IsNullOrWhiteSpace(classId)) cmd.Parameters.AddWithValue("@classId", classId);
                        cmd.Parameters.AddWithValue("@studentId", studentId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Учень успішно оновлений! Натисніть будь-яку клавішу, щоб продовжити...");
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

        private static void DeleteStudent()
        {
            Console.Write("Введіть ID учня для видалення: ");
            string studentId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                @"DELETE FROM Attendance WHERE student_id = @studentId;
          DELETE FROM Grades WHERE student_id = @studentId;
          DELETE FROM Students WHERE student_id = @studentId;
          DELETE FROM Person WHERE person_id = (
              SELECT person_id FROM Students WHERE student_id = @studentId
          )",
                cmd => cmd.Parameters.AddWithValue("@studentId", studentId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Учень та пов'язані записи успішно видалені! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
