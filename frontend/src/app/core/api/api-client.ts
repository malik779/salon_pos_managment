import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiHttpService } from '../services/api-http.service';
import { BookingSlot, Branch, Client, Invoice, Notification, Reports, ServiceItem, Staff,PaginatedResponse } from '../models/domain.models';

@Injectable({ providedIn: 'root' })
export class IdentityApi {
  private readonly baseUrl = '/api/auth';

  constructor(private readonly http: ApiHttpService) {}

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/token`, { email, password });
  }

  register(email: string, password: string, fullName: string, branchId: string, role: string): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.baseUrl}/register`, { email, password, fullName, branchId, role });
  }

  forgotPassword(email: string): Observable<ForgotPasswordResponse> {
    return this.http.post<ForgotPasswordResponse>(`${this.baseUrl}/forgot-password`, { email });
  }

  resetPassword(email: string, resetToken: string, newPassword: string): Observable<ResetPasswordResponse> {
    return this.http.post<ResetPasswordResponse>(`${this.baseUrl}/reset-password`, { email, resetToken, newPassword });
  }

  refreshToken(refreshToken: string): Observable<RefreshTokenResponse> {
    return this.http.post<RefreshTokenResponse>(`${this.baseUrl}/refresh`, { refreshToken });
  }
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
  branchId: string;
}

export interface RegisterResponse {
  userId: string;
  email: string;
  fullName: string;
  role: string;
  branchId: string;
}

export interface ForgotPasswordResponse {
  success: boolean;
  resetToken?: string;
}

export interface ResetPasswordResponse {
  success: boolean;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}
/**
 * Core entity API interface for CRUD operations
 * Provides both paginated and non-paginated query methods
 * Clear naming convention: paginatedList for paginated queries, getAll for all records
 */
export interface EntityApi<T, TId = string> {
  /**
   * Fetches paginated list of entities
   * @param page Page number (1-based)
   * @param pageSize Number of items per page
   * @param searchTerm Optional search term to filter results
   * @returns Observable of paginated response
   */
  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<T>>;
  
  /**
   * Fetches all records without pagination
   * Useful for dropdowns, selects, and other UI components
   * @param searchTerm Optional search term to filter results
   * @returns Observable of array of entities
   */
  getAll(searchTerm?: string): Observable<T[]>;
  
  getById(id: TId): Observable<T>;
  create(item: T): Observable<T>;
  update(id: TId, item: T): Observable<T>;
  delete(id: TId): Observable<void>;
}
@Injectable({ providedIn: 'root' })
export class BranchApi implements EntityApi<Branch> {
  private readonly basePath = '/api/branches';
  constructor(private readonly http: ApiHttpService) {}

  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<Branch>> {
      return this.http.get<PaginatedResponse<Branch>>(this.basePath, { params: { page, pageSize, searchTerm } });
    }

  getAll(searchTerm?: string): Observable<Branch[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<Branch[]>(`${this.basePath}/all`, params ? { params } : {});
  }

