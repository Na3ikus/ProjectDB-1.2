using DatabaseHelpers;

namespace SchoolApp
{
    static class Grade_Manager
    {
        public static void Menu()
        {
            string[] menuItems = {
            "Список оцінок",
            "Додати оцінку",
            "Редагувати оцінку",
            "Видалити оцінку",
            "Повернутися до головного меню"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║              Управління оцінками                   ║");
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
                    ListGrades();
                    break;
                case 1:
                    AddGrade();
                    break;
                case 2:
                    EditGrade();
                    break;
                case 3:
                    DeleteGrade();
                    break;
                case 4:
                    return;
            }
        }

        private static void ListGrades()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Список оцінок                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT g.grade_id, p.fname, p.name, p.sname, 
                   s.name AS subject_name, g.grade, g.grade_date, g.comment
            FROM Grades g
            JOIN Students st ON g.student_id = st.student_id
            JOIN Person p ON st.person_id = p.person_id
            JOIN Subjects s ON g.subject_id = s.subject_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Оцінка #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID оцінки: {reader["grade_id"]}");
                        Console.WriteLine($"║ Учень: {reader["fname"]} {reader["name"]} {reader["sname"]}");
                        Console.WriteLine($"║ Предмет: {reader["subject_name"]}");
                        Console.WriteLine($"║ Оцінка: {reader["grade"]}");
                        Console.WriteLine($"║ Дата: {reader["grade_date"]}");
                        Console.WriteLine($"║ Коментар: {reader["comment"] ?? "Немає коментаря"}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про оцінки.");
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


        private static void AddGrade()
        {
            Console.Write("Введіть ID учня: ");
            string studentId = Console.ReadLine();
            Console.Write("Введіть ID предмета: ");
            string subjectId = Console.ReadLine();
            Console.Write("Введіть оцінку (0-10): ");
            string grade = Console.ReadLine();
            Console.Write("Введіть дату оцінки (РРРР-ММ-ДД): ");
            string gradeDate = Console.ReadLine();
            Console.Write("Введіть коментар (необов'язково): ");
            string comment = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Grades (student_id, subject_id, grade, grade_date, comment) " +
                "VALUES (@studentId, @subjectId, @grade, @gradeDate, NULLIF(@comment, ''))",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@subjectId", subjectId);
                    cmd.Parameters.AddWithValue("@grade", grade);
                    cmd.Parameters.AddWithValue("@gradeDate", gradeDate);
                    cmd.Parameters.AddWithValue("@comment", comment);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Оцінку успішно додано! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditGrade()
        {
            Console.Write("Введіть ID оцінки для редагування: ");
            string gradeId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Нова оцінка (0-10): ");
            string grade = Console.ReadLine();
            Console.Write("Нова дата оцінки (РРРР-ММ-ДД): ");
            string gradeDate = Console.ReadLine();
            Console.Write("Новий коментар: ");
            string comment = Console.ReadLine();

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(grade)) updates.Add("grade = @grade");
            if (!string.IsNullOrWhiteSpace(gradeDate)) updates.Add("grade_date = @gradeDate");
            if (!string.IsNullOrWhiteSpace(comment)) updates.Add("comment = @comment");

            if (updates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $"UPDATE Grades SET {string.Join(", ", updates)} WHERE grade_id = @gradeId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(grade))
                            cmd.Parameters.AddWithValue("@grade", grade);
                        if (!string.IsNullOrWhiteSpace(gradeDate))
                            cmd.Parameters.AddWithValue("@gradeDate", gradeDate);
                        if (!string.IsNullOrWhiteSpace(comment))
                            cmd.Parameters.AddWithValue("@comment", comment);
                        cmd.Parameters.AddWithValue("@gradeId", gradeId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Оцінку успішно оновлено! Натисніть будь-яку клавішу, щоб продовжити...");
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

        private static void DeleteGrade()
        {
            Console.Write("Введіть ID оцінки для видалення: ");
            string gradeId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "DELETE FROM Grades WHERE grade_id = @gradeId",
                cmd => cmd.Parameters.AddWithValue("@gradeId", gradeId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Оцінку успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
