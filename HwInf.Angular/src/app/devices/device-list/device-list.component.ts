import {Component, OnInit, OnDestroy} from '@angular/core';
import {Device} from "../shared/device.model";
import {DeviceService} from "../shared/device.service";
import {Subscription, Observable} from "rxjs";
import {ActivatedRoute} from "@angular/router";
import {URLSearchParams} from "@angular/http";

@Component({
    selector: 'hw-inf-device-list',
    templateUrl: './device-list.component.html',
    styleUrls: ['./device-list.component.scss']
})
export class DeviceListComponent implements OnInit, OnDestroy {
    private subscription: Subscription;
    private currentType: string;
    private devices: Observable<Device[]>;
    private orderByVal: string = 'name';

    constructor(private deviceService: DeviceService,
                private route: ActivatedRoute) { }



    ngOnInit() {
        this.subscription = this.route.params
            .subscribe(
                (params: any) => {
                    this.currentType = params['type'];
                    this.devices = this.deviceService.getDevices(this.currentType);
                }
            );
    }


    /**
     * Update device list with given search params
     * which is executed if deviceListUpdated event is emitted
     * @param params GET params
     */
    private updateList(params: URLSearchParams) {
        this.devices = this.deviceService.getDevices(this.currentType, params);
    }

    /**
     * defaults to name
     * @param event change of order by dropdown
     */
    private updateOrderByValue(event) {
          this.orderByVal = event.target.value;
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}
