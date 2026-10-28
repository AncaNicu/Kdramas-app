import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login-component';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import { of } from 'rxjs';
import { AuthService } from '../../services/auth-service';
import { LoginRequest } from '../../models/login-request';
import { LoginResponse } from '../../models/login-response';

describe('LoginComponent', () => 
{
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  let authServiceMock:
  {
    login: ReturnType<typeof vi.fn>;
  }

  beforeEach(async () => 
  {
    authServiceMock =
    {
      login: vi.fn().mockReturnValue
      (
        of({ message: 'Login successful', token: 'test-token' })
      )
    }

    await TestBed.configureTestingModule
    ({
      imports: [LoginComponent],
      providers:
      [
        {
          provide: AuthService,
          useValue: authServiceMock
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => 
  {
    expect(component).toBeTruthy();
  });

  //================TESTS FOR getEmailErrorMessage()
  it('should not display error message for untouched email field', () =>
  {
    //ARRANGE
    //mark email field as untouched
    component.loginForm.controls.email.markAsUntouched();

    //ACT
    const message = component.getEmailErrorMessage();

    //ASSERT
    expect(message).toBe("");
    expect(component.loginForm.controls.email.touched).toBe(false);
    expect(component.loginForm.controls.email.value).toBe("");
    expect(component.loginForm.valid).toBe(false);
  });

  it('should display error message for touched and empty email field', () =>
  {
    //ARRANGE
    //mark email field as touched and empty
    component.loginForm.controls.email.markAsTouched();
    component.loginForm.controls.email.setValue("");

    //ACT
    const message = component.getEmailErrorMessage();

    //ASSERT
    expect(message).toBe("The email is required");
    expect(component.loginForm.controls.email.touched).toBe(true);
    expect(component.loginForm.controls.email.value).toBe("");
    expect(component.loginForm.valid).toBe(false);
  });

  it('should display error message for touched and invalid email field', () =>
  {
    //ARRANGE
    //mark email field as touched and invalid
    component.loginForm.controls.email.markAsTouched();
    component.loginForm.controls.email.setValue("invalid_email");

    //ACT
    const message = component.getEmailErrorMessage();

    //ASSERT
    expect(message).toBe("The email format must be valid");
    expect(component.loginForm.controls.email.touched).toBe(true);
    expect(component.loginForm.controls.email.value).toBe("invalid_email");
    expect(component.loginForm.valid).toBe(false);
  });

  //================TESTS FOR getPasswordErrorMessage()
  it('should not display error message for untouched password field', () =>
  {
    //ARRANGE
    //mark password field as untouched
    component.loginForm.controls.password.markAsUntouched();

    //ACT
    const message = component.getPasswordErrorMessage();

    //ASSERT
    expect(message).toBe("");
    expect(component.loginForm.controls.password.touched).toBe(false);
    expect(component.loginForm.controls.password.value).toBe("");
    expect(component.loginForm.valid).toBe(false);
  });

  it('should display error message for touched and empty password field', () =>
  {
    //ARRANGE
    //mark password field as touched end empty
    component.loginForm.controls.password.markAsTouched();
    component.loginForm.controls.password.setValue("");

    //ACT
    const message = component.getPasswordErrorMessage();

    //ASSERT
    expect(message).toBe("The password is required");
    expect(component.loginForm.controls.password.touched).toBe(true);
    expect(component.loginForm.controls.password.value).toBe("");
    expect(component.loginForm.valid).toBe(false);
  });

  //================TEST FOR login()
  it('should mark all controls as touched and not submit an invalid form', () =>
  {
    //ACT
    component.login();

    //ASSERT
    expect(component.loginForm.controls.email.touched).toBe(true);
    expect(component.loginForm.controls.password.touched).toBe(true);
    expect(component.loginForm.valid).toBe(false);
  });

  it('should submit a valid form to authService', () =>
  {
    //ARRANGE
    const request: LoginRequest =
    {
      email: "john@test.com",
      password: "aaaaAAAA!"
    };

    const response: LoginResponse = 
    {
      message: "Login successful",
      token: "test-token"
    };

    authServiceMock.login.mockReturnValue(of(response));

    component.loginForm.setValue
    ({
      email: request.email,
      password: request.password
    });

    //ACT
    component.login();

    //ASSERT
    expect(component.loginForm.valid).toBe(true);
    expect(authServiceMock.login).toHaveBeenCalledOnce();
    expect(authServiceMock.login).toHaveBeenCalledWith(request);
  });
});
