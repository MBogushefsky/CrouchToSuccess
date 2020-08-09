import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { InvestingComponent } from './pages/investing/investing.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadChildren: () => import('./pages/home/home.module').then(m => m.HomeModule)
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
            loadChildren: './pages/investing/dashboard/dashboard.module#DashboardModule'
          }
        ]
      },
      {
        path: 'search',
        children: [
          {
            path: '',
            loadChildren: './pages/investing/search/search.module#SearchModule'
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
    loadChildren: () => import('./pages/settings/settings.module').then(m => m.SettingsModule)
  },
  {
    path: 'link-institution',
    loadChildren: () => import('./pages/link-institution/link-institution.module').then(m => m.LinkInstitutionModule)
  },
  {
    path: 'bank-account/:id',
    loadChildren: () => import('./pages/bank-account/bank-account.module').then(m => m.BankAccountModule)
  },
  {
    path: 'bank-account/:id/transactions',
    loadChildren: () => import('./pages/transaction-list/transaction-list.module').then(m => m.TransactionListModule)
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
