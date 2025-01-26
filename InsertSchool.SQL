-- Повне заповнення бази даних школи українською

-- Departments
INSERT INTO Departments (name, description) VALUES 
('Математична кафедра', 'Кафедра математичних наук'),
('Природничих наук', 'Кафедра фізики, хімії та біології'),
('Гуманітарних наук', 'Кафедра соціальних наук та мов');

-- Person
INSERT INTO Person (fname, name, sname, when_born, sex) VALUES 
('Тарас', 'Шевченко', 'Іванович', '2005-09-15', 1),
('Олена', 'Коваленко', 'Петрівна', '2006-03-22', 2),
('Микола', 'Іваненко', 'Степанович', '1975-11-10', 1),
('Світлана', 'Петренко', 'Вікторівна', '1980-05-18', 2),
('Оксана', 'Мельник', 'Романівна', '1985-07-30', 2),
('Максим', 'Ткаченко', 'Андрійович', '1988-04-25', 1),
('Юлія', 'Кравченко', 'Олегівна', '1990-12-05', 2);

-- Class_Flow
INSERT INTO Class_Flow (year) VALUES 
(2023), (2022), (2024);

-- Classes
INSERT INTO Classes (class_flow_id, number) VALUES 
(1, '10-А'), 
(1, '10-Б'),
(2, '11-А'),
(3, '9-В');

-- Subjects
INSERT INTO Subjects (name, description) VALUES 
('Математика', 'Поглиблене вивчення математичних дисциплін'),
('Фізика', 'Вивчення фундаментальних фізичних явищ'),
('Українська мова', 'Вивчення української мови та літератури'),
('Історія', 'Дослідження вітчизняної та світової історії');

-- Students
INSERT INTO Students (person_id, class_id) VALUES 
(1, 1), 
(2, 2),
(3, 3),
(4, 4);

-- Teachers
INSERT INTO Teachers (person_id, department_id, hire_date, position) VALUES 
(5, 1, '2010-09-01', 'Вчитель математики'),
(6, 2, '2005-09-01', 'Старший викладач фізики'),
(7, 3, '2015-09-01', 'Вчитель української мови');

-- Schedules
INSERT INTO Schedules (class_id, subject_id, day_of_week, start_time, end_time) VALUES 
(1, 1, 'Monday', '09:00:00', '10:30:00'),
(2, 2, 'Tuesday', '10:45:00', '12:15:00'),
(3, 3, 'Wednesday', '11:00:00', '12:30:00'),
(4, 4, 'Thursday', '09:30:00', '11:00:00');

-- Administration
INSERT INTO Administration (person_id, position, department, email) VALUES 
(5, 'Директор школи', 'Management', 'director@school.ua'),
(6, 'Завідувач бібліотеки', 'Library', 'library@school.ua');

-- Library
INSERT INTO Library (book_title, author, librarian_id, category) VALUES 
('Кобзар', 'Тарас Шевченко', 2, 'Класична література'),
('Фізика для 10 класу', 'Іванов А.П.', 2, 'Підручники'),
('Історія України', 'Петров С.М.', 2, 'Навчальна література');

-- Catering
INSERT INTO Catering (dish_name, catering_manager_id, cost) VALUES 
('Борщ', 1, 45.50),
('Каша гречана', 1, 35.20),
('Салат овочевий', 1, 25.75);

-- Equipment
INSERT INTO Equipment (name, responsible_staff_id) VALUES 
('Інтерактивна дошка', 1),
('Комп\'ютерний клас', 1),
('Мультимедійний проектор', 1);

-- Classrooms
INSERT INTO Classrooms (office_number, responsible_staff_id) VALUES 
(204, 1),
(305, 1),
(405, 1);

-- Budget
INSERT INTO Budget (department, total_budget, approved_by_id) VALUES 
('Management', 500000.00, 1),
('Library', 100000.00, 1),
('Catering', 250000.00, 1);

-- Teacher_Subjects
INSERT INTO Teacher_Subjects (teacher_id, subject_id) VALUES 
(1, 1), 
(2, 2),
(3, 3);

-- Schedules_Teachers
INSERT INTO Schedules_Teachers (teacher_id, subject_id, day_of_week, start_time, end_time, room) VALUES 
(1, 1, 2, '09:00:00', '10:30:00', '204'),
(2, 2, 3, '10:45:00', '12:15:00', '305'),
(3, 3, 4, '11:00:00', '12:30:00', '405');

-- Attendance
INSERT INTO Attendance (student_id, schedule_id, attendance_date, status) VALUES 
(1, 1, '2024-01-15', 'Present'),
(2, 2, '2024-01-16', 'Late'),
(3, 3, '2024-01-17', 'Excused'),
(4, 4, '2024-01-18', 'Present');

-- Grades
INSERT INTO Grades (student_id, subject_id, grade_date, grade, comment) VALUES 
(1, 1, '2024-01-15', 9.5, 'Відмінна робота з математики'),
(2, 2, '2024-01-16', 8.0, 'Добре знання фізики'),
(3, 3, '2024-01-17', 7.5, 'Задовільно, потрібно покращити'),
(4, 4, '2024-01-18', 8.5, 'Непогані знання з історії');
