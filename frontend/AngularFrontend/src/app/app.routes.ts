import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { ItemList } from './features/item/item-list/item-list';
import { guestGuard } from './core/guards/guest-guard';

export const routes: Routes = [
  { 
    path: '', 
    redirectTo: 'items', 
    pathMatch: 'full' 
  },

  { 
    path: 'items', 
    component: ItemList,
    canActivate: [authGuard]
  },

  { path: 'login', component: Login, canActivate: [guestGuard] }, 
  { path: 'register', component: Register, canActivate: [guestGuard] }, 

  { path: '**', redirectTo: 'login' } 
];