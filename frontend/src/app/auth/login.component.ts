import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  standalone: true,
  selector: 'sp-login',
  imports: [ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <mat-card>
      <h2>Sign in</h2>
      <form [formGroup]="form" (ngSubmit)="submit()">
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Email</mat-label>
          <input matInput formControlName="username" required />
        </mat-form-field>
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Password</mat-label>
          <input matInput type="password" formControlName="password" required />
        </mat-form-field>
        <button mat-flat-button color="primary" [disabled]="form.invalid">Sign in</button>
      </form>
    </mat-card>
  `,
  styles: [
    `
      mat-card {
        max-width: 420px;
        margin: 5rem auto;
      }
      form {
        display: flex;
        flex-direction: column;
        gap: 1rem;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent {
  readonly form = this.fb.group({
    username: [''],
    password: ['']
  });

  constructor(private readonly fb: FormBuilder, private readonly auth: AuthService, private readonly router: Router) {}

  submit() {
    if (this.form.invalid) {
      return;
    }

    const { username, password } = this.form.value;
    this.auth.login(username ?? '', password ?? '').subscribe(() => this.router.navigate(['/']));
  }
}
