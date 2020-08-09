import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { BankAccountRoutingModule } from './bank-account-routing.module';
import { BankAccountComponent } from './bank-account.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    BankAccountRoutingModule
  ],
  declarations: [BankAccountComponent]
})
export class BankAccountModule { }
