import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

export interface ApiOptions {
  baseUrl?: string;
  headers?: HttpHeaders | { [header: string]: string | string[] };
  params?: HttpParams | { [param: string]: string | number | boolean | ReadonlyArray<string | number | boolean> };
}

@Injectable({ providedIn: 'root' })
export class ApiHttpService {
  constructor(private readonly http: HttpClient) {}

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

  private buildUrl(path: string, options?: ApiOptions) {
    if (path.startsWith('http')) {
      return path;
    }

    const base = options?.baseUrl ?? environment.apiBaseUrl;
    return `${base}${path}`;
  }
}
