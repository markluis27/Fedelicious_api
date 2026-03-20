USE fedelicious;
GO

CREATE TABLE MenuItems (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Category NVARCHAR(50) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(10,2) NOT NULL,
    ImageURL NVARCHAR(255)
);
GO

-- Let's insert your two classic items so we have data to test!
INSERT INTO MenuItems (Category, Name, Description, Price, ImageURL)
VALUES 
('WINGS', 'Classic Buffalo Wings', 'Spicy and crispy.', 199.00, 'img/classic.jpg'),
('WINGS', 'Garlic Parmesan Wings', 'Creamy garlic.', 219.00, 'img/garlic.jpg');