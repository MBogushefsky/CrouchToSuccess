import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { InvestingComponent } from './investing/investing.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule)
  },
  {
    path: 'investing',
    component: InvestingComponent,
    children: [
      {
        path: 'dashboard',
        children: [
          {
            path: '',
            loadChildren: './investing/dashboard/dashboard.module#DashboardModule'
          }
        ]
      },
      {
        path: 'search',
        children: [
          {
            path: '',
            loadChildren: './investing/search/search.module#SearchModule'
          }
        ]
      },
      {
        path: '',
        redirectTo: 'investing/dashboard',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: 'settings',
    loadChildren: () => import('./settings/settings.module').then(m => m.SettingsModule)
  },
  {
    path: 'link-institution',
    loadChildren: () => import('./link-institution/link-institution.module').then(m => m.LinkInstitutionModule)
  },
  {
    path: 'folder/:id',
    loadChildren: () => import('./folder/folder.module').then(m => m.FolderPageModule)
  },
  {
    path: 'bank-account/:id',
    loadChildren: () => import('./bank-account/bank-account.module').then(m => m.BankAccountModule)
  },
  {
    path: 'bank-account/:id/transactions',
    loadChildren: () => import('./transaction-list/transaction-list.module').then(m => m.TransactionListModule)
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
