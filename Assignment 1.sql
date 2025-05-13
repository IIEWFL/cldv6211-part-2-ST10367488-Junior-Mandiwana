-- DATABASE CREATION SECTION
USE master;
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EventEase')
DROP DATABASE EventEase;

CREATE DATABASE EventEase;

USE EventEase;

-- Venue Table
CREATE TABLE Venue (
    VenueId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    VenueName VARCHAR(50) UNIQUE NOT NULL,
    [Location] VARCHAR(50) NOT NULL,
    Capacity INT NOT NULL,
    ImageUrl VARCHAR(MAX)
);

-- Event Table
CREATE TABLE [Event] (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    EventName VARCHAR(255) UNIQUE NOT NULL,
    EventDate Date NOT NULL,
    [Description] VARCHAR (250) NOT NULL
);

-- Booking Table
CREATE TABLE Booking (
    BookingId INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    EventId INT FOREIGN KEY REFERENCES [Event](EventId),  
    VenueId INT FOREIGN KEY REFERENCES Venue(VenueId),  
    BookingDate DATE NOT NULL
);

-- Create the View using dynamic SQL to avoid batch separation issues
EXEC('
CREATE VIEW vw_BookingDetails AS
SELECT
    B.BookingId,
    E.EventName,
    E.EventDate,
    E.[Description],
    V.VenueName,
    V.[Location],
    V.Capacity,
    B.BookingDate
FROM Booking B
JOIN [Event] E ON B.EventId = E.EventId
JOIN Venue V ON B.VenueId = V.VenueId;
');

-- Insert data into Venue table
INSERT INTO VENUE(VenueName,[Location], Capacity, ImageURL)
VALUES
('Gold Reef City', 'Johannesburg', 62000, 'https://url');

-- Insert data into Event table
INSERT INTO [Event] (EventName, EventDate, [Description])
VALUES
('Sunset Yoga', '2025-05-10', 'A relaxing yoga session with a sunset view.'),
('Hiking Tour', '2025-06-15', 'Guided hike through scenic mountain trails.'),
('Music Festival', '2025-07-20', 'Live performances by top local artists.'),
('Food and Wine Expo', '2025-08-05', 'Gourmet food tasting and wine pairing.'),
('Botanical Art Show', '2025-09-12', 'Exhibition of nature-inspired art pieces.');

-- Insert data into Booking table
INSERT INTO Booking (EventID, VenueID, BookingDate)
VALUES
(1, 1, '2025-04-01'),
(1, 1, '2025-04-02'),
(2, 1, '2025-04-03');

-- Select data for verification
SELECT * FROM Venue;
SELECT * FROM [Event];
SELECT * FROM Booking;
SELECT * FROM vw_BookingDetails;
