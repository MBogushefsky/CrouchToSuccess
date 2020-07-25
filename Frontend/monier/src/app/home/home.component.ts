import { Component, OnInit, ViewChild } from '@angular/core';
import { Chart } from 'chart.js';
import { Globals } from '../globals';
import { HttpErrorResponse } from '@angular/common/http';
import { MonierService } from '../services/monier.service';
import { Storage } from '@ionic/Storage';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  @ViewChild('barChart') barChart;

  constructor(private globals: Globals, private monierService: MonierService, private storage: Storage) {

  }

  ngOnInit() {
    this.monierService.getOverallStatistics().subscribe(
      (statistics: any) => {
        console.log('Statistics: ', statistics);
        this.storage.set('overall_statistics', statistics);
        this.globals.overallStatistics = statistics;
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
      }
    );
  }

  bars: any;
  colorArray: any;

  ionViewDidEnter() {
    this.createBarChart();
  }

  createBarChart() {
    let barChartDates = [];
    let barChartData = [];
    for (let dailyCost of this.globals.overallStatistics.DailyCostInPast30Days){
      barChartDates.push(dailyCost.Key);
      barChartData.push(dailyCost.Value);
    }
    this.bars = new Chart(this.barChart.nativeElement, {
      type: 'bar',
      data: {
        labels: barChartDates,
        datasets: [{
          label: 'US Dollars ($)',
          data: barChartData,
          backgroundColor: 'rgb(38, 194, 129)', // array should have same number of elements as number of dataset
          borderColor: 'rgb(38, 194, 129)',// array should have same number of elements as number of dataset
          borderWidth: 1
        }]
      },
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }
    });
  }

}
