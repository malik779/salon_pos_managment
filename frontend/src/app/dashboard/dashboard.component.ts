import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { AppStore } from '../core/state/app.store';

@Component({
  standalone: true,
  selector: 'sp-dashboard',
  imports: [CommonModule, MatCardModule],
  template: `
    <div class="grid">
      <mat-card>
        <strong>Salon Overview</strong>
        <p>{{ store.branches().length }} branches live</p>
      </mat-card>
      <mat-card>
        <strong>Upcoming Bookings</strong>
        <p>Sync book across channels in real time.</p>
      </mat-card>
      <mat-card>
        <strong>POS Registers</strong>
        <p>Monitor Z-read progress and tills.</p>
      </mat-card>
    </div>
  `,
  styles: [
    `
      .grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
        gap: 1rem;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {
  constructor(public readonly store: AppStore) {}

  ngOnInit() {
    this.store.loadBranches();
  }
}
