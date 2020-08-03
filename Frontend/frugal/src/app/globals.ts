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
		let date = new Date(Date.parse(dateFormat));
		let month = date.getMonth() + 1;
		return month + '-' + date.getDate() + '-' + date.getFullYear();
	}

	dateFormatToDateTime(dateFormat: string){
		return this.dateFormatToDate(dateFormat) + ' ' + this.dateFormatToTimeOfDay(dateFormat);
	}
	
	dateFormatToTimeOfDay(dateFormat: string){
		let date = new Date(Date.parse(dateFormat));
		let hourAmount = date.getHours();
		let minuteAmount = date.getMinutes();
		let amPm = '';
		if(hourAmount > 12){
			hourAmount -= 12;
			amPm = 'PM';
		}
		else{
			amPm = 'AM';
		}
		return (hourAmount > 9 ? hourAmount : '0' + hourAmount) + ':' + (minuteAmount > 9 ? minuteAmount : '0' + minuteAmount) + ' ' + amPm;
	}
}