import { Injectable } from '@angular/core';
import { Service } from '../interfaces/service.interface';
import { Observable, Subject, empty } from 'rxjs';
import userData from './user.json';
import { User } from '../interfaces/user.interface';
import { HttpClient, HttpHeaders } from '@angular/common/http';
@Injectable({
  providedIn: 'root',
})
export class UserService {
  private userToken:string ='';
  private baseUrl:string = 'https://localhost:7269/api/';
  private currentUser: User = {
    firstName: '',
    lastName: '',
    email: '',
    username: '',
    password: '',
    imageUrl: '',
    role: '',
    bio:''
  };
  readonly httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ',//TODO to concatenate token to every request
    }),
  };

  constructor(private httpClient:HttpClient) {}

  setCurrentUser(user:User){
    this.currentUser = user;
  }

  getCurrentUser(){
    return this.currentUser;
  }
  setToken(value:string):void
  {
    this.userToken=value;
  }
  getToken():string|null
  {
    return this.userToken;
  }
  
  setRole(value:string)
  {
    this.currentUser.role = value;
  }
  getRole():string
  {
    return this.currentUser.role;
  }
  setPassword(value:string)
  {
    this.currentUser.password = value;
  }
  loginUser(payload:any):Observable<any> {
    return this.httpClient.post(this.baseUrl+'customers/login', payload, this.httpOptions);
  }

  loginProvider(payload:any):Observable<any> {
    return this.httpClient.post(this.baseUrl+'providers/login', payload, this.httpOptions);
  }

  changePassword(payload:any)
  {
    return this.httpClient.put(this.baseUrl+'account/change-password', payload,this.httpOptions);
  }

  signupUser(data:any):Observable<any> {
    console.log(data.role);
    if(data.role=='CUSTOMER'){
      const userData = {username:data.username,
        email: data.email,
        password:data.password};
      const payload = {userData:userData,
      firstName:data.firstName,
      lastName:data.lastName};
      console.log(payload);
    return this.httpClient.post(this.baseUrl+'account/register/customer', payload);
    }
    else if(data.role=="PROVIDER"){
    const payload = {userData:{username:data.username,
      email: data.email,
      password:data.password},
    bio:data.bio}
      return this.httpClient.post(this.baseUrl+'account/register/provider', payload);
    }
    
    return  new Observable<any>;
  }
}
