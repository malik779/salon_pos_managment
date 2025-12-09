import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

export const appRoutes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () => import('./dashboard/dashboard.component').then((m) => m.DashboardComponent)
      },
      {
        path: 'branches',
        loadChildren: () => import('./branches/branches.routes').then((m) => m.BRANCH_ROUTES)
      },
      {
        path: 'catalog',
        loadChildren: () => import('./catalog/catalog.routes').then((m) => m.CATALOG_ROUTES)
      },
      {
        path: 'booking',
        loadChildren: () => import('./booking/booking.routes').then((m) => m.BOOKING_ROUTES)
      },
      {
        path: 'pos',
        loadChildren: () => import('./pos/pos.routes').then((m) => m.POS_ROUTES)
      },
      {
        path: 'clients',
        loadChildren: () => import('./clients/clients.routes').then((m) => m.CLIENT_ROUTES)
      },
      {
        path: 'staff',
        loadChildren: () => import('./staff/staff.routes').then((m) => m.STAFF_ROUTES)
      },
      {
        path: 'reports',
        loadChildren: () => import('./reports/reports.routes').then((m) => m.REPORT_ROUTES)
      },
      {
        path: 'settings',
        loadChildren: () => import('./settings/settings.routes').then((m) => m.SETTINGS_ROUTES)
      }
    ]
  },
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.routes').then((m) => m.AUTH_ROUTES)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
