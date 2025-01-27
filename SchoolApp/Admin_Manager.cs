using DatabaseHelpers;

namespace SchoolApp
{
    static class Admin_Manager
    {
        public static void Menu()
        {
            string[] menuItems = {
            "Список адміністративного персоналу",
            "Додати нового адміністратора",
            "Редагувати адміністратора",
            "Видалити адміністратора",
            "Управління бюджетом",
            "Управління бібліотекою",
            "Управління харчуванням",
            "Повернутися до головного меню"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║          Управління адміністрацією                 ║");
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
                    ListAdminStaff();
                    break;
                case 1:
                    AddAdminStaff();
                    break;
                case 2:
                    EditAdminStaff();
                    break;
                case 3:
                    DeleteAdminStaff();
                    break;
                case 4:
                    ManageBudget();
                    break;
                case 5:
                    ManageLibrary();
                    break;
                case 6:
                    ManageCatering();
                    break;
                case 7:
                    return;
            }
        }

        private static void ListAdminStaff()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Список адміністративного персоналу        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT a.admin_id, p.fname, p.name, p.sname, a.position, a.department, a.email 
            FROM Administration a 
            JOIN Person p ON a.person_id = p.person_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Адміністратор #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID: {reader["admin_id"]}");
                        Console.WriteLine($"║ Ім'я: {reader["fname"]} {reader["name"]} {reader["sname"]}");
                        Console.WriteLine($"║ Посада: {reader["position"]}");
                        Console.WriteLine($"║ Відділ: {reader["department"]}");
                        Console.WriteLine($"║ Email: {reader["email"]}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про адміністративний персонал.");
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

        private static void AddAdminStaff()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Додати нового адміністратора              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ім'я: ");
            string fname = Console.ReadLine() ;
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
            Console.Write("Введіть відділ: ");
            string department = Console.ReadLine();
            Console.Write("Введіть email: ");
            string email = Console.ReadLine();

            Database_Helper.ExecuteQuery(
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
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Адміністратора успішно додано!                     ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditAdminStaff()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Редагувати адміністратора                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

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
                Database_Helper.ExecuteQuery(
                    $"UPDATE Administration SET {string.Join(", ", updates)} WHERE admin_id = @adminId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(position)) cmd.Parameters.AddWithValue("@position", position);
                        if (!string.IsNullOrWhiteSpace(department)) cmd.Parameters.AddWithValue("@department", department);
                        if (!string.IsNullOrWhiteSpace(email)) cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@adminId", adminId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║ Адміністратора успішно оновлено!                   ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║ Оновлення не виконано.                             ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        private static void DeleteAdminStaff()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Видалити адміністратора                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID адміністратора для видалення: ");
            string adminId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                @"DELETE FROM Administration WHERE admin_id = @adminId;
              DELETE FROM Person WHERE person_id = (
                  SELECT person_id FROM Administration WHERE admin_id = @adminId
              )",
                cmd => cmd.Parameters.AddWithValue("@adminId", adminId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Адміністратора успішно видалено!                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void ManageBudget()
        {
            string[] menuItems = {
            "Переглянути бюджет",
            "Додати запис про бюджет",
            "Редагувати запис про бюджет",
            "Видалити запис про бюджет",
            "Повернутися до головного меню"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║          Управління бюджетом                       ║");
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
                        HandleBudgetMenuSelection(selectedIndex);
                        if (selectedIndex == menuItems.Length - 1) return;
                        break;
                }
            }
        }

        private static void HandleBudgetMenuSelection(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:
                    ViewBudget();
                    break;
                case 1:
                    AddBudgetRecord();
                    break;
                case 2:
                    EditBudgetRecord();
                    break;
                case 3:
                    DeleteBudgetRecord();
                    break;
                case 4:
                    return;
            }
        }

