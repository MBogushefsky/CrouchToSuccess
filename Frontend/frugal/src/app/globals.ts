import { Injectable } from '@angular/core';
import { User, Transaction, BankAccount } from './models/models';
import { Storage } from '@ionic/storage';

@Injectable()
export class Globals {
	userToken: User;
	overallStatistics: any;
	linkedBankAccounts: BankAccount[];

	constructor(
		private storage: Storage
	) {
	}

	logOut(){
		this.storage.remove('user_token');
		this.userToken = null;
		this.overallStatistics = null;
		this.linkedBankAccounts = null;
	}

	dateFormatToDate(dateFormat: string){
		let date = new Date(dateFormat);
		let month = date.getMonth() + 1;
		if(month == 13){
			month = 1;
		}
		return month + '-' + date.getUTCDate() + '-' + date.getFullYear();
	}
}