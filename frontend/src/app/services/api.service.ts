import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthUser } from './auth.service';

export interface Category {
  id: number;
  slug: string;
  code: string;
  name: string;
  shortName: string;
  summary: string;
  equipmentCount: number;
  sortOrder: number;
  active: boolean;
}

export interface EquipmentSummary {
  id: number;
  slug: string;
  name: string;
  machineType: string;
  summary: string;
  categorySlug: string;
  categoryName: string;
  categoryId: number;
  sortOrder: number;
  active: boolean;
}

export interface EquipmentDetail {
  id: number;
  slug: string;
  name: string;
  machineType: string;
  summary: string;
  description: string;
  typicalUse: string;
  availabilityNote: string;
  active: boolean;
  category: Category;
}

export interface Reason {
  title: string;
  body: string;
}

export interface CompanyValue {
  title: string;
  body: string;
}

export interface Company {
  name: string;
  yearEstablished: number;
  businessType: string;
  industry: string;
  headOffice: string;
  operatingLocation: string;
  contactPerson: string;
  telephone: string;
  email: string;
  websiteNote: string;
  about: string;
  howWeWork: string;
  vision: string;
  mission: string;
  sectors: string[];
  reasons: Reason[];
  values: CompanyValue[];
}

export interface EnquiryPayload {
  fullName: string;
  company?: string;
  phone: string;
  email: string;
  categoryId?: number | null;
  machineType?: string;
  siteLocation?: string;
  requirement: string;
}

export interface Enquiry {
  id: number;
  fullName: string;
  company?: string | null;
  phone: string;
  email: string;
  categoryId?: number | null;
  categoryName?: string | null;
  machineType?: string | null;
  siteLocation?: string | null;
  requirement: string;
  status: string;
  createdAtUtc: string;
}

export interface SliderSlide {
  id: number;
  sortOrder: number;
  alt: string;
  url: string;
  createdAtUtc: string;
}

export interface HeaderLink {
  id: number;
  label: string;
  path: string;
  sortOrder: number;
  visible: boolean;
  isCta: boolean;
  createdAtUtc: string;
}

export interface CategoryPayload {
  name?: string;
  code?: string;
  slug?: string;
  shortName?: string;
  summary?: string;
  sortOrder?: number;
  active?: boolean;
}

export interface EquipmentPayload {
  categoryId: number;
  name: string;
  machineType?: string;
  slug?: string;
  summary?: string;
  description?: string;
  typicalUse?: string;
  availabilityNote?: string;
  sortOrder?: number;
  active?: boolean;
}

export interface HeaderLinkPayload {
  label?: string;
  path?: string;
  sortOrder?: number;
  visible?: boolean;
  isCta?: boolean;
}

export interface RegistrationPayload {
  fullName: string;
  email: string;
  phone: string;
  company?: string;
  city?: string;
  password: string;
  role?: string;
  userType?: string;
  equipmentType: string;
  machineType: string;
}

export interface Registration {
  id: number;
  fullName: string;
  email: string;
  phone: string;
  company?: string | null;
  city?: string | null;
  role: string;
  userType: string;
  equipmentType?: string | null;
  machineType?: string | null;
  createdAtUtc: string;
}

export interface UserMasterPayload {
  username: string;
  fullName: string;
  email: string;
  phone: string;
  password?: string;
  role?: string;
  userType?: string;
  active?: boolean;
}

export interface UserMaster {
  id: number;
  username: string;
  fullName: string;
  email: string;
  phone: string;
  role: string;
  userType: string;
  active: boolean;
  createdAtUtc: string;
}

export interface LoginPayload {
  username: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = '/api';

  getCompany(): Observable<Company> {
    return this.http.get<Company>(`${this.base}/company`);
  }

  getCategories(includeInactive = false): Observable<Category[]> {
    const params = includeInactive ? '?includeInactive=true' : '';
    return this.http.get<Category[]>(`${this.base}/categories${params}`);
  }

  getCategory(id: number) {
    return this.http.get<Category>(`${this.base}/categories/${id}`);
  }

  createCategory(payload: CategoryPayload) {
    return this.http.post<Category>(`${this.base}/categories`, payload);
  }

