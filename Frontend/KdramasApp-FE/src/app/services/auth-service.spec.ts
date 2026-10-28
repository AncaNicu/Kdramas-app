import { TestBed } from '@angular/core/testing';
import { provideHttpClient, HttpErrorResponse } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

import { AuthService } from './auth-service';
import { RegisterRequest } from '../models/register-request';
import { APIResponse } from '../models/api-response';
import { environment } from '../../environments/environment';
import { LoginRequest } from '../models/login-request';
import { LoginResponse } from '../models/login-response';
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

    const expectedResponse: APIResponse = 
    {
      message: "Register successful"
    };

    //ACT + ASSERT
    service.register(request).subscribe(response => 
    {
      //it expects the response to be expectedResponse
      expect(response).toEqual(expectedResponse);
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
    req.flush(expectedResponse);
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
        //deliberately fails the test, if the success fallback runs
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

  it('should return a network error for register', () =>
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

  //==================LOGIN TESTS
  it('should send a POST request to log in a user', () =>
  {
    //ARRANGE
    const request: LoginRequest =
    {
      email: "john@test.com",
      password: "aaaaAAAA!"
    };

    const expectedResponse: LoginResponse = 
    {
      message: "Login successful",
      token: "test-token"
    };

    //ACT + ASSERT
    //it expects the response to be expectedResponse
    service.login(request).subscribe(response =>
    {
      expect(response).toEqual(expectedResponse);
    });

    //find the ongoing request
    //req is the test request
    const req = httpTestingController.expectOne
    ( 
      `${environment.apiUrl}/api/auth/login`
    );

    //check the method of the req
    expect(req.request.method).toBe('POST');
    //check the body of the req
    expect(req.request.body).toEqual(request);
    //simulate the BE response
    req.flush(expectedResponse);
  });

  it('should return a HTTP error when login fails', () =>
  {
    //ARRANGE
    const request: LoginRequest =
    {
      email: "john@test.com",
      password: "aaaaAAAA!"
    };

    //ACT + ASSERT
    service.login(request).subscribe
    ({
      next: () =>
      {
        //deliberately fails the test, if the success fallback runs
        expect.fail('Expect login to fail');
      },
      error: (error: HttpErrorResponse) =>
      {
        //check the status code of the error response
        expect(error.status).toBe(401);
        //check the message of the error response
        expect(error.error.message).toBe("Invalid email or password");
      }
    });

    //find the ongoing HTTP request
    const req = httpTestingController.expectOne
    (
      `${environment.apiUrl}/api/auth/login`
    );

    //check the method of the req
    expect(req.request.method).toBe('POST');
    //check the body of the req
    expect(req.request.body).toEqual(request);
    //simulate the BE response
    req.flush
    (
      //response body
      {
        message: "Invalid email or password"
      },
      {
        status: 401,
        statusText: "Unauthorized"
      }
    );
  });

  it('should return a network error for login', () =>
  {
    //ARRANGE
    const request: LoginRequest = 
    {
      email: "john@test.com",
      password: "aaaaAAAA!"
    };

    //ACT + ASSERT
    service.login(request).subscribe
    ({
      next: () =>
      {
        //deliberately feils the test, if the success fallback runs
        expect.fail('Expect login to fail');
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
      `${environment.apiUrl}/api/auth/login`
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

  //==================logout() TEST
  it('should remove the token from sessionStorage when logout', () =>
  {
    //ARRANGE
    const token = "fake-token";
    sessionStorage.setItem("token", token);

    //ACT
    service.logout();

    //ASSERT
    expect(sessionStorage.getItem("token")).toBeNull();
  });


  //==================saveUserData() TESTS
  it('should save the token to sessionStorage when saveData() is called', () =>
  {
    //ARRANGE
    const token = "fake-token";

    //ACT
    service.saveUserData(token);

    //ASSERT
    expect(sessionStorage.getItem("token")).toBe(token);
  });
});
