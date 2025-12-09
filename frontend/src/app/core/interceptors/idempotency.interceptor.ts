import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { IdempotencyService } from '../services/idempotency.service';

const METHODS_REQUIRING_KEY = ['POST', 'PATCH', 'PUT'];

export const idempotencyInterceptor: HttpInterceptorFn = (req, next) => {
  if (!METHODS_REQUIRING_KEY.includes(req.method)) {
    return next(req);
  }

  if (req.headers.has('Idempotency-Key')) {
    return next(req);
  }

  const idempotency = inject(IdempotencyService);
  const withKey = req.clone({
    setHeaders: {
      'Idempotency-Key': idempotency.generate()
    }
  });

  return next(withKey);
};
