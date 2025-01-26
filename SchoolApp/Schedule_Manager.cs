using DatabaseHelpers;

namespace SchoolApp
{
    static class Schedule_Manager
    {
        public static void Menu()
        {
            string[] menuItems = {
        "Список розкладів учнів",
        "Список розкладів вчителів",
        "Додати розклад для учня",
        "Додати розклад для вчителя",
        "Список предметів",
        "Додати предмет",
        "Редагувати предмет",
        "Видалити предмет",
        "Повернутися до головного меню"
    };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║              Управління розкладом                  ║");
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
                    ListStudentSchedules();
                    break;
                case 1:
                    ListTeacherSchedules();
                    break;
                case 2:
                    AddStudentSchedule();
                    break;
                case 3:
                    AddTeacherSchedule();
                    break;
                case 4:
                    ListSubjects();
                    break;
                case 5:
                    AddSubject();
                    break;
                case 6:
                    EditSubject();
                    break;
                case 7:
                    DeleteSubject();
                    break;
                case 8:
                    return;
            }
        }

        private static void ListStudentSchedules()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Список розкладів учнів               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT s.schedule_id, c.number AS class_number, 
                   sub.name AS subject_name, 
                   s.day_of_week, s.start_time, s.end_time
            FROM Schedules s
            JOIN Classes c ON s.class_id = c.class_id
            JOIN Subjects sub ON s.subject_id = sub.subject_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Розклад #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID розкладу: {reader["schedule_id"]}");
                        Console.WriteLine($"║ Клас: {reader["class_number"]}");
                        Console.WriteLine($"║ Предмет: {reader["subject_name"]}");
                        Console.WriteLine($"║ День: {reader["day_of_week"]}");
                        Console.WriteLine($"║ Час: {reader["start_time"]} - {reader["end_time"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про розклади учнів.");
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


        private static void ListTeacherSchedules()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Список розкладів вчителів             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
        SELECT st.schedule_id, p.fname, p.name, p.sname, 
               sub.name AS subject_name, 
               st.day_of_week, st.start_time, st.end_time, st.room
        FROM Schedules_Teachers st
        JOIN Teachers t ON st.teacher_id = t.teacher_id
        JOIN Person p ON t.person_id = p.person_id
        JOIN Subjects sub ON st.subject_id = sub.subject_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Розклад #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID розкладу: {reader["schedule_id"]}");
                        Console.WriteLine($"║ Вчитель: {reader["fname"]} {reader["name"]} {reader["sname"]}");
                        Console.WriteLine($"║ Предмет: {reader["subject_name"]}");
                        Console.WriteLine($"║ День: {reader["day_of_week"]}");
                        Console.WriteLine($"║ Час: {reader["start_time"]} - {reader["end_time"]}");
                        Console.WriteLine($"║ Аудиторія: {reader["room"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про розклади вчителів.");
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

