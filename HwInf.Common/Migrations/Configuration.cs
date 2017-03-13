﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO.Compression;
using System.Linq;
using HwInf.Common.Models;

namespace HwInf.Common.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<HwInf.Common.DAL.HwInfContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(HwInf.Common.DAL.HwInfContext context)
        {

            var compTypes = new List<ComponentType>
            {
                new ComponentType {FieldType = "text", Name = "TextFeld"},
                new ComponentType {FieldType = "textArea", Name = "TextArea"},
                new ComponentType {FieldType = "checkbox", Name = "Checkbox"},
                new ComponentType {FieldType = "list", Name = "Liste"}
            };

            var compN = new List<Component>
                {
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Prozessor"},
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Arbeitsspeicher" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Grafikkarte" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Festplatte" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "Checkbox"), Name = "DVD-Laufwerk" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Display" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Anschlüsse" }
                };

            var compM = new List<Component>
            {
                new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Bildschirmdiagonale" },
                new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Anschlüsse" }

            };

            var compPC = new List<Component>
                {
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Prozessor"},
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Arbeitsspeicher" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Grafikkarte" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Festplatte" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "Checkbox"), Name = "DVD-Laufwerk" },
                    new Component { ComponentType = compTypes.Single(i => i.Name == "TextFeld"), Name = "Anschlüsse" }
                };


            var type = new List<DeviceType>
                {
                    new DeviceType { Description = "Notebook", Components = compN.ToList()},
                    new DeviceType { Description = "PC", Components = compPC.ToList()},
                    new DeviceType { Description = "Monitor", Components = compM.ToList()}
                };


            var deviceStatus = new List<DeviceStatus>
                {
                    new DeviceStatus { Description = "Verfügbar" },
                    new DeviceStatus { Description = "Ausgeliehen" },
                    new DeviceStatus { Description = "In Reparatur" },
                };

            var orderStatus = new List<OrderStatus>
                {
                    new OrderStatus { Description = "Offen" },
                    new OrderStatus { Description = "Akzeptiert" },
                    new OrderStatus { Description = "Abgelehnt" },
                    new OrderStatus { Description = "Abgeschlossen" }
                };

            var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" },
                    new Role { Name = "Owner" }
                };

            var persons = new List<Person>
                {
                    new Person { Name = "Jan", LastName = "Calanog", Email = "jan.calanog@technikum-wien.at", Role = roles.Single(i => i.Name == "Admin"), uid = "if15b042" },
                    new Person { Name = "Tobias", LastName = "Schlachter", Email = "tobias.schlachter@technikum-wien.at", Role = roles.Single(i => i.Name == "Admin"), uid = "if15b032" },
                    new Person { Name = "Valentin", LastName = "Sagl", Email = "valentin.sagl@technikum-wien.at", Role = roles.Single(i => i.Name == "Admin"), uid = "if15b030" },
                    new Person { Name = "Sebastian", LastName = "Slowak", Email = "sebastian.slowak@technikum-wien.at", Role = roles.Single(i => i.Name == "Admin"), uid = "if15b049" },
            };

            var devMetaPC1 = new List<DeviceMeta>
            {
                new DeviceMeta { Component = compPC.Single(i => i.Name == "Prozessor"), MetaValue = "Intel Core i7-6500" },
                new DeviceMeta { Component = compPC.Single(i => i.Name == "Anschlüsse"), MetaValue = "USB 2.0, USB 3.0" },
                new DeviceMeta { Component = compPC.Single(i => i.Name == "Grafikkarte"), MetaValue = "NVIDIA GeForce 1080" },
                new DeviceMeta { Component = compPC.Single(i => i.Name == "Arbeitsspeicher"), MetaValue = "16GB" }
            };

            var devMetaPC2 = new List<DeviceMeta>
            {
                new DeviceMeta { Component = compPC.Single(i => i.Name == "Prozessor"), MetaValue = "Intel Core i5-12345" },
                new DeviceMeta { Component = compPC.Single(i => i.Name == "Anschlüsse"), MetaValue = "USB 2.0" },
                new DeviceMeta { Component = compPC.Single(i => i.Name == "Grafikkarte"), MetaValue = "NVIDIA GeForce 1060" },
                new DeviceMeta { Component = compPC.Single(i => i.Name == "Arbeitsspeicher"), MetaValue = "8GB" }
            };

            var dev = new List<Device>
               {
                new Device { Name = "Acer PC", Brand = "Acer", Status = deviceStatus.Single(i => i.Description == "Verfügbar"), InvNum = "a5123", Type = type.Single(i => i.Description == "PC"), CreateDate = DateTime.Now, Room = "A0.00", Person = persons.Single(i => i.LastName == "Calanog"), IsActive = true, DeviceMeta = devMetaPC1.ToList()},
                new Device { Name = "Medion PC", Brand = "Medion", Status = deviceStatus.Single(i => i.Description == "Verfügbar"), InvNum = "a57123", Type = type.Single(i => i.Description == "PC"), CreateDate = DateTime.Now, Room = "F0.00", Person = persons.Single(i => i.LastName == "Calanog"), IsActive = true, DeviceMeta = devMetaPC2.ToList()},
               };


            if (context.ComponentTypes.Count() < 1)
            {
                compTypes.ForEach(s => context.ComponentTypes.Add(s));
            }

            if (context.Components.Count() < 1)
            {
                compPC.ForEach(s => context.Components.Add(s));
                compN.ForEach(s => context.Components.Add(s));
                compM.ForEach(s => context.Components.Add(s));
            }

            if (context.DeviceTypes.Count() < 1)
            {
                type.ForEach(s => context.DeviceTypes.Add(s));
            }

            if (context.DeviceStatus.Count() < 1)
            {
                deviceStatus.ForEach(s => context.DeviceStatus.Add(s));
            }

            if (context.OrderStatus.Count() < 1)
            {
                orderStatus.ForEach(s => context.OrderStatus.Add(s));
            }

            if (context.Roles.Count() < 1)
            {
                roles.ForEach(s => context.Roles.Add(s));
            }

            if (context.Persons.Count() < 1)
            {
                persons.ForEach(s => context.Persons.Add(s));
            }

            if (context.Devices.Count() < 1)
            {
                dev.ForEach(s => context.Devices.Add(s));
            }



            base.Seed(context);
        }
    }
}
