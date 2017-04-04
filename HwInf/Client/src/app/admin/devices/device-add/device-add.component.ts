import {Component, OnInit, AfterViewInit} from "@angular/core";
import {DeviceService} from "../../../shared/services/device.service";
import {CustomFieldsService} from "../../../shared/services/custom-fields.service";
import {DeviceType} from "../../../shared/models/device-type.model";
import {FieldGroup} from "../../../shared/models/fieldgroup.model";
import {Observable, Subscription} from "rxjs";
import {FormGroup, FormArray, FormBuilder, Validators, NgForm} from "@angular/forms";
import {Device} from "../../../shared/models/device.model";
import {UserService} from "../../../shared/services/user.service";
import {User} from "../../../shared/models/user.model";
import {ActivatedRoute} from "@angular/router";
import {DeviceMeta} from "../../../shared/models/device-meta.model";

@Component({
  selector: 'hwinf-device-add',
  templateUrl: './device-add.component.html',
  styleUrls: ['./device-add.component.scss']
})
export class DeviceAddComponent implements OnInit {

  private routeSubscription: Subscription;
  private form: FormGroup;
  private formFieldGroups: FormArray;
  private invNums: FormArray;
  private deviceTypes: Observable<DeviceType[]>;
  private fieldGroups: FieldGroup[];
  private owners: User[];
  private currentDevice: Device;
  private formDeviceType: FormGroup;
  private currentTypeSlug: string;

  constructor(
      private deviceService: DeviceService,
      private customFieldsService: CustomFieldsService,
      private userService: UserService,
      private fb: FormBuilder,
      private route: ActivatedRoute
  ) { }

  ngOnInit() {

    this.deviceTypes = this.deviceService.getDeviceTypes();
    this.userService.getOwners()
        .subscribe(
            (next) => { this.owners = next },
            (error) => console.log(error)
        );

    this.form = this.initForm();
    this.formDeviceType = <FormGroup>this.form.controls['DeviceType'];
    this.formFieldGroups = <FormArray>this.form.controls['FieldGroups'];
    this.invNums = <FormArray>this.form.controls['AdditionalInvNums'];
    this.routeSubscription = this.route.params
        .map(params => params['invnum'])
        .flatMap(invnum => this.deviceService.getDevice(invnum))
        .subscribe((device:Device) => {
          this.currentDevice = device;
          this.fillFormWithDeviceData(device);
        });
  }

  private getMetaDataOfFieldGroup(slug: string, metaData: DeviceMeta[]): DeviceMeta[] {
    return metaData.filter((i) => i.FieldGroupSlug === slug);
  }

  private fillFormWIthMetaData(device: Device) {
      device.FieldGroups
          .forEach( (fieldgroup, index) => {
            this.getFieldGroups(index).removeAt(0);
            this.getMetaDataOfFieldGroup(fieldgroup.Slug, device.DeviceMeta)
                .forEach((field) => {
                  this.getFieldGroups(index).push(this.initField(field.Field, +field.Value));
                });
          });
  }

  private fillFormWithDeviceData(device: Device) {
    this.form.get('Name').setValue(device.Name);
    this.form.get('Marke').setValue(device.Marke);
    this.form.get('Raum').setValue(device.Raum);
    this.form.get('Person').setValue({Uid: device.Verwalter.Uid});
    this.form.get('DeviceType').setValue({Slug: device.DeviceType.Slug });
  }


  /**
   * Initializes a the main form
   * @returns {FormGroup}
   */
  private initForm(device: Device = null): FormGroup {
    return this.fb.group({
      Name: ['', Validators.required],
      InvNum: ['', Validators.required],
      AdditionalInvNums: this.fb.array([]),
      Marke: ['', Validators.required],
      Raum: ['', Validators.required],
      DeviceType: this.initDeviceType(),
      Person: this.initPerson(),
      FieldGroups: this.fb.array([]),
    });
  }

  /**
   * Add an additional inventory number
   */
  public addInvNum(): void {
    this.invNums.push(this.initInvNum());
  }

  /**
   * Intializes a new form group
   * @param invNum
   * @returns {FormGroup}
   */
  private initInvNum(invNum: string = ''): FormGroup {
    return this.fb.group({
      InvNum: invNum
    });
  }

  /**
   * remove inventory number field at index i
   * @param index
   */
  public removeInvNum(index: number): void {
    this.invNums.removeAt(index);
  }

  /**
   * Initializes a new Person form group
   * @param uid
   * @returns {FormGroup}
   */
  private initPerson(uid: string = ''): FormGroup {
    return this.fb.group({
      Uid: [uid, Validators.required]
    });
  }

