import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { RouterLink } from '@angular/router';
import { BranchStore } from '@app/branches/store/branch.store';
import { BookingApi, ClientApi, PosApi, StaffApi } from '@app/core/api/api-client';
import { BookingSlot, Client, Invoice, Staff } from '@app/core/models/domain.models';
import { catchError, forkJoin, of } from 'rxjs';

@Component({
  standalone: true,
  selector: 'sp-dashboard',
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatChipsModule,
    RouterLink
  ],
  template: `
    <div class="dashboard-container">
      <div class="dashboard-header animate-fade-in-down">
        <div>
          <h1 class="page-title">Dashboard</h1>
          <p class="page-subtitle">Welcome back! Here's what's happening at your salon.</p>
        </div>
        <div class="header-actions">
          <button mat-raised-button color="primary" routerLink="/booking">
            <mat-icon>add</mat-icon>
            New Booking
          </button>
        </div>
      </div>

      <!-- Stats Grid -->
      <div class="stats-grid">
        <mat-card class="stat-card" [style.animation-delay]="'0.1s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);">
            <mat-icon>store</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">Branches</h3>
            @if(loading()) {
              <mat-progress-spinner mode="indeterminate" diameter="20"></mat-progress-spinner>
            } @else {
              <p class="stat-value">{{ branches().length }}</p>
            }
            <span class="stat-description">Active locations</span>
          </div>
        </mat-card>

        <mat-card class="stat-card" [style.animation-delay]="'0.2s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #d946ef 0%, #f472b6 100%);">
            <mat-icon>event</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">Today's Bookings</h3>
            @if(dataLoading()) {
              <mat-progress-spinner mode="indeterminate" diameter="20"></mat-progress-spinner>
            } @else {
              <p class="stat-value">{{ todayBookings().length }}</p>
            }
            <span class="stat-description">Appointments scheduled</span>
          </div>
        </mat-card>

        <mat-card class="stat-card" [style.animation-delay]="'0.3s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #10b981 0%, #34d399 100%);">
            <mat-icon>attach_money</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">Today's Revenue</h3>
            @if(dataLoading()) {
              <mat-progress-spinner mode="indeterminate" diameter="20"></mat-progress-spinner>
            } @else {
              <p class="stat-value">\${{ todayRevenue() | number:'1.2-2' }}</p>
            }
            <span class="stat-description">Total sales today</span>
          </div>
        </mat-card>

        <mat-card class="stat-card" [style.animation-delay]="'0.4s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);">
            <mat-icon>people</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">Staff Members</h3>
            @if(dataLoading()) {
              <mat-progress-spinner mode="indeterminate" diameter="20"></mat-progress-spinner>
            } @else {
              <p class="stat-value">{{ staff().length }}</p>
            }
            <span class="stat-description">Total staff</span>
          </div>
        </mat-card>

        <mat-card class="stat-card" [style.animation-delay]="'0.5s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #3b82f6 0%, #60a5fa 100%);">
            <mat-icon>person</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">Total Clients</h3>
            @if(dataLoading()) {
              <mat-progress-spinner mode="indeterminate" diameter="20"></mat-progress-spinner>
            } @else {
              <p class="stat-value">{{ clients().length }}</p>
            }
            <span class="stat-description">Registered clients</span>
          </div>
        </mat-card>

        <mat-card class="stat-card" [style.animation-delay]="'0.6s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #ef4444 0%, #f87171 100%);">
            <mat-icon>receipt</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">Total Invoices</h3>
            @if(dataLoading()) {
              <mat-progress-spinner mode="indeterminate" diameter="20"></mat-progress-spinner>
            } @else {
              <p class="stat-value">{{ invoices().length }}</p>
            }
            <span class="stat-description">All time invoices</span>
          </div>
        </mat-card>
      </div>

      <!-- Main Content Grid -->
      <div class="content-grid">
        <!-- Recent Bookings -->
        <mat-card class="content-card" [style.animation-delay]="'0.7s'">
          <div class="card-header">
            <div>
              <h2 class="card-title">
                <mat-icon>event</mat-icon>
                Recent Bookings
              </h2>
              <p class="card-subtitle">Upcoming appointments</p>
            </div>
            <button mat-button routerLink="/booking">View All</button>
          </div>
          @if(dataLoading()) {
            <div class="loading-container">
              <mat-progress-spinner mode="indeterminate"></mat-progress-spinner>
            </div>
          } @else if (recentBookings().length === 0) {
            <div class="empty-state">
              <mat-icon>event_busy</mat-icon>
              <p>No upcoming bookings</p>
            </div>
          } @else {
            <div class="list-container">
              @for (booking of recentBookings(); track booking.id) {
                <div class="list-item">
                  <div class="list-item-content">
                    <div class="list-item-primary">
                      <span class="list-item-title">Booking #{{ booking.id.substring(0, 8) }}</span>
                      <span class="list-item-subtitle">{{ formatDate(booking.startUtc) }}</span>
                    </div>
                    <mat-chip [class]="'status-' + booking.status.toLowerCase()">
                      {{ booking.status }}
                    </mat-chip>
                  </div>
                </div>
              }
            </div>
          }
        </mat-card>

        <!-- Recent Invoices -->
        <mat-card class="content-card" [style.animation-delay]="'0.8s'">
          <div class="card-header">
            <div>
              <h2 class="card-title">
                <mat-icon>receipt</mat-icon>
                Recent Sales
              </h2>
              <p class="card-subtitle">Latest transactions</p>
            </div>
            <button mat-button routerLink="/pos">View All</button>
          </div>
          @if(dataLoading()) {
            <div class="loading-container">
              <mat-progress-spinner mode="indeterminate"></mat-progress-spinner>
            </div>
          } @else if (recentInvoices().length === 0) {
            <div class="empty-state">
              <mat-icon>receipt_long</mat-icon>
              <p>No recent sales</p>
            </div>
          } @else {
            <div class="list-container">
              @for (invoice of recentInvoices(); track invoice.id) {
                <div class="list-item">
                  <div class="list-item-content">
                    <div class="list-item-primary">
                      <span class="list-item-title">Invoice #{{ invoice.id.substring(0, 8) }}</span>
                      <span class="list-item-subtitle">\${{ invoice.total | number:'1.2-2' }}</span>
                    </div>
                    <mat-chip [class]="'status-' + invoice.status.toLowerCase()">
                      {{ invoice.status }}
                    </mat-chip>
                  </div>
                </div>
              }
            </div>
          }
        </mat-card>

        <!-- Staff Overview -->
        <mat-card class="content-card" [style.animation-delay]="'0.9s'">
          <div class="card-header">
            <div>
              <h2 class="card-title">
                <mat-icon>people</mat-icon>
                Staff Overview
              </h2>
              <p class="card-subtitle">Team members</p>
            </div>
            <button mat-button routerLink="/staff">View All</button>
          </div>
          @if(dataLoading()) {
            <div class="loading-container">
              <mat-progress-spinner mode="indeterminate"></mat-progress-spinner>
            </div>
          } @else if (staff().length === 0) {
            <div class="empty-state">
              <mat-icon>person_off</mat-icon>
              <p>No staff members</p>
            </div>
          } @else {
            <div class="list-container">
              @for (member of staff().slice(0, 5); track member.id) {
                <div class="list-item">
                  <div class="list-item-content">
                    <div class="list-item-primary">
                      <span class="list-item-title">{{ member.fullName }}</span>
                      <span class="list-item-subtitle">{{ member.role }}</span>
                    </div>
                    <mat-chip>{{ member.role }}</mat-chip>
                  </div>
                </div>
              }
            </div>
          }
        </mat-card>

        <!-- Quick Actions -->
        <mat-card class="content-card quick-actions" [style.animation-delay]="'1.0s'">
          <div class="card-header">
            <div>
              <h2 class="card-title">
                <mat-icon>flash_on</mat-icon>
                Quick Actions
              </h2>
              <p class="card-subtitle">Common tasks</p>
            </div>
          </div>
          <div class="actions-grid">
            <button mat-raised-button color="primary" routerLink="/booking" class="action-button">
              <mat-icon>event</mat-icon>
              <span>New Booking</span>
            </button>
            <button mat-raised-button color="accent" routerLink="/pos" class="action-button">
              <mat-icon>point_of_sale</mat-icon>
              <span>POS Sale</span>
            </button>
            <button mat-raised-button routerLink="/clients" class="action-button">
              <mat-icon>person_add</mat-icon>
              <span>Add Client</span>
            </button>
            <button mat-raised-button routerLink="/reports" class="action-button">
              <mat-icon>assessment</mat-icon>
              <span>View Reports</span>
            </button>
          </div>
        </mat-card>
      </div>
    </div>
  `,
  styles: [
    `
      .dashboard-container {
        max-width: 1400px;
        margin: 0 auto;
        animation: fadeIn 0.5s ease-in-out;
      }

      .dashboard-header {
        margin-bottom: 2rem;
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        gap: 1rem;
      }

      @media (max-width: 768px) {
        .dashboard-header {
          flex-direction: column;
        }
      }

      .header-actions {
        display: flex;
        gap: 0.5rem;
      }

      .page-title {
        font-size: 2.5rem;
        font-weight: 800;
        color: #0f172a;
        margin: 0 0 0.5rem 0;
        background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
      }

      @media (max-width: 768px) {
        .page-title {
          font-size: 2rem;
        }
      }

      .page-subtitle {
        font-size: 1.125rem;
        color: #64748b;
        margin: 0;
      }

      .stats-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 1.5rem;
        margin-bottom: 2rem;
      }

      @media (max-width: 640px) {
        .stats-grid {
          grid-template-columns: 1fr;
        }
      }

      .stat-card {
        padding: 1.5rem;
        display: flex;
        align-items: center;
        gap: 1.25rem;
        transition: all 0.3s ease;
        cursor: pointer;
        border-left: 4px solid transparent;
      }

      .stat-card:hover {
        transform: translateY(-4px);
        border-left-color: #6366f1;
        box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04) !important;
      }

      .stat-icon-wrapper {
        width: 64px;
        height: 64px;
        border-radius: 16px;
        display: flex;
        align-items: center;
        justify-content: center;
        color: white;
        flex-shrink: 0;
        box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
      }

      .stat-icon-wrapper mat-icon {
        font-size: 32px;
        width: 32px;
        height: 32px;
      }

      .stat-content {
        flex: 1;
        text-align: center;
      }

      .stat-label {
        font-size: 0.875rem;
        font-weight: 600;
        color: #64748b;
        margin: 0 0 0.5rem 0;
        text-transform: uppercase;
        letter-spacing: 0.05em;
      }

      .stat-value {
        font-size: 2rem;
        font-weight: 800;
        color: #0f172a;
        margin: 0 0 0.25rem 0;
        line-height: 1;
      }

      .stat-description {
        font-size: 0.875rem;
        color: #94a3b8;
      }

      .content-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
        gap: 1.5rem;
        margin-bottom: 2rem;
      }

      @media (max-width: 768px) {
        .content-grid {
          grid-template-columns: 1fr;
        }
      }

      .content-card {
        padding: 1.5rem;
        transition: all 0.3s ease;
      }

      .content-card:hover {
        box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04) !important;
      }

      .card-header {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        margin-bottom: 1.5rem;
      }

      .card-title {
        font-size: 1.25rem;
        font-weight: 700;
        color: #0f172a;
        margin: 0 0 0.25rem 0;
        display: flex;
        align-items: center;
        gap: 0.5rem;
      }

      .card-title mat-icon {
        font-size: 24px;
        width: 24px;
        height: 24px;
        color: #6366f1;
      }

      .card-subtitle {
        font-size: 0.875rem;
        color: #64748b;
        margin: 0;
      }

      .list-container {
        display: flex;
        flex-direction: column;
        gap: 0.75rem;
      }

      .list-item {
        padding: 1rem;
        border-radius: 8px;
        background: #f8fafc;
        transition: all 0.2s ease;
      }

      .list-item:hover {
        background: #f1f5f9;
        transform: translateX(4px);
      }

      .list-item-content {
        display: flex;
        justify-content: space-between;
        align-items: center;
      }

      .list-item-primary {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .list-item-title {
        font-weight: 600;
        color: #0f172a;
        font-size: 0.9375rem;
      }

      .list-item-subtitle {
        font-size: 0.875rem;
        color: #64748b;
      }

      .loading-container {
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 3rem;
      }

      .empty-state {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 3rem;
        color: #94a3b8;
        text-align: center;
      }

      .empty-state mat-icon {
        font-size: 48px;
        width: 48px;
        height: 48px;
        margin-bottom: 1rem;
        opacity: 0.5;
      }

      .empty-state p {
        margin: 0;
        font-size: 0.875rem;
      }

      .status-pending {
        background-color: #fef3c7 !important;
        color: #92400e !important;
      }

      .status-confirmed {
        background-color: #d1fae5 !important;
        color: #065f46 !important;
        }

      .status-completed {
        background-color: #dbeafe !important;
        color: #1e40af !important;
      }

      .status-cancelled {
        background-color: #fee2e2 !important;
        color: #991b1b !important;
      }

      .quick-actions {
        grid-column: span 1;
      }

      .actions-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 1rem;
      }

      @media (max-width: 640px) {
        .actions-grid {
          grid-template-columns: 1fr;
        }
      }

      .action-button {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 0.5rem;
        padding: 1.5rem !important;
        height: auto;
      }

      .action-button mat-icon {
        font-size: 32px;
        width: 32px;
        height: 32px;
      }

      /* Animations */
      .animate-fade-in-down {
        animation: fadeInDown 0.6s ease-out;
      }

      @keyframes fadeInDown {
        from {
          opacity: 0;
          transform: translateY(-20px);
        }
        to {
          opacity: 1;
          transform: translateY(0);
        }
      }

      @keyframes fadeIn {
        from {
          opacity: 0;
        }
        to {
          opacity: 1;
        }
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {
  private readonly bookingApi = inject(BookingApi);
  private readonly posApi = inject(PosApi);
  private readonly staffApi = inject(StaffApi);
  private readonly clientApi = inject(ClientApi);
  readonly branchStore = inject(BranchStore);

  readonly branches = computed(() => this.branchStore.allItems() ?? []);
  readonly loading = computed(() => this.branchStore.loading());
  
  readonly bookings = signal<BookingSlot[]>([]);
  readonly invoices = signal<Invoice[]>([]);
  readonly staff = signal<Staff[]>([]);
  readonly clients = signal<Client[]>([]);
  readonly dataLoading = signal(true);

  readonly todayBookings = computed(() => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    
    return this.bookings().filter(booking => {
      const bookingDate = new Date(booking.startUtc);
      return bookingDate >= today && bookingDate < tomorrow;
    });
  });

  readonly todayRevenue = computed(() => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    
    return this.invoices()
      .filter(invoice => {
        // Assuming invoices have a date field - adjust based on actual Invoice model
        return invoice.status === 'completed' || invoice.status === 'paid';
      })
      .reduce((sum, invoice) => sum + (invoice.total || 0), 0);
  });

  readonly recentBookings = computed(() => {
    return this.bookings()
      .sort((a, b) => new Date(b.startUtc).getTime() - new Date(a.startUtc).getTime())
      .slice(0, 5);
  });

  readonly recentInvoices = computed(() => {
    return this.invoices()
      .sort((a, b) => {
        // Sort by most recent - adjust based on actual Invoice model
        return b.id.localeCompare(a.id);
      })
      .slice(0, 5);
  });

  ngOnInit() {
    this.branchStore.loadAll();
    this.loadDashboardData();
  }

  private loadDashboardData() {
    this.dataLoading.set(true);
    
    forkJoin({
      bookings: this.bookingApi.getAll().pipe(catchError(() => of([] as BookingSlot[]))),
      invoices: this.posApi.getAll().pipe(catchError(() => of([] as Invoice[]))),
      staff: this.staffApi.getAll().pipe(catchError(() => of([] as Staff[]))),
      clients: this.clientApi.getAll().pipe(catchError(() => of([] as Client[])))
    }).subscribe({
      next: (data) => {
        this.bookings.set(data.bookings);
        this.invoices.set(data.invoices);
        this.staff.set(data.staff);
        this.clients.set(data.clients);
        this.dataLoading.set(false);
      },
      error: () => {
        this.dataLoading.set(false);
      }
    });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
