using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SchoolManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════╗");
                Console.WriteLine("║        Система управління школою     ║");
                Console.WriteLine("╚══════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("1. Управління учнями");
                Console.WriteLine("2. Управління вчителями");
                Console.WriteLine("3. Управління адміністрацією");
                Console.WriteLine("4. Управління розкладом");
                Console.WriteLine("5. Управління оцінками");
                Console.WriteLine("6. Управління відвідуваністю");
                Console.WriteLine("7. Вихід");
                Console.ResetColor();
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Виберіть опцію: ");
                Console.ResetColor();

                switch (Console.ReadLine())
                {
                    case "1":
                        StudentManager.Menu();
                        break;
                    case "2":
                        TeacherManager.Menu();
                        break;
                    case "3":
                        AdminManager.Menu();
                        break;
                    case "4":
                        ScheduleManager.Menu();
                        break;
                    case "5":
                        GradeManager.Menu();
                        break;
                    case "6":
                        AttendanceManager.Menu();
                        break;
                    case "7":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("До побачення!");
                        Console.ResetColor();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Невірна опція. Натисніть будь-яку клавішу, щоб спробувати ще раз...");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    static class DatabaseHelper
    {
        private static string connectionString = "Server=localhost;Database=School;Uid=root;Pwd=123456789;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static void ExecuteQuery(string query, Action<MySqlCommand> parameterize = null, Action<MySqlDataReader> process = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    parameterize?.Invoke(command);
                    if (process != null)
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            process(reader);
                        }
                    }
                    else
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    static class MenuHelper
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

    static class StudentManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Управління учнями", new Dictionary<string, Action>
            {
                { "Список усіх учнів", ListStudents },
                { "Додати нового учня", AddStudent },
                { "Редагувати учня", EditStudent },
                { "Видалити учня", DeleteStudent }
            });
        }

        private static void ListStudents()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        Список усіх учнів                           ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.ResetColor();

            DatabaseHelper.ExecuteQuery(@"
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
                        Console.WriteLine($"╔════════════════════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Учень #{count}");
                        Console.WriteLine($"╠────────────────────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID учня: {reader["student_id"]}");
                        Console.WriteLine($"║ Ім'я: {reader["fname"]} {reader["name"]} {reader["sname"]}");
                        Console.WriteLine($"║ ID класу: {reader["class_id"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"╚════════════════════════════════════════════════════════════════════╝");
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
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ Натисніть будь-яку клавішу, щоб продовжити...                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
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

            DatabaseHelper.ExecuteQuery(
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
                DatabaseHelper.ExecuteQuery(
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

            // Видаляємо пов'язані записи з таблиць Attendance, Grades і Students
            DatabaseHelper.ExecuteQuery(
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

    static class TeacherManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Управління вчителями", new Dictionary<string, Action>
            {
                { "Список усіх вчителів", ListTeachers },
                { "Додати нового вчителя", AddTeacher },
                { "Редагувати вчителя", EditTeacher },
                { "Видалити вчителя", DeleteTeacher },
                { "Призначити предмет", AssignSubject }
            });
        }

        private static void ListTeachers()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      Список усіх вчителів                          ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.ResetColor();

            DatabaseHelper.ExecuteQuery(@"
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
                        Console.WriteLine($"╔════════════════════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Вчитель #{count}");
                        Console.WriteLine($"╠────────────────────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID вчителя: {reader["teacher_id"]}");
                        Console.WriteLine($"║ Ім'я: {reader["fname"]} {reader["name"]} {reader["sname"]}");
                        Console.WriteLine($"║ Відділ: {reader["department"]}");
                        Console.WriteLine($"║ Посада: {reader["position"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"╚════════════════════════════════════════════════════════════════════╝");
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
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ Натисніть будь-яку клавішу, щоб продовжити...                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
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

            DatabaseHelper.ExecuteQuery(
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
                DatabaseHelper.ExecuteQuery(
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

            // Видаляємо пов'язані записи з таблиць Schedules_Teachers і Teacher_Subjects
            DatabaseHelper.ExecuteQuery(
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

            DatabaseHelper.ExecuteQuery(
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

    static class ScheduleManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Управління розкладом", new Dictionary<string, Action>
            {
                { "Список розкладів учнів", ListStudentSchedules },
                { "Список розкладів вчителів", ListTeacherSchedules },
                { "Додати розклад для учня", AddStudentSchedule },
                                { "Додати розклад для вчителя", AddTeacherSchedule },
                { "Редагувати розклад учня", EditStudentSchedule },
                { "Редагувати розклад вчителя", EditTeacherSchedule },
                { "Видалити розклад учня", DeleteStudentSchedule },
                { "Видалити розклад вчителя", DeleteTeacherSchedule }
            });
        }

        private static void ListStudentSchedules()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT s.schedule_id, c.number AS class_number, 
                       sub.name AS subject_name, 
                       s.day_of_week, s.start_time, s.end_time
                FROM Schedules s
                JOIN Classes c ON s.class_id = c.class_id
                JOIN Subjects sub ON s.subject_id = sub.subject_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID розкладу: {reader["schedule_id"]}, " +
                            $"Клас: {reader["class_number"]}, " +
                            $"Предмет: {reader["subject_name"]}, " +
                            $"День: {reader["day_of_week"]}, " +
                            $"Час: {reader["start_time"]} - {reader["end_time"]}");
                    }
                });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void ListTeacherSchedules()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT st.schedule_id, p.fname, p.name, p.sname, 
                       sub.name AS subject_name, 
                       st.day_of_week, st.start_time, st.end_time, st.room
                FROM Schedules_Teachers st
                JOIN Teachers t ON st.teacher_id = t.teacher_id
                JOIN Person p ON t.person_id = p.person_id
                JOIN Subjects sub ON st.subject_id = sub.subject_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID розкладу: {reader["schedule_id"]}, " +
                            $"Вчитель: {reader["fname"]} {reader["name"]} {reader["sname"]}, " +
                            $"Предмет: {reader["subject_name"]}, " +
                            $"День: {reader["day_of_week"]}, " +
                            $"Час: {reader["start_time"]} - {reader["end_time"]}, " +
                            $"Аудиторія: {reader["room"]}");
                    }
                });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
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

            DatabaseHelper.ExecuteQuery(
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

            DatabaseHelper.ExecuteQuery(
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
                DatabaseHelper.ExecuteQuery(
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
                DatabaseHelper.ExecuteQuery(
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

            DatabaseHelper.ExecuteQuery(
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

            DatabaseHelper.ExecuteQuery(
                "DELETE FROM Schedules_Teachers WHERE schedule_id = @scheduleId",
                cmd => cmd.Parameters.AddWithValue("@scheduleId", scheduleId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Розклад вчителя успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }

    static class GradeManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Управління оцінками", new Dictionary<string, Action>
            {
                { "Список оцінок", ListGrades },
                { "Додати оцінку", AddGrade },
                { "Редагувати оцінку", EditGrade },
                { "Видалити оцінку", DeleteGrade }
            });
        }

        private static void ListGrades()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT g.grade_id, p.fname, p.name, p.sname, 
                       s.name AS subject_name, g.grade, g.grade_date, g.comment
                FROM Grades g
                JOIN Students st ON g.student_id = st.student_id
                JOIN Person p ON st.person_id = p.person_id
                JOIN Subjects s ON g.subject_id = s.subject_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID оцінки: {reader["grade_id"]}, " +
                            $"Учень: {reader["fname"]} {reader["name"]} {reader["sname"]}, " +
                            $"Предмет: {reader["subject_name"]}, " +
                            $"Оцінка: {reader["grade"]}, " +
                            $"Дата: {reader["grade_date"]}, " +
                            $"Коментар: {reader["comment"] ?? "Немає коментаря"}");
                    }
                });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
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

            DatabaseHelper.ExecuteQuery(
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
                DatabaseHelper.ExecuteQuery(
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

            DatabaseHelper.ExecuteQuery(
                "DELETE FROM Grades WHERE grade_id = @gradeId",
                cmd => cmd.Parameters.AddWithValue("@gradeId", gradeId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Оцінку успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }

    static class AttendanceManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Управління відвідуваністю", new Dictionary<string, Action>
            {
                { "Переглянути відвідуваність", ViewAttendance },
                { "Додати запис про відвідуваність", AddAttendance },
                { "Редагувати запис про відвідуваність", EditAttendance },
                { "Видалити запис про відвідуваність", DeleteAttendance }
            });
        }

        private static void ViewAttendance()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT a.attendance_id, p.fname, p.name, p.sname, s.name AS subject_name, 
                       a.attendance_date, a.status 
                FROM Attendance a
                JOIN Students st ON a.student_id = st.student_id
                JOIN Person p ON st.person_id = p.person_id
                JOIN Schedules sc ON a.schedule_id = sc.schedule_id
                JOIN Subjects s ON sc.subject_id = s.subject_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID запису: {reader["attendance_id"]}, " +
                            $"Учень: {reader["fname"]} {reader["name"]} {reader["sname"]}, " +
                            $"Предмет: {reader["subject_name"]}, " +
                            $"Дата: {reader["attendance_date"]}, " +
                            $"Статус: {reader["status"]}");
                    }
                });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
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

            DatabaseHelper.ExecuteQuery(
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
                DatabaseHelper.ExecuteQuery(
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

            DatabaseHelper.ExecuteQuery(
                "DELETE FROM Attendance WHERE attendance_id = @attendanceId",
                cmd => cmd.Parameters.AddWithValue("@attendanceId", attendanceId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Запис про відвідуваність успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }

    static class AdminManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Управління адміністрацією", new Dictionary<string, Action>
            {
                { "Список адміністративного персоналу", ListAdminStaff },
                { "Додати нового адміністратора", AddAdminStaff },
                { "Редагувати адміністратора", EditAdminStaff },
                { "Видалити адміністратора", DeleteAdminStaff },
                { "Управління бюджетом", ManageBudget },
                { "Управління бібліотекою", ManageLibrary },
                { "Управління харчуванням", ManageCatering }
            });
        }

        private static void ListAdminStaff()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT a.admin_id, p.fname, p.name, p.sname, a.position, a.department, a.email 
                FROM Administration a 
                JOIN Person p ON a.person_id = p.person_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["admin_id"]}, " +
                            $"Ім'я: {reader["fname"]} {reader["name"]} {reader["sname"]}, " +
                            $"Посада: {reader["position"]}, " +
                            $"Відділ: {reader["department"]}, " +
                            $"Email: {reader["email"]}");
                    }
                });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void AddAdminStaff()
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
            Console.Write("Введіть посаду: ");
            string position = Console.ReadLine();
            Console.Write("Введіть відділ (Управління/Бібліотека/Харчування/Об'єкти/Фінанси): ");
            string department = Console.ReadLine();
            Console.Write("Введіть email: ");
            string email = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                @"INSERT INTO Person (fname, name, sname, when_born, sex) 
                  VALUES (@fname, @name, @sname, @birthDate, @sex);
                  INSERT INTO Administration (person_id, position, department, email) 
                  VALUES (LAST_INSERT_ID(), @position, @department, @email)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@sname", sname);
                    cmd.Parameters.AddWithValue("@birthDate", birthDate);
                    cmd.Parameters.AddWithValue("@sex", sex);
                    cmd.Parameters.AddWithValue("@position", position);
                    cmd.Parameters.AddWithValue("@department", department);
                    cmd.Parameters.AddWithValue("@email", email);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Адміністратора успішно додано! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditAdminStaff()
        {
            Console.Write("Введіть ID адміністратора для редагування: ");
            string adminId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Нова посада: ");
            string position = Console.ReadLine();
            Console.Write("Новий відділ: ");
            string department = Console.ReadLine();
            Console.Write("Новий email: ");
            string email = Console.ReadLine();

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(position)) updates.Add("position = @position");
            if (!string.IsNullOrWhiteSpace(department)) updates.Add("department = @department");
            if (!string.IsNullOrWhiteSpace(email)) updates.Add("email = @email");

            if (updates.Count > 0)
            {
                DatabaseHelper.ExecuteQuery(
                    $"UPDATE Administration SET {string.Join(", ", updates)} WHERE admin_id = @adminId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(position))
                            cmd.Parameters.AddWithValue("@position", position);
                        if (!string.IsNullOrWhiteSpace(department))
                            cmd.Parameters.AddWithValue("@department", department);
                        if (!string.IsNullOrWhiteSpace(email))
                            cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@adminId", adminId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Адміністратора успішно оновлено! Натисніть будь-яку клавішу, щоб продовжити...");
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

        private static void DeleteAdminStaff()
        {
            Console.Write("Введіть ID адміністратора для видалення: ");
            string adminId = Console.ReadLine();

            // Видаляємо пов'язані записи з таблиць Budget, Library, Catering (якщо вони існують)
            DatabaseHelper.ExecuteQuery(
                @"DELETE FROM Budget WHERE approved_by_id = @adminId;
          DELETE FROM Library WHERE librarian_id = @adminId;
          DELETE FROM Catering WHERE catering_manager_id = @adminId;
          DELETE FROM Administration WHERE admin_id = @adminId;
          DELETE FROM Person WHERE person_id = (
              SELECT person_id FROM Administration WHERE admin_id = @adminId
          )",
                cmd => cmd.Parameters.AddWithValue("@adminId", adminId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Адміністратора та пов'язані записи успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void ManageBudget()
        {
            MenuHelper.ShowMenu("Управління бюджетом", new Dictionary<string, Action>
            {
                { "Переглянути бюджет", ViewBudget },
                { "Оновити бюджет", UpdateBudget }
            });
        }

        private static void ViewBudget()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT b.budget_id, b.department, b.total_budget, 
                       a.position as approved_by 
                FROM Budget b 
                LEFT JOIN Administration a ON b.approved_by_id = a.admin_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID бюджету: {reader["budget_id"]}, " +
                            $"Відділ: {reader["department"]}, " +
                            $"Загальний бюджет: {reader["total_budget"]}, " +
                            $"Затверджено: {reader["approved_by"] ?? "Не затверджено"}");
                    }
                });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void UpdateBudget()
        {
            Console.Write("Введіть ID бюджету: ");
            string budgetId = Console.ReadLine();
            Console.Write("Введіть новий загальний бюджет: ");
            string totalBudget = Console.ReadLine();
            Console.Write("Введіть ID адміністратора, який затверджує: ");
            string approvedById = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "UPDATE Budget SET total_budget = @totalBudget, approved_by_id = @approvedById WHERE budget_id = @budgetId",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@totalBudget", totalBudget);
                    cmd.Parameters.AddWithValue("@approvedById", approvedById);
                    cmd.Parameters.AddWithValue("@budgetId", budgetId);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Бюджет успішно оновлено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void ManageLibrary()
        {
            MenuHelper.ShowMenu("Управління бібліотекою", new Dictionary<string, Action>
            {
                { "Список книг", ListBooks },
                { "Додати книгу", AddBook },
                { "Видалити книгу", RemoveBook }
            });
        }

        private static void ListBooks()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT l.book_id, l.book_title, l.author, l.category, 
                       a.position as librarian 
                FROM Library l 
                LEFT JOIN Administration a ON l.librarian_id = a.admin_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID книги: {reader["book_id"]}, " +
                            $"Назва: {reader["book_title"]}, " +
                            $"Автор: {reader["author"]}, " +
                            $"Категорія: {reader["category"]}, " +
                            $"Бібліотекар: {reader["librarian"] ?? "Не призначено"}");
                    }
                });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void AddBook()
        {
            Console.Write("Введіть назву книги: ");
            string bookTitle = Console.ReadLine();
            Console.Write("Введіть автора: ");
            string author = Console.ReadLine();
            Console.Write("Введіть категорію: ");
            string category = Console.ReadLine();
            Console.Write("Введіть ID бібліотекаря (необов'язково): ");
            string librarianId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "INSERT INTO Library (book_title, author, category, librarian_id) " +
                "VALUES (@bookTitle, @author, @category, NULLIF(@librarianId, ''))",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@bookTitle", bookTitle);
                    cmd.Parameters.AddWithValue("@author", author);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@librarianId", string.IsNullOrWhiteSpace(librarianId) ? DBNull.Value : (object)librarianId);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Книгу успішно додано! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void RemoveBook()
        {
            Console.Write("Введіть ID книги для видалення: ");
            string bookId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "DELETE FROM Library WHERE book_id = @bookId",
                cmd => cmd.Parameters.AddWithValue("@bookId", bookId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Книгу успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void ManageCatering()
        {
            MenuHelper.ShowMenu("Управління харчуванням", new Dictionary<string, Action>
            {
                { "Список страв", ListDishes },
                { "Додати страву", AddDish },
                { "Видалити страву", RemoveDish }
            });
        }

        private static void ListDishes()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT c.catering_id, c.dish_name, c.cost, 
                       a.position as catering_manager 
                FROM Catering c 
                LEFT JOIN Administration a ON c.catering_manager_id = a.admin_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID страви: {reader["catering_id"]}, " +
                            $"Назва: {reader["dish_name"]}, " +
                            $"Вартість: {reader["cost"]}, " +
                            $"Менеджер: {reader["catering_manager"] ?? "Не призначено"}");
                    }
                });
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void AddDish()
        {
            Console.Write("Введіть назву страви: ");
            string dishName = Console.ReadLine();
            Console.Write("Введіть вартість: ");
            string cost = Console.ReadLine();
            Console.Write("Введіть ID менеджера харчування (необов'язково): ");
            string cateringManagerId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "INSERT INTO Catering (dish_name, cost, catering_manager_id) " +
                "VALUES (@dishName, @cost, NULLIF(@cateringManagerId, ''))",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@dishName", dishName);
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@cateringManagerId", string.IsNullOrWhiteSpace(cateringManagerId) ? DBNull.Value : (object)cateringManagerId);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Страву успішно додано! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void RemoveDish()
        {
            Console.Write("Введіть ID страви для видалення: ");
            string cateringId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "DELETE FROM Catering WHERE catering_id = @cateringId",
                cmd => cmd.Parameters.AddWithValue("@cateringId", cateringId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Страву успішно видалено! Натисніть будь-яку клавішу, щоб продовжити...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}