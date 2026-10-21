-- =====================================================================
-- CivicResolve - Municipal Smart Grievance & Complaint Redressal Portal
-- Database: MySQL 8.0+ / MariaDB
-- Compatibility: phpMyAdmin, MySQL Workbench, XAMPP, Command-line mysql
-- =====================================================================

CREATE DATABASE IF NOT EXISTS `civicresolve_db` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `civicresolve_db`;

-- --------------------------------------------------------
-- Table: Departments
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `Departments` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(100) NOT NULL,
    `Code` VARCHAR(10) NOT NULL UNIQUE,
    `Description` VARCHAR(300) NULL,
    `HodName` VARCHAR(100) NOT NULL,
    `ContactEmail` VARCHAR(150) NOT NULL,
    `ContactPhone` VARCHAR(20) NOT NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------
-- Table: Categories
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `Categories` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(100) NOT NULL,
    `DepartmentId` INT NOT NULL,
    `SlaHours` INT NOT NULL DEFAULT 48,
    `Description` VARCHAR(250) NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_Categories_DepartmentId` (`DepartmentId`),
    CONSTRAINT `FK_Categories_Departments` FOREIGN KEY (`DepartmentId`) 
        REFERENCES `Departments` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------
-- Table: Complaints
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `Complaints` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `TrackingNumber` VARCHAR(20) NOT NULL UNIQUE,
    `CitizenName` VARCHAR(100) NOT NULL,
    `CitizenEmail` VARCHAR(150) NOT NULL,
    `CitizenPhone` VARCHAR(15) NOT NULL,
    `WardNumber` INT NOT NULL,
    `Address` VARCHAR(200) NOT NULL,
    `Landmark` VARCHAR(100) NULL,
    `CategoryId` INT NOT NULL,
    `DepartmentId` INT NOT NULL,
    `Priority` INT NOT NULL DEFAULT 2, -- 1=Low, 2=Medium, 3=High, 4=Critical
    `Status` INT NOT NULL DEFAULT 1,   -- 1=Submitted, 2=UnderReview, 3=Assigned, 4=InProgress, 5=Resolved, 6=Rejected
    `Title` VARCHAR(120) NOT NULL,
    `Description` TEXT NOT NULL,
    `AttachmentPath` VARCHAR(255) NULL,
    `AssignedOfficer` VARCHAR(100) NULL,
    `OfficerRemarks` TEXT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` DATETIME NULL,
    `SlaDeadline` DATETIME NULL,
    `ResolvedAt` DATETIME NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_Complaints_TrackingNumber` (`TrackingNumber`),
    KEY `IX_Complaints_CitizenPhone` (`CitizenPhone`),
    KEY `IX_Complaints_DepartmentId` (`DepartmentId`),
    KEY `IX_Complaints_CategoryId` (`CategoryId`),
    CONSTRAINT `FK_Complaints_Departments` FOREIGN KEY (`DepartmentId`) 
        REFERENCES `Departments` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Complaints_Categories` FOREIGN KEY (`CategoryId`) 
        REFERENCES `Categories` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------
-- Table: ComplaintStatusHistories
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ComplaintStatusHistories` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `ComplaintId` INT NOT NULL,
    `OldStatus` INT NOT NULL,
    `NewStatus` INT NOT NULL,
    `ActionBy` VARCHAR(100) NOT NULL DEFAULT 'System',
    `Remarks` VARCHAR(500) NOT NULL,
    `ChangedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    KEY `IX_StatusHistory_ComplaintId` (`ComplaintId`),
    CONSTRAINT `FK_Histories_Complaints` FOREIGN KEY (`ComplaintId`) 
        REFERENCES `Complaints` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------
