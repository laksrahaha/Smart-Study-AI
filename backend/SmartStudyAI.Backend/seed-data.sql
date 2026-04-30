-- Smart Study AI - Database Seed Script
-- Run this to populate sample data

-- Clear existing data (for fresh start)
DELETE FROM Courses;
DELETE FROM Users;

-- Insert Users
INSERT INTO Users (Username) VALUES ('john');
INSERT INTO Users (Username) VALUES ('jane');
INSERT INTO Users (Username) VALUES ('bob');

-- Insert Courses (UserId 1 = john, 2 = jane, 3 = bob)
INSERT INTO Courses (Title, Description, UserId) VALUES ('Math 101', 'Basic mathematics', 1);
INSERT INTO Courses (Title, Description, UserId) VALUES ('English 101', 'Introduction to English', 1);
INSERT INTO Courses (Title, Description, UserId) VALUES ('Science 101', 'Basic science', 2);
INSERT INTO Courses (Title, Description, UserId) VALUES ('History 101', 'World history', 3);