﻿using System.Collections.Generic;
using System.Linq;
using HwInf.Common.BL;
using HwInf.Common.Models;
using static System.String;

namespace HwInf.ViewModels
{
    public class DeviceViewModel
    {
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public string InvNum { get; set; }
        public string Marke { get; set; }
        public string Room { get; set; }
        public DeviceTypeViewModel DeviceType { get; set; }
        public UserViewModel Owner { get; set; }
        public DeviceStatusViewModel Status { get;set; }
        public IEnumerable<DeviceMetaViewModel> DeviceMeta { get; set; }
        public bool IsActive { get; set; } = true;

        public DeviceViewModel()
        {
        }

        public DeviceViewModel(Device obj)
        {
            Refresh(obj);
        }

        public void Refresh(Device obj)
        {
            var target = this;
            var source = obj;


            target.DeviceId = source.DeviceId;
            target.Name = source.Name;
            target.InvNum = source.InvNum;
            target.Marke = source.Brand;
            target.Status = new DeviceStatusViewModel(source.Status);
            target.DeviceType= new DeviceTypeViewModel(source.Type).LoadComponents(source.Type);
            target.Room = source.Room;
            target.Owner = new UserViewModel(source.Person);
            target.IsActive = source.IsActive;
        }

        public void ApplyChanges(Device obj, BL bl)
        {
            var target = obj;
            var source = this;

            if(IsNullOrWhiteSpace(Status.Description))
            {
                Status.Description = "Verfügbar";
            }

            target.Name = source.Name;
            target.InvNum = source.InvNum;
            target.Brand = source.Marke;
            target.Status = bl.GetDeviceStatus(source.Status.StatusId);
            target.Type = bl.GetDeviceType(source.DeviceType.DeviceTypeId);
            target.Person = bl.GetPerson(source.Owner.Uid);
            target.Room = source.Room;
            target.IsActive = source.IsActive;
            target.DeviceMeta = new List<DeviceMeta>();
            source.DeviceMeta.ToList().ForEach(i => target.DeviceMeta.Add(i));

        }

        public DeviceViewModel LoadMeta(Device d)
        {
            DeviceMeta = d.DeviceMeta
                .Select(i => new DeviceMetaViewModel(i));

              
            return this; // fluent interface
        }
    }
}