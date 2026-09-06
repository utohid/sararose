import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { AboutComponent } from './pages/about/about.component';
import { EquipmentComponent } from './pages/equipment/equipment.component';
import { EquipmentDetailComponent } from './pages/equipment-detail/equipment-detail.component';
import { WhyUsComponent } from './pages/why-us/why-us.component';
import { ValuesComponent } from './pages/values/values.component';
import { ContactComponent } from './pages/contact/contact.component';
import { LoginComponent } from './pages/login/login.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { AdminShellComponent } from './pages/admin-shell/admin-shell.component';
import { SliderManagerComponent } from './pages/slider-manager/slider-manager.component';
import { SliderAddComponent } from './pages/slider-add/slider-add.component';
import { SliderViewComponent } from './pages/slider-view/slider-view.component';
import { HeaderManagerComponent } from './pages/header-manager/header-manager.component';
import { HeaderAddComponent } from './pages/header-add/header-add.component';
import { HeaderViewComponent } from './pages/header-view/header-view.component';
import { RegisterComponent } from './pages/register/register.component';
import { RegistrationsAdminComponent } from './pages/registrations-admin/registrations-admin.component';
import { CatalogMasterComponent } from './pages/catalog-master/catalog-master.component';
import { EquipmentMasterAddComponent } from './pages/equipment-master-add/equipment-master-add.component';
import { EquipmentMasterViewComponent } from './pages/equipment-master-view/equipment-master-view.component';
import { MachineMasterAddComponent } from './pages/machine-master-add/machine-master-add.component';
import { MachineMasterViewComponent } from './pages/machine-master-view/machine-master-view.component';
import { authGuard } from './auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent, title: 'SARA ROSE Nigeria Limited' },
  { path: 'about', component: AboutComponent, title: 'About — SARA ROSE' },
  { path: 'equipment', component: EquipmentComponent, title: 'Equipment portfolio — SARA ROSE' },
  { path: 'equipment/:slug', component: EquipmentDetailComponent, title: 'Equipment — SARA ROSE' },
  { path: 'why-sara-rose', component: WhyUsComponent, title: 'Why SARA ROSE' },
  { path: 'vision-values', component: ValuesComponent, title: 'Vision & values — SARA ROSE' },
  { path: 'contact', component: ContactComponent, title: 'Enquire — SARA ROSE' },
  { path: 'login', component: LoginComponent, title: 'Login — SARA ROSE' },
  { path: 'register', component: RegisterComponent, title: 'Registration — SARA ROSE' },
  {
    path: 'dashboard',
    component: AdminShellComponent,
    title: 'Dashboard — SARA ROSE',
    canActivate: [authGuard],
    children: [
      { path: '', component: DashboardComponent },
      { path: 'slider/add', component: SliderAddComponent, title: 'Add slide — SARA ROSE' },
      { path: 'slider/view', component: SliderViewComponent, title: 'View slides — SARA ROSE' },
      { path: 'slider', component: SliderManagerComponent, title: 'Home slider — SARA ROSE' },
      { path: 'header/add', component: HeaderAddComponent, title: 'Add header link — SARA ROSE' },
      { path: 'header/view', component: HeaderViewComponent, title: 'View header links — SARA ROSE' },
      { path: 'header', component: HeaderManagerComponent, title: 'Header links — SARA ROSE' },
      { path: 'masters/equipment/add', component: EquipmentMasterAddComponent, title: 'Add equipment group — SARA ROSE' },
      { path: 'masters/equipment', component: EquipmentMasterViewComponent, title: 'Equipment master — SARA ROSE' },
      { path: 'masters/machines/add', component: MachineMasterAddComponent, title: 'Add machine type — SARA ROSE' },
      { path: 'masters/machines', component: MachineMasterViewComponent, title: 'Machine type master — SARA ROSE' },
      { path: 'masters', component: CatalogMasterComponent, title: 'Catalogue masters — SARA ROSE' },
      { path: 'registrations', component: RegistrationsAdminComponent, title: 'Registrations — SARA ROSE' }
    ]
  },
  { path: '**', redirectTo: '' }
];
