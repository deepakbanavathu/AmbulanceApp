import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.js';
import { RegisterComponent } from './features/auth/register/register.js';
import { HomeComponent } from './features/dashboard/home/home.js';
import { RequestComponent } from './features/booking/request/request.js';
import { MapComponent } from './features/tracking/map/map.js';
import { DetailsComponent } from './features/profile/details/details.js';

export const routes: Routes = [
    { path: 'auth/login', component: LoginComponent},
    { path: 'auth/register', component: RegisterComponent},
    { path: 'dashboard', component: HomeComponent},
    { path: 'booking', component: RequestComponent},
    { path: 'tracking', component: MapComponent},
    { path: 'profile', component: DetailsComponent},
    { path: '', redirectTo: 'auth/login', pathMatch:'full'},
    { path: '**', redirectTo: 'auth/login'}
];
