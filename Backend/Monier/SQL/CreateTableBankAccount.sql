CREATE TABLE `monier`.`bank_account` (
  `ID` VARCHAR(36) NOT NULL,
  `InstitutionID` VARCHAR(45) NOT NULL,
  `PlaidAccountID` VARCHAR(45) NOT NULL,
  `Name` VARCHAR(255) NOT NULL,
  `FullName` VARCHAR(255) NOT NULL,
  `Type` VARCHAR(45) NOT NULL,
  `SubType` VARCHAR(45) NOT NULL,
  `Mask` VARCHAR(45) NOT NULL,
  `AvailableBalance` DECIMAL NOT NULL,
  `CurrentBalance` DECIMAL NOT NULL,
  `LimitBalance` DECIMAL NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC) VISIBLE,
  UNIQUE INDEX `PlaidAccountID_UNIQUE` (`PlaidAccountID` ASC) VISIBLE);
  ALTER TABLE `monier`.`bank_account` 
CHANGE COLUMN `PlaidAccountID` `PlaidAccountID` VARCHAR(45) NOT NULL AFTER `ID`;
ALTER TABLE `monier`.`bank_account` 
ADD COLUMN `UserID` VARCHAR(36) NOT NULL AFTER `ID`;
ALTER TABLE `monier`.`bank_account` 
CHANGE COLUMN `FullName` `FullName` VARCHAR(255) NULL ;
ALTER TABLE `monier`.`bank_account` 
CHANGE COLUMN `PlaidAccountID` `PlaidAccountID` VARCHAR(45) NOT NULL AFTER `ID`;
ALTER TABLE `monier`.`bank_account` 
CHANGE COLUMN `AvailableBalance` `AvailableBalance` DECIMAL(10,2) NOT NULL ,
CHANGE COLUMN `CurrentBalance` `CurrentBalance` DECIMAL(10,2) NOT NULL ,
CHANGE COLUMN `LimitBalance` `LimitBalance` DECIMAL(10,2) NULL DEFAULT NULL ;
