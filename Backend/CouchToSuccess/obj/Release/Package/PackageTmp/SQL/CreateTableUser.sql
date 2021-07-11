CREATE TABLE `frugal`.`user` (
  `ID` VARCHAR(36) NOT NULL,
  `Username` VARCHAR(255) NOT NULL,
  `PasswordHash` VARCHAR(255) NOT NULL,
  `Email` VARCHAR(255) NOT NULL,
  `FirstName` VARCHAR(255) NOT NULL,
  `LastName` VARCHAR(255) NOT NULL,
  `PhoneNumber` VARCHAR(16) NULL,
  `Admin` BIT(1) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC) VISIBLE,
  UNIQUE INDEX `Username_UNIQUE` (`Username` ASC) VISIBLE,
  UNIQUE INDEX `Email_UNIQUE` (`Email` ASC) VISIBLE,
  UNIQUE INDEX `PhoneNumber_UNIQUE` (`PhoneNumber` ASC) VISIBLE);
)
ALTER TABLE `frugal`.`user` 
ADD COLUMN `AccessToken` VARCHAR(64) NULL AFTER `ID`,
ADD UNIQUE INDEX `AccessToken_UNIQUE` (`AccessToken` ASC) VISIBLE;
;
ALTER TABLE `frugal`.`user` 
CHANGE COLUMN `AccessToken` `AccessToken` VARCHAR(64) NULL DEFAULT NULL AFTER `PhoneNumber`;
ALTER TABLE `frugal`.`user` 
DROP COLUMN `AccessToken`,
DROP INDEX `AccessToken_UNIQUE` ;
;
ALTER TABLE `frugal`.`user` 
ADD COLUMN `FakeStockExchangeBalance` DECIMAL(10,2) NOT NULL AFTER `Admin`;
ALTER TABLE `frugal`.`user` 
DROP COLUMN `FakeStockExchangeBalance`;
