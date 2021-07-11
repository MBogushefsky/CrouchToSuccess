import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { InvestingRoutingModule } from './investing-routing.module';
import { InvestingComponent } from './investing.component';
import { SharedModule } from '../../shared.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SharedModule,
    InvestingRoutingModule
  ],
  declarations: [InvestingComponent]
})
export class InvestingModule { }
