import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { StockViewComponent } from './stock-view.component';

const routes: Routes = [
  {
    path: '',
    component: StockViewComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StockViewRoutingModule {}
