import { Routes } from '@angular/router';

export const CLIENT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./clients-shell.component').then((m) => m.ClientsShellComponent)
  }
];