        private static void ViewBudget()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Перегляд бюджету                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT b.budget_id, b.department, b.total_budget, 
                   a.position as approved_by 
            FROM Budget b 
            LEFT JOIN Administration a ON b.approved_by_id = a.admin_id",
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
                        Console.WriteLine($"║ ID бюджету: {reader["budget_id"]}");
                        Console.WriteLine($"║ Відділ: {reader["department"]}");
                        Console.WriteLine($"║ Загальний бюджет: {reader["total_budget"]}");
                        Console.WriteLine($"║ Затверджено: {reader["approved_by"] ?? "Не затверджено"}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про бюджет.");
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

        private static void AddBudgetRecord()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Додати запис про бюджет                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть відділ: ");
            string department = Console.ReadLine();
            Console.Write("Введіть загальний бюджет: ");
            string totalBudget = Console.ReadLine();
            Console.Write("Введіть ID адміністратора, який затвердив бюджет: ");
            string approvedById = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Budget (department, total_budget, approved_by_id) " +
                "VALUES (@department, @totalBudget, @approvedById)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@department", department);
                    cmd.Parameters.AddWithValue("@totalBudget", totalBudget);
                    cmd.Parameters.AddWithValue("@approvedById", approvedById);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Запис про бюджет успішно додано!                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditBudgetRecord()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Редагувати запис про бюджет               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID запису про бюджет для редагування: ");
            string budgetId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Новий відділ: ");
            string department = Console.ReadLine();
            Console.Write("Новий загальний бюджет: ");
            string totalBudget = Console.ReadLine();
            Console.Write("Введіть ID адміністратора, який затвердив бюджет (необов'язково): ");
            string approvedById = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Budget (department, total_budget, approved_by_id) " +
                "VALUES (@department, @totalBudget, NULLIF(@approvedById, ''))",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@department", department);
                    cmd.Parameters.AddWithValue("@totalBudget", totalBudget);
                    cmd.Parameters.AddWithValue("@approvedById", string.IsNullOrWhiteSpace(approvedById) ? DBNull.Value : approvedById);
                });

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(department)) updates.Add("department = @department");
            if (!string.IsNullOrWhiteSpace(totalBudget)) updates.Add("total_budget = @totalBudget");
            if (!string.IsNullOrWhiteSpace(approvedById)) updates.Add("approved_by_id = @approvedById");

            if (updates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $"UPDATE Budget SET {string.Join(", ", updates)} WHERE budget_id = @budgetId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(department)) cmd.Parameters.AddWithValue("@department", department);
                        if (!string.IsNullOrWhiteSpace(totalBudget)) cmd.Parameters.AddWithValue("@totalBudget", totalBudget);
                        if (!string.IsNullOrWhiteSpace(approvedById)) cmd.Parameters.AddWithValue("@approvedById", approvedById);
                        cmd.Parameters.AddWithValue("@budgetId", budgetId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║ Запис про бюджет успішно оновлено!                 ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║ Оновлення не виконано.                             ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        private static void DeleteBudgetRecord()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Видалити запис про бюджет                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID запису про бюджет для видалення: ");
            string budgetId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "DELETE FROM Budget WHERE budget_id = @budgetId",
                cmd => cmd.Parameters.AddWithValue("@budgetId", budgetId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Запис про бюджет успішно видалено!                 ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void ManageLibrary()
        {
            string[] menuItems = {
            "Переглянути книги",
            "Додати книгу",
            "Редагувати книгу",
            "Видалити книгу",
            "Повернутися до головного меню"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║          Управління бібліотекою                    ║");
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
                        HandleLibraryMenuSelection(selectedIndex);
                        if (selectedIndex == menuItems.Length - 1) return;
                        break;
                }
            }
        }

        private static void HandleLibraryMenuSelection(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:
                    ViewBooks();
                    break;
                case 1:
                    AddBook();
                    break;
                case 2:
                    EditBook();
                    break;
                case 3:
                    DeleteBook();
                    break;
                case 4:
                    return;
            }
        }

        private static void ViewBooks()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Перегляд книг                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT l.book_id, l.book_title, l.author, l.category, 
                   a.position as librarian 
            FROM Library l 
            LEFT JOIN Administration a ON l.librarian_id = a.admin_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Книга #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID книги: {reader["book_id"]}");
                        Console.WriteLine($"║ Назва: {reader["book_title"]}");
                        Console.WriteLine($"║ Автор: {reader["author"]}");
                        Console.WriteLine($"║ Категорія: {reader["category"]}");
                        Console.WriteLine($"║ Бібліотекар: {reader["librarian"] ?? "Не призначено"}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про книги.");
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

        private static void AddBook()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Додати книгу                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть назву книги: ");
            string bookTitle = Console.ReadLine();
            Console.Write("Введіть автора: ");
            string author = Console.ReadLine();
            Console.Write("Введіть категорію: ");
            string category = Console.ReadLine();
            Console.Write("Введіть ID бібліотекаря (необов'язково): ");
            string librarianId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Library (book_title, author, category, librarian_id) " +
                "VALUES (@bookTitle, @author, @category, NULLIF(@librarianId, ''))",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@bookTitle", bookTitle);
                    cmd.Parameters.AddWithValue("@author", author);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@librarianId", string.IsNullOrWhiteSpace(librarianId) ? DBNull.Value : librarianId);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Книгу успішно додано!                              ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditBook()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Редагувати книгу                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID книги для редагування: ");
            string bookId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Нова назва: ");
            string bookTitle = Console.ReadLine();
            Console.Write("Новий автор: ");
            string author = Console.ReadLine();
            Console.Write("Нова категорія: ");
            string category = Console.ReadLine();
            Console.Write("Новий ID бібліотекаря: ");
            string librarianId = Console.ReadLine();

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(bookTitle)) updates.Add("book_title = @bookTitle");
            if (!string.IsNullOrWhiteSpace(author)) updates.Add("author = @author");
            if (!string.IsNullOrWhiteSpace(category)) updates.Add("category = @category");
            if (!string.IsNullOrWhiteSpace(librarianId)) updates.Add("librarian_id = @librarianId");

            if (updates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $"UPDATE Library SET {string.Join(", ", updates)} WHERE book_id = @bookId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(bookTitle)) cmd.Parameters.AddWithValue("@bookTitle", bookTitle);
                        if (!string.IsNullOrWhiteSpace(author)) cmd.Parameters.AddWithValue("@author", author);
                        if (!string.IsNullOrWhiteSpace(category)) cmd.Parameters.AddWithValue("@category", category);
                        if (!string.IsNullOrWhiteSpace(librarianId)) cmd.Parameters.AddWithValue("@librarianId", librarianId);
                        cmd.Parameters.AddWithValue("@bookId", bookId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║ Книгу успішно оновлено!                            ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║ Оновлення не виконано.                             ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        private static void DeleteBook()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Видалити книгу                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID книги для видалення: ");
            string bookId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "DELETE FROM Library WHERE book_id = @bookId",
                cmd => cmd.Parameters.AddWithValue("@bookId", bookId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Книгу успішно видалено!                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void ManageCatering()
        {
            string[] menuItems = {
            "Переглянути страви",
            "Додати страву",
            "Редагувати страву",
            "Видалити страву",
            "Повернутися до головного меню"
        };

            int selectedIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║          Управління харчуванням                    ║");
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
                        HandleCateringMenuSelection(selectedIndex);
                        if (selectedIndex == menuItems.Length - 1) return;
                        break;
                }
            }
        }

        private static void HandleCateringMenuSelection(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:
                    ViewDishes();
                    break;
                case 1:
                    AddDish();
                    break;
                case 2:
                    EditDish();
                    break;
                case 3:
                    DeleteDish();
                    break;
                case 4:
                    return;
            }
        }

        private static void ViewDishes()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Перегляд страв                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Database_Helper.ExecuteQuery(@"
            SELECT c.catering_id, c.dish_name, c.cost, 
                   a.position as catering_manager 
            FROM Catering c 
            LEFT JOIN Administration a ON c.catering_manager_id = a.admin_id",
                process: reader =>
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count++;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╔════════════════════════════════════════════════════╗");
                        Console.WriteLine($"║ Страва #{count}");
                        Console.WriteLine("╠────────────────────────────────────────────────────╣");
                        Console.ResetColor();
                        Console.WriteLine($"║ ID страви: {reader["catering_id"]}");
                        Console.WriteLine($"║ Назва: {reader["dish_name"]}");
                        Console.WriteLine($"║ Вартість: {reader["cost"]}");
                        Console.WriteLine($"║ Менеджер: {reader["catering_manager"] ?? "Не призначено"}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("╚════════════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("║ Немає даних про страви.");
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

        private static void AddDish()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Додати страву                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть назву страви: ");
            string dishName = Console.ReadLine();
            Console.Write("Введіть вартість: ");
            string cost = Console.ReadLine();
            Console.Write("Введіть ID менеджера харчування (необов'язково): ");
            string cateringManagerId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "INSERT INTO Catering (dish_name, cost, catering_manager_id) " +
                "VALUES (@dishName, @cost, NULLIF(@cateringManagerId, ''))",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@dishName", dishName);
                    cmd.Parameters.AddWithValue("@cost", cost);
                    cmd.Parameters.AddWithValue("@cateringManagerId", string.IsNullOrWhiteSpace(cateringManagerId) ? DBNull.Value : cateringManagerId);
                });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Страву успішно додано!                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EditDish()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Редагувати страву                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID страви для редагування: ");
            string cateringId = Console.ReadLine();

            Console.WriteLine("Залиште поля порожніми, щоб пропустити оновлення");
            Console.Write("Нова назва: ");
            string dishName = Console.ReadLine();
            Console.Write("Нова вартість: ");
            string cost = Console.ReadLine();
            Console.Write("Новий ID менеджера харчування: ");
            string cateringManagerId = Console.ReadLine();

            var updates = new List<string>();
            if (!string.IsNullOrWhiteSpace(dishName)) updates.Add("dish_name = @dishName");
            if (!string.IsNullOrWhiteSpace(cost)) updates.Add("cost = @cost");
            if (!string.IsNullOrWhiteSpace(cateringManagerId)) updates.Add("catering_manager_id = @cateringManagerId");

            if (updates.Count > 0)
            {
                Database_Helper.ExecuteQuery(
                    $"UPDATE Catering SET {string.Join(", ", updates)} WHERE catering_id = @cateringId",
                    cmd =>
                    {
                        if (!string.IsNullOrWhiteSpace(dishName)) cmd.Parameters.AddWithValue("@dishName", dishName);
                        if (!string.IsNullOrWhiteSpace(cost)) cmd.Parameters.AddWithValue("@cost", cost);
                        if (!string.IsNullOrWhiteSpace(cateringManagerId)) cmd.Parameters.AddWithValue("@cateringManagerId", cateringManagerId);
                        cmd.Parameters.AddWithValue("@cateringId", cateringId);
                    });

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║ Страву успішно оновлено!                           ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("╔════════════════════════════════════════════════════╗");
                Console.WriteLine("║ Оновлення не виконано.                             ║");
                Console.WriteLine("╚════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        private static void DeleteDish()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║          Видалити страву                           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.Write("Введіть ID страви для видалення: ");
            string cateringId = Console.ReadLine();

            Database_Helper.ExecuteQuery(
                "DELETE FROM Catering WHERE catering_id = @cateringId",
                cmd => cmd.Parameters.AddWithValue("@cateringId", cateringId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║ Страву успішно видалено!                           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
