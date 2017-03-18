import { Injectable } from '@angular/core';
import {JwtHttpService} from "./jwt-http.service";
import {FieldGroup} from "../models/fieldgroup.model";
import {Observable} from "rxjs";
import {Headers, RequestOptions, Response } from "@angular/http";

@Injectable()
export class CustomFieldsService {

  constructor(
      private http: JwtHttpService
  ) { }

  public addFieldGroup(body: FieldGroup): Observable<FieldGroup> {
    let bodyString = JSON.stringify(body);
    let headers = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({headers: headers});
    return this.http.post('/api/customfields/fieldgroups', bodyString, options)
        .map((response: Response) => response.json());
  }


  public getFieldGroups(): Observable<FieldGroup[]> {
    return this.http.get('/api/customfields/fieldgroups')
        .map((response: Response) => response.json());
  }

  public getFieldGroupsOfType ( deviceTypeSlug: string): Observable<FieldGroup[]> {
    return this.http.get('/api/customfields/fieldgroups/' + deviceTypeSlug)
        .map((response: Response) => response.json());
  }
}