import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth-service';
import { RegisterRequest } from '../../models/register-request';
import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandlerService } from '../../services/error-handler-service';
import { ToastService } from '../../services/toast-service';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-register-component',
  styleUrl: './register-component.css',
  templateUrl: './register-component.html',
})

export class RegisterComponent 
{
  constructor(
    private authService: AuthService,
    private errorHandlerService: ErrorHandlerService,
    private toastService: ToastService
  ){}
  
  registerForm = new FormGroup
  (
    {
      name: new FormControl('', 
        [Validators.required, Validators.maxLength(250)]),
      email: new FormControl('', 
        [Validators.required, Validators.email, Validators.maxLength(320)]),
      password: new FormControl('', 
        [Validators.required, Validators.maxLength(250), Validators.minLength(8), 
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).*$/)]),
      confirmPassword: new FormControl('', 
        [Validators.required])
    },
    {
      validators: this.passwordsMatch
    }
  );

  //shows if a request is in process
  isSubmitting = false;

  passwordsMatch(control: AbstractControl) 
  {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    //pasword and confirm password match
    if (password === confirmPassword) 
    {
      return null;
    }

    //at least one of them is empty
    if (!password || !confirmPassword) 
    {
      return null;
    }

    return { passwordMismatch: true };
  }

  getUsernameErrorMessage(): string
  {
    var control = this.registerForm.controls.name;

    if(!control.touched)
    {
      return "";
    }
    if(control.hasError('required'))
    {
      return "The username is required";
    }
    if(control.hasError('maxlength'))
    {
      return "The username should not exceed 250 characters";
    }
    return "";
  }

  getEmailErrorMessage(): string
  {
    var control = this.registerForm.controls.email;

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
    if(control.hasError('maxlength'))
    {
      return "The email should not exceed 320 characters";
    }
    return "";
  }

  getPasswordErrorMessage(): string
  {
    var control = this.registerForm.controls.password;

    if(!control.touched)
    {
      return "";
    }
    if(control.hasError('required'))
    {
      return "The password is required";
    }
    if(control.hasError('maxlength'))
    {
      return "The password should not exceed 250 characters";
    }
    if(control.hasError('minlength'))
    {
      return "The password should be at least 8 characters";
    }
    if(control.hasError('pattern'))
    {
      return "The password should contain at least one lowercase letter, one uppercase letter and one special character from !@#$%^&*";
    }
    return "";
  }

  getConfirmPasswordErrorMessage(): string
  {
    var control = this.registerForm.controls.confirmPassword;

    if(!control.touched)
    {
      return "";
    }
    if(control.hasError('required'))
    {
      return "The confirm password is required"
    }

    if(this.registerForm.hasError('passwordMismatch'))
    {
      return "The confirm password and password should match"
    }

    return "";
  }

  //returns the error message to be displayed in the toast
  private getRegistrationErrorMessage(error: HttpErrorResponse): string 
  {
    // No usable HTTP response was received.
    if (error.status === 0) {
      return 'Could not connect to the server. Please check your connection and try again.';
    }

    // The backend rejected the request because of invalid input.
    if (error.status === 400) {
      return this.errorHandlerService.getBackendErrorMessage(error)
        ?? 'Please check your registration details.';
    }

    // The email already exists.
    if (error.status === 409) {
      return this.errorHandlerService.getBackendErrorMessage(error)
        ?? 'There is already an account with the same email.';
    }

    // An unexpected server-side error occurred.
    if (error.status >= 500) {
      return this.errorHandlerService.getBackendErrorMessage(error)
        ?? 'An unexpected server error occurred. Please try again later.';
    }

    // Handle other HTTP errors.
    return this.errorHandlerService.getBackendErrorMessage(error)
      ?? 'Registration failed. Please try again later.';
  }

  register(): void
  {
    if(this.registerForm.invalid || this.isSubmitting)
    {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const control = this.registerForm.controls;

    const request: RegisterRequest = 
    {
      name: control.name.value ?? '',
      email: control.email.value ?? '',
      password: control.password.value ?? ''
    };

    this.authService.register(request).subscribe
    ({
        next: (response) =>
        {
          this.isSubmitting = false;
          this.toastService.showSuccessToast(response.message);
          this.registerForm.reset();
        },
        error: (error: HttpErrorResponse) => 
        {
          this.isSubmitting = false;
          this.toastService.showErrorToast(this.getRegistrationErrorMessage(error));
          this.registerForm.reset();
        }
    });
  }
}
