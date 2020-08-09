import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { StockViewRoutingModule } from './stock-view-routing.module';
import { StockViewComponent } from './stock-view.component';
import { SharedModule } from '../../../shared.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SharedModule,
    StockViewRoutingModule
  ],
  declarations: [StockViewComponent]
})
export class StockViewModule { }
