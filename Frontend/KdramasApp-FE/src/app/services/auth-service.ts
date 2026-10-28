//Marks the class as a service that Angular's 
//dependency injection system can provide.
import { Injectable } from '@angular/core';
//Sends the HTTP request.
import { HttpClient } from '@angular/common/http';
//Represents the asynchronous result of the HTTP request.
import { Observable } from 'rxjs';

import { RegisterRequest } from '../models/register-request';
import { environment } from '../../environments/environment';
import { APIResponse } from '../models/api-response';
import { LoginRequest } from '../models/login-request';
import { LoginResponse } from '../models/login-response';

@Injectable({
  providedIn: 'root'
})

export class AuthService 
{
    constructor(private http: HttpClient){}

    register(request: RegisterRequest): Observable<APIResponse>
    {
        return this.http.post<APIResponse>
            (`${environment.apiUrl}/api/auth/register`, request);
    }

    login(request: LoginRequest): Observable<LoginResponse>
    {
        return this.http.post<LoginResponse>
            (`${environment.apiUrl}/api/auth/login`, request);
    }

    logout(): void
    {
        sessionStorage.removeItem('token');
    }

    saveUserData(token: string): void
    {
        sessionStorage.setItem('token', token);
    }
}