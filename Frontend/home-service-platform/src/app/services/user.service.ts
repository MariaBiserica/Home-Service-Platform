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
    role: '',
  };

  constructor() {}

  setCurrentUser(user:User){
    this.currentUser = user;
  }

  getCurrentUser(){
    return this.currentUser;
  }

  loginUser(userLogin: User): boolean {
    const index = this.userList.findIndex(
      (user) => user.username == userLogin.username
    );
    if (index != -1) {
      console.log('User accepted');
      this.setCurrentUser(this.userList[index]);
      return true;
    } else {
      console.log('User denied');
      return false;
    }
  }

  signUpUser(userSignUp: User) {
    this.userList.push(userSignUp);

    //! Save the new user list to the json file -- Not working
    //var json = JSON.stringify(this.userList);
    //const fs = require('fs');
    //fs.writeFile('user.json', json);
  }
}
