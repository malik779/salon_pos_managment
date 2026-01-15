export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
export interface Branch {
  id: string;
  name: string;
  timezone: string;
  address: string;
}

export interface Staff {
  id: string;
  fullName: string;
  role: string;
  defaultBranchId: string;
}

export interface Client {
  id: string;
  firstName: string;
  lastName: string;
  phone: string;
  email?: string;
}

export interface ServiceItem {
  id: string;
  name: string;
  durationMinutes: number;
  price: number;
}

export interface BookingSlot {
  id: string;
  branchId: string;
  staffId: string;
  clientId: string;
  startUtc: string;
  endUtc: string;
  status: string;
}

export interface Invoice {
  id: string;
  total: number;
  status: string;
  branchId: string;
  clientId: string;
}
export interface Notification {
  id: string;
  title: string;
  message: string;
  target: string;
  templateName: string;
  variables: Record<string, string>;
}
export interface DeviceSyncState {
  deviceId: string;
  platform: string;
  lastSyncedAtUtc: string;
  sequence: number;
}
export interface NotificationMessage {
  id: string;
  clientId: string;
  channel: string;
  templateCode: string;
  payload: string;
  status: string;
}
export interface Reports {
  id: string;
  branchId: string;
  date: string;
  totalSales: number;
  totalTax: number;
  totalDiscount: number;
  totalRevenue: number;
}
