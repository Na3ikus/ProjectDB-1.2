CREATE TABLE Person (
    person_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    fname VARCHAR(100) NOT NULL,
    name VARCHAR(100) NOT NULL,
    sname VARCHAR(100) NOT NULL,
    when_born DATE NOT NULL,
    sex TINYINT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Classes (
    class_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    class_flow_id BIGINT NOT NULL,
    number VARCHAR(10) NOT NULL,
    FOREIGN KEY (class_flow_id) REFERENCES Class_Flow(class_flow_id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Subjects (
    subject_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Schedules (
    schedule_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    class_id BIGINT NOT NULL,
    subject_id BIGINT NOT NULL,
    day_of_week ENUM('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday') NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    FOREIGN KEY (class_id) REFERENCES Classes(class_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE (class_id, day_of_week, start_time, end_time)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE Students (
    student_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    person_id BIGINT NOT NULL,
    class_id BIGINT NOT NULL,
    FOREIGN KEY (person_id) REFERENCES Person(person_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (class_id) REFERENCES Classes(class_id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Attendance (
    attendance_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    student_id BIGINT NOT NULL,
    schedule_id BIGINT NOT NULL,
    attendance_date DATE NOT NULL,
    status ENUM('Present', 'Absent', 'Late', 'Excused') NOT NULL,
    FOREIGN KEY (student_id) REFERENCES Students(student_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (schedule_id) REFERENCES Schedules(schedule_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE (student_id, schedule_id, attendance_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Grades (
    grade_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    student_id BIGINT NOT NULL,
    subject_id BIGINT NOT NULL,
    grade_date DATE NOT NULL,
    grade DECIMAL(5, 2) NOT NULL,
    comment TEXT,
    FOREIGN KEY (student_id) REFERENCES Students(student_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE (student_id, subject_id, grade_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Departments (
    department_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Teachers (
    teacher_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    person_id BIGINT NOT NULL,
    department_id BIGINT NOT NULL,
    hire_date DATE NOT NULL,
    position VARCHAR(100) NOT NULL,
    FOREIGN KEY (person_id) REFERENCES Person(person_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (department_id) REFERENCES Departments(department_id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Teacher_Subjects (
    teacher_subject_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    teacher_id BIGINT NOT NULL,
    subject_id BIGINT NOT NULL,
    FOREIGN KEY (teacher_id) REFERENCES Teachers(teacher_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Schedules_Teachers (
    schedule_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    teacher_id BIGINT NOT NULL,
    subject_id BIGINT NOT NULL,
    day_of_week TINYINT NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    room VARCHAR(50) NOT NULL,
    FOREIGN KEY (teacher_id) REFERENCES Teachers(teacher_id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Administration (
    admin_id INT AUTO_INCREMENT PRIMARY KEY,
    person_id BIGINT NOT NULL,
    position VARCHAR(200) NOT NULL,
    department ENUM('Management', 'Library', 'Catering', 'Facilities', 'Finance') NOT NULL,
    email VARCHAR(150) UNIQUE,
    FOREIGN KEY (person_id) REFERENCES Person(person_id)
        ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Library (
    book_id INT AUTO_INCREMENT PRIMARY KEY,
    book_title VARCHAR(255) NOT NULL,
    author VARCHAR(150),
    librarian_id INT,
    category VARCHAR(100),
    FOREIGN KEY (librarian_id) REFERENCES Administration(admin_id)
        ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Catering (
    catering_id INT AUTO_INCREMENT PRIMARY KEY,
    dish_name VARCHAR(200) NOT NULL,
    catering_manager_id INT,
    cost DECIMAL(8, 2),
    FOREIGN KEY (catering_manager_id) REFERENCES Administration(admin_id)
        ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Classrooms (
    office_number INT PRIMARY KEY,
    responsible_staff_id INT,
    FOREIGN KEY (responsible_staff_id) REFERENCES Administration(admin_id)
        ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE Budget (
    budget_id INT AUTO_INCREMENT PRIMARY KEY,
    department ENUM('Management', 'Library', 'Catering', 'Facilities', 'Finance') NOT NULL,
    total_budget DECIMAL(15, 2),
    approved_by_id INT,
    FOREIGN KEY (approved_by_id) REFERENCES Administration(admin_id)
        ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
