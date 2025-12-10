import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor(private readonly snackBar: MatSnackBar) {}

  success(message: string) {
    this.snackBar.open(message, 'Close', { duration: 3000, panelClass: ['bg-green-600', 'text-white'] });
  }

  error(message: string) {
    this.snackBar.open(message, 'Dismiss', { duration: 4000, panelClass: ['bg-red-600', 'text-white'] });
  }
}
