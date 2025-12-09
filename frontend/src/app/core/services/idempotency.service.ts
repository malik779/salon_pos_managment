import { Injectable } from '@angular/core';
import { v4 as uuid } from 'uuid';

@Injectable({ providedIn: 'root' })
export class IdempotencyService {
  private readonly header = 'Idempotency-Key';

  generate(): string {
    return uuid();
  }

  headerName() {
    return this.header;
  }
}
