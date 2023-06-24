import { Injectable } from '@angular/core';
import { Service } from '../interfaces/service.interface';
import { Subject } from 'rxjs';
import userData from './user.json';
import { User } from '../interfaces/user.interface';
@Injectable({
  providedIn: 'root',
})
export class UserService {
  private userList: User[] = userData;
  private currentUser: User = {
    firstName: '',
    lastName: '',
    email: '',
    username: '',
    password: '',
    imageUrl: '',
  };
  constructor() {}

  set users(usersToSet: any) {
    this.currentUser = usersToSet;
  }

  loginUser(userLogin: User) {
    const index = this.userList.findIndex(
      (user) => user.username == userLogin.username
    );
    if (index != -1) {
      console.log('User accepted');
    } else {
      console.log('User denied');
    }
  }
  signUpUser(userSignUp: User) {
    this.userList.push(userSignUp);
    //var json = JSON.stringify(this.userList);
    //const fs = require('fs');
    //fs.writeFile('user.json', json);
  }
}
