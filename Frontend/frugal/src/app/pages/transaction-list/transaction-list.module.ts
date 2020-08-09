import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { TransactionListRoutingModule } from './transaction-list-routing.module';
import { TransactionListComponent } from './transaction-list.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    TransactionListRoutingModule
  ],
  declarations: [TransactionListComponent]
})
export class TransactionListModule { }
