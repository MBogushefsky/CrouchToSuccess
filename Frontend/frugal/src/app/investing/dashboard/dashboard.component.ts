import { Component, OnInit } from '@angular/core';
import { FrugalService } from '../../services/frugal.service';
import { AlertController } from '@ionic/angular';
import { HttpErrorResponse } from '@angular/common/http';
import { Globals } from '../../globals';
import { Storage } from '@ionic/storage';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  isLoading = false;
  currentBuyingPower = 0.00;
  currentPortfolio: any;

  constructor(private frugalService: FrugalService,
              private alertController: AlertController,
              public globals: Globals,
              private storage: Storage) { }

  ngOnInit() {
    this.getCurrentPortfolio();
  }

  getCurrentPortfolio(){
    this.frugalService.getStockExchangeCurrentBuyingPower().subscribe(
      (receivedBuyingPower: any) => {
        this.currentBuyingPower = receivedBuyingPower;
        this.frugalService.getStockExchangeCurrentPortfolio().subscribe(
          (receivedPortfolio: any) => {
            this.currentPortfolio = receivedPortfolio;
          },
          (error: HttpErrorResponse) => {
            console.log('Error: ', error.message);
          }
        );
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
      }
    );
  }
}
