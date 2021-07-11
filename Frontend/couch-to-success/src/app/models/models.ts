export class User {
	Id: string;
	Username: string;
	PasswordHash: string;
	Email: string;
	FirstName: string;
	LastName: string;
	PhoneNumber: string;
	Admin: boolean;
}

export class BankAccount {
	Id: string;
	UserId: string;
	PlaidAccountId: string;
	InstitutionId: string;
	Name: string;
	FullName: string;
	Type: string;
	SubType: string;
	Mask: string;
	AvailableBalance: number;
	CurrentBalance: number;
	LimitBalance: number;
}

export class Transaction {
	Id: string;
	PlaidTransactionId: string;
	UserId: string;
	PlaidAccountId: string;
	MerchantName: string;
	Name: string;
	CostAmount: number;
	Pending: boolean;
	PaymentChannel: string;
	Categories: string[];
	TransactionDate: Date;
}

export class StockExchangeTransaction {
	Id: string;
	UserId: string;
	Symbol: string;
	Type: string;
	CurrencyAmount: number;
	StockAmount: number;
	StockRate: number;
	CreatedDate: Date;
}