        private static void AddStudentSchedule()
        {
            Console.Write("Введіть ID класу: ");
            string classId = Console.ReadLine();
            Console.Write("Введіть ID предмета: ");
            string subjectId = Console.ReadLine();
            Console.Write("Введіть день тижня: ");
            string dayOfWeek = Console.ReadLine();
            Console.Write("Введіть час початку (ГГ:ХХ): ");
            string startTime = Console.ReadLine();
            Console.Write("Введіть час закінчення (ГГ:ХХ): ");
            string endTime = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Schedules (class_id, subject_id, day_of_week, start_time, end_time) " +
                "VALUES (@classId, @subjectId, @dayOfWeek, @startTime, @endTime)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@classId", classId);
                    cmd.Parameters.AddWithValue("@subjectId", subjectId);
                    cmd.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                    cmd.Parameters.AddWithValue("@startTime", startTime);
                    cmd.Parameters.AddWithValue("@endTime", endTime);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Розклад для учня успішно додано! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void AddTeacherSchedule()
        {
            Console.Write("Введіть ID вчителя: ");
            string teacherId = Console.ReadLine();
            Console.Write("Введіть ID предмета: ");
            string subjectId = Console.ReadLine();
            Console.Write("Введіть день тижня (1-7): ");
            string dayOfWeek = Console.ReadLine();
            Console.Write("Введіть час початку (ГГ:ХХ): ");
            string startTime = Console.ReadLine();
            Console.Write("Введіть час закінчення (ГГ:ХХ): ");
            string endTime = Console.ReadLine();
            Console.Write("Введіть аудиторію: ");
            string room = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Schedules_Teachers (teacher_id, subject_id, day_of_week, start_time, end_time, room) " +
                "VALUES (@teacherId, @subjectId, @dayOfWeek, @startTime, @endTime, @room)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);
                    cmd.Parameters.AddWithValue("@subjectId", subjectId);
                    cmd.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                    cmd.Parameters.AddWithValue("@startTime", startTime);
                    cmd.Parameters.AddWithValue("@endTime", endTime);
                    cmd.Parameters.AddWithValue("@room", room);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Розклад для вчителя успішно додано! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditStudentSchedule()
        {
            Console.Write("Введіть ID розкладу учня для редагування: ");
            string scheduleId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Новий ID класу: ");
            string classId = Console.ReadLine();
            Console.Write("Новий ID предмета: ");
            string subjectId = Console.ReadLine();
            Console.Write("Новий день тижня: ");
            string dayOfWeek = Console.ReadLine();
            Console.Write("Новий час початку (ГГ:ХХ): ");
            string startTime = Console.ReadLine();
            Console.Write("Новий час закінчення (ГГ:ХХ): ");
            string endTime = Console.ReadLine();

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(classId)) updates.Add("class_id = @classId");
            if (!string.IsNullOrWhiteSpace(subjectId)) updates.Add("subject_id = @subjectId");
            if (!string.IsNullOrWhiteSpace(dayOfWeek)) updates.Add("day_of_week = @dayOfWeek");
            if (!string.IsNullOrWhiteSpace(startTime)) updates.Add("start_time = @startTime");
            if (!string.IsNullOrWhiteSpace(endTime)) updates.Add("end_time = @endTime");

            if (updates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $"UPDATE Schedules SET {string.Join(", ", updates)} WHERE schedule_id = @scheduleId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(classId)) cmd.Parameters.AddWithValue("@classId", classId);
                        if (!string.IsNullOrWhiteSpace(subjectId)) cmd.Parameters.AddWithValue("@subjectId", subjectId);
                        if (!string.IsNullOrWhiteSpace(dayOfWeek)) cmd.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                        if (!string.IsNullOrWhiteSpace(startTime)) cmd.Parameters.AddWithValue("@startTime", startTime);
                        if (!string.IsNullOrWhiteSpace(endTime)) cmd.Parameters.AddWithValue("@endTime", endTime);
                        cmd.Parameters.AddWithValue("@scheduleId", scheduleId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Розклад учня успішно оновлено! Натисніть будь-яку клавішу, щоб продовжити...");
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

        private static void EditTeacherSchedule()
        {
            Console.Write("Введіть ID розкладу вчителя для редагування: ");
            string scheduleId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Новий ID вчителя: ");
            string teacherId = Console.ReadLine();
            Console.Write("Новий ID предмета: ");
            string subjectId = Console.ReadLine();
            Console.Write("Новий день тижня (1-7): ");
            string dayOfWeek = Console.ReadLine();
            Console.Write("Новий час початку (ГГ:ХХ): ");
            string startTime = Console.ReadLine();
            Console.Write("Новий час закінчення (ГГ:ХХ): ");
            string endTime = Console.ReadLine();
            Console.Write("Нова аудиторія: ");
            string room = Console.ReadLine();

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(teacherId)) updates.Add("teacher_id = @teacherId");
            if (!string.IsNullOrWhiteSpace(subjectId)) updates.Add("subject_id = @subjectId");
            if (!string.IsNullOrWhiteSpace(dayOfWeek)) updates.Add("day_of_week = @dayOfWeek");
            if (!string.IsNullOrWhiteSpace(startTime)) updates.Add("start_time = @startTime");
            if (!string.IsNullOrWhiteSpace(endTime)) updates.Add("end_time = @endTime");
            if (!string.IsNullOrWhiteSpace(room)) updates.Add("room = @room");

            if (updates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $"UPDATE Schedules_Teachers SET {string.Join(", ", updates)} WHERE schedule_id = @scheduleId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(teacherId)) cmd.Parameters.AddWithValue("@teacherId", teacherId);
                        if (!string.IsNullOrWhiteSpace(subjectId)) cmd.Parameters.AddWithValue("@subjectId", subjectId);
                        if (!string.IsNullOrWhiteSpace(dayOfWeek)) cmd.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                        if (!string.IsNullOrWhiteSpace(startTime)) cmd.Parameters.AddWithValue("@startTime", startTime);
                        if (!string.IsNullOrWhiteSpace(endTime)) cmd.Parameters.AddWithValue("@endTime", endTime);
                        if (!string.IsNullOrWhiteSpace(room)) cmd.Parameters.AddWithValue("@room", room);
                        cmd.Parameters.AddWithValue("@scheduleId", scheduleId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Розклад вчителя успішно оновлено! Натисніть будь-яку клавішу, щоб продовжити...");
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

        private static void DeleteStudentSchedule()
        {
            Console.Write("Введіть ID розкладу учня для видалення: ");
            string scheduleId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "DELETE FROM Schedules WHERE schedule_id = @scheduleId",
                cmd => cmd.Parameters.AddWithValue("@scheduleId", scheduleId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Розклад учня успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void DeleteTeacherSchedule()
        {
            Console.Write("Введіть ID розкладу вчителя для видалення: ");
            string scheduleId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "DELETE FROM Schedules_Teachers WHERE schedule_id = @scheduleId",
                cmd => cmd.Parameters.AddWithValue("@scheduleId", scheduleId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Розклад вчителя успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void ListSubjects()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Список предметів                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT subject_id, name, description
            FROM Subjects",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Предмет #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID: {reader["subject_id"]}");
                        Console.WriteLine($"║ Назва: {reader["name"]}");
                        Console.WriteLine($"║ Опис: {reader["description"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про предмети.");
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

        private static void AddSubject()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Додати новий предмет                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть назву предмета: ");
            string name = Console.ReadLine();
            Console.Write("Введіть опис предмета: ");
            string description = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Subjects (name, description) VALUES (@name, @description)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Предмет успішно додано! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditSubject()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Редагувати предмет                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID предмета для редагування: ");
            string subjectId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Нова назва предмета: ");
            string name = Console.ReadLine();
            Console.Write("Новий опис предмета: ");
            string description = Console.ReadLine();

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(name)) updates.Add("name = @name");
            if (!string.IsNullOrWhiteSpace(description)) updates.Add("description = @description");

            if (updates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $"UPDATE Subjects SET {string.Join(", ", updates)} WHERE subject_id = @subjectId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(name)) cmd.Parameters.AddWithValue("@name", name);
                        if (!string.IsNullOrWhiteSpace(description)) cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@subjectId", subjectId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Предмет успішно оновлено! Натисніть будь-яку клавішу, щоб продовжити...");
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

        private static void DeleteSubject()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║              Видалити предмет                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID предмета для видалення: ");
            string subjectId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "DELETE FROM Subjects WHERE subject_id = @subjectId",
                cmd => cmd.Parameters.AddWithValue("@subjectId", subjectId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Предмет успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
