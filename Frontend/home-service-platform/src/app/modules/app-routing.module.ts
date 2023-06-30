import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '../components/login/login.component';
import { SignupComponent } from '../components/signup/signup.component';
import { ForgotPasswordComponent } from '../components/forgot-password/forgot-password.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'signup',
    component: SignupComponent
  },
  { 
    path: 'dashboard', 
    loadChildren: () => import('./dashboard-feature.module').then(m => m.DashboardFeatureModule) 
  },
  {
    path: 'forgot-password',
    component: ForgotPasswordComponent
  },
  {
    path:'**',
    redirectTo: 'login'
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
