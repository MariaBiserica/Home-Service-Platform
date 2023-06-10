import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  usernameOrEmail: string = "";
  password: string = "";
  rememberMe: boolean = false;

  type: string = "password";
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash";

  form!: FormGroup;
  isDisabled: boolean = true;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      usernameOrEmail: ['', [Validators.required]],
      password: ['', [Validators.required]],
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

  onSubmit() {
    this.usernameOrEmail = this.form.value.usernameOrEmail;
    this.password = this.form.value.password;
    this.rememberMe = this.form.value.rememberMe;

    console.log(this.usernameOrEmail, this.password, this.rememberMe);

    // Construct the request payload
    var payload = {
        usernameOrEmail: this.usernameOrEmail,
        password: this.password
    };

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
