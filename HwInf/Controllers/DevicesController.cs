﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HwInf.Models;
using HwInf.Common.DAL;

namespace HwInf.Controllers
{
    [RoutePrefix("api/devices")]
    public class DevicesController : ApiController
    {
        private HwInfContext db = new HwInfContext();

        // GET: api/devices/all
        /// <summary>
        /// Returns a List of all Devices
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IEnumerable<DeviceViewModel> GetAll()
        {

            var devices = db.Devices.Include(x => x.Type);

            var json = devices
                .Where(i => i.DeviceId > 0)
                .Take(10000)
                .ToList() // execl SQL
                .Select(i => new DeviceViewModel(i).loadMeta(db)) // Convert to viewmodel
                .ToList();

            return json;
        }

        // GET: api/devices/filter/{type}/{filters?}
        /// <summary>
        /// Filters the Devices with given Parameters
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filters"></param>
        /// <returns></returns>

        // [Authorize]
        [Route("{type}/{filters?}")]
        public IEnumerable<DeviceViewModel> GetFilter(string type, string filters = "all")
        {

            var devices = db.Devices.Include(x => x.Type);

            var data = devices
                .Where(i => i.Type.Name.ToLower().Contains(type.ToLower()))
                .Take(10000)
                .ToList() // execl SQL
                .Select(i => new DeviceViewModel(i).loadMeta(db)) // Convert to viewmodel
                .ToList();

            var json = new List<DeviceViewModel>();
            json = data.ToList();
            if (filters != "all")
            {
                var parameters = filters.Split('|');
                json.Clear();
                foreach (var m in parameters)
                {
                    json = new List<DeviceViewModel>(json.Union(data.Where(i => i.DeviceMetaData.Values.Any(v => v.ToLower().Contains(m.ToLower()))).ToList()));
                    json = new List<DeviceViewModel>(json.Union(data.Where(i => i.Brand.ToLower() == m.ToLower()))); 
                }
            }

            return json;
        }

        // GET: api/devices/filter/types
        /// <summary>
        /// Returns all DeviceTypes
        /// </summary>
        /// <returns></returns>
        [Route("types")]
        public IEnumerable<string> GetDeviceTypes()
        {
            var deviceTypes = db.DeviceTypes;


            var typesList = deviceTypes
                .Select(i => i.Name)
                .ToList();

            return typesList;
        }

        // GET: api/devices/filter/types/{type}
        /// <summary>
        /// Returns all MetaKeys of a DeviceType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("types/{type}")]
        public IEnumerable<string> GetFilters(string type)
        {
            var devices = db.Devices.Include(x => x.Type);
            var meta = db.DeviceMeta.Include(x => x.DeviceType);

            var deviceFilters = new List<string>();
            deviceFilters.Add("Marke");

            deviceFilters = new List<string>(deviceFilters
                .Union(meta
                    .Where(i => i.DeviceType.Name.ToLower() == type.ToLower())
                    .Select(i => i.MetaKey)
                    .Distinct()
                    .ToList()
                    )
                );

            return deviceFilters;
        }

        // GET: api/devices/filter/types/{type}/{filterKey}
        /// <summary>
        /// Returns all MetaValues of a Filter
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        [Route("types/{type}/{filterKey}")]
        public IEnumerable<string> GetFiltersValues(string type, string filterKey)
        {
            var filterValues = new List<string>();
            if (filterKey.ToLower() == "marke")
            {
                var devices = db.Devices.Include(x => x.Type);
                filterValues = devices
                    .Where(i => i.Type.Name.ToLower() == type.ToLower())
                    .OrderBy(i => i.Brand)
                    .Select(i => i.Brand)
                    .Distinct()
                    .ToList();

                filterValues.Sort();
            } else
            {
                var meta = db.DeviceMeta.Include(x => x.DeviceType);
                filterValues = meta
                    .Where(i => i.DeviceType.Name.ToLower() == type.ToLower())
                    .Where(i => i.MetaKey.ToLower() == filterKey.ToLower())
                    .OrderBy(i => i.MetaValue)
                    .Select(i => i.MetaValue)
                    .Distinct()
                    .ToList();

                filterValues.Sort();
            }

            return filterValues;
        }

        // GET: api/devices/{id}
        /// <summary>
        /// Returns device of given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [Route("id/{id}")]
        public IHttpActionResult GetDevice(int id)
        {
            var devices = db.Devices.Include(x => x.Type);
            var json = devices
             .Where(i => i.DeviceId == id)
             .ToList() // execl SQL
             .Select(i => new DeviceViewModel(i).loadMeta(db)) // Convert to viewmodel
             .ToList();

            if (json == null)
            {
                return NotFound();
            }

            return Ok(json);
        }

        // PUT: api/Devicee/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Dev"></param>
        /// <returns></returns>
        //[Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDevice(int id, DBDevice Dev)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Dev.DeviceId)
            {
                return BadRequest();
            }

            db.Entry(Dev).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/devices/create
        /// <summary>
        /// Creates a new Device in Database
        /// </summary>
        /// <param name="vmdl"></param>
        /// <returns></returns>
        //[Authorize]
        [Route("create")]
        [ResponseType(typeof(DBDevice))]
        public IHttpActionResult PostDevice([FromBody]DeviceViewModel vmdl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(db.Devices.Count(i => i.InvNum == vmdl.InvNum) > 0)
            {
                return BadRequest("Device already exists!");
            }

            DBDevice dev = new DBDevice();
            dev.Name = vmdl.Name;
            dev.InvNum = vmdl.InvNum;
            dev.Status = vmdl.Status;
            dev.Type = db.DeviceTypes.Single(i => i.TypeId == vmdl.TypeId);

            db.Devices.Add(dev);

            foreach (var m in vmdl.DeviceMetaData)
            {
                db.DeviceMeta.Add(new DBDeviceMeta
                {
                    MetaKey = m.Key,
                    MetaValue = m.Value,
                    Device = dev,
                    DeviceType = dev.Type
                });
            }

            db.SaveChanges();

            return Ok(vmdl);
        }

        // DELETE: api/devicee/{id}
        /// <summary>
        /// Deletes the device with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(DBDevice))]
        public IHttpActionResult DeleteDevice(int id)
        {
            DBDevice Dev = db.Devices.Find(id);
            if (Dev == null)
            {
                return NotFound();
            }

            db.Devices.Remove(Dev);
            db.SaveChanges();

            return Ok(Dev);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DeviceExists(int id)
        {
            return db.Devices.Count(e => e.DeviceId == id) > 0;
        }
    }

}