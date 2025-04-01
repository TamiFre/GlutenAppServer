    
   

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

--יצירת טבלת סטטוס
CREATE TABLE Statuses(
StatusID INT PRIMARY KEY,
StatusDesc NVARCHAR (50)
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
UserEmail NVARCHAR (50),
TypeID INT,
FOREIGN KEY (TypeID) REFERENCES TypeUserId(TypeID)  --Foreign key to show user types
);

--יצירת טבלת מסעדות
CREATE TABLE Restaurants(
RestID INT PRIMARY KEY IDENTITY,               --Primary Key
RestAddress NVARCHAR (100),                     --Address
RestName NVARCHAR(70),
IsSterile INT,                                 --0 not sterile 1 sterile
UserID INT,
FOREIGN KEY (UserID) REFERENCES Users(UserID), --Foreign key from users to show the restaurant manager
TypeFoodID INT,
FOREIGN KEY (TypeFoodID) REFERENCES TypeFood(TypeFoodID), --Foreign key from type food to show the type
StatusID INT,                                      --Foreign key - status
FOREIGN KEY (StatusID) REFERENCES Statuses(StatusID)
);




--יצירת טבלת ביקורות
CREATE TABLE Critics(
CriticID INT PRIMARY KEY IDENTITY,  --Primary Key
CriticText NVARCHAR (1000),         --The Text 
Rate INT,
UserID INT,
FOREIGN KEY (UserID) REFERENCES Users(UserID), --Foreign key - the writer 
RestID INT,
FOREIGN KEY (RestID) REFERENCES Restaurants(RestID) --Foreign key - the said restaurant
);

--יצירת טבלת מתכונים
CREATE TABLE Recipes(
RecipeID INT PRIMARY KEY IDENTITY,  --Primary key
RecipeText NVARCHAR (1000),         -- The actual recipe
RecipeHeadLine NVARCHAR (100),
TypeFoodID INT,
FOREIGN KEY (TypeFoodID) REFERENCES TypeFood(TypeFoodID),
UserID INT,
FOREIGN KEY (UserID) REFERENCES Users(UserID), --Foreign key - the writer
StatusID INT,                                      --Foreign key - status
FOREIGN KEY (StatusID) REFERENCES Statuses(StatusID)
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
INSERT INTO Users VALUES ('Gal', 'GalTheBest123','Gal@gmail.com', 1)        --gal the first user. userid = 1, typeid = 1
INSERT INTO Users VALUES ('Shahar', 'ShaharTheBest123','Shahar@gmail.com', 3)  --shahar the first restaurant manager. userid = 2, typeid = 3
INSERT INTO Users VALUES ('Tami', 'TamiFre123','Tami@gmail.com', 2)          --tami the admin
INSERT INTO Statuses VALUES (1,'Approved')
INSERT INTO Statuses VALUES (2,'Pending')
INSERT INTO Statuses VALUES (3,'Declined')
INSERT INTO Restaurants VALUES ('Weizmann Street 13, Kfar Saba, Israel','Abu Falafel ',0, 2, 1,2)       --shahar's restaurant - restid =1, typefood = italian
INSERT INTO Restaurants VALUES ('Chaim Levanon Street 4, Tel Aviv-Yafo, Israel','Burgers',0,2,1,2)
INSERT INTO Restaurants VALUES ('Ramatayim Road 5, Hod Hasharon, Israel','Steaks & Wine',0,2,2,1)
INSERT INTO Restaurants VALUES ('Ramatayim Road 3, Hod Hasharon, Israel','Feel (Gluten)Free ',1,2,3,1)
INSERT INTO Restaurants VALUES ('Levinski Street 3, Tel Aviv-Yafo, Israel','Shawarma',1,2,4,1)
INSERT INTO Restaurants VALUES ('Levinski Street 10, Tel Aviv-Yafo, Israel','Gluten Free Euphoria',1,2,1,2)
INSERT INTO Restaurants VALUES ('Dizengoff Street 3, Tel Aviv-Yafo, Israel','Heart Of Tel Aviv',0,2,1,3)
INSERT INTO Recipes VALUES ('Take Two rice papers. Put egg on them. Fry them ontop of one other','Malawah',2, 1, 2)                  --gal's recipe
INSERT INTO Critics VALUES ('Good food, good vibes and a fine dining experience',4, 1,1)  --gal's critic - about shahar's restaurant restid = 1, userid = 1 
INSERT INTO Critics VALUES ('enjoyed the food, felt good afterwards meaning no traces of gluten',4, 2,1)  --gal's critic - about shahar's restaurant restid = 1, userid = 1 
INSERT INTO Critics VALUES ('amazing gluten free food and staff',5, 1,2)
INSERT INTO Critics VALUES ('average food, not sterile at all ',3, 1,3)
INSERT INTO Critics VALUES ('almost no options for gluten free. a hair in the food',2, 3,1)
INSERT INTO Critics VALUES ('good',5, 3,1)
INSERT INTO Critics VALUES ('the worst gluten free options i have ever seen',1, 3,1)
INSERT INTO Critics VALUES ('nice gluten free dishes',4, 2,3)
INSERT INTO Critics VALUES ('average service and food',3, 2,4)
INSERT INTO Critics VALUES ('i do not recommend going here',2, 2,3)


--insert to restaurants and recipes pending
INSERT INTO Recipes VALUES ('Heat Up What you have in the bag for ten minutes','Burekas',3,1,2)
INSERT INTO Recipes VALUES ('break an egg into a hot pan and wait till you hear sizzling','Omlet',4,1,2)
INSERT INTO Recipes VALUES ('Heat up Water and insert an egg','Hard Boiled Egg',1,1,3)
INSERT INTO Recipes VALUES ('Dont eat','Starve',2,1,1)

SELECT * FROM Statuses
SELECT * FROM TypeFood

SELECT * FROM TypeFood
SELECT * FROM Recipes
SELECT * FROM Users
SELECT * FROM Restaurants
SELECT * FROM Critics

--EF Code
/*
scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=GlutenFree_DB;User ID=AppAdminLogin;Password=Tami;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context GlutenFree_DB_Context -DataAnnotations -force
*/
