import { Routes } from '@angular/router';

export const BOOKING_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./booking-calendar.component').then((m) => m.BookingCalendarComponent)
  }
];
