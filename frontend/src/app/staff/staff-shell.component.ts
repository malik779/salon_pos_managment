import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { StaffApi } from '../core/api/api-client';
import { Staff } from '../core/models/domain.models';

@Component({
  standalone: true,
  selector: 'sp-staff-shell',
  imports: [CommonModule, MatListModule],
  template: `
    <h2>Staff & Commissions</h2>
    <mat-nav-list>
      <a mat-list-item *ngFor="let staff of staffMembers">
        {{ staff.fullName }}
        <span class="role">{{ staff.role }}</span>
      </a>
    </mat-nav-list>
  `,
  styles: [
    `
      .role {
        margin-left: auto;
        font-size: 0.85rem;
        opacity: 0.7;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StaffShellComponent implements OnInit {
  staffMembers: Staff[] = [];

  constructor(private readonly api: StaffApi) {}

  ngOnInit(): void {
    this.api.getAll()?.subscribe((staff) => (this.staffMembers = staff));
  }
}