  getById(id: string): Observable<Branch> {
    return this.http.get(`${this.basePath}/${id}`);
  }
  create(payload: Partial<Branch>): Observable<Branch> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string, payload: Partial<Branch>): Observable<Branch> {
    return this.http.post(`${this.basePath}/${id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class CatalogApi implements EntityApi<ServiceItem> {
  private readonly basePath = '/api/catalog';
  constructor(private readonly http: ApiHttpService) {}

  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<ServiceItem>> {
    return this.http.get<PaginatedResponse<ServiceItem>>(`${this.basePath}/services`, { params: { page, pageSize, searchTerm } });
  }

  getAll(searchTerm?: string): Observable<ServiceItem[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<ServiceItem[]>(`${this.basePath}/services/all`, params ? { params } : {});
  }
  getById(id: string): Observable<ServiceItem> {
    return this.http.get(`${this.basePath}/${id}`);
  }
  create(payload: Partial<ServiceItem>): Observable<ServiceItem> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string, payload: Partial<ServiceItem>): Observable<ServiceItem> {
    return this.http.post(`${this.basePath}/${payload.id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class BookingApi implements EntityApi<BookingSlot> {
  private readonly basePath = '/api/booking';
  constructor(private readonly http: ApiHttpService) {}
  
  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<BookingSlot>> {
    return this.http.get<PaginatedResponse<BookingSlot>>(`${this.basePath}/bookings`, { params: { page, pageSize, searchTerm } });
  }

  getAll(searchTerm?: string): Observable<BookingSlot[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<BookingSlot[]>(`${this.basePath}/bookings/all`, params ? { params } : {});
  }
  getById(id: string): Observable<BookingSlot> {
    throw new Error('Method not implemented.');
  }

  getCalendar(branchId: string, from: string, to: string): Observable<BookingSlot[]> {
    return this.http.get(`${this.basePath}/calendar?branchId=${branchId}&from=${from}&to=${to}`);
  }

  create(payload: Partial<BookingSlot>): Observable<BookingSlot> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string, payload: Partial<BookingSlot>): Observable<BookingSlot> {
    return this.http.post(`${this.basePath}/${id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class PosApi implements EntityApi<Invoice> {
  private readonly basePath = '/api/pos';
  constructor(private readonly http: ApiHttpService) {}

  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<Invoice>> {
    return this.http.get<PaginatedResponse<Invoice>>(this.basePath, { params: { page, pageSize, searchTerm } });
  }

  getAll(searchTerm?: string): Observable<Invoice[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<Invoice[]>(`${this.basePath}/all`, params ? { params } : {});
  }

  getById(id: string): Observable<Invoice> {
    return this.http.get(`${this.basePath}/${id}`);
  }
  create(payload: Partial<Invoice>): Observable<Invoice> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string, payload: Partial<Invoice>): Observable<Invoice> {
    return this.http.post(`${this.basePath}/${id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }

  closeDay(branchId: string) {
    return this.http.post(`${this.basePath}/closereads`, { branchId, businessDate: new Date().toISOString().substring(0, 10) });
  }
}

@Injectable({ providedIn: 'root' })
export class ClientApi implements EntityApi<Client> {
  private readonly basePath = '/api/client';
  constructor(private readonly http: ApiHttpService) {}

  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<Client>> {
    return this.http.get<PaginatedResponse<Client>>(this.basePath, { params: { page, pageSize, searchTerm } });
  }

  getAll(searchTerm?: string): Observable<Client[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<Client[]>(`${this.basePath}/all`, params ? { params } : {});
  }
  getById(id: string): Observable<Client> {
    return this.http.get(`${this.basePath}/${id}`);
  }
  create(payload: Partial<Client>): Observable<Client> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string, payload: Partial<Client>): Observable<Client> {
    return this.http.post(`${this.basePath}/${id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class StaffApi implements EntityApi<Staff> {
  private readonly basePath = '/api/staff';
  constructor(private readonly http: ApiHttpService) {}

  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<Staff>> {
    return this.http.get<PaginatedResponse<Staff>>(this.basePath, { params: { page, pageSize, searchTerm } });
  }

  getAll(searchTerm?: string): Observable<Staff[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<Staff[]>(`${this.basePath}/all`, params ? { params } : {});
  }
  getById(id: string): Observable<Staff> {
    return this.http.get(`${this.basePath}/${id}`);
  }
  create(payload: Partial<Staff>): Observable<Staff> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string, payload: Partial<Staff>): Observable<Staff> {
    return this.http.post(`${this.basePath}/${id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }
  }

@Injectable({ providedIn: 'root' })
export class ReportsApi implements EntityApi<Reports> {
  private readonly basePath = '/api/reports';
  constructor(private readonly http: ApiHttpService) {}
  getById(id: string): Observable<Reports> {
    return this.http.get(`${this.basePath}/${id}`);
  }

  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<Reports>> {
    return this.http.get<PaginatedResponse<Reports>>(this.basePath, { params: { page, pageSize, searchTerm } });
  }

  getAll(searchTerm?: string): Observable<Reports[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<Reports[]>(`${this.basePath}/all`, params ? { params } : {});
  }
  create(payload: Partial<Reports>): Observable<Reports> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string, payload: Partial<Reports>): Observable<Reports> {
    return this.http.post(`${this.basePath}/${id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }
}
@Injectable({ providedIn: 'root' })
export class NotificationsApi implements EntityApi<Notification> {
  private readonly basePath = '/api/notifications';
  constructor(private readonly http: ApiHttpService) {}

  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<Notification>> {
    return this.http.get<PaginatedResponse<Notification>>(this.basePath, { params: { page, pageSize, searchTerm } });
  }

  getAll(searchTerm?: string): Observable<Notification[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<Notification[]>(`${this.basePath}/all`, params ? { params } : {});
  }
  getById(id: string): Observable<Notification> {
    return this.http.get(`${this.basePath}/${id}`);
  }
  create(payload: Partial<Notification>): Observable<Notification> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string,  payload: Partial<Notification>): Observable<Notification> {
    return this.http.post(`${this.basePath}/${id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }
  }

@Injectable({ providedIn: 'root' })
export class SyncApi implements EntityApi<any> {
  private readonly basePath = '/api/sync';
  constructor(private readonly http: ApiHttpService) {}
  
  paginatedList(page: number, pageSize: number, searchTerm: string): Observable<PaginatedResponse<any>> {
    return this.http.get<PaginatedResponse<any>>(this.basePath, { params: { page, pageSize, searchTerm } });
  }

  getAll(searchTerm?: string): Observable<any[]> {
    const params = searchTerm ? { searchTerm } : undefined;
    return this.http.get<any[]>(`${this.basePath}/all`, params ? { params } : {});
  }
  getById(id: string): Observable<any> {
    return this.http.get(`${this.basePath}/${id}`);
  }
  create(payload: Partial<any>): Observable<any> {
    return this.http.post(`${this.basePath}/Create`, payload);
  }
  update(id: string, payload: Partial<any>): Observable<any> {
    return this.http.post(`${this.basePath}/${id}`, payload);
  }
  delete(id: string): Observable<void> {
    return this.http.delete(`${this.basePath}/${id}`);
  }
}