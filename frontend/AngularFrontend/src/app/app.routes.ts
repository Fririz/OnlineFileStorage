import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [

    {
    path: '',
    canActivate: [authGuard],
    children: [
        //TODO add
      //{ path: 'files', component: FileListComponent },
      
      { path: '', redirectTo: 'files', pathMatch: 'full' } 
    ]
  },
  { path: '**', redirectTo: 'login' }
];
