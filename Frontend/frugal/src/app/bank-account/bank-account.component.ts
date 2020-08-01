import { Component, OnInit, ViewChild } from '@angular/core';
import { Chart } from 'chart.js';
import { Globals } from '../globals';
import { FrugalService } from '../services/frugal.service';
import { BankAccount } from '../models/models';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-bank-account',
  templateUrl: './bank-account.component.html',
  styleUrls: ['./bank-account.component.scss'],
})
export class BankAccountComponent implements OnInit {
  isLoading: boolean;
  bankAccountId: string;
  currentBankAccount: BankAccount;

  constructor(private globals: Globals, private frugalService: FrugalService, private route: ActivatedRoute) {

  }

  ngOnInit() {
    this.isLoading = true;
    this.route.params.subscribe(params => {
      this.bankAccountId = params['id'];
      this.frugalService.getBankAccountById(this.bankAccountId).subscribe(
        (bankAccount: BankAccount) => {
          this.currentBankAccount = bankAccount;
          this.isLoading = false;
        },
        (error: HttpErrorResponse) => {
          console.log('Error: ', error.message);
        }
      );
    });
  }
}
