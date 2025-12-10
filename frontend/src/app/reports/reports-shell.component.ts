import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ReportsApi } from '../core/api/api-client';

@Component({
  standalone: true,
  selector: 'sp-reports-shell',
  imports: [CommonModule, MatCardModule, ReactiveFormsModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="load()" class="report-form">
      <input type="text" placeholder="Branch Id" formControlName="branchId" />
      <button type="submit">Load</button>
    </form>
    <mat-card *ngFor="let report of reports">
      <strong>{{ report.businessDate }}</strong>
      <p>Sales: {{ report.sales | currency }}</p>
      <p>Bookings: {{ report.bookings }}</p>
      <p>Commission: {{ report.commissionPayout | currency }}</p>
    </mat-card>
  `,
  styles: [
    `
      .report-form {
        display: flex;
        gap: 1rem;
        margin-bottom: 1rem;
      }
      mat-card {
        margin-bottom: 0.75rem;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReportsShellComponent {
  reports: any[] = [];
  readonly form = this.fb.group({ branchId: ['00000000-0000-0000-0000-000000000001'] });

  constructor(private readonly fb: FormBuilder, private readonly api: ReportsApi) {}

  load() {
    const branchId = this.form.value.branchId ?? '';
    this.api.loadBranchDaily(branchId).subscribe((reports: any) => (this.reports = reports as any[]));
  }
}
