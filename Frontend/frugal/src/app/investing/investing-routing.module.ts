import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { InvestingComponent } from './investing.component';

const routes: Routes = [
  {
    path: '',
    component: InvestingComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class InvestingRoutingModule {}
