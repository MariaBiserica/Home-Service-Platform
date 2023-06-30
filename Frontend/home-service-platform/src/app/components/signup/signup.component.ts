  import { Component, OnInit } from '@angular/core';
  import { FormBuilder, FormGroup, Validators } from '@angular/forms';
  import { CustomValidators } from 'src/app/helpers/validators';
  import { UserService } from 'src/app/services/user.service';
  import { User } from 'src/app/interfaces/user.interface';
  import { Router } from '@angular/router';

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

  constructor(private fb: FormBuilder, private userService:UserService, private router:Router) { }

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

  onSubmit()
  {
      const signupData: User = {
      firstName: '',
      lastName: '',
      email: '',
      username: '',
      password: '',
      imageUrl: '',
      role: '',
      bio:''
    };

    signupData.username=this.form.controls['username'].value;
    signupData.email=this.form.controls['email'].value;
    signupData.password=this.form.controls['password'].value;
    signupData.firstName=this.form.controls['firstName'].value;
    signupData.lastName=this.form.controls['lastName'].value; 
    signupData.role=this.form.controls['role'].value;
    this.userService.signupUser(signupData).subscribe({
      next:(response)=>{
        console.log(response);
      this.router.navigateByUrl('');
      },
      error:(error)=>{
        console.log(error);
      }
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
