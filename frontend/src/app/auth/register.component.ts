import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IdentityApi } from '../core/api/api-client';
import { BranchStore } from '@app/branches/store/branch.store';
import { Subject, takeUntil } from 'rxjs';

@Component({
  standalone: true,
  selector: 'sp-register',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule
  ],
  template: `
    <div class="register-container">
      <div class="background-decoration">
        <div class="gradient-orb orb-1"></div>
        <div class="gradient-orb orb-2"></div>
        <div class="gradient-orb orb-3"></div>
      </div>

      <div class="register-content">
        <div class="brand-section animate-fade-in-down">
          <div class="brand-logo">
            <div class="logo-icon">
              <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path
                  d="M12 2L2 7L12 12L22 7L12 2Z"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
                <path
                  d="M2 17L12 22L22 17"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
                <path
                  d="M2 12L12 17L22 12"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
              </svg>
            </div>
            <h1 class="brand-title">Salon Platform</h1>
            <p class="brand-subtitle">Create your account</p>
          </div>
        </div>

        <mat-card class="register-card animate-fade-in-up">
          <div class="card-header">
            <h2 class="card-title">Sign Up</h2>
            <p class="card-subtitle">Create an account to get started</p>
          </div>

          <form [formGroup]="form" (ngSubmit)="submit()" class="register-form">
            <mat-form-field appearance="outline" class="form-field">
              <mat-label>Full Name</mat-label>
              <mat-icon matPrefix>person</mat-icon>
              <input matInput type="text" formControlName="fullName" required autocomplete="name" />
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field">
              <mat-label>Email Address</mat-label>
              <mat-icon matPrefix>email</mat-icon>
              <input matInput type="email" formControlName="email" required autocomplete="email" />
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field">
              <mat-label>Password</mat-label>
              <mat-icon matPrefix>lock</mat-icon>
              <input
                matInput
                [type]="hidePassword ? 'password' : 'text'"
                formControlName="password"
                required
                autocomplete="new-password"
              />
              <button
                mat-icon-button
                matSuffix
                type="button"
                (click)="hidePassword = !hidePassword"
                [attr.aria-label]="'Hide password'"
                [attr.aria-pressed]="hidePassword"
              >
                <mat-icon>{{ hidePassword ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field">
              <mat-label>Confirm Password</mat-label>
              <mat-icon matPrefix>lock</mat-icon>
              <input
                matInput
                [type]="hideConfirmPassword ? 'password' : 'text'"
                formControlName="confirmPassword"
                required
                autocomplete="new-password"
              />
              <button
                mat-icon-button
                matSuffix
                type="button"
                (click)="hideConfirmPassword = !hideConfirmPassword"
                [attr.aria-label]="'Hide password'"
                [attr.aria-pressed]="hideConfirmPassword"
              >
                <mat-icon>{{ hideConfirmPassword ? 'visibility_off' : 'visibility' }}</mat-icon>
              </button>
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field">
              <mat-label>Branch</mat-label>
              <mat-icon matPrefix>store</mat-icon>
              <mat-select formControlName="branchId" required>
                <mat-option *ngFor="let branch of branches()" [value]="branch.id">
                  {{ branch.name }}
                </mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field">
              <mat-label>Role</mat-label>
              <mat-icon matPrefix>badge</mat-icon>
              <mat-select formControlName="role" required>
                <mat-option *ngFor="let role of roleOptions" [value]="role.value">
                  {{ role.label }}
                </mat-option>
              </mat-select>
            </mat-form-field>

            <div class="form-actions">
              <button
                mat-raised-button
                color="primary"
                type="submit"
                [disabled]="form.invalid || isLoading"
                class="submit-button"
              >
                <span *ngIf="!isLoading">Create Account</span>
                <span *ngIf="isLoading" class="loading-spinner">
                  <span class="spinner"></span>
                  Creating account...
                </span>
              </button>
            </div>
          </form>

          <div class="card-footer">
            <p class="footer-text">
              Already have an account?
              <a href="#" class="link" (click)="navigateToLogin(); $event.preventDefault()">Sign in</a>
            </p>
          </div>
        </mat-card>
      </div>
    </div>
  `,
  styles: [
    `
      .register-container {
        min-height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 1rem;
        position: relative;
        overflow: hidden;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      }

      .background-decoration {
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        overflow: hidden;
        z-index: 0;
      }

      .gradient-orb {
        position: absolute;
        border-radius: 50%;
        filter: blur(80px);
        opacity: 0.3;
        animation: float 6s ease-in-out infinite;
      }

      .orb-1 {
        width: 400px;
        height: 400px;
        background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
        top: -100px;
        left: -100px;
        animation-delay: 0s;
      }

      .orb-2 {
        width: 300px;
        height: 300px;
        background: linear-gradient(135deg, #d946ef 0%, #f472b6 100%);
        bottom: -50px;
        right: -50px;
        animation-delay: 2s;
      }

      .orb-3 {
        width: 250px;
        height: 250px;
        background: linear-gradient(135deg, #8b5cf6 0%, #6366f1 100%);
        top: 50%;
        right: 10%;
        animation-delay: 4s;
      }

      .register-content {
        position: relative;
        z-index: 1;
        width: 100%;
        max-width: 500px;
      }

      .brand-section {
        text-align: center;
        color: white;
        padding: 1.5rem 0;
      }

      .brand-logo {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 1rem;
      }

      .logo-icon {
        width: 80px;
        height: 80px;
        background: rgba(255, 255, 255, 0.2);
        backdrop-filter: blur(10px);
        border-radius: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
        color: white;
        animation: scaleIn 0.6s ease-out;
      }

      .logo-icon svg {
        width: 48px;
        height: 48px;
      }

      .brand-title {
        font-size: 2.5rem;
        font-weight: 800;
        margin: 0;
        background: linear-gradient(135deg, #ffffff 0%, #e0e7ff 100%);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
        letter-spacing: -0.02em;
      }

      .brand-subtitle {
        font-size: 1.125rem;
        color: rgba(255, 255, 255, 0.9);
        margin: 0.5rem 0 0 0;
        font-weight: 400;
      }

      .register-card {
        background: rgba(255, 255, 255, 0.95);
        backdrop-filter: blur(20px);
        border: 1px solid rgba(255, 255, 255, 0.3);
        padding: 2rem;
        width: 100%;
        box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      }

      .card-header {
        text-align: center;
        margin-bottom: 2rem;
      }

      .card-title {
        font-size: 1.875rem;
        font-weight: 700;
        color: #0f172a;
        margin: 0 0 0.5rem 0;
        line-height: 1.2;
      }

      .card-subtitle {
        color: #64748b;
        font-size: 0.9375rem;
        margin: 0;
        line-height: 1.5;
      }

      .register-form {
        display: flex;
        flex-direction: column;
        gap: 1.25rem;
        width: 100%;
      }

      .form-field {
        width: 100%;
        margin-bottom: 0;
      }

      .form-actions {
        margin-top: 0.75rem;
        width: 100%;
      }

      .submit-button {
        width: 100%;
        height: 52px;
        font-size: 1rem;
        font-weight: 600;
        background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%) !important;
        color: white !important;
        border: none;
        border-radius: 12px;
        transition: all 0.3s ease;
        text-transform: none;
        letter-spacing: 0.01em;
        box-shadow: 0 4px 6px -1px rgba(99, 102, 241, 0.3), 0 2px 4px -1px rgba(99, 102, 241, 0.2);
      }

      .submit-button:hover:not(:disabled) {
        transform: translateY(-2px);
        box-shadow: 0 10px 25px -5px rgba(99, 102, 241, 0.4), 0 4px 6px -2px rgba(99, 102, 241, 0.3);
        background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%) !important;
      }

      .submit-button:disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }

      .loading-spinner {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
      }

      .spinner {
        width: 16px;
        height: 16px;
        border: 2px solid rgba(255, 255, 255, 0.3);
        border-top-color: white;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
      }

      .card-footer {
        margin-top: 1.5rem;
        text-align: center;
        padding-top: 1rem;
        border-top: 1px solid #e2e8f0;
      }

      .footer-text {
        margin: 0;
        font-size: 0.875rem;
        color: #64748b;
        line-height: 1.5;
      }

      .link {
        color: #6366f1;
        text-decoration: none;
        font-weight: 500;
        transition: all 0.2s ease;
        display: inline-block;
        margin-left: 0.25rem;
      }

      .link:hover {
        color: #8b5cf6;
        text-decoration: underline;
      }

      @keyframes float {
        0%, 100% {
          transform: translateY(0px);
        }
        50% {
          transform: translateY(-20px);
        }
      }

      @keyframes scaleIn {
        from {
          transform: scale(0.8);
          opacity: 0;
        }
        to {
          transform: scale(1);
          opacity: 1;
        }
      }

      @keyframes spin {
        to {
          transform: rotate(360deg);
        }
      }

      .animate-fade-in-down {
        animation: fadeInDown 0.8s ease-out;
      }

      .animate-fade-in-up {
        animation: fadeInUp 0.8s ease-out;
      }

      @keyframes fadeInDown {
        from {
          opacity: 0;
          transform: translateY(-20px);
        }
        to {
          opacity: 1;
          transform: translateY(0);
        }
      }

      @keyframes fadeInUp {
        from {
          opacity: 0;
          transform: translateY(20px);
        }
        to {
          opacity: 1;
          transform: translateY(0);
        }
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegisterComponent implements OnInit, OnDestroy {
  private readonly destroy$ = new Subject<void>();
  private readonly branchSelection = signal('');
  hidePassword = true;
  hideConfirmPassword = true;
  isLoading = false;
  readonly branchStore = inject(BranchStore);
  readonly branches = computed(() => this.branchStore.allItems() ?? []);
  readonly roleOptions = [
    { value: 'SuperAdmin', label: 'Super Admin' },
    { value: 'SalonOwner', label: 'Salon Owner' },
    { value: 'BranchManager', label: 'Branch Manager' },
    { value: 'Staff', label: 'Staff' },
    { value: 'Receptionist', label: 'Receptionist' },
    { value: 'Customer', label: 'Customer' }
  ];
  readonly form = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]],
    salonId: ['', [Validators.required]],
    branchId: ['', [Validators.required]],
    role: ['', [Validators.required]]
  }, { validators: this.passwordMatchValidator });

  constructor(
    private readonly fb: FormBuilder,
    private readonly identityApi: IdentityApi,
    private readonly router: Router
  ) {
    this.setupSalonSync();
  }

  passwordMatchValidator(group: any) {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  ngOnInit(): void {
    this.branchStore.loadAll();
    this.branchSelection.set(this.form.controls.branchId.value ?? '');
    this.form.controls.branchId.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe((branchId) => this.branchSelection.set(branchId ?? ''));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private setupSalonSync() {
    effect(() => {
      const branchId = this.branchSelection();
      const branch = this.branches().find((item) => item.id === branchId);
      const salonId = branch?.salonId ?? '';

      if (this.form.controls.salonId.value !== salonId) {
        this.form.controls.salonId.setValue(salonId, { emitEvent: false });
      }
    });
  }

  submit() {
    if (this.form.invalid) {
      return;
    }

    this.isLoading = true;
    const { email, password, fullName, salonId, branchId, role } = this.form.value;
    
    this.identityApi.register(
      email ?? '',
      password ?? '',
      fullName ?? '',
      salonId ?? '',
      branchId ?? '',
      role ?? ''
    ).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/auth/login']);
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  navigateToLogin() {
    this.router.navigate(['/auth/login']);
  }
}
