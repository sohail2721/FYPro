CREATE DATABASE FYPro;

USE FYPro;

-- USERS TABLE
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY,
    UserName VARCHAR(25) NOT NULL,
    Email VARCHAR(50) NOT NULL UNIQUE,
    UserType VARCHAR(25) CHECK (UserType IN ('Administrator', 'Supervisor', 'Student')) NOT NULL,
    Password VARCHAR(50) NOT NULL,
    FirstName VARCHAR(20) NOT NULL,
    LastName VARCHAR(20) NOT NULL,
    CNIC VARCHAR(13) NOT NULL UNIQUE,
    DOB DATE NOT NULL,
    PhoneNumber VARCHAR(11)
);

-- Inserting data into Users Table
INSERT INTO Users (UserName, Email, UserType, Password, FirstName, LastName, CNIC, DOB, PhoneNumber) VALUES
('amirrehman', 'amir.rehman@nu.edu.pk', 'Administrator', 'amirrehman123', 'Amir', 'Rehman', '1234512345671', '1990-01-01', '03001234567'),
('akhterjamil', 'akhter.jamil@nu.edu.pk', 'Supervisor', 'akhterjamil123', 'Akhter', 'Jamil', '1234512345672', '1982-02-02', '03011234567'),
('adnantariq', 'adnan.tariq@nu.edu.pk', 'Supervisor', 'adnantaiq123', 'Adnan', 'Tariq', '1234512395672', '1982-02-02', '03061234967'),
('fareeahnaseem', 'i210500@nu.edu.pk', 'Student', 'fareeahnaseem123', 'Fareeah', 'Naseem', '3120288334098', '2002-09-28', '03339149695'),
('sohailshahbaz', 'i211356@nu.edu.pk', 'Student', 'sohailshahbaz123', 'Sohail', 'Shahbaz', '3120223095293', '2002-03-04', '03108202308'),
('faisalrehman', 'i212578@nu.edu.pk', 'Student', 'faisalrehman123', 'Faisal', 'Rehman', '3120429373794', '2000-09-23', '03129980695'),
('sarahkhan', 'i201356@nu.edu.pk', 'Student', 'sarahkhan123', 'Sarah', 'Khan', '3120512345678', '2001-05-15', '03331234567'),
('alihaider', 'i190936@nu.edu.pk', 'Student', 'alihaider123', 'Ali', 'Haider', '3120612345679', '2001-06-20', '03441234567'),
('zainabasif', 'i201624@nu.edu.pk', 'Student', 'zainabasif123', 'Zainab', 'Asif', '3120712345680', '2001-07-25', '03551234567'),
('umarfarooq', 'i211234@nu.edu.pk', 'Student', 'umarfarooq123', 'Umar', 'Farooq', '3120812345681', '2001-08-30', '03661234567'),
('mohsinahmed', 'i192394@nu.edu.pk', 'Student', 'mohsinahmed123', 'Mohsin', 'Ahmed', '3120912345682', '2001-09-05', '03771234567'),
('fatimazahra', 'i201932@nu.edu.pk', 'Student', 'fatimazahra123', 'Fatima', 'Zahra', '3121012345683', '2001-10-10', '03881234567'),
('naveedaslam', 'naveed.aslam@nu.edu.pk', 'Supervisor', 'naveedaslam123', 'Naveed', 'Aslam', '1234567890123', '1975-03-15', '03121234567'),
('sadiqkhan', 'sadiq.khan@nu.edu.pk', 'Supervisor', 'sadiqkhan123', 'Sadiq', 'Khan', '1234567890124', '1980-07-20', '03231234567'),
('saimaasif', 'saima.asif@nu.edu.pk', 'Supervisor', 'saimaasif123', 'Saima', 'Asif', '1234567890125', '1985-11-25', '03341234567'),
('kamrankhalid', 'kamran.khalid@nu.edu.pk', 'Supervisor', 'kamrankhalid123', 'Kamran', 'Khalid', '1234567890126', '1990-12-30', '03451234567');

SELECT * FROM Users;

SELECT COUNT(*) COUNT FROM Users WHERE Email = 'i210500@nu.edu.pk';

SELECT * FROM Users WHERE Email = 'i210500@nu.edu.pk';

-- STUDENTS TABLE 
-- (foreign key to the ‘many’ side of the relationship with Projects)
CREATE TABLE Students (
    RollNumber VARCHAR(8) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    BatchNumber INT NOT NULL,
    Campus VARCHAR(50) NOT NULL CHECK (Campus IN ('Islamabad', 'Karachi', 'Lahore', 'Peshawar', 'Chiniot-Faisalabad')),
    Department VARCHAR(50) NOT NULL CHECK (Department IN ('Computing', 'Electrical Engineering', 'Management')),
    Degree VARCHAR(50) NOT NULL CHECK (Degree IN ('AI', 'CS', 'DS', 'CY', 'SE', 'FinTech', 'BA', 'ANF', 'EE', 'CE')),
    Program VARCHAR(50) NOT NULL CHECK (Program IN ('Bachelor''s', 'Master''s')),
    ParentsPhoneNumber VARCHAR(11) NOT NULL,
    ProjectID INT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (ProjectID) REFERENCES Projects(ProjectID),
    CHECK (RollNumber LIKE '[0-9][0-9][IKLPF]-[0-9][0-9][0-9][0-9]')
);

