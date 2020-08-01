import { Component, OnInit } from '@angular/core';
import { FrugalService } from '../services/frugal.service';
import { AlertController } from '@ionic/angular';
import { HttpErrorResponse } from '@angular/common/http';
import { Globals } from '../globals';
import { BankAccount } from '../models/models';
import { Storage } from '@ionic/storage';
import { YodleeService } from '../services/yodlee.service';

@Component({
  selector: 'app-investing',
  templateUrl: './investing.component.html',
  styleUrls: ['./investing.component.scss'],
})
export class InvestingComponent implements OnInit {
  isLoading = false;

  constructor(private frugalService: FrugalService,
              private alertController: AlertController,
              public globals: Globals,
              private storage: Storage,
              private yodleeService: YodleeService) { }

  ngOnInit() {}

  
}
