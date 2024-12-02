CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125165312_InitUser')
BEGIN
    CREATE TABLE `Users` (
        `ID` bigint NOT NULL AUTO_INCREMENT,
        `Username` varchar(30) NOT NULL,
        `Passhash` varchar(100) NOT NULL,
        PRIMARY KEY (`ID`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125165312_InitUser')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241125165312_InitUser', '8.0.11');
END;

COMMIT;

START TRANSACTION;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125173052_Hash128')
BEGIN
    ALTER TABLE `Users` MODIFY `Passhash` varchar(128) NOT NULL;
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125173052_Hash128')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241125173052_Hash128', '8.0.11');
END;

COMMIT;

START TRANSACTION;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125175346_UniqueUsername')
BEGIN
    CREATE UNIQUE INDEX `IX_Users_Username` ON `Users` (`Username`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125175346_UniqueUsername')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241125175346_UniqueUsername', '8.0.11');
END;

COMMIT;

START TRANSACTION;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125180935_TooLengthyPassHash')
BEGIN
    ALTER TABLE `Users` MODIFY `Passhash` varchar(64) NOT NULL;
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125180935_TooLengthyPassHash')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241125180935_TooLengthyPassHash', '8.0.11');
END;

COMMIT;

START TRANSACTION;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125181208_CreatedDate')
BEGIN
    ALTER TABLE `Users` ADD `CreatedAt` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000';
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20241125181208_CreatedDate')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20241125181208_CreatedDate', '8.0.11');
END;

COMMIT;

