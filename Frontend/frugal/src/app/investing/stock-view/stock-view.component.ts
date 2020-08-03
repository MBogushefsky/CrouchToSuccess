import { Component, OnInit, ViewChild, Input, ElementRef } from '@angular/core';
import { FrugalService } from '../../services/frugal.service';
import { AlertController } from '@ionic/angular';
import { HttpErrorResponse } from '@angular/common/http';
import { Globals } from '../../globals';
import { Storage } from '@ionic/storage';
import { Chart } from 'chart.js';

@Component({
  selector: 'app-stock-view',
  templateUrl: './stock-view.component.html',
  styleUrls: ['./stock-view.component.scss'],
})
export class StockViewComponent implements OnInit {
  @Input() symbol: string;
  @ViewChild('symbolLineChart', {static: false}) symbolLineChart;
  isLoading = false;
  symbolData: any;
  selectedDuration = 'Day';
  stockToBuyInput: number;
  stockToSellInput: number;

  constructor(private frugalService: FrugalService,
              private alertController: AlertController,
              public globals: Globals,
              private storage: Storage) { }

  ngOnInit() {
    if (this.symbol != null) {
      this.searchForSymbol();
    }
  }

  buyStock(){

  }

  sellStock(){

  }

  searchForSymbol(){
    this.isLoading = true;
    this.frugalService.getStockExchangeSymbolData(this.symbol).subscribe(
      (symbolData: any) => {
        console.debug(symbolData);
        this.symbolData = symbolData;
        this.isLoading = false;
        this.createDayDataOfSymbolLineChart();
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
      }
    );
  }

  createDayDataOfSymbolLineChart(){
    this.selectedDuration = 'Day';
    let dataLabels = [];
    let dataSet = [];
    for(let data of this.symbolData.Day){
      dataLabels.push(this.globals.dateFormatToDateTime(data.date));
      dataSet.push(data.close);
    }
    dataLabels.reverse();
    dataSet.reverse();
    let data = {
      datasets: [{
          label: 'US Dollars ($)',
          borderColor: 'rgb(38, 194, 129)',
          fill: false,
          data: dataSet
      }],
      labels: dataLabels
    };
    var myLineChart = new Chart(this.symbolLineChart.nativeElement, {
      type: 'line',
      data: data,
      options: null
    });
  }

  createWeekDataOfSymbolLineChart(){
    this.selectedDuration = 'Week';
    let dataLabels = [];
    let dataSet = [];
    for(let dataKey in this.symbolData.Week){
      dataLabels.push(this.globals.dateFormatToDateTime(dataKey));
      dataSet.push(this.symbolData.Week[dataKey].close);
    }
    dataLabels.reverse();
    dataSet.reverse();
    let data = {
      datasets: [{
          label: 'US Dollars ($)',
          borderColor: 'rgb(38, 194, 129)',
          fill: false,
          data: dataSet
      }],
      labels: dataLabels
    };
    var myLineChart = new Chart(this.symbolLineChart.nativeElement, {
      type: 'line',
      data: data,
      options: null
    });
  }

  createMonthDataOfSymbolLineChart(){
    this.selectedDuration = 'Month';
    let dataLabels = [];
    let dataSet = [];
    for(let dataKey in this.symbolData.Month){
      dataLabels.push(this.globals.dateFormatToDateTime(dataKey));
      dataSet.push(this.symbolData.Month[dataKey].close);
    }
    dataLabels.reverse();
    dataSet.reverse();
    let data = {
      datasets: [{
          label: 'US Dollars ($)',
          borderColor: 'rgb(38, 194, 129)',
          fill: false,
          data: dataSet
      }],
      labels: dataLabels
    };
    var myLineChart = new Chart(this.symbolLineChart.nativeElement, {
      type: 'line',
      data: data,
      options: null
    });
  }
}
