import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RegisterComponent } from './register-component';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { Observable, of } from 'rxjs';
import { AuthService } from '../../services/auth-service';
import { RegisterRequest } from '../../models/register-request';
import { APIResponse } from '../../models/api-response';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;

  let authServiceMock: 
  {
    register: ReturnType<typeof vi.fn>;
  }

  beforeEach(async () => 
  {
    authServiceMock =
    {
      register: vi.fn().mockReturnValue(
        of({ message: 'Registration successful' })
      )   
    };


    await TestBed.configureTestingModule
    ({
      imports: [RegisterComponent],
      providers:
      [
        {
          provide: AuthService,
          useValue: authServiceMock
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => 
  {
    expect(component).toBeTruthy();
  });

  //================TESTS FOR getUsernameErrorMessage()

  it('should not display error message for untouched username field', () =>
  {
    //ARRANGE
    //mark name field as untouched
    component.registerForm.controls.name.markAsUntouched();

    //ACT
    const message = component.getUsernameErrorMessage();

    //ASSERT
    expect(message).toBe("");
    expect(component.registerForm.controls.name.touched).toBe(false);
    expect(component.registerForm.controls.name.value).toBe("");
  });

  it('should display error message for touched and empty username field', () =>
  {
    //ARRANGE
    //mark name field as touched
    component.registerForm.controls.name.markAsTouched();
    //set the name value as ""
    component.registerForm.controls.name.setValue("");

    //ACT
    const message = component.getUsernameErrorMessage();

    //ASSERT
    expect(message).toBe("The username is required");
    expect(component.registerForm.controls.name.touched).toBe(true);
    expect(component.registerForm.controls.name.value).toBe("");
    expect(component.registerForm.valid).toBe(false);
  });

  it('should display error message for touched and too long username', () =>
  {
    //ARRANGE
    const name = 'a'.repeat(300);
    //mark name field as touched
    component.registerForm.controls.name.markAsTouched();
    //set the name value
    component.registerForm.controls.name.setValue(name);

    //ACT
    const message = component.getUsernameErrorMessage();

    //ASSERT
    expect(message).toBe("The username should not exceed 250 characters");
    expect(component.registerForm.controls.name.touched).toBe(true);
    expect(component.registerForm.controls.name.value).toBe(name);
    expect(component.registerForm.valid).toBe(false);
  });

  //================TESTS FOR getEmailErrorMessage()

  it('should display error message for touched and invalid email', () =>
  {
    //ARRANGE
    const email = "invalid_email"
    //mark name field as touched
    component.registerForm.controls.email.markAsTouched();
    //set the email value
    component.registerForm.controls.email.setValue(email);

    //ACT
    const message = component.getEmailErrorMessage();

    //ASSERT
    expect(message).toBe("The email format must be valid");
    expect(component.registerForm.controls.email.touched).toBe(true);
    expect(component.registerForm.controls.email.value).toBe(email);
    expect(component.registerForm.valid).toBe(false);
  });

  //================TESTS FOR getPasswordErrorMessage()

  it('should display error message for touched and invalid password', () =>
  {
    //ARRANGE
    const password = "invalid_password"
    //mark name field as touched
    component.registerForm.controls.password.markAsTouched();
    //set the password value
    component.registerForm.controls.password.setValue(password);

    //ACT
    const message = component.getPasswordErrorMessage();

    //ASSERT
    expect(message).toBe("The password should contain at least one lowercase letter, one uppercase letter and one special character from !@#$%^&*");
    expect(component.registerForm.controls.password.touched).toBe(true);
    expect(component.registerForm.controls.password.value).toBe(password);
    expect(component.registerForm.valid).toBe(false);
  });

  //================TESTS FOR getConfirmPasswordErrorMessage()

  it('should display error message for touched and confirm password that do not match the password', () =>
  {
    //ARRANGE
    const password = "password"
    const confirm_password = "confirm_password"
    //mark name field as touched
    component.registerForm.controls.confirmPassword.markAsTouched();
    //set the password value 
    component.registerForm.controls.password.setValue(password);
    //set the confirmPassword value 
    component.registerForm.controls.confirmPassword.setValue(confirm_password);

    //ACT
    const message = component.getConfirmPasswordErrorMessage();

    //ASSERT
    expect(message).toBe("The confirm password and password should match");
    expect(component.registerForm.controls.confirmPassword.touched).toBe(true);
    expect(component.registerForm.controls.confirmPassword.value).toBe(confirm_password);
    expect(component.registerForm.valid).toBe(false);
  });

  //================TEST FOR CALLING THE SERVICE
  it('should call the service to register', () => 
  {

  });

  //================TEST FOR register()
  it('should mark all controls as touched and not submit an invalid form', () =>
  {
    //ACT
    component.register();

    //ASSERT
    //it expects the form to be invalid
    expect(component.registerForm.invalid).toBe(true);

    //it expects the controls to be all touched
    expect(component.registerForm.controls.name.touched).toBe(true);
    expect(component.registerForm.controls.email.touched).toBe(true);
    expect(component.registerForm.controls.password.touched).toBe(true);
    expect(component.registerForm.controls.confirmPassword.touched).toBe(true);

    //it expects the service to not have been called
    expect(authServiceMock.register).not.toHaveBeenCalled();
  });

  it('should submit a valid form to AuthService', () =>
  {
    //ARRANGE
    const request: RegisterRequest = 
    {
      name: "John",
      email: "john@test.com",
      password: "aaaaAAAA!"
    };

    component.registerForm.setValue
    ({
      name: request.name,
      email: request.email,
      password: request.password,
      confirmPassword: request.password
    });

    //it expects the form to be valid
    expect(component.registerForm.valid).toBe(true);
    //ACT
    component.register();

    //ASSERT

    //it expects the service to have been called once
    expect(authServiceMock.register).toHaveBeenCalledOnce();
    //with values
    expect(authServiceMock.register).toHaveBeenCalledWith(request);
  });  
});
