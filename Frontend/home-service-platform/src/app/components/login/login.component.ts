import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../helpers/validators';
import { UserService } from 'src/app/services/user.service';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { LoginPayload } from 'src/app/interfaces/login.payload.interface';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  usernameOrEmail: string = '';
  username: string = '';
  email: string = '';
  password: string = '';
  rememberMe: boolean = false;
  newPassword!:string;

  type: string = 'password';
  isText: boolean = false;
  eyeIcon: string = 'fa-eye-slash';

  isVisibleAll:boolean = true;
  isVisibleForgot:boolean = false;
  form!: FormGroup;
  isDisabled: boolean = true;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router,
    private loginMessage: NzMessageService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      username: [localStorage.getItem('username') || '', [Validators.required]],
      email: [
        localStorage.getItem('email') || '',
        [Validators.required, Validators.email],
      ],
      password: [
        localStorage.getItem('password') || '',
        [Validators.required, CustomValidators.password],
      ],
      rememberMe: [false],
    });

    this.form.valueChanges.subscribe(() => {
      this.isDisabled = this.form.invalid;
    });
  }

  hideShowPassword() {
    this.isText = !this.isText;
    this.isText ? (this.eyeIcon = 'fa-eye') : (this.eyeIcon = 'fa-eye-slash');
    this.isText ? (this.type = 'text') : (this.type = 'password');
  }

  getErrorMessage(controlName: string): string {
    const control = this.form.get(controlName);
    if (control?.errors) {
      if (control.errors['invalidPassword']) {
        return control.errors['message'];
      }
    }
    return '';
  }

  onRememberMeChange(event: any) {
    const checked = event.target.checked;
    if (checked) {
      // Store the user's login credentials in local storage
      localStorage.setItem('username', this.username);
      localStorage.setItem('email', this.email);
      localStorage.setItem('password', this.password);
    } else {
      // Clear the stored login credentials from local storage
      localStorage.removeItem('username');
      localStorage.removeItem('email');
      localStorage.removeItem('password');
    }
  }

  onSubmit() {
    this.username = this.form.value.username;
    this.email = this.form.value.email;
    this.password = this.form.value.password;
    this.rememberMe = this.form.value.rememberMe;
    const payload: LoginPayload = {
      email: this.email,
      password: this.password,
    };
    // Check the user credentials
    this.userService.loginUser(payload).subscribe({
      next: (response) => {
        console.log(response);
        this.userService.setToken(response.token);
        sessionStorage.setItem('token', response.token);
        if (this.rememberMe) {
          localStorage.setItem('username', this.username);
          localStorage.setItem('email', this.email);
          localStorage.setItem('password', this.password);
        } else {
          // Clear the stored login credentials if the "Remember me" checkbox is not checked
          localStorage.removeItem('username');
          localStorage.removeItem('email');
          localStorage.removeItem('password');
        }
        this.userService.setRole('CUSTOMER');
        this.loginMessage.create('success', 'Login successful');
        this.router.navigateByUrl('/dashboard');
      },
      error: (error) => {
        this.userService.loginProvider(payload).subscribe({
          next: (response) => {
            console.log(response);
            this.userService.setToken(response.token);
            sessionStorage.setItem('token', response.token);
            if (this.rememberMe) {
              localStorage.setItem('username', this.username);
              localStorage.setItem('email', this.email);
              localStorage.setItem('password', this.password);
              localStorage.setItem('token', response.token);
            } else {
              // Clear the stored login credentials if the "Remember me" checkbox is not checked
              localStorage.removeItem('username');
              localStorage.removeItem('email');
              localStorage.removeItem('password');
              localStorage.removeItem('token');
            }
            this.userService.setRole('PROVIDER');
            this.loginMessage.create('success', 'Login successful');
            this.router.navigateByUrl('/dashboard');
          },
          error: (error) => {
            this.loginMessage.create('warning', 'Login error');
          },
        });
      },
    });
  }
  forgotPushed()
  {
    this.isVisibleAll = false;
    this.isVisibleForgot = true;
  }
  setNewPassword(event:any)
  {
    this.newPassword = event;
    this.userService.setPassword(this.newPassword);
    this.isVisibleAll = true;
    this.isVisibleForgot = false;
    this.loginMessage.create('success', 'Password changed successfully');
    console.log(this.newPassword);
  }
}
