import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { addDays, addMinutes, formatISO } from 'date-fns';
import { BookingApi } from '../core/api/api-client';
import { BookingSlot } from '../core/models/domain.models';
import { IdempotencyService } from '../core/services/idempotency.service';
import { AuthService } from '@app/core/services/auth.service';

@Component({
  standalone: true,
  selector: 'sp-booking-calendar',
  imports: [CommonModule, MatButtonModule, MatCardModule],
  template: `
    <section class="calendar-header">
      <h2>Booking Calendar</h2>
      <button mat-stroked-button (click)="createMockBooking()">Quick book</button>
    </section>
    <div class="calendar-grid">
      <mat-card *ngFor="let slot of slots()">
        <strong>{{ slot.startUtc | date: 'shortTime' }} - {{ slot.endUtc | date: 'shortTime' }}</strong>
        <p>{{ slot.status }}</p>
      </mat-card>
    </div>
  `,
  styles: [
    `
      .calendar-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
        gap: 1rem;
      }
      .calendar-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 1rem;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BookingCalendarComponent implements OnInit {
  readonly slots = signal<BookingSlot[]>([]);
  readonly authService = inject(AuthService);
  constructor(private readonly bookingApi: BookingApi, private readonly idempotency: IdempotencyService) {}

  ngOnInit(): void {
    const from = formatISO(new Date());
    const to = formatISO(addDays(new Date(), 1));
    this.bookingApi.getAll().subscribe((slots) => this.slots.set(slots));
  }

  createMockBooking() {
    this.bookingApi
      .create(
        {
          branchId: '00000000-0000-0000-0000-000000000001',
          clientId: '00000000-0000-0000-0000-000000000002',
          staffId: '00000000-0000-0000-0000-000000000003',
          startUtc: new Date().toISOString(),
          endUtc: addMinutes(new Date(), 60).toISOString()
        },
        // this.idempotency.generate()
      )
      .subscribe();
  }
}
