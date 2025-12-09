import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AsyncPipe, NgFor } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { OfflineIndicatorComponent } from './shared/components/offline-indicator.component';
import { SyncStatusService } from './core/services/sync-status.service';

@Component({
  selector: 'sp-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NgFor, AsyncPipe, MatToolbarModule, MatButtonModule, MatIconModule, OfflineIndicatorComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  readonly syncStatus$ = this.syncStatus.status$;

  readonly navLinks = [
    { path: '/', label: 'Dashboard' },
    { path: '/booking', label: 'Booking' },
    { path: '/pos', label: 'POS' },
    { path: '/clients', label: 'Clients' },
    { path: '/staff', label: 'Staff' },
    { path: '/reports', label: 'Reports' }
  ];

  constructor(private readonly syncStatus: SyncStatusService) {}
}
