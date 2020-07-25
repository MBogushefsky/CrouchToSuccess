import { Component, OnInit } from '@angular/core';
import { MonierService } from '../services/monier.service';
import { AlertController } from '@ionic/angular';
import { HttpErrorResponse } from '@angular/common/http';
import { Globals } from '../globals';
import { BankAccount } from '../models/models';
import { Storage } from '@ionic/Storage';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
})
export class SettingsComponent implements OnInit {
  isLoading: boolean;

  constructor(private monierService: MonierService,
              private alertController: AlertController,
              private globals: Globals,
              private storage: Storage) { }

  ngOnInit() {}

  syncAllBankAccounts(){
    this.isLoading = true;
    this.monierService.syncAllBankAccounts().subscribe(
      (data: any) => {
        this.syncSucceeded();
        this.monierService.getAllBankAccounts().subscribe(
          (bankAccounts: BankAccount[]) => {
            this.globals.linkedBankAccounts = bankAccounts;
            this.isLoading = false;
          },
          (error: HttpErrorResponse) => {
            console.log('Error: ', error.message);
          }
        );
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
        this.syncFailed(error.message);
        this.isLoading = false;
      }
    );
  }

  async syncSucceeded(){
    const alert = await this.alertController.create({
      header: 'Sync Succeeded',
      buttons: ['OK']
    });

    await alert.present();
  }

  async syncFailed(error: string){
    const alert = await this.alertController.create({
      header: 'Sync Failed',
      message: error,
      buttons: ['OK']
    });

    await alert.present();
  }
}
