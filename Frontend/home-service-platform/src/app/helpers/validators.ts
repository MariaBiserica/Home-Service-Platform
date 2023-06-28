import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class CustomValidators {
  static password: ValidatorFn = (control: AbstractControl) => 
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
      
      if (!lowercaseRegex.test(password)) {
        return { invalidPassword: true, message: 'Password must contain at least one lowercase letter.' };
      }

      if (!uppercaseRegex.test(password)) {
        return { invalidPassword: true, message: 'Password must contain at least one uppercase letter.' };
      }
      
      if (!digitRegex.test(password)) {
        return { invalidPassword: true, message: 'Password must contain at least one digit.' };
      }
      
      if (!specialCharRegex.test(password)) {
        return { invalidPassword: true, message: 'Password must contain at least one special character.' };
      }
      
      return null; // Password is valid
  };

  static confirmPassword: ValidatorFn = (control: AbstractControl) => 
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
  
  static checkRange(min: number, max: number): ValidatorFn 
  {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (value === null || value === undefined || isNaN(value)) {
        return null;
      }
      if (value < min || value > max) {
        return { range: true };
      }
      return null;
    };
  }

  static imageUrl(control: AbstractControl): ValidationErrors | null 
  {
    const value = control.value;
    if (value === null || value === undefined || value.trim() === '') {
      return null;
    }
    const pattern = /^https?:\/\/\S+\.(jpg|jpeg|png)$/i;
    if (!pattern.test(value)) {
      return { url: true };
    }
    return null;
  }

  static userRole: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    
    if (value === 'CUSTOMER' || value === 'PROVIDER') {
      return null; // User role is valid
    }
    
    return { invalidUserType: true };
  };

  static emailOrPhone(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (value === null || value === undefined || value.trim() === '') {
      return null;
    }
    const emailRegex = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;
    // const phoneRegex = /^(\d{10}|\d{4}-\d{3}-\d{3})$/;
    const phoneRegex = /^(\d{4}[-\s]?\d{3}[-\s]?\d{3}|\d{10})$/;
    if (!emailRegex.test(value) && !phoneRegex.test(value)) {
      return { emailOrPhone: true };
    }
    return null;
  }  
}
