import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { LinkInstitutionRoutingModule } from './link-institution-routing.module';
import { LinkInstitutionComponent } from './link-institution.component';
import { SharedModule } from '../../shared.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SharedModule,
    LinkInstitutionRoutingModule
  ],
  declarations: [LinkInstitutionComponent]
})
export class LinkInstitutionModule { }
