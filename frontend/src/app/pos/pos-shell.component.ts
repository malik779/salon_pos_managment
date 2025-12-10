import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { PosApi } from '../core/api/api-client';
import { MatCardModule } from '@angular/material/card';

@Component({
  standalone: true,
  selector: 'sp-pos-shell',
  imports: [CommonModule, MatButtonModule, MatInputModule, MatFormFieldModule, ReactiveFormsModule, MatCardModule],
  template: `
    <mat-card>
      <h2>POS Register</h2>
      <form [formGroup]="form" (ngSubmit)="checkout()">
        <div class="form-grid">
          <mat-form-field appearance="outline">
            <mat-label>Item SKU</mat-label>
            <input matInput formControlName="sku" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Qty</mat-label>
            <input matInput formControlName="quantity" type="number" />
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Unit Price</mat-label>
            <input matInput formControlName="price" type="number" />
          </mat-form-field>
        </div>
        <button mat-flat-button color="primary" type="submit">Add line</button>
      </form>
      <section class="cart">
        <div *ngFor="let line of lines()" class="cart-line">
          <span>{{ line.itemId }} x{{ line.quantity }}</span>
          <span>{{ line.unitPrice | currency }}</span>
        </div>
        <button mat-raised-button color="accent" (click)="finalize()" [disabled]="!lines().length">Finalize invoice</button>
      </section>
    </mat-card>
  `,
  styles: [
    `
      .form-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
        gap: 1rem;
      }
      .cart {
        margin-top: 1.5rem;
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
      }
      .cart-line {
        display: flex;
        justify-content: space-between;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PosShellComponent {
  readonly lines = signal<{ itemId: string; itemType: string; quantity: number; unitPrice: number }[]>([]);
  readonly form = this.fb.group({ sku: [''], quantity: [1], price: [0] });

  constructor(private readonly fb: FormBuilder, private readonly posApi: PosApi) {}

  checkout() {
    const { sku, quantity, price } = this.form.value;
    this.lines.update((lines) => [
      ...lines,
      {
        itemId: sku ?? 'sku',
        itemType: 'product',
        quantity: Number(quantity ?? 1),
        unitPrice: Number(price ?? 0)
      }
    ]);
    this.form.reset({ quantity: 1, price: 0 });
  }

  finalize() {
    this.posApi.createInvoice(this.lines(), '00000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000001').subscribe(() => {
      this.lines.set([]);
    });
  }
}
