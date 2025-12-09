import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { BranchApi } from '../core/api/api-client';
import { Branch } from '../core/models/domain.models';

@Component({
  standalone: true,
  selector: 'sp-branch-list',
  imports: [CommonModule, MatListModule],
  template: `
    <h2>Branches</h2>
    <mat-nav-list>
      <a mat-list-item *ngFor="let branch of branches">{{ branch.name }} – {{ branch.timezone }}</a>
    </mat-nav-list>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BranchListComponent implements OnInit {
  branches: Branch[] = [];

  constructor(private readonly branchApi: BranchApi) {}

  ngOnInit(): void {
    this.branchApi.list().subscribe((data) => (this.branches = data));
  }
}
