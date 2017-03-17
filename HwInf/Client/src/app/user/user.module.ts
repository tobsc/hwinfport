import {NgModule} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import { DashboardComponent } from './dashboard/dashboard.component';
import { DeviceListComponent } from './devices/device-list.component';
import { DeviceFilterComponent } from './devices/device-filter.component';
import { DevicesStatusDirective } from './devices/devices-status.directive';
import { CoreModule } from "../core/core.module";
import { AccordionModule } from "ng2-bootstrap";
import { CartComponent } from './cart/cart.component';
import { OrderStep1Component } from './cart/order-step1/order-step1.component';
import { OrderStep2Component } from './cart/order-step2/order-step2.component';
import { OrderStep3Component } from './cart/order-step3/order-step3.component';


@NgModule({
    declarations: [
        DashboardComponent,
        DeviceListComponent,
        DeviceFilterComponent,
        DevicesStatusDirective,
        CartComponent,
        OrderStep1Component,
        OrderStep2Component,
        OrderStep3Component],
    imports: [
        CommonModule,
        FormsModule,
        CoreModule,
        AccordionModule.forRoot()
    ]
})
export class UserModule {}