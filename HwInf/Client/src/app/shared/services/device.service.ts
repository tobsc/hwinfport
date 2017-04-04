import { Injectable } from '@angular/core';
import {JwtHttpService} from "./jwt-http.service";
import {Observable} from "rxjs";
import {DeviceType} from "../models/device-type.model";
import {Response, URLSearchParams, RequestOptions, Headers} from "@angular/http";
import { Device } from "../models/device.model";
import { IDictionary } from "../../shared/common/dictionary.interface";
import { Dictionary } from "../../shared/common/dictionary.class";
import { DeviceComponent } from "../models/component.model";
import {DeviceList} from "../models/device-list.model";

@Injectable()
export class DeviceService {

    private url: string = '/api/devices/';
    private deviceTypes: Observable<string[]> = null;
    private deviceComponents: IDictionary<Observable<DeviceComponent[]>> = new Dictionary<Observable<DeviceComponent[]>>();


    constructor(private http: JwtHttpService) { }

    /**
     * Returns Devices of given type
     * empty type string returns all types
     * @param type DeviceType.TypeName
     * @param quantity
     * @param offset
     * @param params
     * @returns {Observable<Device[]>}
     */
    public getDevices(
        type: string = "",
        limit: number = 100,
        offset: number = 0,
        params: URLSearchParams = new URLSearchParams()
    ): Observable<DeviceList> {

        params.set('limit', limit + '');
        params.set('offset', offset + '');

        let options = new RequestOptions({
            search: params,
        });
        return this.http.get(this.url + type + '/', options)
            .map((response: Response) => response.json());
    }

    public getDevice(invNum: string): Observable<Device> {
        return this.http.get(this.url + 'invnum/' + encodeURI(invNum))
            .map((response: Response) => response.json());
    }

    /**
     * Returns all device types. e.g Notebook, PC, Monitor, ...
     * @returns {Observable<DeviceType[]>}
     */
    public getDeviceTypes(): Observable<DeviceType[]> {
        return this.http.get(this.url + 'types')
            .map((response: Response) => response.json());
    }

    /**
     * Returns objects of type DeviceComponent, which holds the component name e.g Prozessor
     * and all values present in the database
     * @param type
     * @returns {Observable<DeviceComponent[]>}
     */
    public getComponentsAndValues(type: string): Observable<DeviceComponent[]> {
        if (!this.deviceComponents.containsKey(type)) {
            this.deviceComponents.add(
                type,
                this.http.get(this.url + 'types/' + type)
                    .map((response: Response) => response.json())
            );
        }
        return this.deviceComponents.get(type);
    }

    public addDeviceType(body: DeviceType): Observable<DeviceType> {
        let bodyString = JSON.stringify(body);
        let headers = new Headers({
           'Content-Type': 'application/json'
        });
        let options = new RequestOptions({headers: headers});
        return this.http.post(this.url + 'types', bodyString, options)
            .map((response: Response) => response.json());
    }

    public addNewDevice(body: Device): Observable<Device> {
        let bodyString = JSON.stringify(body);
        let headers = new Headers({
            'Content-Type': 'application/json'
        });
        let options = new RequestOptions({headers: headers});
        return this.http.post(this.url, bodyString, options)
            .map((response: Response) => response.json());
    }

    public editDevice(body: Device): Observable<Device> {
        let bodyString = JSON.stringify(body);
        let headers = new Headers({
            'Content-Type': 'application/json'
        });
        let params: URLSearchParams = new URLSearchParams();
        params.append('id', ""+body.DeviceId);
        let options = new RequestOptions({headers: headers, });
        return this.http.put(this.url, bodyString, options)
            .map((response: Response) => response.json());
    }

}