-- Table: CitizenFeedbacks
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `CitizenFeedbacks` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `ComplaintId` INT NOT NULL,
    `Rating` INT NOT NULL,
    `Comments` VARCHAR(500) NULL,
    `SubmittedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    KEY `IX_Feedback_ComplaintId` (`ComplaintId`),
    CONSTRAINT `FK_Feedback_Complaints` FOREIGN KEY (`ComplaintId`) 
        REFERENCES `Complaints` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------
-- Seed Data: Departments
-- --------------------------------------------------------
INSERT INTO `Departments` (`Id`, `Name`, `Code`, `Description`, `HodName`, `ContactEmail`, `ContactPhone`, `IsActive`) VALUES
(1, 'Solid Waste & Sanitation', 'SANIT', 'Garbage collection, public bins, street sweeping, and dumping ground hygiene.', 'Dr. Rajesh K. Verma', 'sanitation@municipal.gov.in', '9823012345', 1),
(2, 'Roads & Civil Infrastructure', 'ROADS', 'Pothole repairs, footpath maintenance, asphalt laying, and storm drainage covers.', 'Er. Sunita Rao', 'roads@municipal.gov.in', '9876543210', 1),
(3, 'Water Supply & Sewerage', 'WATER', 'Potable water pipeline leaks, contaminated supply, low pressure, and sewage overflow.', 'Er. Amit S. Deshmukh', 'watersupply@municipal.gov.in', '9821098765', 1),
(4, 'Streetlights & Electrical', 'ELEC', 'Dark streetlights, malfunctioning timers, damaged poles, and exposed live wires.', 'Er. Vikram Nair', 'electrical@municipal.gov.in', '9765432109', 1),
(5, 'Public Health & Pest Control', 'HEALTH', 'Mosquito fogging, dengue/malaria outbreak prevention, and stray animal vaccination.', 'Dr. Neha Sharma', 'health@municipal.gov.in', '9845012398', 1);

-- --------------------------------------------------------
-- Seed Data: Categories
-- --------------------------------------------------------
INSERT INTO `Categories` (`Id`, `Name`, `DepartmentId`, `SlaHours`, `Description`) VALUES
(1, 'Uncollected Garbage / Full Public Bin', 1, 24, 'Overflowing waste bin or garbage left on roadside.'),
(2, 'Dead Animal Removal', 1, 12, 'Carcass of animal needing urgent hygienic disposal.'),
(3, 'Illegal Dumping of Construction Waste', 1, 48, 'Debris dumped on open plots or roadways.'),
(4, 'Severe Pothole / Crater', 2, 48, 'Road hazard causing vehicle damage or accidents.'),
(5, 'Broken Footpath / Manhole Cover Missing', 2, 24, 'Pedestrian safety hazard.'),
(6, 'Waterlogging / Blocked Storm Drain', 2, 24, 'Stagnant rainwater causing traffic blockages.'),
(7, 'Major Pipeline Burst / Water Leakage', 3, 12, 'Fresh drinking water wastage on street or main line.'),
(8, 'Contaminated / Muddy Water Supply', 3, 24, 'Tap water smelling foul or containing sediment.'),
(9, 'Sewage Overflow on Street', 3, 18, 'Foul-smelling sewer leakage hazardous to public health.'),
(10, 'Streetlight Not Working (Entire Street Dark)', 4, 36, 'Cluster of streetlights off at night.'),
(11, 'Exposed Electric Wire / Tilted Pole', 4, 6, 'High-risk shock hazard needing immediate response.'),
(12, 'Mosquito Breeding / Fogging Request', 5, 48, 'Preventive chemical fogging for stagnant water spots.');

-- --------------------------------------------------------
-- Seed Data: Sample Complaints
-- --------------------------------------------------------
INSERT INTO `Complaints` (`Id`, `TrackingNumber`, `CitizenName`, `CitizenEmail`, `CitizenPhone`, `WardNumber`, `Address`, `Landmark`, `CategoryId`, `DepartmentId`, `Priority`, `Status`, `Title`, `Description`, `AssignedOfficer`, `OfficerRemarks`, `CreatedAt`, `UpdatedAt`, `SlaDeadline`, `ResolvedAt`) VALUES
(1, 'CR-2026-10101', 'Aarav Patel', 'aarav.patel@example.com', '9820123456', 14, 'Crossroad 4, Near Gandhi Garden, Ward 14', 'Opposite State Bank ATM', 4, 2, 3, 4, 'Deep dangerous pothole near school crossing', 'There is a massive pothole roughly 2 feet wide right in front of St. Mary\'s School entrance. Two two-wheelers have skidded here during rainfall. Please repair urgently before an accident occurs.', 'Inspector Ramesh G.', 'Inspected site on Oct 19. Cold-mix asphalt repair team scheduled for road resurfacing.', DATE_SUB(NOW(), INTERVAL 3 DAY), DATE_SUB(NOW(), INTERVAL 1 DAY), DATE_ADD(NOW(), INTERVAL 1 DAY), NULL),
(2, 'CR-2026-10102', 'Pooja Kulkarni', 'pooja.k@example.com', '9890234567', 8, 'Flat 302, Green Avenue, Shivaji Nagar', 'Behind Municipal Market', 7, 3, 4, 5, 'Underground pipeline burst, potable water gushing onto main road', 'A main 6-inch water supply pipe ruptured early morning. Gallons of clean drinking water are being wasted and water pressure in our colony is zero.', 'Eng. Nilesh Patil', 'Emergency valve closed, damaged connector replaced and pressure restored successfully.', DATE_SUB(NOW(), INTERVAL 5 DAY), DATE_SUB(NOW(), INTERVAL 4 DAY), DATE_SUB(NOW(), INTERVAL 4 DAY), DATE_SUB(NOW(), INTERVAL 4 DAY)),
(3, 'CR-2026-10103', 'Mohammed Farhan', 'farhan.m@example.com', '9765123890', 22, 'Plot 55, Sector 9, Nehru Road', 'Near Community Hall', 1, 1, 2, 1, 'Community dumpster not cleared for 4 days', 'The municipal garbage truck has not visited our street since Saturday. Waste is spilling over into the pedestrian walkway causing severe stench and stray animals gathering.', NULL, NULL, DATE_SUB(NOW(), INTERVAL 10 HOUR), NULL, DATE_ADD(NOW(), INTERVAL 14 HOUR), NULL),
(4, 'CR-2026-10104', 'Sneha Iyer', 'sneha.iyer@example.com', '9845112233', 5, 'Street 12, Shanti Vihar', 'Near Public Library', 10, 4, 1, 3, 'Entire 200m stretch of streetlights dark for 3 days', 'Streetlights from pole no. 14 to 22 are completely non-functional. The lane is pitch dark at night, causing safety concerns for women and elderly residents returning late.', 'Lineman S. Jadhav', 'Assigned to regional lighting maintenance depot.', DATE_SUB(NOW(), INTERVAL 1 DAY), NULL, DATE_ADD(NOW(), INTERVAL 1 DAY), NULL),
(5, 'CR-2026-10105', 'Vikram Singh', 'vikram.singh@example.com', '9823998877', 3, 'Corner of Market Road, Ward 3', 'Near Bus Stop #4', 11, 4, 4, 2, 'Exposed hanging cable touching tree branches near bus stop', 'A snapped low-tension power cable is hanging at chest height from pole #8. It was sparking during morning drizzle. Immediate attention required before anyone touches it.', NULL, NULL, DATE_SUB(NOW(), INTERVAL 2 HOUR), NULL, DATE_ADD(NOW(), INTERVAL 4 HOUR), NULL);

-- --------------------------------------------------------
-- Seed Data: Status Audit Histories
-- --------------------------------------------------------
INSERT INTO `ComplaintStatusHistories` (`ComplaintId`, `OldStatus`, `NewStatus`, `ActionBy`, `Remarks`, `ChangedAt`) VALUES
(1, 1, 2, 'Ward Helpdesk', 'Grievance received and verified with Ward 14 records.', DATE_SUB(NOW(), INTERVAL 70 HOUR)),
(1, 2, 4, 'Er. Sunita Rao (HOD)', 'Assigned to road repair squad. Inspector Ramesh dispatched.', DATE_SUB(NOW(), INTERVAL 48 HOUR)),
(2, 1, 4, 'Water Control Room', 'Urgent dispatch to site repair team.', DATE_SUB(NOW(), INTERVAL 119 HOUR)),
(2, 4, 5, 'Eng. Nilesh Patil', 'Ruptured pipe replaced and clean water supply restored.', DATE_SUB(NOW(), INTERVAL 96 HOUR));

-- --------------------------------------------------------
-- Seed Data: Citizen Feedback
-- --------------------------------------------------------
INSERT INTO `CitizenFeedbacks` (`ComplaintId`, `Rating`, `Comments`, `SubmittedAt`) VALUES
(2, 5, 'Extremely impressed with the rapid response! The pipeline repair was completed within 3 hours of lodging the complaint.', DATE_SUB(NOW(), INTERVAL 94 HOUR));

-- End of civicresolve_mysql_schema_and_seed.sql
