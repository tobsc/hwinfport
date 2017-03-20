import { Injectable } from '@angular/core';
import { CanActivate, Router, RouterStateSnapshot, ActivatedRouteSnapshot } from "@angular/router";
import { AuthService } from "./auth.service";
import { JwtService } from "../shared/services/jwt.service";

@Injectable()
export class VerwalterGuard implements CanActivate {

    constructor(private router: Router, private authService: AuthService, private jwtService: JwtService) { }

    canActivate() {

        if (!this.authService.isLoggedIn()) {
            this.router.navigate(['/login']);
            return false;
        }

        return this.jwtService.isVerwalter();

        //oberer code equivalent zu?
        //this.getRole(localStorage.getItem('auth_token')).toLowerCase() === 'admin';
    }

    

   
    
}
