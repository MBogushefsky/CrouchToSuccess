import { Component, OnInit } from '@angular/core';
import { BankAccount, Transaction } from '../models/models';
import { Globals } from '../globals';
import { FrugalService } from '../services/frugal.service';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-transaction-list',
  templateUrl: './transaction-list.component.html',
  styleUrls: ['./transaction-list.component.scss'],
})
export class TransactionListComponent implements OnInit {
  isLoading: boolean;
  bankAccountId: string;
  currentBankAccount: BankAccount;
  transactionsToShow: Transaction[];

  constructor(private globals: Globals, private frugalService: FrugalService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.isLoading = true;
    this.route.params.subscribe(params => {
      this.bankAccountId = params['id'];
      this.frugalService.getBankAccountById(this.bankAccountId).subscribe(
        (bankAccount: BankAccount) => {
          this.currentBankAccount = bankAccount;
          this.frugalService.getTransactionsByPlaidBankAccountId(this.currentBankAccount.PlaidAccountId).subscribe(
            (transactions: Transaction[]) => {
              this.transactionsToShow = transactions;
              this.isLoading = false;
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
    });
  }

}
