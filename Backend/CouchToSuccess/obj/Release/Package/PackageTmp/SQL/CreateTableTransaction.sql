CREATE TABLE `frugal`.`transaction` (
  `ID` VARCHAR(36) NOT NULL,
  `UserID` VARCHAR(36) NOT NULL,
  `PlaidAccountID` VARCHAR(64) NOT NULL,
  `MerchantName` VARCHAR(255) NULL,
  `Name` VARCHAR(255) NOT NULL,
  `Amount` DECIMAL(15,2) NOT NULL,
  `Pending` BIT(1) NOT NULL,
  `PaymentChannel` VARCHAR(45) NOT NULL,
  `Categories` VARCHAR(255) NULL,
  `Date` DATE NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `idtransaction_UNIQUE` (`ID` ASC) VISIBLE);
  ALTER TABLE `frugal`.`transaction` 
CHANGE COLUMN `Date` `TransactionDate` DATE NOT NULL ;
ALTER TABLE `frugal`.`transaction` 
CHANGE COLUMN `Amount` `CostAmount` DECIMAL(15,2) NOT NULL ;
ALTER TABLE `frugal`.`transaction` 
ADD COLUMN `PlaidTransactionID` VARCHAR(64) NOT NULL AFTER `ID`,
ADD UNIQUE INDEX `PlaidTransactionID_UNIQUE` (`PlaidTransactionID` ASC) VISIBLE;
;
