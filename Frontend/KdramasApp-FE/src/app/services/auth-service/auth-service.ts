//Marks the class as a service that Angular's 
//dependency injection system can provide.
import { Injectable } from '@angular/core';
//Sends the HTTP request.
import { HttpClient } from '@angular/common/http';
//Represents the asynchronous result of the HTTP request.
import { Observable } from 'rxjs';

//for decoding the JWT token
import { jwtDecode } from 'jwt-decode';

import { RegisterRequest } from '../../models/register-request';
import { environment } from '../../../environments/environment';
import { APIResponse } from '../../models/api-response';
import { LoginRequest } from '../../models/login-request';
import { LoginResponse } from '../../models/login-response';

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

    isAdmin(): boolean
    {
        //get the token from sessionStorage
        const token = sessionStorage.getItem('token');

        //if there is no token => the user is not logged in
        //=> the user is not an admin
        if(!token)
        {
            return false;
        }
        try
        {
            //decode the JWT payload
            const payload = jwtDecode
            <{
                role?: string;
                exp?: number;
                [claim: string]: unknown;
            }>(token);

            //check if the token is expired or not existent
            //payload.exp is in milliseconds and exp in seconds
            if(!payload.exp || payload.exp * 1000 <= Date.now())
            {
                return false;
            }

            //get the role from the payload
            const roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
            const role = payload["role"] ?? payload[roleClaim];

            return role === "Admin";
        }
        //the token is malformed or can't be decoded
        catch
        {
            return false;
        }
    }
}