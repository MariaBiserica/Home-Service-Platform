import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class CustomValidators {
  static passwordValidator: ValidatorFn = (control: AbstractControl) => 
  {
      const password = control.value;
      
      // Regex patterns for password requirements
      const uppercaseRegex = /[A-Z]/;
      const lowercaseRegex = /[a-z]/;
      const digitRegex = /[0-9]/;
      const specialCharRegex = /[!@#$%^&*(),.?":{}|<>]/;
      
      if (!password || password.length < 6) {
        return { invalidPassword: true, message: 'Password must be at least 6 characters long.' };
      }
      
      if (!uppercaseRegex.test(password)) {
        return { invalidPassword: true, message: 'Password must contain at least one uppercase letter.' };
      }
      
      if (!lowercaseRegex.test(password)) {
        return { invalidPassword: true, message: 'Password must contain at least one lowercase letter.' };
      }
      
      if (!digitRegex.test(password)) {
        return { invalidPassword: true, message: 'Password must contain at least one digit.' };
      }
      
      if (!specialCharRegex.test(password)) {
        return { invalidPassword: true, message: 'Password must contain at least one special character.' };
      }
      
      return null; // Password is valid
  };

  static confirmPasswordValidator: ValidatorFn = (control: AbstractControl) => 
  {
      const passwordControl = control.get('password');
      const confirmPasswordControl = control.get('confirmPassword');
  
      if (!passwordControl || !confirmPasswordControl) {
        return null; // Controls not found, skip validation
      }
  
      const password = passwordControl.value;
      const confirmPassword = confirmPasswordControl.value;
  
      if (password !== confirmPassword) {
        return { passwordMismatch: true };
      }
  
      return null; // Passwords match
  };

  static descriptionWordsCount(minWords: number): ValidatorFn 
  {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (value === null || value === undefined || value.trim() === '') {
        return null;
      }
      const words = value.trim().split(/\s+/);
      if (words.length < minWords) {
        return { wordsCount: true };
      }
      return null;
    };
  }  
}
