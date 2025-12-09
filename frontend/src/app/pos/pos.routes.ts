import { Routes } from '@angular/router';

export const POS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pos-shell.component').then((m) => m.PosShellComponent)
  }
];
