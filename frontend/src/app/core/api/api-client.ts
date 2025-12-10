import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiHttpService } from '../services/api-http.service';
import { BookingSlot, Branch, Client, Invoice, ServiceItem, Staff } from '../models/domain.models';

@Injectable({ providedIn: 'root' })
export class IdentityApi {
  private readonly baseUrl = '/auth';

  constructor(private readonly http: ApiHttpService) {}

  login(username: string, password: string): Observable<{ accessToken: string; refreshToken: string }> {
    return this.http.post(`${this.baseUrl}/token`, { username, password }, { baseUrl: resolveServiceBase(5001) });
  }
}

@Injectable({ providedIn: 'root' })
export class BranchApi {
  private readonly basePath = '/branches';
  constructor(private readonly http: ApiHttpService) {}

  list(): Observable<Branch[]> {
    return this.http.get(this.basePath, { baseUrl: resolveServiceBase(5003) });
  }
}

@Injectable({ providedIn: 'root' })
export class CatalogApi {
  private readonly basePath = '/catalog';
  constructor(private readonly http: ApiHttpService) {}

  listServices(): Observable<ServiceItem[]> {
    return this.http.get(`${this.basePath}/services`, { baseUrl: resolveServiceBase(5004) });
  }
}

@Injectable({ providedIn: 'root' })
export class BookingApi {
  private readonly basePath = '/bookings';
  constructor(private readonly http: ApiHttpService) {}

  getCalendar(branchId: string, from: string, to: string): Observable<BookingSlot[]> {
    return this.http.get(`/calendar?branchId=${branchId}&from=${from}&to=${to}`, { baseUrl: resolveServiceBase(5002) });
  }

  create(payload: Partial<BookingSlot>, idempotencyKey: string) {
    return this.http.post(this.basePath, payload, {
      baseUrl: resolveServiceBase(5002),
      headers: { 'Idempotency-Key': idempotencyKey }
    });
  }
}

@Injectable({ providedIn: 'root' })
export class PosApi {
  private readonly basePath = '/invoices';
  constructor(private readonly http: ApiHttpService) {}

  createInvoice(lines: { itemId: string; itemType: string; quantity: number; unitPrice: number }[], clientId: string, branchId: string) {
    return this.http.post(this.basePath, { lines, clientId, branchId }, { baseUrl: resolveServiceBase(5005) });
  }

  closeDay(branchId: string) {
    return this.http.post('/closereads', { branchId, businessDate: new Date().toISOString().substring(0, 10) }, { baseUrl: resolveServiceBase(5005) });
  }
}

@Injectable({ providedIn: 'root' })
export class ClientApi {
  private readonly basePath = '/clients';
  constructor(private readonly http: ApiHttpService) {}

  list(): Observable<Client[]> {
    return this.http.get(this.basePath, { baseUrl: resolveServiceBase(5008) });
  }
}

@Injectable({ providedIn: 'root' })
export class StaffApi {
  private readonly basePath = '/staff';
  constructor(private readonly http: ApiHttpService) {}

  list(): Observable<Staff[]> {
    return this.http.get(this.basePath, { baseUrl: resolveServiceBase(5007) });
  }
}

@Injectable({ providedIn: 'root' })
export class ReportsApi {
  constructor(private readonly http: ApiHttpService) {}

  loadBranchDaily(branchId: string) {
    return this.http.get(`/reports/branches/${branchId}/daily`, { baseUrl: resolveServiceBase(5010) });
  }
}

@Injectable({ providedIn: 'root' })
export class NotificationsApi {
  constructor(private readonly http: ApiHttpService) {}

  sendReceipt(target: string, templateName: string, variables: Record<string, string>) {
    return this.http.post('/notifications/send', { channel: 'email', target, templateName, variables }, { baseUrl: resolveServiceBase(5009) });
  }
}

@Injectable({ providedIn: 'root' })
export class SyncApi {
  constructor(private readonly http: ApiHttpService) {}

  registerDevice(deviceId: string) {
    return this.http.post('/sync/register', { deviceId, platform: 'web', publicKey: 'browser' }, { baseUrl: resolveServiceBase(5011) });
  }
}

function resolveServiceBase(port: number) {
  return `http://localhost:${port}`;
}
*** End of File