import { Component, OnInit } from '@angular/core';
import { MonierService } from '../services/monier.service';
import { AlertController } from '@ionic/angular';
import { HttpErrorResponse } from '@angular/common/http';
import { Globals } from '../globals';
import { BankAccount } from '../models/models';
import { Storage } from '@ionic/storage';
import { YodleeService } from '../services/yodlee.service';
declare var window;

@Component({
  selector: 'app-link-institution',
  templateUrl: './link-institution.component.html',
  styleUrls: ['./link-institution.component.scss'],
})
export class LinkInstitutionComponent implements OnInit {
  isLoading = false;
  isLinkingBankInstitution = false;

  constructor(private monierService: MonierService,
              private alertController: AlertController,
              public globals: Globals,
              private storage: Storage,
              private yodleeService: YodleeService) { }

  ngOnInit() {}

  linkBankInstitution(){
    this.isLoading = true;
    this.isLinkingBankInstitution = true;
    var isLinkingBankInstitutionFalseFunction = () => {
      this.isLinkingBankInstitution = false;
    };
    this.yodleeService.getUserAccessToken('sbMem5f1f1767457d71').subscribe(
      (data: any) => {
          window.fastlink.close();
          this.isLoading = false;
          window.fastlink.open({
            fastLinkURL: 'https://node.sandbox.yodlee.com/authenticate/restserver',
            accessToken: 'Bearer ' + data.token.accessToken,
            params: {
              userExperienceFlow : 'Aggregation plus Verification'
            },
            onSuccess: function (data) {
              console.log('SUCCESS', data);
            },
            onError: function (data) {
              console.log('ERROR', data);
              isLinkingBankInstitutionFalseFunction();
            },
            onExit: function (data) {
              console.log('EXIT', data);
              isLinkingBankInstitutionFalseFunction();
            },
            onEvent: function (data) {
              console.log('EVENT', data);
            }
          },
          'container-fastlink');
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
        isLinkingBankInstitutionFalseFunction();
      }
    );
  }
}
