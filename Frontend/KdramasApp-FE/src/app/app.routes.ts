import { Routes } from '@angular/router';
import { RegisterComponent } from './components/register-component/register-component';
import { LoginComponent } from './components/login-component/login-component';

export const routes: Routes = [
    { path: 'register', component: RegisterComponent },
    { path: 'login', component: LoginComponent }
];
