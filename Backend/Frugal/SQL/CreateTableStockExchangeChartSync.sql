CREATE TABLE `frugal`.`stock_exchange_chart_sync` (
  `ID` VARCHAR(36) NOT NULL,
  `Symbol` VARCHAR(45) NOT NULL,
  `RangeFound` VARCHAR(45) NOT NULL,
  `LastUpdatedDate` TIMESTAMP NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE INDEX `ID_UNIQUE` (`ID` ASC));
