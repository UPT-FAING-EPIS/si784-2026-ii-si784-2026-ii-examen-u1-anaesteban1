export type ItemType = 'Lost' | 'Found';
export type ItemStatus = 'Reported' | 'InCustody' | 'Claimed' | 'Returned';

export interface Item {
  id: string;
  name: string;
  description: string;
  category: string;
  location: string;
  faculty: string;
  reportDate: string;
  itemType: ItemType;
  status: ItemStatus;
  photoUrl?: string;
  characteristics: string;
  claims: Claim[];
  returnRecord?: ReturnRecord | null;
}

export interface Claim {
  id: string;
  itemId: string;
  claimantName: string;
  claimantEmail: string;
  ownershipEvidence: string;
  requestedAt: string;
  isVerified: boolean;
  verifiedAt?: string | null;
}

export interface ReturnRecord {
  id: string;
  itemId: string;
  claimId: string;
  responsiblePerson: string;
  returnDate: string;
  ownerConfirmation: boolean;
  notes?: string;
}

export interface ItemPayload {
  name: string;
  description: string;
  category: string;
  location: string;
  faculty: string;
  reportDate: string;
  photoUrl?: string;
  characteristics: string;
}

const apiUrl = import.meta.env.VITE_API_URL ?? 'http://localhost:5195';

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${apiUrl}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Error HTTP ${response.status}`);
  }

  return response.json() as Promise<T>;
}

export async function getItems(filters: Record<string, string>): Promise<Item[]> {
  const params = new URLSearchParams();
  Object.entries(filters).forEach(([key, value]) => {
    if (value) params.set(key, value);
  });
  const query = params.toString();
  return request<Item[]>(`/items${query ? `?${query}` : ''}`);
}

export async function getItem(id: string): Promise<Item> {
  return request<Item>(`/items/${id}`);
}

export async function createLostItem(payload: ItemPayload): Promise<Item> {
  return request<Item>('/lost-items', { method: 'POST', body: JSON.stringify(payload) });
}

export async function createFoundItem(payload: ItemPayload): Promise<Item> {
  return request<Item>('/found-items', { method: 'POST', body: JSON.stringify(payload) });
}

export async function claimItem(id: string, payload: {
  claimantName: string;
  claimantEmail: string;
  ownershipEvidence: string;
}): Promise<Claim> {
  return request<Claim>(`/items/${id}/claim`, { method: 'POST', body: JSON.stringify(payload) });
}

export async function verifyOwner(id: string, claimId: string): Promise<Claim> {
  return request<Claim>(`/items/${id}/verify-owner`, {
    method: 'POST',
    body: JSON.stringify({ claimId })
  });
}

export async function returnItem(id: string, payload: {
  claimId: string;
  responsiblePerson: string;
  ownerConfirmation: boolean;
  notes?: string;
}): Promise<ReturnRecord> {
  return request<ReturnRecord>(`/items/${id}/return`, {
    method: 'POST',
    body: JSON.stringify({ ...payload, returnDate: new Date().toISOString() })
  });
}
