using DatabaseHelpers;

namespace SchoolApp
{
    static class Teacher_Manager
    {
        public static void Menu()
        {
            string[] menuItems = {
            "Список усіх вчителів",
            "Додати нового вчителя",
            "Редагувати вчителя",
            "Видалити вчителя",
            "Повернутися до головного меню"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();

                // Заголовок меню
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║              Управління вчителями                  ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                // Виведення меню з рамками
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

                // Підказка для користувача
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  Використовуйте стрілки вгору/вниз для вибору та Enter для підтвердження.");
                Console.ResetColor();

                // Обробка вводу
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
                    ListTeachers();
                    break;
                case 1:
                    AddTeacher();
                    break;
                case 2:
                    EditTeacher();
                    break;
                case 3:
                    DeleteTeacher();
                    break;
                case 4:
                    return;
            }
        }

        private static void ListTeachers()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║                Список усіх вчителів                ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT t.teacher_id, p.fname, p.name, p.sname, d.name as department, t.position 
            FROM Teachers t 
            JOIN Person p ON t.person_id = p.person_id 
            JOIN Departments d ON t.department_id = d.department_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Вчитель #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID вчителя: {reader["teacher_id"]}");
                        Console.WriteLine($"║ Ім'я: {reader["fname"]} {reader["name"]} {reader["sname"]}");
                        Console.WriteLine($"║ Відділ: {reader["department"]}");
                        Console.WriteLine($"║ Посада: {reader["position"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про вчителів.");
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

        private static void AddTeacher()
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
            Console.Write("Введіть ID відділу: ");
            string departmentId = Console.ReadLine();
            Console.Write("Введіть посаду: ");
            string position = Console.ReadLine();
            Console.Write("Введіть дату прийому на роботу (РРРР-ММ-ДД): ");
            string hireDate = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                @"INSERT INTO Person (fname, name, sname, when_born, sex) 
                  VALUES (@fname, @name, @sname, @birthDate, @sex);
                  INSERT INTO Teachers (person_id, department_id, hire_date, position) 
                  VALUES (LAST_INSERT_ID(), @departmentId, @hireDate, @position)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@sname", sname);
                    cmd.Parameters.AddWithValue("@birthDate", birthDate);
                    cmd.Parameters.AddWithValue("@sex", sex);
                    cmd.Parameters.AddWithValue("@departmentId", departmentId);
                    cmd.Parameters.AddWithValue("@hireDate", hireDate);
                    cmd.Parameters.AddWithValue("@position", position);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Вчитель успішно доданий! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditTeacher()
        {
            Console.Write("Введіть ID вчителя для редагування: ");
            string teacherId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Новий ID відділу: ");
            string departmentId = Console.ReadLine();
            Console.Write("Нова посада: ");
            string position = Console.ReadLine();

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(departmentId)) updates.Add("department_id = @departmentId");
            if (!string.IsNullOrWhiteSpace(position)) updates.Add("position = @position");

            if (updates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $"UPDATE Teachers SET {string.Join(", ", updates)} WHERE teacher_id = @teacherId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(departmentId))
                            cmd.Parameters.AddWithValue("@departmentId", departmentId);
                        if (!string.IsNullOrWhiteSpace(position))
                            cmd.Parameters.AddWithValue("@position", position);
                        cmd.Parameters.AddWithValue("@teacherId", teacherId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Вчитель успішно оновлений! Натисніть будь-яку клавішу, щоб продовжити...");
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

        private static void DeleteTeacher()
        {
            Console.Write("Введіть ID вчителя для видалення: ");
            string teacherId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                @"DELETE FROM schedules_teachers WHERE teacher_id = @teacherId;
          DELETE FROM teacher_subjects WHERE teacher_id = @teacherId;
          DELETE FROM Teachers WHERE teacher_id = @teacherId;
          DELETE FROM Person WHERE person_id = (
              SELECT person_id FROM Teachers WHERE teacher_id = @teacherId
          )",
                cmd => cmd.Parameters.AddWithValue("@teacherId", teacherId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Вчитель та пов'язані записи успішно видалені! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void AssignSubject()
        {
            Console.Write("Введіть ID вчителя: ");
            string teacherId = Console.ReadLine();
            Console.Write("Введіть ID предмета: ");
            string subjectId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Teacher_Subjects (teacher_id, subject_id) VALUES (@teacherId, @subjectId)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);
                    cmd.Parameters.AddWithValue("@subjectId", subjectId);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Предмет успішно призначено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
