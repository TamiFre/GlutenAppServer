
   

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

--יצירת טבלת סוג אוכל
CREATE TABLE TypeFood(
TypeFoodID INT PRIMARY KEY IDENTITY,   --Primary Key
TypeFoodName NVARCHAR (30)             --Types of food categories
);

--יצירת טבלת סוג משתמשים
CREATE TABLE TypeUserID(
TypeID INT PRIMARY KEY IDENTITY,       --Primary Key
TypeName NVARCHAR (20)                 --Type Name
);

--יצירת טבלת משתמשים
CREATE TABLE Users(
UserID INT PRIMARY KEY IDENTITY,         --Primary key
UserName NVARCHAR (20),                  --The Username
UserPass NVARCHAR (20),                  --Password
TypeID INT,
FOREIGN KEY (TypeID) REFERENCES TypeUserId(TypeID)  --Foreign key to show user types
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


--יצירת טבלת ביקורות
CREATE TABLE Critics(
CriticID INT PRIMARY KEY IDENTITY,  --Primary Key
CriticText NVARCHAR (1000),         --The Text 
UserID INT,
FOREIGN KEY (UserID) REFERENCES Users(UserID), --Foreign key - the writer 
RestID INT,
FOREIGN KEY (RestID) REFERENCES Restaurants(RestID) --Foreign key - the said restaurant
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

--הכנסת דמה לכל הטבלאות
INSERT INTO Users VALUES ('Gal', 'GalTheBest123', 1)        --gal the first user. userid = 1, typeid = 1
INSERT INTO Users VALUES ('Shahar', 'ShaharTheBest123', 3)  --shahar the first restaurant manager. userid = 2, typeid = 3
INSERT INTO Users VALUES ('Tami', 'TamiFre123', 2)  --tami the admin
INSERT INTO Restaurants VALUES ('Ramon School', 2, 1)       --shahar's restaurant - restid =1, typefood = italian
INSERT INTO Recipes VALUES ('lalalala', 1)                  --gal's recipe
INSERT INTO Critics VALUES ('I love this restaurant', 1,1)  --gal's critic - about shahar's restaurant restid = 1, userid = 1 
INSERT INTO Information VALUES ('Gluten is tasty - Shahar Shalgi')  --shahar's information - will be deleted

SELECT * FROM Users

--EF Code
/*
scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=MyAppName_DB;User ID=TaskAdminLogin;Password=kukuPassword;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context TamiDBContext -DataAnnotations -force

 scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=GlutenFree_DB;User ID=AppAdminLogin;Password=Tami;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context GlutenFree_DB_Context -DataAnnotations -force
*/
