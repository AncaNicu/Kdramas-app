import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth-service';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-register-component',
  styleUrl: './register-component.css',
  templateUrl: './register-component.html',
  standalone: true
})

export class RegisterComponent 
{
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
    else
    {
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
  }

  getEmailErrorMessage(): string
  {
    var control = this.registerForm.controls.email;

    if(!control.touched)
    {
      return "";
    }
    else
    {
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
  }

  getPasswordErrorMessage(): string
  {
    var control = this.registerForm.controls.password;

    if(!control.touched)
    {
      return "";
    }
    else
    {
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
  }

  getConfirmPasswordErrorMessage(): string
  {
    var control = this.registerForm.controls.confirmPassword;

    if(!control.touched)
    {
      return "";
    }
    else
    {
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
  }

  register(): void
  {
    if(this.registerForm.invalid)
    {
      this.registerForm.markAllAsTouched();
      return;
    }
  }
}
