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
