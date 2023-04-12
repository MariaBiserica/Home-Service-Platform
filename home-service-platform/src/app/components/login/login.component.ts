import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  usernameOrEmail: string = "";
  password: string = "";
  rememberMe: boolean = false;
  @ViewChild('rememberMeCheckbox')
  rememberMeCheckbox!: ElementRef;
  @ViewChild('usernameOrEmailInput')
  usernameOrEmailInput!: ElementRef;
  @ViewChild('passwordInput')
  passwordInput!: ElementRef;

  type: string = "password";
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash";

  constructor() { }

  ngOnInit(): void {}

  hideShowPassword() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  onSubmit() {
    this.usernameOrEmail = this.usernameOrEmailInput.nativeElement.innerText;
    this.password = this.passwordInput.nativeElement.innerText;
    this.rememberMe = this.rememberMeCheckbox.nativeElement.checked;

    console.log(this.usernameOrEmail, this.password, this.rememberMe);
  }
  
}
