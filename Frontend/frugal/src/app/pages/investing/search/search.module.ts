import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { SearchRoutingModule } from './search-routing.module';
import { SearchComponent } from './search.component';
import { SharedModule } from '../../../shared.module';
import { StockViewComponent } from '../stock-view/stock-view.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SharedModule,
    SearchRoutingModule
  ],
  declarations: [SearchComponent, StockViewComponent]
})
export class SearchModule { }
