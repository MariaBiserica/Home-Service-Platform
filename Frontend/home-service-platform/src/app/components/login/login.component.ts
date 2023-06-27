import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from '../../helpers/validators';
import { UserService } from 'src/app/services/user.service';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  usernameOrEmail:string = "";
  username: string = "";
  email: string = "";
  password: string = "";
  rememberMe: boolean = false;

  type: string = "password";
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash";

  form!: FormGroup;
  isDisabled: boolean = true;

  constructor(private fb: FormBuilder, private userService: UserService, private router: Router, private loginMessage:NzMessageService) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      username: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, CustomValidators.password]],
      rememberMe: [false]
    });

    this.form.valueChanges.subscribe(() => {
      this.isDisabled = this.form.invalid;
    });
  }

  hideShowPassword() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
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

  onSubmit() {

    this.username = this.form.value.username;
    this.email = this.form.value.email;
    this.password = this.form.value.password;
    this.rememberMe = this.form.value.rememberMe;
    //console.log(this.username,this.email, this.password, this.rememberMe);
    // Construct the request payload
    var payload = {
      usernameOrEmail: this.usernameOrEmail,
      password: this.password
    };
    
    if(this.userService.loginUser({firstName:"",lastName:"", email: this.email, username:this.username,imageUrl:"", password:this.password}))
    {
      this.router.navigateByUrl('/dashboard');
    }
    else{
        this.loginMessage.create('warning', 'Login unsuccessful');
    }

    // Make the API call to https://reqres.in/api/login
    fetch('https://reqres.in/api/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
    .then(response => response.json())
    .then(data => {
        // Handle the API response
        if (data.token) {
            // Login successful, redirect to another page or perform further actions
            console.log("Login successful");
        } else {
            // Login failed, display an error message or perform other error handling
            console.error("Login failed");
        }
    })
    .catch(error => {
        // Handle any errors that occurred during the API call
        console.error("API request failed:", error);
    });
  }
  
}
