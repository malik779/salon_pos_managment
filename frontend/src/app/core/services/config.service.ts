import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map, shareReplay, tap } from 'rxjs/operators';
import { AppConfig } from '../models/app-config.model';

/**
 * Runtime configuration service
 * Loads configuration from config.json at application startup
 * Implements Singleton pattern and provides configuration throughout the application lifecycle
 */
@Injectable({ providedIn: 'root' })
export class ConfigService {
  private config$: Observable<AppConfig> | null = null;
  private configCache: AppConfig | null = null;

  constructor(private readonly http: HttpClient) {}

  /**
   * Loads configuration from config.json file
   * Uses shareReplay to cache the result and prevent multiple HTTP calls
   * @returns Observable of AppConfig
   */
  loadConfig(): Observable<AppConfig> {
    if (this.configCache) {
      return of(this.configCache);
    }

    if (!this.config$) {
      this.config$ = this.http.get<AppConfig>('/assets/config.json').pipe(
        tap((config) => {
          // Validate required configuration
          if (!config.apiBaseUrl) {
            throw new Error('apiBaseUrl is required in configuration');
          }
          if (!config.signalrHub) {
            throw new Error('signalrHub is required in configuration');
          }
          this.configCache = config;
        }),
        shareReplay(1),
        catchError((error) => {
          console.error('Failed to load configuration:', error);
          // Fallback to default configuration
          const fallbackConfig: AppConfig = {
            apiBaseUrl: 'http://localhost:5000',
            signalrHub: 'http://localhost:5000/hubs/updates',
            environment: 'development'
          };
          this.configCache = fallbackConfig;
          return of(fallbackConfig);
        })
      );
    }

    return this.config$;
  }

  /**
   * Gets the current configuration synchronously
   * Throws error if config is not loaded yet
   * @returns AppConfig
   */
  getConfig(): AppConfig {
    if (!this.configCache) {
      throw new Error('Configuration not loaded. Call loadConfig() first.');
    }
    return this.configCache;
  }

  /**
   * Gets API base URL
   * @returns API base URL string
   */
  getApiBaseUrl(): string {
    return this.getConfig().apiBaseUrl;
  }

  /**
   * Gets SignalR Hub URL
   * @returns SignalR Hub URL string
   */
  getSignalrHubUrl(): string {
    return this.getConfig().signalrHub;
  }

  /**
   * Gets environment name
   * @returns Environment name string
   */
  getEnvironment(): string {
    return this.getConfig().environment || 'development';
  }

  /**
   * Checks if a feature is enabled
   * @param featureName Feature flag name
   * @returns True if feature is enabled, false otherwise
   */
  isFeatureEnabled(featureName: string): boolean {
    return this.getConfig().features?.[featureName] ?? false;
  }
}
