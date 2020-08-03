import { Component, OnInit, ViewChild, EventEmitter, Output } from '@angular/core';
import { FrugalService } from '../../services/frugal.service';
import { AlertController } from '@ionic/angular';
import { HttpErrorResponse } from '@angular/common/http';
import { Globals } from '../../globals';
import { Storage } from '@ionic/storage';
import { Chart } from 'chart.js';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent implements OnInit {
  @ViewChild('eodOfSymbolLineChart') eodOfSymbolLineChart;
  isLoading = false;
  searchInput = '';
  searchedSymbol: string;

  constructor(private frugalService: FrugalService,
              private alertController: AlertController,
              public globals: Globals,
              private storage: Storage,
              private route: ActivatedRoute,
              private router: Router) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.searchInput = params['search'];
      this.searchedSymbol = params['search'];
    });
  }

  search(){
    /*this.frugalService.getStockExchangeEODDataBySymbol(this.searchInput).subscribe(
      (eodData: any) => {
        this.createEODOfSymbolLineChart(eodData);
        this.isLoading = false;
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
      }
    );*/
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        search: this.searchInput
      },
      queryParamsHandling: 'merge'
    });
    this.searchedSymbol = this.searchInput;
  }

  /*createEODOfSymbolLineChart(eodData: any){
    let dataLabels = [];
    let dataSet = [];
    for(let dataKey in eodData){
      dataLabels.push(this.globals.dateFormatToDate(dataKey));
      dataSet.push(eodData[dataKey]);
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
    var myLineChart = new Chart(this.eodOfSymbolLineChart.nativeElement, {
      type: 'line',
      data: data,
      options: null
    });
  }*/
}
