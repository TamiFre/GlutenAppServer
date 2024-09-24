
   

    USE master
    GO
    IF EXISTS (SELECT * FROM sys.databases WHERE name = N'GlutenFree_DB')
    BEGIN
    DROP DATABASE GlutenFree_DB;
END
Go
Create Database GlutenFree_DB
Go
Use GlutenFree_DB
Go

--יצירת טבלת משתמשים
CREATE TABLE Users(
UserID INT PRIMARY KEY IDENTITY,         --Primary key
UserName NVARCHAR (20),                  --The Username
UserPass NVARCHAR (20),                  --Password
TypeID INT,
FOREIGN KEY (TypeID) REFERENCES TypeUserId(TypeID)  --Foreign key to show user types
);

--יצירת טבלת סוג משתמשים
CREATE TABLE TypeUserID(
TypeID INT PRIMARY KEY IDENTITY,       --Primary Key
TypeName NVARCHAR (20)                 --Type Name
);

--יצירת טבלת מסעדות
CREATE TABLE Restaurants(
RestID INT PRIMARY KEY IDENTITY,               --Primary Key
RestAddress NVARCHAR (70),                     --Address
UserID INT,
FOREIGN KEY (UserID) REFERENCES Users(UserID), --Foreign key from users to show the restaurant manager
TypeFoodID INT,
FOREIGN KEY (TypeFoodID) REFERENCES TypeFood(TypeFoodID) --Foreign key from type food to show the type
);

--יצירת טבלת סוג אוכל
CREATE TABLE TypeFood(
TypeFoodID INT PRIMARY KEY IDENTITY,   --Primary Key
TypeFoodName NVARCHAR (30)             --Types of food categories
);

--יצירת טבלת ביקורות
CREATE TABLE Critics(
CriticID INT PRIMARY KEY IDENTITY,  --Primary Key
CriticText NVARCHAR (1000),         --The Text 
UserID INT,
FOREIGN KEY (UserID) REFERENCES Users(UserID) --Foreign key - the writer 
);

--יצירת טבלת מתכונים
CREATE TABLE Recipes(
RecipeID INT PRIMARY KEY IDENTITY,  --Primary key
RecipeText NVARCHAR (1000),         -- The actual recipe
UserID INT,
FOREIGN KEY (UserID) REFERENCES Users(UserID) --Foreign key - the writer
);

--יצירת טבלת מידע
CREATE TABLE Information(
InfoID INT PRIMARY KEY IDENTITY,   --Primary key
InfoText NVARCHAR (1000)           --The fun fact
);

--לוגין לאדמין 
CREATE LOGIN [AppAdminLogin] WITH PASSWORD = 'Tami';
Go

--יצירת יוזר בשביל הלוגין
CREATE USER [AppAdminUser] FOR LOGIN [AppAdminLogin];
Go

--להוסיף את היוזר לפריבילגיות של אדמין
ALTER ROLE DB_Owner ADD MEMBER [AppAdminUser];
Go

--הכנסת ערכים לטבלאות שצריכות דיפולט

--נכניס סוגי יוזרז לטבלת סוגי יוזרז
INSERT INTO TypeUserID VALUES ('Regular')
INSERT INTO TypeUserID VALUES ('Admin')
INSERT INTO TypeUserID VALUES ('RestManager')

--נכניס סוגי אוכל לטבלת סוגי האוכל
INSERT INTO TypeFood VALUES ('Italian')
INSERT INTO TypeFood VALUES ('Asian')
INSERT INTO TypeFood VALUES ('Mexican')
INSERT INTO TypeFood VALUES ('BBQ')
INSERT INTO TypeFood VALUES ('French')

