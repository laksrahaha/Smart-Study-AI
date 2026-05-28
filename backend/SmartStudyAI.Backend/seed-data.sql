-- Smart Study AI - Database Seed Script
-- Run this to populate sample data

-- Clear existing data for fresh start
DELETE FROM Courses;
DELETE FROM Users;

-- Reset SQLite auto-increment counters
DELETE FROM sqlite_sequence WHERE name='Courses';
DELETE FROM sqlite_sequence WHERE name='Users';

-- Insert Users
INSERT INTO Users (Username, Email, PasswordHash)
VALUES ('john', 'john@example.com', 'password123');

INSERT INTO Users (Username, Email, PasswordHash)
VALUES ('jane', 'jane@example.com', 'password123');

INSERT INTO Users (Username, Email, PasswordHash)
VALUES ('bob', 'bob@example.com', 'password123');

-- Insert Courses
INSERT INTO Courses (Title, Description, UserId)
VALUES ('Math 101', 'Basic mathematics', 1);

INSERT INTO Courses (Title, Description, UserId)
VALUES ('English 101', 'Introduction to English', 1);

INSERT INTO Courses (Title, Description, UserId)
VALUES ('Science 101', 'Basic science', 2);

INSERT INTO Courses (Title, Description, UserId)
VALUES ('History 101', 'World history', 3);