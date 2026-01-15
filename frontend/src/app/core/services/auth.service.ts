import { Injectable, computed, signal } from '@angular/core';
import { tap } from 'rxjs/operators';
import { IdentityApi } from '../api/api-client';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly accessToken = signal<string | null>(localStorage.getItem('sp.accessToken'));
  readonly isAuthenticated = computed(() => Boolean(this.accessToken()));

  constructor(private readonly identityApi: IdentityApi) {}

  login(email: string, password: string) {
    return this.identityApi.login(email, password).pipe(
      tap((response) => {
        this.accessToken.set(response.accessToken);
        localStorage.setItem('sp.accessToken', response.accessToken);
        localStorage.setItem('sp.refreshToken', response.refreshToken);
        localStorage.setItem('sp.userId', response.userId);
        localStorage.setItem('sp.userEmail', response.email);
        localStorage.setItem('sp.userName', response.fullName);
      })
    );
  }

  logout() {
    this.accessToken.set(null);
    localStorage.removeItem('sp.accessToken');
    localStorage.removeItem('sp.refreshToken');
  }

  get token(): string | null {
    return this.accessToken() ?? localStorage.getItem('sp.accessToken');
  }
}
