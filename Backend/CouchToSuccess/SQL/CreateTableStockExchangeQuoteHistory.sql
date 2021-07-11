CREATE TABLE `frugal`.`stock_exchange_quote_history` (
  `ID` VARCHAR(36) NOT NULL,
  `Symbol` VARCHAR(45) NOT NULL,
  `Price` DECIMAL(10,2) NOT NULL,
  `Timestamp` TIMESTAMP NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC));
