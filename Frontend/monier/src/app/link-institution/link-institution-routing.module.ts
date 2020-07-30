import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LinkInstitutionComponent } from './link-institution.component';

const routes: Routes = [
  {
    path: '',
    component: LinkInstitutionComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LinkInstitutionRoutingModule {}
