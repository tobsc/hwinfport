﻿using System.Linq;
using HwInf.DataAccess.Context;
using HwInf.DataAccess.Entities;
using HwInf.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HwInf.DataAccess
{
    public class DataAccess : IDataAccessLayer
    {
        private readonly HwInfContext _context;
        public DataAccess(HwInfContext context)
        {
            _context = context;
        }

        public IQueryable<Device> Devices => _context.Devices
            .Include(x => x.Type)
            .Include(x => x.DeviceMeta)
            .Include(x => x.Type.DeviceTypesFieldGroups.Select(y => y.FieldGroup.Fields));

        public IQueryable<DeviceMeta> DeviceMeta => _context.DeviceMeta;

        public IQueryable<DeviceType> DeviceTypes => _context.DeviceTypes
            .Include(x => x.DeviceTypesFieldGroups.Select(y => y.FieldGroup.DeviceTypeFieldGroups))
            .Include(x => x.DeviceTypesFieldGroups.Select(y => y.FieldGroup.Fields));

        public IQueryable<DeviceStatus> DeviceStatus => _context.DeviceStatus;
        public IQueryable<OrderStatus> OrderStatus => _context.OrderStatus;
        public IQueryable<Person> Persons => _context.Persons;
        public IQueryable<Role> Roles => _context.Roles;
        public IQueryable<Order> Orders => _context.Orders
            .Include(x => x.OrderItems)
            .Include(x => x.OrderItems.Select(y => y.Device))
            .Include(x => x.OrderStatus);

        public IQueryable<OrderItem> OrderItems => _context.OrderItems;
        public IQueryable<Field> Fields => _context.Fields;

        public IQueryable<FieldGroup> FieldGroups => _context.FieldGroups
            .Include(x => x.DeviceTypeFieldGroups.Select(y => y.DeviceType))
            .Include(x => x.Fields);

        public IQueryable<Setting> Settings => _context.Settings;
        public IQueryable<Damage> Damages => _context.Damages;
        public IQueryable<DamageStatus> DamageStatus => _context.DamageStatus;
        public IQueryable<Accessory> Accessories => _context.Accessories;

        public Device CreateDevice()
        {
            var device = new Device();
            _context.Devices.Add(device);
            return device;
        }

        public DeviceType CreateDeviceType()
        {
            var deviceType = new DeviceType();
            _context.DeviceTypes.Add(deviceType);
            return deviceType;
        }

        public DeviceStatus CreatDeviceStatus()
        {
            throw new System.NotImplementedException();
        }

        public OrderStatus CreateOrderStatus()
        {
            throw new System.NotImplementedException();
        }

        public DamageStatus CreateDamageStatus()
        {
            throw new System.NotImplementedException();
        }

        public Person CreatePerson()
        {
            var person = new Person();
            _context.Persons.Add(person);
            return person;
        }

        public Order CreateOrder()
        {
            var order = new Order();
            _context.Orders.Add(order);
            return order;
        }

        public Field CreateField()
        {
            var field = new Field();
            _context.Fields.Add(field);
            return field;
        }

        public FieldGroup CreateFieldGroup()
        {
            var fieldGroup = new FieldGroup();
            _context.FieldGroups.Add(fieldGroup);
            return fieldGroup;
        }

        void IDataAccessLayer.SaveChanges()
        {
            _context.SaveChanges();
        }

        public void DeleteDevice()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDeviceMeta(DeviceMeta deviceMeta)
        {
            _context.DeviceMeta.Remove(deviceMeta);
        }


        public void UpdateObject(object obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }

        public Device AddDeviceMetaToDevice(DeviceMeta deviceMeta, HwInfContext context)
        {
            var device = _context.Find<Device>(deviceMeta.Device);
            device.AddDeviceMeta(deviceMeta);
            return device;
        }

        public void AddFieldToFieldGroup(FieldGroup fieldGroup, HwInfContext context)
        {
            throw new System.NotImplementedException();
        }

        public void AddDeviceTypeToFieldGroup(DeviceType deviceType, HwInfContext context)
        {
            throw new System.NotImplementedException();
        }

        public void AddFieldGroupToDeviceType(FieldGroup fieldGroup, HwInfContext context)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDeviceType(DeviceType deviceType)
        {
            _context.DeviceTypes.Remove(deviceType);
        }

        public void DeleteField(Field field)
        {
            _context.Fields.Remove(field);
        }

        public DeviceStatus CreateDeviceStatus()
        {
            throw new System.NotImplementedException();
        }

        public DeviceMeta CreateDeviceMeta()
        {
            var deviceMeta = new DeviceMeta();
            _context.DeviceMeta.Add(deviceMeta);
            return deviceMeta;
        }

        public void DeleteFieldGroup(FieldGroup fieldGroup)
        {
            _context.FieldGroups.Remove(fieldGroup);
        }

        public OrderItem CreateOrderItem()
        {
            var orderItem = new OrderItem();
            _context.OrderItems.Add(orderItem);
            return orderItem;
        }

        public Setting CreateSetting()
        {
            var setting = new Setting();
            _context.Settings.Add(setting);
            return setting;
        }

        public Accessory CreateAccessory()
        {
            var accessory = new Accessory();
            _context.Accessories.Add(accessory);
            return accessory;
        }

        public void DeleteSetting(Setting setting)
        {
            _context.Settings.Remove(setting);
        }

        public Damage CreateDamage()
        {
            var damage = new Damage();
            _context.Damages.Add(damage);
            return damage;
        }

        public void DeleteDamage(Damage damage)
        {
            _context.Damages.Remove(damage);
        }

        public void DeleteAccessory(Accessory accessory)
        {
            _context.Accessories.Remove(accessory);
        }
    }
}