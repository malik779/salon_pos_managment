import { Routes } from '@angular/router';

export const STAFF_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./staff-shell.component').then((m) => m.StaffShellComponent)
  }
];