CREATE TRIGGER CheckRollNumber
ON Students
AFTER INSERT, UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM inserted WHERE RollNumber LIKE '[0-9][0-9][IKLPF]-[0-9][0-9][0-9][0-9]')
    BEGIN
        RAISERROR ('RollNumber does not match the required pattern.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END

-- Inserting data into Students Table
INSERT INTO Students (RollNumber, UserID, BatchNumber, Campus, Department, Degree, Program, ParentsPhoneNumber, ProjectID) VALUES
('21I-0500', 4, 2021, 'Islamabad', 'Computing', 'CS', 'Bachelor''s', '03121234567', 2),
('21I-1356', 5, 2021, 'Islamabad', 'Computing', 'CS', 'Bachelor''s', '03722036512', 3),
('21I-2578', 6, 2021, 'Islamabad', 'Computing', 'CS', 'Bachelor''s', '03109373084', 4),
('20I-1356', 7, 2020, 'Islamabad', 'Computing', 'AI', 'Bachelor''s', '03331234000', 2),
('19I-0936', 8, 2019, 'Islamabad', 'Computing', 'SE', 'Bachelor''s', '03722036522', 3),
('20I-1624', 9, 2020, 'Islamabad', 'Computing', 'CS', 'Bachelor''s', '03109373077', 4),
('21I-1234', 10, 2021, 'Islamabad', 'Computing', 'CS', 'Bachelor''s', '03121334567', 2),
('19I-2394', 11, 2019, 'Islamabad', 'Computing', 'DS', 'Bachelor''s', '03721651201', 3),
('20I-1932', 12, 2020, 'Islamabad', 'Computing', 'CY', 'Bachelor''s', '03105373084', 4);

-- UPDATE Students SET ProjectID = 2 WHERE RollNumber = '21I-1356';

-- DELETE FROM Students;

SELECT * FROM Students;


-- SUPERVISORS TABLE
CREATE TABLE Supervisors (
    FacultyNumber VARCHAR(4) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    Department VARCHAR(50) NOT NULL CHECK (Department IN ('Computing', 'Electrical Engineering', 'Management')),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CHECK (FacultyNumber LIKE 'F[0-9][0-9][0-9]')
);

CREATE TRIGGER CheckFacultyNumber
ON Supervisors
AFTER INSERT, UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM inserted WHERE FacultyNumber LIKE 'F[0-9][0-9][0-9]')
    BEGIN
        RAISERROR ('FacultyNumber does not match the required pattern.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END

-- Inserting data into Supervisors Table
INSERT INTO Supervisors (FacultyNumber, UserID, Department) VALUES
('F001', 2, 'Computing'),
('F002', 3, 'Computing'),
('F003', 13, 'Computing'),
('F004', 14, 'Computing'),
('F005', 15, 'Electrical Engineering'),
('F006', 16, 'Computing');

SELECT * FROM Supervisors;


-- ADMINS TABLE
CREATE TABLE Admins (
    AdminNumber VARCHAR(4) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CHECK (AdminNumber LIKE 'A[0-9][0-9][0-9]')
);

CREATE TRIGGER CheckAdminNumber
ON Admins
AFTER INSERT, UPDATE
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM inserted WHERE AdminNumber LIKE 'A[0-9][0-9][0-9]')
    BEGIN
        RAISERROR ('AdminNumber does not match the required pattern.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END

-- Inserting data into Admins Table
INSERT INTO Admins (AdminNumber, UserID) VALUES
('A001', 1);

SELECT * FROM Admins;


-- PROJECTS TABLE
CREATE TABLE Projects (
    ProjectID INT PRIMARY KEY IDENTITY(1,1),
    ProjectName VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    Status VARCHAR(50) DEFAULT 'Active' CHECK (Status IN ('Active', 'Inactive', 'Completed')),
    FacultyNumber VARCHAR(4),
    FOREIGN KEY (FacultyNumber) REFERENCES Supervisors(FacultyNumber)
);

-- Inserting data into Projects Table
INSERT INTO Projects (ProjectName, Description, Status, FacultyNumber) VALUES
('Project Alpha', 'An innovative project.', 'Active', 'F001'),
('Project Beta', 'A groundbreaking exploration of new technology.', 'Active', 'F002'),
('Project Gamma', 'Developing a novel approach to solving industry problems.', 'Active', 'F001');

-- DELETE FROM Projects;

SELECT * FROM Projects;


-- TASKS TABLE
CREATE TABLE Tasks (
    TaskID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT,
    TaskName VARCHAR(100) NOT NULL,
    Description TEXT,
    Status VARCHAR(50) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'In Progress', 'Completed')),
    AssignedTo VARCHAR(8),
    AssignedBy VARCHAR(4),
    FOREIGN KEY (ProjectID) REFERENCES Projects(ProjectID),
    FOREIGN KEY (AssignedTo) REFERENCES Students(RollNumber),
    FOREIGN KEY (AssignedBy) REFERENCES Supervisors(FacultyNumber)
);

