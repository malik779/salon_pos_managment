import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';

export interface ApiOptions {
  baseUrl?: string;
  headers?: HttpHeaders | { [header: string]: string | string[] };
  params?: HttpParams | { [param: string]: string | number | boolean | ReadonlyArray<string | number | boolean> };
}

/**
 * HTTP service for API calls
 * Uses ConfigService to get API base URL from runtime configuration
 */
@Injectable({ providedIn: 'root' })
export class ApiHttpService {
  constructor(
    private readonly http: HttpClient,
    private readonly configService: ConfigService
  ) {}

  get<T>(path: string, options?: ApiOptions) {
    return this.http.get<T>(this.buildUrl(path, options), options);
  }

  post<T>(path: string, body: unknown, options?: ApiOptions) {
    return this.http.post<T>(this.buildUrl(path, options), body, options);
  }

  patch<T>(path: string, body: unknown, options?: ApiOptions) {
    return this.http.patch<T>(this.buildUrl(path, options), body, options);
  }

  delete<T>(path: string, options?: ApiOptions) {
    return this.http.delete<T>(this.buildUrl(path, options), options);
  }

  private buildUrl(path: string, options?: ApiOptions): string {
    if (path.startsWith('http')) {
      return path;
    }

    // Use provided baseUrl or get from runtime configuration
    const base = options?.baseUrl ?? this.configService.getApiBaseUrl();
    return `${base}${path}`;
  }
}
