CREATE TABLE `frugal`.`stock_exchange_data` (
  `ID` VARCHAR(36) NOT NULL,
  `Symbol` VARCHAR(45) NOT NULL,
  `Date` TIMESTAMP NOT NULL,
  `RangeFound` VARCHAR(45) NOT NULL,
  `Open` DECIMAL(10,2) NULL,
  `Close` DECIMAL(10,2) NULL,
  `High` DECIMAL(10,2) NULL,
  `Low` DECIMAL(10,2) NULL,
  `Volume` INT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC));
  ALTER TABLE `frugal`.`stock_exchange_data` 
RENAME TO  `frugal`.`stock_exchange_chart` ;
ALTER TABLE `frugal`.`stock_exchange_chart` 
CHANGE COLUMN `Date` `Timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP ;
