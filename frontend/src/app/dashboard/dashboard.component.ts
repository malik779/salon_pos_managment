import { ChangeDetectionStrategy, Component, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { BranchStore } from '@app/branches/store/branch.store';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  standalone: true,
  selector: 'sp-dashboard',
  imports: [CommonModule, MatCardModule, MatIconModule, MatProgressSpinnerModule],
  template: `
    <div class="dashboard-container">
      <div class="dashboard-header animate-fade-in-down">
        <h1 class="page-title">Dashboard</h1>
        <p class="page-subtitle">Welcome back! Here's what's happening at your salon.</p>
      </div>

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

        <mat-card class="stat-card " [style.animation-delay]="'0.2s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #d946ef 0%, #f472b6 100%);">
            <mat-icon>event</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">Upcoming Bookings</h3>
            <p class="stat-value">--</p>
            <span class="stat-description">Today's appointments</span>
          </div>
        </mat-card>

        <mat-card class="stat-card" [style.animation-delay]="'0.3s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #10b981 0%, #34d399 100%);">
            <mat-icon>point_of_sale</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">POS Registers</h3>
            <p class="stat-value">--</p>
            <span class="stat-description">Active terminals</span>
          </div>
        </mat-card>

        <mat-card class="stat-card" [style.animation-delay]="'0.4s'">
          <div class="stat-icon-wrapper" style="background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);">
            <mat-icon>people</mat-icon>
          </div>
          <div class="stat-content">
            <h3 class="stat-label">Staff Members</h3>
            <p class="stat-value">--</p>
            <span class="stat-description">On duty today</span>
          </div>
        </mat-card>
      </div>

      <div class="features-grid">
        <mat-card class="feature-card" [style.animation-delay]="'0.5s'">
          <div class="feature-header">
            <mat-icon class="feature-icon">sync</mat-icon>
            <h3>Real-time Sync</h3>
          </div>
          <p>Sync bookings across channels in real time. Never miss an appointment.</p>
        </mat-card>

        <mat-card class="feature-card" [style.animation-delay]="'0.6s'">
          <div class="feature-header">
            <mat-icon class="feature-icon">analytics</mat-icon>
            <h3>Analytics</h3>
          </div>
          <p>Monitor Z-read progress and tills. Track your salon's performance.</p>
        </mat-card>

        <mat-card class="feature-card" [style.animation-delay]="'0.7s'">
          <div class="feature-header">
            <mat-icon class="feature-icon">security</mat-icon>
            <h3>Secure & Reliable</h3>
          </div>
          <p>Enterprise-grade security with offline support. Your data is safe.</p>
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

      .features-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        gap: 1.5rem;
      }

      @media (max-width: 768px) {
        .features-grid {
          grid-template-columns: 1fr;
        }
      }

      .feature-card {
        padding: 2rem;
        transition: all 0.3s ease;
        cursor: pointer;
      }

      .feature-card:hover {
        transform: translateY(-4px);
        box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04) !important;
      }

      .feature-header {
        display: flex;
        align-items: center;
        gap: 1rem;
        margin-bottom: 1rem;
      }

      .feature-icon {
        width: 48px;
        height: 48px;
        font-size: 48px;
        color: #6366f1;
      }

      .feature-header h3 {
        font-size: 1.25rem;
        font-weight: 700;
        color: #0f172a;
        margin: 0;
      }

      .feature-card p {
        color: #64748b;
        line-height: 1.6;
        margin: 0;
      }

      /* Animations */
      .animate-fade-in-down {
        animation: fadeInDown 0.6s ease-out;
      }

      .animate-fade-in-up {
        animation: fadeInUp 0.6s ease-out;
        opacity: 0;
        animation-fill-mode: forwards;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {
  readonly branchStore = inject(BranchStore);
  
  branches = computed(() => this.branchStore.allItems() ?? []);
  loading = computed(() => this.branchStore.loading());
  constructor() {}

  ngOnInit() {
    this.branchStore.loadAll();
  }
}
