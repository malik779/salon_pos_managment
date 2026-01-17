import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { OfflineIndicatorComponent } from './shared/components/offline-indicator.component';
import { SyncStatusService } from './core/services/sync-status.service';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'sp-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AsyncPipe, MatToolbarModule, MatButtonModule, MatIconModule, MatMenuModule, MatDividerModule, OfflineIndicatorComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  readonly syncStatus$ = this.syncStatus.status$;
  readonly isAuthenticated = computed(() => this.authService.isAuthenticated());
  readonly userName = computed(() => localStorage.getItem('sp.userName') || 'User');
  readonly userEmail = computed(() => localStorage.getItem('sp.userEmail') || '');

  readonly navLinks = [
    { path: '/', label: 'Dashboard' },
    { path: '/booking', label: 'Booking' },
    { path: '/pos', label: 'POS' },
    { path: '/clients', label: 'Clients' },
    { path: '/staff', label: 'Staff' },
    { path: '/reports', label: 'Reports' }
  ];

  constructor(private readonly syncStatus: SyncStatusService) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}