  /**
   * Initializes a new DeviceType form group
   * @param slug
   * @returns {FormGroup}
   */
  private initDeviceType( slug: string = '' ): FormGroup {
      return this.fb.group({
          Slug: [slug, Validators.required]
      });
  }

  /**
   * Remove all FieldGroups
   */
  private clearFieldGroups(): void {
    let len = this.formFieldGroups.length;
    for ( let i = len; i--;) {
      this.removeFieldGroup(i);
    }
  }

  /**
   * Remove FieldGroup at given index i
   * @param index
   */
  public removeFieldGroup(index: number): void {
    this.formFieldGroups.removeAt(index);
  }

  /**
   * Add new FieldGroup
   */
  public addFieldGroup(): void {
    this.formFieldGroups.push(this.initFieldGroup());
  }

  /**
   * Initializes a new FieldGroup with 1 Field
   * @returns {FormGroup}
   */
  private initFieldGroup(): FormGroup {
    return this.fb.group({
      Slug: ['', Validators.required],
      Fields: this.fb.array([this.initField()])
    });
  }

  /**
   * Initializes a Field form group
   * @returns {FormGroup}
   */
  public initField(name: string = '', quantity :number = 1): FormGroup {
    return this.fb.group({
      Name: [name, Validators.required],
      Quantity: [quantity, Validators.required]
    });
  }

  /**
   * Add new Field to FieldGroup with given index
   * @param fieldGroupIndex
   */
  public addField(fieldGroupIndex: number): void {
    this.getFieldGroups(fieldGroupIndex).push(this.initField());
  }

  /**
   * Remove Field from at index i from FieldGroup with index j
   * @param fieldGroupIndex
   * @param fieldIndex
   */
  removeField(fieldGroupIndex: number, fieldIndex: number) {
    this.getFieldGroups(fieldGroupIndex).removeAt(fieldIndex);
  }

  /**
   * @param fieldGroupIndex
   * @returns {FormArray}  of FieldGroups
   */
  private getFieldGroups(fieldGroupIndex: number): FormArray {
    let fieldGroup: FormGroup = <FormGroup> this.formFieldGroups.at(fieldGroupIndex);
    return <FormArray> fieldGroup.controls['Fields'];
  }

  /**
   * if the selected DeviceType is changed,
   * generate FieldGroups and Field according to the FieldGroups of the DeviceType
   * @param $event
   */
  public onSelectedTypeChange( $event ): void {
    this.clearFieldGroups();
    this.currentTypeSlug = $event;
    this.customFieldsService.getFieldGroupsOfType( $event )
      .subscribe(
            (data) => {
              this.fieldGroups = data;
              this.fieldGroups.forEach(() => this.addFieldGroup());

              if (this.currentDevice && this.currentDevice.DeviceType.Slug === this.currentTypeSlug) {
                this.fillFormWIthMetaData(this.currentDevice);
              }
            }
        );
  }

  /**
   * Initializes DeviceMeta field group with given params
   * @param fieldGroup  slug of the FieldGroup
   * @param field       Name of the Field
   * @param value       Quantity of the field
   * @returns {FormGroup}
   */
  private initDeviceMeta(fieldGroup: string, field: string,  value: string): FormGroup {
    return this.fb.group({
      Field: [field, Validators.required],
      FieldGroupSlug: [fieldGroup, Validators.required],
      Value: [value, Validators.required]
    });
  }

  /**
   * Submit
   * @param form
   */
  public onSubmit(form: NgForm): void {
   this.deviceService.addNewDevice(this.mappedForm(form)).subscribe(
        (next) => { console.log(next) },
        (error) => { console.log(error) }
    );
  }

  private mappedForm(form: NgForm): Device {
    let resultForm: FormGroup = this.fb.group({
      Name: [form.value.Name, Validators.required],
      InvNum: [form.value.InvNum, Validators.required],
      Marke: [form.value.Marke, Validators.required],
      Raum: [form.value.Raum, Validators.required],
      DeviceType: this.initDeviceType(form.value.DeviceType.Slug),
      Verwalter: this.initPerson(form.value.Person.Uid),
      DeviceMeta: this.fb.array([]),
      AdditionalInvNums: this.fb.array(
          form.value.AdditionalInvNums
      ),
    });
    let deviceMeta: FormArray = <FormArray>resultForm.controls['DeviceMeta'];

    form.value.FieldGroups
        .forEach(i => {
          i.Fields.forEach(j => {
            deviceMeta.push(this.initDeviceMeta(i.Slug, j.Name, j.Quantity))
          })
        });
    return <Device>resultForm.value;
  }
}
