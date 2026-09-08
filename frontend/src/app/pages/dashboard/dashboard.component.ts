import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiService, Category, Company, Enquiry, EquipmentSummary, Registration, UserMaster } from '../../services/api.service';
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
  private readonly route = inject(ActivatedRoute);

  readonly appName = 'SARA ROSE NIGERIA LIMITED';
  readonly user = this.auth.user;
  readonly isAdmin = this.auth.isAdmin;
  readonly isStaff = this.auth.isStaff;
  readonly today = new Date();
  company = signal<Company | null>(null);
  categories = signal<Category[]>([]);
  equipment = signal<EquipmentSummary[]>([]);
  enquiries = signal<Enquiry[]>([]);
  registrations = signal<Registration[]>([]);
  userMasters = signal<UserMaster[]>([]);
  error = signal<string | null>(null);
  denied = signal(false);

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
    this.denied.set(this.route.snapshot.queryParamMap.get('denied') === '1');
    const staff = this.isStaff();
    forkJoin({
      company: this.api.getCompany().pipe(catchError(() => of(null))),
      categories: this.api.getCategories(staff).pipe(catchError(() => of([] as Category[]))),
      equipment: this.api.getEquipment(null, staff).pipe(catchError(() => of([] as EquipmentSummary[]))),
      enquiries: staff ? this.api.getEnquiries().pipe(catchError(() => of([] as Enquiry[]))) : of([] as Enquiry[]),
      registrations: staff ? this.api.getRegistrations().pipe(catchError(() => of([] as Registration[]))) : of([] as Registration[]),
      userMasters: this.isAdmin() ? this.api.getUserMasters().pipe(catchError(() => of([] as UserMaster[]))) : of([] as UserMaster[])
    }).subscribe({
      next: ({ company, categories, equipment, enquiries, registrations, userMasters }) => {
        this.company.set(company);
        this.categories.set(categories);
        this.equipment.set(equipment);
        this.enquiries.set(enquiries);
        this.registrations.set(registrations);
        this.userMasters.set(userMasters);
        if (!company) {
          this.error.set('Company details could not be loaded. The dashboard still shows the application name.');
        }
      }
    });
  }
}
