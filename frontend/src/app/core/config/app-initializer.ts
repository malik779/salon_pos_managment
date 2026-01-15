import { inject } from '@angular/core';
import { ConfigService } from '../services/config.service';

/**
 * Application initializer factory
 * Loads runtime configuration before the application starts
 * @returns Promise that resolves when configuration is loaded
 */
export function appInitializer(): () => Promise<void> {
  const configService = inject(ConfigService);
  
  return () => {
    return new Promise<void>((resolve, reject) => {
      configService.loadConfig().subscribe({
        next: () => {
          console.log('Configuration loaded successfully');
          resolve();
        },
        error: (error) => {
          console.error('Failed to load configuration:', error);
          // Still resolve to allow app to start with fallback config
          resolve();
        }
      });
    });
  };
}
