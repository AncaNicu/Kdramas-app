import { TestBed } from '@angular/core/testing';
import { provideHttpClient, HttpErrorResponse } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

import { AuthService } from './auth-service';
import { RegisterRequest } from '../models/register-request';
import { APIResponse } from '../models/api-response';
import { environment } from '../../environments/environment';
import { errorContext } from 'rxjs/internal/util/errorContext';

describe('AuthService', () => {
  let service: AuthService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule
    ({
      providers: 
      [
        AuthService, 
        provideHttpClient(), //provide HTTP client
        provideHttpClientTesting() //replace the real BE with a testing BE
      ]
    });

    service = TestBed.inject(AuthService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => 
  {
    //ensure there is no HTTP pending request
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  //==================REGISTER TESTS
  it('should send a POST request to register a user', () => 
  {
    //ARRANGE
    const request: RegisterRequest = 
    {
      name: "John",
      email: "john@test.com",
      password: "aaaaAAAA!"
    };

    const expectedRespponse: APIResponse = 
    {
      message: "Register successful"
    };

    //ACT + ASSERT
    service.register(request).subscribe(response => 
    {
      //it expects the response to be expectedRespponse
      expect(response).toEqual(expectedRespponse);
    });

    //find the outgoing HTTP request
    //req is the test request
    const req = httpTestingController.expectOne
    (
      `${environment.apiUrl}/api/auth/register`
    );

    //check the method of the req
    expect(req.request.method).toBe('POST');
    //check the body of the req
    expect(req.request.body).toEqual(request);
    //simulate the BE response
    req.flush(expectedRespponse);
  });

  it('should return a HTTP error when registration fails', () =>
  {
    //ARRANGE
    const request: RegisterRequest = 
    {
      name: "John",
      email: "john@test.com",
      password: "aaaaAAAA!"
    };
    
    //ACT + ASSERT
    service.register(request).subscribe
    ({
      next: () =>
      {
        //deliberately feils the test, if the success fallback runs
        expect.fail('Expect registration to fail');
      },
      error: (error: HttpErrorResponse) =>
      {
        //check the status code of the error response
        expect(error.status).toBe(409);
        //check the message of the error response
        expect(error.error.message).toBe
          ("There is already an account with the same email");
      }
    });

    //find the outgoing HTTP request
    const req = httpTestingController.expectOne
    (
      `${environment.apiUrl}/api/auth/register`
    );

    //check the method of the req
    expect(req.request.method).toBe('POST');
    //check the body of the req
    expect(req.request.body).toEqual(request);
    //simulate the BE response
    req.flush
    (
      //response body
      { message: "There is already an account with the same email" },
      //response status
      {
        status: 409,
        statusText: "Conflict"
      }
    );
  });

  it('should return a network error', () =>
  {
    //ARRANGE
    const request: RegisterRequest = 
    {
      name: "John",
      email: "john@test.com",
      password: "aaaaAAAA!"
    };

    //ACT + ASSERT
    service.register(request).subscribe
    ({
      next: () =>
      {
        //deliberately feils the test, if the success fallback runs
        expect.fail('Expect registration to fail');
      },
      error: (error: HttpErrorResponse) =>
      {
        //check the status code of the error response
        expect(error.status).toBe(500);
        //check the message of the error response
        expect(error.error.message).toBe
          ("An unexpected error has occurred");
      }
    });

    //find the outgoing HTTP request
    const req = httpTestingController.expectOne
    (
      `${environment.apiUrl}/api/auth/register`
    );

    //check the method of the req
    expect(req.request.method).toBe('POST');
    //check the body of the req
    expect(req.request.body).toEqual(request);
    //simulate the BE response
    req.flush
    (
      //response body
      { message: "An unexpected error has occurred" },
      //response status
      {
        status: 500,
        statusText: "InternalServferError"
      }
    );    
  });

});
