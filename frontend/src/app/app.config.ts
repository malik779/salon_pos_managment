import { ApplicationConfig, APP_INITIALIZER, importProvidersFrom } from '@angular/core';
import {
  provideRouter,
  withComponentInputBinding,
  withInMemoryScrolling,
  withRouterConfig
} from '@angular/router';
import {
  provideHttpClient,
  withFetch,
  withInterceptors
} from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { appRoutes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { idempotencyInterceptor } from './core/interceptors/idempotency.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { appInitializer } from './core/config/app-initializer';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(
      appRoutes,
      withComponentInputBinding(),
      withInMemoryScrolling({ scrollPositionRestoration: 'enabled' }),
      withRouterConfig({ onSameUrlNavigation: 'reload' })
    ),
    provideHttpClient(withFetch(), withInterceptors([authInterceptor, idempotencyInterceptor, errorInterceptor])),
    provideAnimations(),
    importProvidersFrom(MatSnackBarModule, MatDialogModule),
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializer,
      multi: true
    }
  ]
};
