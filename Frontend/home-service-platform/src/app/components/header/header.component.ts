import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { User } from 'src/app/interfaces/user.interface';
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  constructor(private userService:UserService) { }
  user: User = {
    firstName: '',
    lastName: '',
    email: '',
    username: '',
    password: '',
    imageUrl: '',
  };
  ngOnInit(): void {
    this.user = this.userService.getCurrentUser();
  }

}