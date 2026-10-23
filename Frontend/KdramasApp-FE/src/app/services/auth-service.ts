import { Service } from '@angular/core';
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
}
