CREATE TABLE `frugal`.`stock_exchange_transactions` (
  `ID` VARCHAR(36) NOT NULL,
  `Symbol` VARCHAR(36) NOT NULL,
  `Type` VARCHAR(36) NOT NULL,
  `Amount` INT NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC));
  ALTER TABLE `frugal`.`stock_exchange_transactions` 
ADD COLUMN `UserID` VARCHAR(36) NOT NULL AFTER `ID`;
ALTER TABLE `frugal`.`stock_exchange_transactions` 
CHANGE COLUMN `Symbol` `Symbol` VARCHAR(36) NULL ;
ALTER TABLE `frugal`.`stock_exchange_transactions` 
ADD COLUMN `StockAmount` INT NULL AFTER `CurrencyAmount`,
CHANGE COLUMN `Amount` `CurrencyAmount` INT(11) NULL ;
ALTER TABLE `frugal`.`stock_exchange_transactions` 
RENAME TO  `frugal`.`stock_exchange_transaction` ;
ALTER TABLE `frugal`.`stock_exchange_transaction` 
CHANGE COLUMN `CurrencyAmount` `CurrencyAmount` DECIMAL(10,2) NULL DEFAULT NULL ;
ALTER TABLE `frugal`.`stock_exchange_transaction` 
DROP COLUMN `CurrencyAmount`,
ADD COLUMN `StockRate` DECIMAL(10,2) NULL AFTER `StockAmount`;
ALTER TABLE `frugal`.`stock_exchange_transaction` 
ADD COLUMN `CurrencyAmount` DECIMAL(10,2) NULL AFTER `Type`;
ALTER TABLE `frugal`.`stock_exchange_transaction` 
ADD COLUMN `CreatedDate` DATETIME NOT NULL AFTER `StockRate`;
ALTER TABLE `frugal`.`stock_exchange_transaction` 
CHANGE COLUMN `CreatedDate` `CreatedDate` TIMESTAMP NOT NULL ;
