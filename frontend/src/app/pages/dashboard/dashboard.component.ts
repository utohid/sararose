import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiService, Category, Company, Enquiry, EquipmentSummary, Registration } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly auth = inject(AuthService);

  readonly appName = 'SARA ROSE NIGERIA LIMITED';
  readonly user = this.auth.user;
  readonly today = new Date();
  company = signal<Company | null>(null);
  categories = signal<Category[]>([]);
  equipment = signal<EquipmentSummary[]>([]);
  enquiries = signal<Enquiry[]>([]);
  registrations = signal<Registration[]>([]);
  error = signal<string | null>(null);

  greeting(): string {
    const hour = this.today.getHours();
    if (hour < 12) {
      return 'Good morning';
    }
    if (hour < 17) {
      return 'Good afternoon';
    }
    return 'Good evening';
  }

  initials(): string {
    const name = this.user()?.fullName || 'SR';
    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part[0])
      .join('')
      .toUpperCase();
  }

  ngOnInit(): void {
    forkJoin({
      company: this.api.getCompany().pipe(catchError(() => of(null))),
      categories: this.api.getCategories().pipe(catchError(() => of([] as Category[]))),
      equipment: this.api.getEquipment().pipe(catchError(() => of([] as EquipmentSummary[]))),
      enquiries: this.api.getEnquiries().pipe(catchError(() => of([] as Enquiry[]))),
      registrations: this.api.getRegistrations().pipe(catchError(() => of([] as Registration[])))
    }).subscribe({
      next: ({ company, categories, equipment, enquiries, registrations }) => {
        this.company.set(company);
        this.categories.set(categories);
        this.equipment.set(equipment);
        this.enquiries.set(enquiries);
        this.registrations.set(registrations);
        if (!company) {
          this.error.set('Company details could not be loaded. The dashboard still shows the application name.');
        }
      }
    });
  }
}
