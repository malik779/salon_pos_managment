/**
 * Application runtime configuration interface
 * This configuration is loaded at runtime from config.json
 */
export interface AppConfig {
  /** Base URL for API Gateway */
  apiBaseUrl: string;
  /** SignalR Hub URL for real-time updates */
  signalrHub: string;
  /** Application version */
  version?: string;
  /** Environment name (development, staging, production) */
  environment?: string;
  /** Feature flags */
  features?: {
    [key: string]: boolean;
  };
}
