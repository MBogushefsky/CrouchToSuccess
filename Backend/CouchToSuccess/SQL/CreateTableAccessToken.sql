CREATE TABLE `frugal`.`access_token` (
  `ID` VARCHAR(36) NOT NULL,
  `UserID` VARCHAR(36) NOT NULL,
  `AccessToken` VARCHAR(64) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC) VISIBLE,
  UNIQUE INDEX `AccessToken_UNIQUE` (`AccessToken` ASC) VISIBLE);
  ALTER TABLE `frugal`.`access_token` 
CHANGE COLUMN `AccessToken` `Token` VARCHAR(64) NOT NULL ;
