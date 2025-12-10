import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { ClientApi } from '../core/api/api-client';
import { Client } from '../core/models/domain.models';

@Component({
  standalone: true,
  selector: 'sp-clients-shell',
  imports: [CommonModule, MatTableModule],
  template: `
    <h2>Clients</h2>
    <table mat-table [dataSource]="clients">
      <ng-container matColumnDef="name">
        <th mat-header-cell *matHeaderCellDef>Name</th>
        <td mat-cell *matCellDef="let client">{{ client.firstName }} {{ client.lastName }}</td>
      </ng-container>
      <ng-container matColumnDef="phone">
        <th mat-header-cell *matHeaderCellDef>Phone</th>
        <td mat-cell *matCellDef="let client">{{ client.phone }}</td>
      </ng-container>
      <ng-container matColumnDef="email">
        <th mat-header-cell *matHeaderCellDef>Email</th>
        <td mat-cell *matCellDef="let client">{{ client.email }}</td>
      </ng-container>
      <tr mat-header-row *matHeaderRowDef="columns"></tr>
      <tr mat-row *matRowDef="let row; columns: columns"></tr>
    </table>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ClientsShellComponent implements OnInit {
  clients: Client[] = [];
  readonly columns = ['name', 'phone', 'email'];

  constructor(private readonly api: ClientApi) {}

  ngOnInit(): void {
    this.api.list().subscribe((clients) => (this.clients = clients));
  }
}
