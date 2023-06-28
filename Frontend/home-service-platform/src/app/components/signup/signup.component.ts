import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from 'src/app/helpers/validators';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {
  
  type: string = "password";
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash";

  form!: FormGroup;
  isDisabled: boolean = true;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      username: ['', [Validators.required]],
      role: ['', [Validators.required, CustomValidators.userRole]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [CustomValidators.password]],
      confirmPassword: ['', [Validators.required]],
    },
    {
      validator: CustomValidators.confirmPassword
    });

    this.form.valueChanges.subscribe(() => {
      this.isDisabled = this.form.invalid;
    });
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

  hideShowPassword() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }
}
