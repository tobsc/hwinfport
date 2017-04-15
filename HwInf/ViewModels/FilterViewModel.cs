﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Compilation;
using HwInf.Common.BL;
using HwInf.Common.Models;
using MigraDoc.DocumentObjectModel.Shapes.Charts;

namespace HwInf.ViewModels
{
    public class FilterViewModel
    {

        public string DeviceType { get; set; }
        public string Order { get; set; }
        public string OrderBy { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public ICollection<DeviceMetaViewModel> MetaQuery { get; set; }


        public FilterViewModel()
        {
            
        }


        public ICollection<Device> FilteredList(BL bl)
        {

            var metaViewModel = MetaQuery.Select(i =>
            {
                var deviceMeta = new DeviceMeta();
                i.ApplyValues(deviceMeta);
                return deviceMeta;

            }).ToList();

            var bla = bl.GetFilteredDevices(metaViewModel, DeviceType, Order, OrderBy, Offset, Limit)
                .ToList();


            return bla;
        }
    }
}