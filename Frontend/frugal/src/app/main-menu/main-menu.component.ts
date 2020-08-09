import { Component, OnInit } from '@angular/core';
import { Globals } from '../globals';

@Component({
  selector: 'app-main-menu',
  templateUrl: './main-menu.component.html',
  styleUrls: ['./main-menu.component.scss'],
})
export class MainMenuComponent implements OnInit {
  public selectedPage: string;
  public appPages = [
    {
      title: 'Home',
      url: '/home',
      icon: 'home'
    },
    {
      title: 'Investing',
      url: '/investing/dashboard',
      icon: 'bar-chart-outline'
    },
    {
      title: 'Settings',
      url: '/settings',
      icon: 'hammer'
    }/*,
    {
      title: 'Link Bank Institution',
      url: '/link-institution',
      icon: 'link'
    }*/
  ];

  constructor(
    public globals: Globals
  ) {
  }

  ngOnInit() {
    const path = window.location.pathname.split('/')[1];
    if (path !== undefined) {
      this.selectedPage = '/' + path;
      //this.selectedIndex = this.appPages.findIndex(page => page.title.toLowerCase() === path.toLowerCase());
    }
  }

  logout() {
    this.globals.logOut();
  }
}
