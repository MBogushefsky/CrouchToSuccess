import { Injectable } from '@angular/core';
import { User, Transaction, BankAccount } from './models/models';

@Injectable()
export class Globals {
	userToken: User;
	overallStatistics: any;
	linkedBankAccounts: BankAccount[];
}