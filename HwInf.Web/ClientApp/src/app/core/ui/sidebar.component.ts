import { Component, OnInit } from '@angular/core';
import { DeviceService } from "../../shared/services/device.service";
import { OrderService } from "../../shared/services/order.service";
import {Observable} from "rxjs";
import { DeviceType } from "../../shared/models/device-type.model";
import { User } from "../../shared/models/user.model";
import { AdminGuard } from "../../authentication/admin.guard";
import { VerwalterGuard } from "../../authentication/verwalter.guard";
import { UserService } from "../../shared/services/user.service"
import { CanActivate, Router, RouterStateSnapshot, ActivatedRouteSnapshot } from "@angular/router";



@Component({
  selector: 'hwinf-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit, CanActivate {

    public deviceTypes: Observable<DeviceType[]>;

    public verwalter : User
    public newDevicesCount: number =0;

  constructor(
      public deviceService: DeviceService,
      public userService: UserService,
      public orderService: OrderService,
      public router: Router,
      public adminGuard: AdminGuard,
      public verwalterGuard: VerwalterGuard
  ) { }

  canActivate(): Observable<boolean> | Promise<boolean> |
    boolean {
    return this.verwalterGuard.canActivate();
  }

  canActivateSettings(): boolean{
    return this.adminGuard.canActivate();
  }

  ngOnInit() {
      this.deviceTypes = this.deviceService.getDeviceTypes(false);
      this.userService.getUser().subscribe(x => this.verwalter = x);
    console.log(this.router.url.match('(.*admin\/geraete.*)'));
      if (this.verwalterGuard.canActivate()) {
          this.orderService.getOrders().subscribe(x => {
              this.newDevicesCount = x.filter(x => x.Verwalter.Uid == this.verwalter.Uid && x.OrderStatus.Slug == "offen").length;
          });
      }
  }

}
