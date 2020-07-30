// Credits to https://stackoverflow.com/questions/28828915/how-set-color-family-to-pie-chart-in-chart-js
// Credits to chartjs.org
import { Component, OnInit, ViewChild } from '@angular/core';
import { Chart } from 'chart.js';
import { Globals } from '../globals';
import { HttpErrorResponse } from '@angular/common/http';
import { MonierService } from '../services/monier.service';
import { Storage } from '@ionic/storage';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  @ViewChild('netWorthInPast30DaysLineChart') netWorthInPast30DaysLineChart;
  @ViewChild('barChart') barChart;
  @ViewChild('transactionsInPast30DaysPieChart') transactionsInPast30DaysPieChart;
  @ViewChild('typeOfPaymentsInPast30DaysPie') typeOfPaymentsInPast30DaysPie;

  isLoading = true;

  constructor(public globals: Globals, private monierService: MonierService, private storage: Storage) {

  }

  ngOnInit() {
    this.monierService.getOverallStatistics().subscribe(
      (statistics: any) => {
        console.log('Statistics: ', statistics);
        this.storage.set('overall_statistics', statistics);
        this.globals.overallStatistics = statistics;
        this.createNetWorthInPast30DaysLineChart();
        this.createBarChart();
        this.createTransactionsInPast30DaysPie();
        this.createTypeOfPaymentsInPast30DaysPie();
        this.isLoading = false;
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
      }
    );
  }

  bars: any;
  colorArray: any;


  createNetWorthInPast30DaysLineChart(){
    let dataSet = [];
    let dataLabels = [];
    for(let data of this.globals.overallStatistics.NetWorthInPast30Days){
      dataSet.push(data.Value);
      dataLabels.push(data.Key);
    }
    let data = {
      datasets: [{
          label: 'US Dollars ($)',
          backgroundColor: 'rgb(38, 194, 129)',
          data: dataSet
      }],
      labels: dataLabels
    };
    var myLineChart = new Chart(this.netWorthInPast30DaysLineChart.nativeElement, {
      type: 'line',
      data: data,
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
          backgroundColor: 'rgb(38, 194, 129)',
          borderColor: 'rgb(38, 194, 129)',
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
  createTransactionsInPast30DaysPie(){
    

      let dataSet = [];
      let dataLabels = [];
      let dataBackground = [];
      for(let dataKey in this.globals.overallStatistics.TransactionsInPast30DaysPie){
        dataSet.push(this.globals.overallStatistics.TransactionsInPast30DaysPie[dataKey]);
        dataLabels.push(dataKey);
        let r = Math.floor(Math.random() * 200);
        let g = Math.floor(Math.random() * 200);
        let b = Math.floor(Math.random() * 200);
        let color = 'rgb(' + r + ', ' + g + ', ' + b + ')';
        dataBackground.push(color);
      }
      let data = {
        datasets: [{
            data: dataSet,
            backgroundColor: dataBackground
        }],
        labels: dataLabels
      };
      var myDoughnutChart = new Chart(this.transactionsInPast30DaysPieChart.nativeElement, {
        type: 'doughnut',
        data: data
      });
    }

    createTypeOfPaymentsInPast30DaysPie(){
      let dataSet = [];
      let dataLabels = [];
      let dataBackground = [];
      for(let dataKey in this.globals.overallStatistics.TypeOfPaymentsInPast30DaysPie){
        dataSet.push(this.globals.overallStatistics.TypeOfPaymentsInPast30DaysPie[dataKey]);
        dataLabels.push(dataKey);
        let r = Math.floor(Math.random() * 200);
        let g = Math.floor(Math.random() * 200);
        let b = Math.floor(Math.random() * 200);
        let color = 'rgb(' + r + ', ' + g + ', ' + b + ')';
        dataBackground.push(color);
      }
      let data = {
        datasets: [{
            data: dataSet,
            label: 'US Dollars ($)',
            backgroundColor: dataBackground
        }],
        labels: dataLabels
      };
      var myPieChart = new Chart(this.typeOfPaymentsInPast30DaysPie.nativeElement, {
        type: 'pie',
        data: data
      });
    }
}