  updateCategory(id: number, payload: CategoryPayload) {
    return this.http.put<Category>(`${this.base}/categories/${id}`, payload);
  }

  deleteCategory(id: number) {
    return this.http.delete(`${this.base}/categories/${id}`);
  }

  getEquipment(category?: string | null, includeInactive = false): Observable<EquipmentSummary[]> {
    const query = new URLSearchParams();
    if (category) {
      query.set('category', category);
    }
    if (includeInactive) {
      query.set('includeInactive', 'true');
    }
    const params = query.toString() ? `?${query.toString()}` : '';
    return this.http.get<EquipmentSummary[]>(`${this.base}/equipment${params}`);
  }

  getEquipmentBySlug(slug: string): Observable<EquipmentDetail> {
    return this.http.get<EquipmentDetail>(`${this.base}/equipment/${encodeURIComponent(slug)}`);
  }

  getEquipmentItem(id: number) {
    return this.http.get<EquipmentDetail>(`${this.base}/equipment/item/${id}`);
  }

  createEquipmentItem(payload: EquipmentPayload) {
    return this.http.post<EquipmentDetail>(`${this.base}/equipment`, payload);
  }

  updateEquipmentItem(id: number, payload: EquipmentPayload) {
    return this.http.put<EquipmentDetail>(`${this.base}/equipment/${id}`, payload);
  }

  deleteEquipmentItem(id: number) {
    return this.http.delete(`${this.base}/equipment/${id}`);
  }

  createEnquiry(payload: EnquiryPayload) {
    return this.http.post(`${this.base}/enquiries`, payload);
  }

  getEnquiries(): Observable<Enquiry[]> {
    return this.http.get<Enquiry[]>(`${this.base}/enquiries`);
  }

  getSlides(): Observable<SliderSlide[]> {
    return this.http.get<SliderSlide[]>(`${this.base}/slides`);
  }

  getSlide(id: number): Observable<SliderSlide> {
    return this.http.get<SliderSlide>(`${this.base}/slides/${id}`);
  }

  uploadSlide(file: File, alt: string) {
    const body = new FormData();
    body.append('file', file);
    body.append('alt', alt);
    return this.http.post<SliderSlide>(`${this.base}/slides`, body);
  }

  updateSlide(id: number, payload: { alt?: string; sortOrder?: number }) {
    return this.http.put<SliderSlide>(`${this.base}/slides/${id}`, payload);
  }

  deleteSlide(id: number) {
    return this.http.delete(`${this.base}/slides/${id}`);
  }

  getHeaderLinks() {
    return this.http.get<HeaderLink[]>(`${this.base}/header-links`);
  }

  createHeaderLink(payload: HeaderLinkPayload) {
    return this.http.post<HeaderLink>(`${this.base}/header-links`, payload);
  }

  updateHeaderLink(id: number, payload: HeaderLinkPayload) {
    return this.http.put<HeaderLink>(`${this.base}/header-links/${id}`, payload);
  }

  deleteHeaderLink(id: number) {
    return this.http.delete(`${this.base}/header-links/${id}`);
  }

  createRegistration(payload: RegistrationPayload) {
    return this.http.post<Registration>(`${this.base}/registrations`, payload);
  }

  getRegistrations() {
    return this.http.get<Registration[]>(`${this.base}/registrations`);
  }

  getRegistration(id: number) {
    return this.http.get<Registration>(`${this.base}/registrations/${id}`);
  }

  getUserMasters() {
    return this.http.get<UserMaster[]>(`${this.base}/user-masters`);
  }

  getUserMaster(id: number) {
    return this.http.get<UserMaster>(`${this.base}/user-masters/${id}`);
  }

  createUserMaster(payload: UserMasterPayload) {
    return this.http.post<UserMaster>(`${this.base}/user-masters`, payload);
  }

  updateUserMaster(id: number, payload: UserMasterPayload) {
    return this.http.put<UserMaster>(`${this.base}/user-masters/${id}`, payload);
  }

  deleteUserMaster(id: number) {
    return this.http.delete(`${this.base}/user-masters/${id}`);
  }

  login(payload: LoginPayload) {
    return this.http.post<AuthUser>(`${this.base}/auth/login`, payload);
  }
}
