import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { MainMenuComponent } from './main-menu.component';
import { SharedModule } from '../shared.module';
import { AppRoutingModule } from '../app-routing.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SharedModule,
    AppRoutingModule
  ],
  exports: [
    MainMenuComponent
  ],
  declarations: [MainMenuComponent]
})
export class MainMenuModule { }
