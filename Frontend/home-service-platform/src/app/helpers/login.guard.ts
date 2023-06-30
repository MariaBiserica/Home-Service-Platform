import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { Observable } from "rxjs";

@Injectable({
    providedIn:'root',
})
export class LoginGuard implements CanActivate
{
    constructor(private router: Router)
    {}
    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ):
    | boolean
    | UrlTree
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>{
        if(localStorage.getItem('token') ||
        sessionStorage.getItem('token'))
        {
            return true;
        }
        this.router.navigateByUrl('/login');
        return false;
    }
}