import { Component, OnInit } from '@angular/core';
import { FrugalService } from '../../services/frugal.service';
import { AlertController } from '@ionic/angular';
import { HttpErrorResponse } from '@angular/common/http';
import { Globals } from '../../globals';
import { BankAccount } from '../../models/models';
import { Storage } from '@ionic/storage';
import { YodleeService } from '../../services/yodlee.service';
declare var window;

@Component({
  selector: 'app-link-institution',
  templateUrl: './link-institution.component.html',
  styleUrls: ['./link-institution.component.scss'],
})
export class LinkInstitutionComponent implements OnInit {
  isLoading = false;
  isLinkingBankInstitution = false;

  constructor(private frugalService: FrugalService,
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
    this.yodleeService.getUserAccessToken('46085511-1181-4225-a1f4-88312958420e_ADMIN').subscribe(
      (data: any) => {
          window.fastlink.close();
          this.isLoading = false;
          window.fastlink.open({
            fastLinkURL: 'https://development.node.yodlee.com/authenticate/USDevexPreProd2-355/?channelAppName=usdevexpreprod2',
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
