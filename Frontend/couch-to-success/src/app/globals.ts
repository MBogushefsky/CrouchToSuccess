import { Injectable } from '@angular/core';
import { User, Transaction, BankAccount } from './models/models';
import { Storage } from '@ionic/storage';
import { AlertController } from '@ionic/angular';

@Injectable()
export class Globals {
	userToken: User;
	overallStatistics: any;
	linkedBankAccounts: BankAccount[];
	stockExchangeData: any;

	constructor(
		private storage: Storage,
		private alertController: AlertController
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

	async succeedMessage(message: string){
		const alert = await this.alertController.create({
			header: message,
			buttons: ['OK']
		});
		await alert.present();
	}

	async errorMessage(error: any){
		const alert = await this.alertController.create({
			header: 'Error',
			message: error.error,
			buttons: ['OK']
		});
		await alert.present();
	}
}