Drop Table Tasks;

-- Alter the Tasks table to add the AssignedBy column
ALTER TABLE Tasks
ADD AssignedBy VARCHAR(4);

-- Add a foreign key constraint to ensure integrity between Tasks and Faculty tables
ALTER TABLE Tasks
ADD CONSTRAINT FK_Tasks_Faculty
    FOREIGN KEY (AssignedBy) REFERENCES Faculty(facultyNumber);


-- Inserting data into Tasks Table
INSERT INTO Tasks (ProjectID, TaskName, Description, Status, AssignedTo, AssignedBy) VALUES
(2, 'Literature Review', 'Review related literature.', 'Pending', '21I-0500', 'F001');

SELECT * FROM Tasks;

SELECT t.* FROM Tasks t
INNER JOIN Supervisors s ON t.AssignedBy = s.FacultyNumber
INNER JOIN Users u ON s.UserID = u.UserID
WHERE u.Email = 'akhter.jamil@nu.edu.pk';

-- MEETINGS TABLE
CREATE TABLE Meetings (
    MeetingID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT,
    SupervisorFacultyNumber VARCHAR(4),
    RollNumber VARCHAR(8),
    MeetingDateTime DATETIME NOT NULL,
    Agenda TEXT,
    Complete BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (ProjectID) REFERENCES Projects(ProjectID),
    FOREIGN KEY (SupervisorFacultyNumber) REFERENCES Supervisors(FacultyNumber),
    FOREIGN KEY (RollNumber) REFERENCES Students(RollNumber)
);

ALTER TABLE Meetings
ADD Complete BIT NOT NULL DEFAULT 0;

-- Inserting data into Meetings Table
INSERT INTO Meetings (ProjectID, SupervisorFacultyNumber, RollNumber, MeetingDateTime, Agenda) VALUES
(2, 'F001', '21I-0500', '2024-03-25 10:00:00', 'Discuss project progress');

SELECT * FROM Meetings;


-- DOCUMENTS TABLE
CREATE TABLE Documents (
    DocumentID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT,
    DocumentName VARCHAR(100) NOT NULL,
    FilePath VARCHAR(255) NOT NULL,
    UploadedBy INT,
    FOREIGN KEY (ProjectID) REFERENCES Projects(ProjectID),
    FOREIGN KEY (UploadedBy) REFERENCES Users(UserID)
);

-- Inserting data into Documents Table
INSERT INTO Documents (ProjectID, DocumentName, FilePath, UploadedBy) VALUES
(2, 'Proposal.docx', '/path/to/proposal.docx', 4);

SELECT * FROM Documents;


-- FEEDBACK TABLE
CREATE TABLE Feedback (
    FeedbackID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT,
    RollNumber VARCHAR(8),
    SupervisorFacultyNumber VARCHAR(4),
    Content TEXT NOT NULL,
    Evaluation VARCHAR(50) CHECK (Evaluation IN ('Excellent', 'Good', 'Fair', 'Poor')),
    FOREIGN KEY (ProjectID) REFERENCES Projects(ProjectID),
    FOREIGN KEY (RollNumber) REFERENCES Students(RollNumber),
    FOREIGN KEY (SupervisorFacultyNumber) REFERENCES Supervisors(FacultyNumber)
);

-- Inserting data into Feedback Table
INSERT INTO Feedback (ProjectID, RollNumber, SupervisorFacultyNumber, Content, Evaluation) VALUES
(2, '21I-0500', 'F001', 'Excellent progress so far.', 'Excellent');

SELECT * FROM Feedback;


-- DEFENSES TABLE
CREATE TABLE Defenses (
    DefenseID INT PRIMARY KEY IDENTITY(1,1),
    ProjectID INT,
    DateScheduled DATETIME NOT NULL,
    Location VARCHAR(100),
    FOREIGN KEY (ProjectID) REFERENCES Projects(ProjectID)
);

-- Inserting data into Defenses Table
INSERT INTO Defenses (ProjectID, DateScheduled, Location) VALUES
(2, '2024-06-15 09:00:00', 'Auditorium');

SELECT * FROM Defenses;


-- AUDITTRAIL TABLE
CREATE TABLE AuditTrail (
    AuditID INT PRIMARY KEY IDENTITY(1,1),
    Action VARCHAR(100) NOT NULL,
    UserID INT,
    Timestamp DATETIME NOT NULL,
    Details TEXT,
	FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Inserting into AuditTrail Table
INSERT INTO AuditTrail (Action, UserID, Timestamp, Details) VALUES
('User Login', 1, '2024-03-22 08:00:00', 'Admin logged in');

SELECT * FROM AuditTrail;

-- DISCUSSION POSTS TABLE
CREATE TABLE DiscussionPosts (
    PostID INT PRIMARY KEY IDENTITY,
    UserID INT NOT NULL,
    PostDateTime DATETIME NOT NULL DEFAULT GETDATE(),
    Content TEXT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

SELECT * FROM DiscussionPosts;