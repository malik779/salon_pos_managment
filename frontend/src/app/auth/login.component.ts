import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'sp-login',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="login-container">
      <!-- Animated Background Elements -->
      <div class="background-decoration">
        <div class="gradient-orb orb-1"></div>
        <div class="gradient-orb orb-2"></div>
        <div class="gradient-orb orb-3"></div>
      </div>

      <!-- Main Content -->
      <div class="login-content">
        <!-- Brand Section -->
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
            <p class="brand-subtitle">Professional POS & Management System</p>
          </div>
        </div>

        <!-- Login Card -->
        <mat-card class="login-card animate-fade-in-up">
          <div class="card-header">
            <h2 class="card-title">Welcome Back</h2>
            <p class="card-subtitle">Sign in to continue to your account</p>
          </div>

          <form [formGroup]="form" (ngSubmit)="submit()" class="login-form">
            <mat-form-field appearance="outline" class="form-field">
              <mat-label>Email Address</mat-label>
              <mat-icon matPrefix>email</mat-icon>
              <input matInput type="email" formControlName="email" required autocomplete="email" class="form-input" />
            </mat-form-field>

            <mat-form-field appearance="outline" class="form-field">
              <mat-label>Password</mat-label>
              <mat-icon matPrefix>lock</mat-icon>
              <input
                matInput
                [type]="hidePassword ? 'password' : 'text'"
                formControlName="password"
                required
                autocomplete="current-password"
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

            <div class="form-actions">
              <button
                mat-raised-button
                color="primary"
                type="submit"
                [disabled]="form.invalid || isLoading"
                class="submit-button"
              >
                <span *ngIf="!isLoading">Sign In</span>
                <span *ngIf="isLoading" class="loading-spinner">
                  <span class="spinner"></span>
                  Signing in...
                </span>
              </button>
            </div>
          </form>

          <div class="card-footer">
            <p class="footer-text">
              <a href="#" class="link" (click)="navigateToRegister(); $event.preventDefault()">Don't have an account? Register</a>
            </p>
            <p class="footer-text" style="margin-top: 0.5rem;">
              <a href="#" class="link">Forgot your password?</a>
            </p>
          </div>
        </mat-card>

        <!-- Features Section (Desktop only) -->
        <div class="features-section animate-fade-in">
          <div class="feature-item">
            <div class="feature-icon">📊</div>
            <h3>Real-time Analytics</h3>
            <p>Track your salon performance</p>
          </div>
          <div class="feature-item">
            <div class="feature-icon">📅</div>
            <h3>Smart Booking</h3>
            <p>Manage appointments seamlessly</p>
          </div>
          <div class="feature-item">
            <div class="feature-icon">💳</div>
            <h3>POS Integration</h3>
            <p>Complete payment solutions</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .login-container {
        min-height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 1rem;
        position: relative;
        overflow: hidden;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      }

      @media (max-width: 640px) {
        .login-container {
          padding: 0.75rem;
          align-items: flex-start;
          padding-top: 2rem;
        }
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

      .login-content {
        position: relative;
        z-index: 1;
        width: 100%;
        max-width: 1200px;
        display: grid;
        grid-template-columns: 1fr;
        gap: 2rem;
        align-items: center;
      }

      @media (min-width: 1024px) {
        .login-content {
          grid-template-columns: 1fr 1fr;
        }
      }

      .brand-section {
        text-align: center;
        color: white;
        padding: 1.5rem 0;
      }

      @media (min-width: 640px) {
        .brand-section {
          padding: 2rem;
        }
      }

      @media (min-width: 1024px) {
        .brand-section {
          text-align: left;
          padding: 0;
        }
      }

      .brand-logo {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 1rem;
      }

      @media (min-width: 1024px) {
        .brand-logo {
          align-items: flex-start;
        }
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

      @media (min-width: 768px) {
        .brand-title {
          font-size: 3.5rem;
        }
      }

      .brand-subtitle {
        font-size: 1.125rem;
        color: rgba(255, 255, 255, 0.9);
        margin: 0.5rem 0 0 0;
        font-weight: 400;
      }

      .login-card {
        background: rgba(255, 255, 255, 0.95);
        backdrop-filter: blur(20px);
        border: 1px solid rgba(255, 255, 255, 0.3);
        padding: 2rem;
        max-width: 450px;
        margin: 0 auto;
        width: 100%;
        box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      }

      @media (min-width: 640px) {
        .login-card {
          padding: 2.5rem;
        }
      }

      @media (min-width: 1024px) {
        .login-card {
          margin: 0;
        }
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

      .login-form {
        display: flex;
        flex-direction: column;
        gap: 1.25rem;
        width: 100%;
      }

      .form-field {
        width: 100%;
        margin-bottom: 0;
      }

      .form-field ::ng-deep .mat-mdc-text-field-wrapper {
        padding-bottom: 0;
      }

      .form-field ::ng-deep .mat-mdc-form-field-subscript-wrapper {
        margin-top: 4px;
        min-height: 20px;
      }

      .form-field ::ng-deep .mat-mdc-form-field-icon-prefix {
        padding-right: 12px;
        padding-left: 4px;
        color: #6366f1;
        opacity: 0.8;
      }

      .form-field ::ng-deep .mat-mdc-form-field-icon-suffix {
        padding-left: 8px;
        padding-right: 4px;
        color: #64748b;
        opacity: 0.7;
      }

      .form-field ::ng-deep .mdc-text-field--outlined {
        padding-left: 0;
        padding-right: 0;
      }

      .form-field mat-icon[matPrefix] {
        color: #6366f1;
        opacity: 0.8;
        margin-right: 0;
        width: 20px;
        height: 20px;
        font-size: 20px;
      }

      .form-field mat-icon[matSuffix] {
        color: #64748b;
        opacity: 0.7;
        width: 20px;
        height: 20px;
        font-size: 20px;
      }

      .form-field ::ng-deep .mdc-text-field__input {
        padding-left: 12px;
        padding-right: 12px;
        font-size: 1rem;
        line-height: 1.5;
      }

      .form-field ::ng-deep .mdc-floating-label {
        font-size: 1rem;
      }

      .form-field ::ng-deep .mdc-text-field--outlined .mdc-notched-outline {
        top: 0;
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

      .submit-button:active:not(:disabled) {
        transform: translateY(0);
      }

      .submit-button:disabled {
        opacity: 0.6;
        cursor: not-allowed;
        transform: none;
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

      @keyframes spin {
        to {
          transform: rotate(360deg);
        }
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
      }

      .link:hover {
        color: #8b5cf6;
        text-decoration: underline;
        transform: translateY(-1px);
      }

      .features-section {
        display: none;
        flex-direction: column;
        gap: 2rem;
        color: white;
      }

      @media (min-width: 1024px) {
        .features-section {
          display: flex;
        }
      }

      .feature-item {
        display: flex;
        flex-direction: column;
        gap: 0.75rem;
        padding: 1.5rem;
        background: rgba(255, 255, 255, 0.1);
        backdrop-filter: blur(10px);
        border-radius: 16px;
        border: 1px solid rgba(255, 255, 255, 0.2);
        transition: all 0.3s ease;
      }

      .feature-item:hover {
        background: rgba(255, 255, 255, 0.15);
        transform: translateX(8px);
      }

      .feature-icon {
        font-size: 2rem;
        margin-bottom: 0.25rem;
      }

      .feature-item h3 {
        font-size: 1.25rem;
        font-weight: 600;
        margin: 0;
      }

      .feature-item p {
        font-size: 0.9375rem;
        color: rgba(255, 255, 255, 0.8);
        margin: 0;
      }

      /* Animations */
      .animate-fade-in {
        animation: fadeIn 0.8s ease-out;
      }

      .animate-fade-in-up {
        animation: fadeInUp 0.8s ease-out;
      }

      .animate-fade-in-down {
        animation: fadeInDown 0.8s ease-out;
      }
    `
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent {
  hidePassword = true;
  isLoading = false;

  readonly form = this.fb.group({
    email: [''],
    password: ['']
  });

  constructor(private readonly fb: FormBuilder, private readonly auth: AuthService, private readonly router: Router) {}

  submit() {
    if (this.form.invalid) {
      return;
    }

    this.isLoading = true;
    const { email, password } = this.form.value;
    this.auth.login(email ?? '', password ?? '').subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/']);
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  navigateToRegister() {
    this.router.navigate(['/auth/register']);
  }
}
