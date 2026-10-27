import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth-service';
import { ErrorHandlerService } from '../../services/error-handler-service';
import { HttpErrorResponse } from '@angular/common/http';
import { LoginRequest } from '../../models/login-request';
import { ToastService } from '../../services/toast-service';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-login-component',
  styleUrl: './login-component.css',
  templateUrl: './login-component.html',
})
export class LoginComponent 
{
  constructor(
    private authService: AuthService,
    private errorHandlerService: ErrorHandlerService,
    private toastService: ToastService
  ) {}

  loginForm = new FormGroup
  (
    {
      email: new FormControl('', 
        [Validators.required, Validators.email]),
      password: new FormControl('',
        [Validators.required]
      )
    }
  )

  //shows if a request is in process
  isSubmitting = false;

  getEmailErrorMessage(): string
  {
    var control = this.loginForm.controls.email;
    
    if(!control.touched)
    {
      return "";
    }
    if(control.hasError('required'))
    {
      return "The email is required";
    }
    if(control.hasError('email'))
    {
      return "The email format must be valid";
    }
    return "";
  }

  getPasswordErrorMessage(): string
  {
    var control = this.loginForm.controls.password;

    if(!control.touched)
    {
      return "";
    }
    if(control.hasError('required'))
    {
      return "The password is required";
    }
    return "";
  }

  //returns the error message to be displayed in the toast
  private getLoginErrorMessage(error: HttpErrorResponse): string 
  {
    // No usable HTTP response was received.
    if (error.status === 0) {
      return 'Could not connect to the server. Please check your connection and try again.';
    }

    // The backend rejected the request because of invalid input.
    if (error.status === 400) {
      return this.errorHandlerService.getBackendErrorMessage(error)
        ?? 'Please check your login details.';
    }

    // The email or password were incorrect
    if (error.status === 401) {
      return this.errorHandlerService.getBackendErrorMessage(error)
        ?? 'Invalid email or password.';
    }

    // An unexpected server-side error occurred.
    if (error.status >= 500) {
      return this.errorHandlerService.getBackendErrorMessage(error)
        ?? 'An unexpected server error occurred. Please try again later.';
    }

    // Handle other HTTP errors.
    return this.errorHandlerService.getBackendErrorMessage(error)
      ?? 'Login failed. Please try again later.';
  }

  login(): void
  {
    //check if the form is invalid or a request is currently in process
    if(this.loginForm.invalid || this.isSubmitting)
    {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const controls = this.loginForm.controls;

    const request: LoginRequest = 
    {
      email: controls.email.value ?? '',
      password: controls.password.value ?? ''
    };

    //call the service
    this.authService.login(request).subscribe
    ({
      next: (response) =>
      {
        this.isSubmitting = false;

        //check if the token is not null or empty
        if (response.token === null || response.token.trim().length === 0)
        {
            let message: string = 'Login failed: the server did not return a valid token.';
            this.toastService.showErrorToast(message);
            return;
        }

        this.authService.saveUserData(response.token);

        this.toastService.showSuccessToast(response.message);

        this.loginForm.reset();
      },
      error: (error: HttpErrorResponse) =>
      {
        this.isSubmitting = false;
        this.toastService.showErrorToast(this.getLoginErrorMessage(error));

        this.loginForm.reset();
      }
    });
  }
}
