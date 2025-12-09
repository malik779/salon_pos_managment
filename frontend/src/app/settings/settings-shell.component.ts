import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatCardModule } from '@angular/material/card';

@Component({
  standalone: true,
  selector: 'sp-settings-shell',
  imports: [CommonModule, MatSlideToggleModule, MatCardModule],
  template: `
    <mat-card>
      <h2>Settings</h2>
      <mat-slide-toggle>Enable offline mode</mat-slide-toggle>
      <mat-slide-toggle>Enable two-step approvals</mat-slide-toggle>
    </mat-card>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SettingsShellComponent {}
