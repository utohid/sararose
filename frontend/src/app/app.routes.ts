import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { AboutComponent } from './pages/about/about.component';
import { EquipmentComponent } from './pages/equipment/equipment.component';
import { EquipmentDetailComponent } from './pages/equipment-detail/equipment-detail.component';
import { WhyUsComponent } from './pages/why-us/why-us.component';
import { ValuesComponent } from './pages/values/values.component';
import { ContactComponent } from './pages/contact/contact.component';
import { LoginComponent } from './pages/login/login.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, title: 'SARA ROSE Nigeria Limited' },
  { path: 'about', component: AboutComponent, title: 'About — SARA ROSE' },
  { path: 'equipment', component: EquipmentComponent, title: 'Equipment portfolio — SARA ROSE' },
  { path: 'equipment/:slug', component: EquipmentDetailComponent, title: 'Equipment — SARA ROSE' },
  { path: 'why-sara-rose', component: WhyUsComponent, title: 'Why SARA ROSE' },
  { path: 'vision-values', component: ValuesComponent, title: 'Vision & values — SARA ROSE' },
  { path: 'contact', component: ContactComponent, title: 'Enquire — SARA ROSE' },
  { path: 'login', component: LoginComponent, title: 'Login — SARA ROSE' },
  { path: '**', redirectTo: '' }
];
