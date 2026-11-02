import { Routes } from '@angular/router';
import { RegisterComponent } from './components/register-component/register-component';
import { LoginComponent } from './components/login-component/login-component';
import { ActorListComponent } from './components/actor-list-component/actor-list-component';
import { ActorDetailsComponent } from './components/actor-details-component/actor-details-component';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'register', component: RegisterComponent },
    { path: 'login', component: LoginComponent },
    { path: 'actors/:id', component: ActorDetailsComponent },
    { path: 'actors', component: ActorListComponent },
    { path: '**', redirectTo: 'login' }
];
