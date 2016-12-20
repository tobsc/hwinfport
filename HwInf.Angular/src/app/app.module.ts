import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {HttpModule, XHRBackend, RequestOptions} from '@angular/http';

import { AppComponent } from './app.component';

import { DeviceService } from "./devices/shared/device.service";

import {routing} from "./app.routing";

import { LoginComponent } from './authentication/login.component';
import {AuthService} from "./authentication/auth.service";
import {AuthGuard} from "./authentication/auth.guard";
import {DevicesModule} from "./devices/devices.module";
import {SharedModule} from "./shared/shared.module";
import {ErrorMessageService} from "./shared/error-message/error-message.service";
import {JwtHttpService} from "./shared/jwt-http.service";
import {Router} from "@angular/router";
import {DashboardModule} from "./dashboard/dashboard.module";
import {CartModule} from "./cart/cart.module";
import {CartService} from "./cart/cart.service";
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        PageNotFoundComponent,
    ],
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        routing,
        DevicesModule,
        SharedModule,
        DashboardModule,
        CartModule,
    ],
    providers: [
      {
        provide: JwtHttpService,
        useFactory: (backend: XHRBackend, options: RequestOptions, auth: AuthService, router: Router) => {
          return new JwtHttpService(backend, options, auth, router);
        },
        deps: [XHRBackend, RequestOptions, AuthService, Router]
      },
      ErrorMessageService,
      DeviceService,
      AuthService,
      CartService,
      AuthGuard
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
