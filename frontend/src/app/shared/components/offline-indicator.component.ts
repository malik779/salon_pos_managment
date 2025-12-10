import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';

type SyncStatus = 'online' | 'syncing' | 'offline' | null;

@Component({
  selector: 'offline-indicator',
  standalone: true,
  imports: [NgClass],
  template: `
    <span class="indicator" [ngClass]="status">
      <span class="dot"></span>
      {{ status ?? 'unknown' }}
    </span>
  `,
  styles: [
    `
      .indicator {
        display: inline-flex;
        align-items: center;
        gap: 0.35rem;
        text-transform: capitalize;
        font-size: 0.85rem;
      }
      .dot {
        width: 8px;
        height: 8px;
        border-radius: 50%;
        background: #22c55e;
      }
      .offline .dot {
        background: #dc2626;
      }
      .syncing .dot {
        background: #facc15;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OfflineIndicatorComponent {
  @Input() status: SyncStatus = 'online';
}
