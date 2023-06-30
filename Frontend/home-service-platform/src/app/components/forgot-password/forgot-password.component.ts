import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomValidators } from 'src/app/helpers/validators';
import { UserService } from 'src/app/services/user.service';
import { User } from 'src/app/interfaces/user.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {
  @Output() passwordEmitter: EventEmitter<string> = new EventEmitter<string>()

  type:string = "password";
  isText:boolean = false;
  eyeIcon:string = "fa-eye-slash";

  form!: FormGroup;
  isDisabled: boolean = true;

  constructor(private fb: FormBuilder, private userService:UserService, private router:Router){}

  ngOnInit(): void {
    this.form = this.fb.group({
      password:['', Validators.required],
      confirmPassword:['', [Validators.required, CustomValidators.password]],
    },
    {
      validator: CustomValidators.confirmPassword
    });

    this.form.valueChanges.subscribe(()=>{
      this.isDisabled = this.form.invalid;
    });
  }
  onSubmit()
  {
    this.passwordEmitter.emit(this.form.controls['password'].value);
  }

  getErrorMessage(controlName: string):string
  {
    const control = this.form.get(controlName);
    if (control?.errors) {
      if (control.errors['invalidPassword']) {
        return control.errors['message'];
      }
    }
    return '';
  }

  hideShowPassword(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

}
