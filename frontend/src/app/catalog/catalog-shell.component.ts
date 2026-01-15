import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { CatalogApi } from '../core/api/api-client';
import { ServiceItem } from '../core/models/domain.models';

@Component({
  standalone: true,
  selector: 'sp-catalog-shell',
  imports: [CommonModule, MatTableModule],
  template: `
    <h2>Services & Packages</h2>
    <table mat-table [dataSource]="services">
      <ng-container matColumnDef="name">
        <th mat-header-cell *matHeaderCellDef>Name</th>
        <td mat-cell *matCellDef="let item">{{ item.name }}</td>
      </ng-container>

      <ng-container matColumnDef="duration">
        <th mat-header-cell *matHeaderCellDef>Duration</th>
        <td mat-cell *matCellDef="let item">{{ item.durationMinutes }} min</td>
      </ng-container>

      <ng-container matColumnDef="price">
        <th mat-header-cell *matHeaderCellDef>Base price</th>
        <td mat-cell *matCellDef="let item">{{ item.price | currency }}</td>
      </ng-container>

      <tr mat-header-row *matHeaderRowDef="displayed"></tr>
      <tr mat-row *matRowDef="let row; columns: displayed"></tr>
    </table>
  `,
  styles: [
    `
      table {
        width: 100%;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CatalogShellComponent implements OnInit {
  services: ServiceItem[] = [];
  readonly displayed = ['name', 'duration', 'price'];

  constructor(private readonly api: CatalogApi) {}

  ngOnInit(): void {
    this.api.getAll()?.subscribe((services) => (this.services = services));
  }
}
