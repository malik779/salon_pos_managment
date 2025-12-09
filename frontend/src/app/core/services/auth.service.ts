import { Injectable, computed, signal } from '@angular/core';
import { tap } from 'rxjs/operators';
import { IdentityApi } from '../api/api-client';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly accessToken = signal<string | null>(localStorage.getItem('sp.accessToken'));
  readonly isAuthenticated = computed(() => Boolean(this.accessToken()));

  constructor(private readonly identityApi: IdentityApi) {}

  login(username: string, password: string) {
    return this.identityApi.login(username, password).pipe(
      tap((tokens) => {
        this.accessToken.set(tokens.accessToken);
        localStorage.setItem('sp.accessToken', tokens.accessToken);
        localStorage.setItem('sp.refreshToken', tokens.refreshToken);
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
