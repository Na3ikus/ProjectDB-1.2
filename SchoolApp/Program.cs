using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace SchoolManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== School Management System ===");
                Console.WriteLine("1. Manage Students");
                Console.WriteLine("2. Manage Teachers");
                Console.WriteLine("3. Manage Administration");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

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
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
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
                Console.WriteLine($"=== {title} ===");
                int i = 1;
                foreach (var option in options)
                {
                    Console.WriteLine($"{i}. {option.Key}");
                    i++;
                }
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= options.Count)
                {
                    if (choice == 0) break;
                    options[options.Keys.ToArray()[choice - 1]].Invoke();
                }
                else
                {
                    Console.WriteLine("Invalid choice. Press any key to try again...");
                    Console.ReadKey();
                }
            }
        }
    }

    static class StudentManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Manage Students", new Dictionary<string, Action>
        {
            { "List All Students", ListStudents },
            { "Add New Student", AddStudent },
            { "Edit Student", EditStudent },
            { "Delete Student", DeleteStudent }
        });
        }

        private static void ListStudents()
        {
            DatabaseHelper.ExecuteQuery(@"
            SELECT s.student_id, p.fname, p.name, p.sname, s.class_id 
            FROM Students s
            JOIN Person p ON s.person_id = p.person_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["student_id"]}, " +
                            $"Name: {reader["fname"]} {reader["name"]} {reader["sname"]}, " +
                            $"Class ID: {reader["class_id"]}");
                    }
                });
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void AddStudent()
        {
            Console.Write("Enter First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string sname = Console.ReadLine();
            Console.Write("Enter Birth Date (YYYY-MM-DD): ");
            string birthDate = Console.ReadLine();
            Console.Write("Enter Sex (0/1): ");
            string sex = Console.ReadLine();
            Console.Write("Enter Class ID: ");
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

            Console.WriteLine("Student added successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void EditStudent()
        {
            Console.Write("Enter Student ID to edit: ");
            string studentId = Console.ReadLine();

            Console.WriteLine("Leave fields blank to skip updating");
            Console.Write("New First Name: ");
            string fname = Console.ReadLine();
            Console.Write("New Name: ");
            string name = Console.ReadLine();
            Console.Write("New Last Name: ");
            string sname = Console.ReadLine();
            Console.Write("New Class ID: ");
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

                Console.WriteLine("Student updated successfully! Press any key to continue...");
            }
            else
            {
                Console.WriteLine("No updates made. Press any key to continue...");
            }

            Console.ReadKey();
        }

        private static void DeleteStudent()
        {
            Console.Write("Enter Student ID to delete: ");
            string studentId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                @"DELETE FROM Attendance WHERE student_id = @studentId;
          DELETE FROM Grades WHERE student_id = @studentId;
          DELETE FROM Students WHERE student_id = @studentId;
          DELETE FROM Person WHERE person_id = (
              SELECT person_id FROM Students WHERE student_id = @studentId
          )",
                cmd => cmd.Parameters.AddWithValue("@studentId", studentId));

            Console.WriteLine("Student and related records deleted successfully! Press any key to continue...");
            Console.ReadKey();
        }
    }

    static class TeacherManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Manage Teachers", new Dictionary<string, Action>
            {
                { "List All Teachers", ListTeachers },
                { "Add New Teacher", AddTeacher },
                { "Edit Teacher", EditTeacher },
                { "Delete Teacher", DeleteTeacher },
                { "Assign Subject", AssignSubject }
            });
        }

        private static void ListTeachers()
        {
            DatabaseHelper.ExecuteQuery(@"
                SELECT t.teacher_id, p.fname, p.name, p.sname, d.name as department, t.position 
                FROM Teachers t 
                JOIN Person p ON t.person_id = p.person_id 
                JOIN Departments d ON t.department_id = d.department_id",
                process: reader =>
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["teacher_id"]}, " +
                            $"Name: {reader["fname"]} {reader["name"]} {reader["sname"]}, " +
                            $"Department: {reader["department"]}, " +
                            $"Position: {reader["position"]}");
                    }
                });
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void AddTeacher()
        {
            Console.Write("Enter First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string sname = Console.ReadLine();
            Console.Write("Enter Birth Date (YYYY-MM-DD): ");
            string birthDate = Console.ReadLine();
            Console.Write("Enter Sex (0/1): ");
            string sex = Console.ReadLine();
            Console.Write("Enter Department ID: ");
            string departmentId = Console.ReadLine();
            Console.Write("Enter Position: ");
            string position = Console.ReadLine();
            Console.Write("Enter Hire Date (YYYY-MM-DD): ");
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

            Console.WriteLine("Teacher added successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void EditTeacher()
        {
            Console.Write("Enter Teacher ID to edit: ");
            string teacherId = Console.ReadLine();

            Console.WriteLine("Leave fields blank to skip updating");
            Console.Write("New Department ID: ");
            string departmentId = Console.ReadLine();
            Console.Write("New Position: ");
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

                Console.WriteLine("Teacher updated successfully! Press any key to continue...");
            }
            else
            {
                Console.WriteLine("No updates made. Press any key to continue...");
            }

            Console.ReadKey();
        }

        private static void DeleteTeacher()
        {
            Console.Write("Enter Teacher ID to delete: ");
            string teacherId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                @"DELETE FROM Teachers WHERE teacher_id = @teacherId;
                  DELETE FROM Person WHERE person_id = (
                      SELECT person_id FROM Teachers WHERE teacher_id = @teacherId
                  )",
                cmd => cmd.Parameters.AddWithValue("@teacherId", teacherId));

            Console.WriteLine("Teacher deleted successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void AssignSubject()
        {
            Console.Write("Enter Teacher ID: ");
            string teacherId = Console.ReadLine();
            Console.Write("Enter Subject ID: ");
            string subjectId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "INSERT INTO Teacher_Subjects (teacher_id, subject_id) VALUES (@teacherId, @subjectId)",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@teacherId", teacherId);
                    cmd.Parameters.AddWithValue("@subjectId", subjectId);
                });

            Console.WriteLine("Subject assigned successfully! Press any key to continue...");
            Console.ReadKey();
        }
    }

    static class AdminManager
    {
        public static void Menu()
        {
            MenuHelper.ShowMenu("Manage Administration", new Dictionary<string, Action>
            {
                { "List Administration Staff", ListAdminStaff },
                { "Add New Admin", AddAdminStaff },
                { "Edit Admin Staff", EditAdminStaff },
                { "Delete Admin Staff", DeleteAdminStaff },
                { "Manage Budget", ManageBudget },
                { "Manage Library", ManageLibrary },
                { "Manage Catering", ManageCatering }
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
                            $"Name: {reader["fname"]} {reader["name"]} {reader["sname"]}, " +
                            $"Position: {reader["position"]}, " +
                            $"Department: {reader["department"]}, " +
                            $"Email: {reader["email"]}");
                    }
                });
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void AddAdminStaff()
        {
            Console.Write("Enter First Name: ");
            string fname = Console.ReadLine();
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            string sname = Console.ReadLine();
            Console.Write("Enter Birth Date (YYYY-MM-DD): ");
            string birthDate = Console.ReadLine();
            Console.Write("Enter Sex (0/1): ");
            string sex = Console.ReadLine();
            Console.Write("Enter Position: ");
            string position = Console.ReadLine();
            Console.Write("Enter Department (Management/Library/Catering/Facilities/Finance): ");
            string department = Console.ReadLine();
            Console.Write("Enter Email: ");
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

            Console.WriteLine("Admin staff added successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void EditAdminStaff()
        {
            Console.Write("Enter Admin ID to edit: ");
            string adminId = Console.ReadLine();

            Console.WriteLine("Leave fields blank to skip updating");
            Console.Write("New Position: ");
            string position = Console.ReadLine();
            Console.Write("New Department: ");
            string department = Console.ReadLine();
            Console.Write("New Email: ");
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

                Console.WriteLine("Admin staff updated successfully! Press any key to continue...");
            }
            else
            {
                Console.WriteLine("No updates made. Press any key to continue...");
            }

            Console.ReadKey();
        }

        private static void DeleteAdminStaff()
        {
            Console.Write("Enter Admin ID to delete: ");
            string adminId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                @"DELETE FROM Administration WHERE admin_id = @adminId;
                  DELETE FROM Person WHERE person_id = (
                      SELECT person_id FROM Administration WHERE admin_id = @adminId
                  )",
                cmd => cmd.Parameters.AddWithValue("@adminId", adminId));

            Console.WriteLine("Admin staff deleted successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void ManageBudget()
        {
            MenuHelper.ShowMenu("Manage Budget", new Dictionary<string, Action>
            {
                { "View Budget", ViewBudget },
                { "Update Budget", UpdateBudget }
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
                        Console.WriteLine($"Budget ID: {reader["budget_id"]}, " +
                            $"Department: {reader["department"]}, " +
                            $"Total Budget: {reader["total_budget"]}, " +
                            $"Approved By: {reader["approved_by"] ?? "Not Approved"}");
                    }
                });
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void UpdateBudget()
        {
            Console.Write("Enter Budget ID: ");
            string budgetId = Console.ReadLine();
            Console.Write("Enter New Total Budget: ");
            string totalBudget = Console.ReadLine();
            Console.Write("Enter Approving Admin ID: ");
            string approvedById = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "UPDATE Budget SET total_budget = @totalBudget, approved_by_id = @approvedById WHERE budget_id = @budgetId",
                cmd =>
                {
                    cmd.Parameters.AddWithValue("@totalBudget", totalBudget);
                    cmd.Parameters.AddWithValue("@approvedById", approvedById);
                    cmd.Parameters.AddWithValue("@budgetId", budgetId);
                });

            Console.WriteLine("Budget updated successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void ManageLibrary()
        {
            MenuHelper.ShowMenu("Manage Library", new Dictionary<string, Action>
            {
                { "List Books", ListBooks },
                { "Add Book", AddBook },
                { "Remove Book", RemoveBook }
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
                        Console.WriteLine($"Book ID: {reader["book_id"]}, " +
                            $"Title: {reader["book_title"]}, " +
                            $"Author: {reader["author"]}, " +
                            $"Category: {reader["category"]}, " +
                            $"Librarian: {reader["librarian"] ?? "Unassigned"}");
                    }
                });
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void AddBook()
        {
            Console.Write("Enter Book Title: ");
            string bookTitle = Console.ReadLine();
            Console.Write("Enter Author: ");
            string author = Console.ReadLine();
            Console.Write("Enter Category: ");
            string category = Console.ReadLine();
            Console.Write("Enter Librarian ID (optional): ");
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

            Console.WriteLine("Book added successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void RemoveBook()
        {
            Console.Write("Enter Book ID to remove: ");
            string bookId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "DELETE FROM Library WHERE book_id = @bookId",
                cmd => cmd.Parameters.AddWithValue("@bookId", bookId));

            Console.WriteLine("Book removed successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void ManageCatering()
        {
            MenuHelper.ShowMenu("Manage Catering", new Dictionary<string, Action>
            {
                { "List Dishes", ListDishes },
                { "Add Dish", AddDish },
                { "Remove Dish", RemoveDish }
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
                        Console.WriteLine($"Dish ID: {reader["catering_id"]}, " +
                            $"Name: {reader["dish_name"]}, " +
                            $"Cost: {reader["cost"]}, " +
                            $"Manager: {reader["catering_manager"] ?? "Unassigned"}");
                    }
                });
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void AddDish()
        {
            Console.Write("Enter Dish Name: ");
            string dishName = Console.ReadLine();
            Console.Write("Enter Dish Cost: ");
            string cost = Console.ReadLine();
            Console.Write("Enter Catering Manager ID (optional): ");
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

            Console.WriteLine("Dish added successfully! Press any key to continue...");
            Console.ReadKey();
        }

        private static void RemoveDish()
        {
            Console.Write("Enter Dish ID to remove: ");
            string cateringId = Console.ReadLine();

            DatabaseHelper.ExecuteQuery(
                "DELETE FROM Catering WHERE catering_id = @cateringId",
                cmd => cmd.Parameters.AddWithValue("@cateringId", cateringId));

            Console.WriteLine("Dish removed successfully! Press any key to continue...");
            Console.ReadKey();
        }
    }
}
