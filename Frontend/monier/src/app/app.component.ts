import { Component, OnInit } from '@angular/core';

import { Platform, AlertController } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { Storage } from '@ionic/storage';
import { MonierService } from './services/monier.service';
import { HttpErrorResponse } from '@angular/common/http';
import { User, Transaction, BankAccount } from './models/models';
import { Globals } from './globals';
declare var Plaid;

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss']
})
export class AppComponent implements OnInit {
  private usernameInput = '';
  private passwordInput = '';
  private confirmPasswordInput = '';
  private isLoading: boolean;
  private signUpUser: User = {
    Id: null,
    Username: null,
    PasswordHash: null,
    Email: null,
    FirstName: null,
    LastName: null,
    PhoneNumber: null,
    Admin: false
  };
  public inSignUp = false;
  public selectedPage: string;
  public appPages = [
    {
      title: 'Home',
      url: '/home',
      icon: 'home'
    },
    {
      title: 'Settings',
      url: '/settings',
      icon: 'hammer'
    }
  ];
  public labels = ['Family', 'Friends', 'Notes', 'Work', 'Travel', 'Reminders'];

  constructor(
    private platform: Platform,
    private splashScreen: SplashScreen,
    private statusBar: StatusBar,
    private monierService: MonierService,
    private storage: Storage,
    private alertController: AlertController,
    private globals: Globals
  ) {
    this.initializeApp();
  }

  initializeApp() {
    this.platform.ready().then(() => {
      this.statusBar.styleDefault();
      this.splashScreen.hide();
    });
  }

  ngOnInit() {
    this.isLoading = true;
    this.initializeLoggedInState();
  }

  initializeLoggedInState(){
    const path = window.location.pathname.split('/')[1];
    if (path !== undefined) {
      this.selectedPage = '/' + path;
      //this.selectedIndex = this.appPages.findIndex(page => page.title.toLowerCase() === path.toLowerCase());
    }
    this.storage.get('user_token').then((val) => {
      console.log('Your user token is', val);
      this.globals.userToken = val;
      if (this.globals.userToken != null) {
        this.monierService.getAllBankAccounts().subscribe(
          (bankAccounts: BankAccount[]) => {
            this.globals.linkedBankAccounts = bankAccounts;
            this.isLoading = false;
          },
          (error: HttpErrorResponse) => {
            console.log('Error: ', error.message);
          }
        );
      }
      else {
        this.isLoading = false;
      }
    });
  }

  login() {
    this.isLoading = true;
    console.log("Login");
    this.monierService.getUserByCredentials(this.usernameInput, this.passwordInput).subscribe(
      (data: User) => {
        console.log("USER ID: ", data);
        this.storage.set('user_token', data).then((val) => {
          this.globals.userToken = data;
          this.initializeLoggedInState();
        });
        
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
        this.loginFailed();
        this.isLoading = false;
      }
    );
  }

  linkBankAccount() {
    var linkBankAccountFunction = (publicToken) => {
      this.monierService.linkBankAccountToUser(publicToken).subscribe(
          (data: any) => {
            
          },
          (error: HttpErrorResponse) => {
            console.log('Error: ', error.message);
          }
        );
    }
    var handler = Plaid.create({
      clientName: 'Monier',
      env: 'development',
      key: 'ce84441114a95a4795f66b9bddb36f',
      product: ['auth'],
      onLoad: function () {
        console.log("Plaid Load");
      },
      onSuccess: function (public_token, metadata) {
        console.log("Plaid Success: ", public_token);
        linkBankAccountFunction(public_token);
      },
      onExit: function (err, metadata) {
        if (err != null) {

        }
      }
    });
    handler.open();
  }

  logout() {
    this.globals.logOut();
  }

  signUp() {
    this.isLoading = true;
    console.log("Sign Up");
    if (this.passwordInput !== this.confirmPasswordInput) {
      this.signUpFailed('Passwords don\'t match');
      this.isLoading = false;
      return;
    }
    this.signUpUser.Username = this.usernameInput;
    this.signUpUser.PasswordHash = btoa(this.confirmPasswordInput);
    this.monierService.createUser(this.signUpUser).subscribe(
      (data: any) => {
        this.inSignUp = false;
        this.signUpSucceed();
        this.isLoading = false;
      },
      (error: HttpErrorResponse) => {
        console.log('Error: ', error.message);
        this.signUpFailed(error.message);
        this.isLoading = false;
      }
    );
  }

  async loginFailed() {
    const alert = await this.alertController.create({
      header: 'Login Failed',
      message: 'Username or password was incorrect',
      buttons: ['OK']
    });

    await alert.present();
  }

  async signUpSucceed() {
    const alert = await this.alertController.create({
      header: 'Sign Up Success',
      buttons: ['OK']
    });

    await alert.present();
  }

  async signUpFailed(error: string) {
    const alert = await this.alertController.create({
      header: 'Sign Up Failed',
      message: error,
      buttons: ['OK']
    });

    await alert.present();
  }
}
