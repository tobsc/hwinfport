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
using HwInf.Common.BL;

namespace HwInf.Controllers
{
    [RoutePrefix("api/devices")]

    public class DevicesController : ApiController
    {
        private HwInfContext db = new HwInfContext();
        private BL _bl;

        public DevicesController()
        {
            _bl = new BL(db);
        }

        // GET: api/devices/all
        /// <summary>
        /// Returns a list of all devices
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(DeviceViewModel))]
        [Route("")]
        public IHttpActionResult GetAll()
        {
                var vmdl = _bl.GetDevices()
                    .ToList() // execl SQL
                    .Select(i => new DeviceViewModel(i).loadMeta(_bl)) // Convert to viewmodel
                    .ToList();

                return Ok(vmdl);
        }


        // GET: api/devices/{id}
        /// <summary>
        /// Returns device of given id
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <returns></returns>
        [ResponseType(typeof(DeviceViewModel))]
        [Route("id/{id}")]
        public IHttpActionResult GetDevice(int id)
        {
            try
            {
                var devices = db.Devices.Include(x => x.Type);
                var vmdl = devices
                 .Where(i => i.DeviceId == id)
                 .ToList() // execl SQL
                 .Select(i => new DeviceViewModel(i).loadMeta(_bl)) // Convert to viewmodel
                 .ToList();

                if (vmdl == null)
                {
                    return NotFound();
                }

                return Ok(vmdl);
            } catch
            {
                return InternalServerError();
            }
            
        }



        // GET: api/devices/{type}/{filters?}
        /// <summary>
        /// Filters the devices with given parameters
        /// </summary>
        /// <param name="type">Device Type</param>
        /// <returns></returns>

        [ResponseType(typeof(List<DeviceViewModel>))]
        [Route("{type}")]
        public IHttpActionResult GetFilter(string type)
        {
                var parameterQuery = Request.GetQueryNameValuePairs();
                var devices = db.Devices.Include(x => x.Type);


                var data = _bl.GetDevices(true, type)
                    .ToList() // execl SQL
                    .Select(i => new DeviceViewModel(i).loadMeta(_bl)) // Convert to viewmodel
                    .ToList();

                var response = new List<DeviceViewModel>();
                response = data.ToList();

                if (parameterQuery.Count() != 0)
                {
                    response.Clear();

                    var searchData = data.ToList();

                    foreach (var p in parameterQuery)
                    {
                        response.Clear();
                        var parameters = parameterQuery.Where(i => i.Key == p.Key).Select(i => i.Value).ToList();

                        foreach (var m in parameters)
                        {
                            response = new List<DeviceViewModel>(response.Union(searchData.Where(i => i.DeviceMetaData.Values.Any(v => v.ToLower() == m.ToLower())).ToList()));
                            response = new List<DeviceViewModel>(response.Union(searchData.Where(i => i.Marke.ToLower() == m.ToLower())));
                            response = new List<DeviceViewModel>(response.Union(searchData.Where(i => i.Name.ToLower().Contains(m.ToLower()))));
                        }

                        searchData = response.ToList();
                    }

                }
                return Ok(response.OrderBy(o => o.Marke).ToList());
           
        }

        // GET: api/devices/types
        /// <summary>
        /// Returns all device types
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(List<string>))]
        [Route("types")]
        public IHttpActionResult GetDeviceTypes()
        {
            try
            {
                var deviceTypes = db.DeviceTypes;


                var typesList = deviceTypes
                    .Select(i => i.Description)
                    .ToList();

                return Ok(typesList);
            } catch
            {
                return InternalServerError();
            }
           
        }

        // GET: api/devices/filter/components/{type}
        /// <summary>
        /// Returns all components of a device with their values
        /// </summary>
        /// <param name="type">Device Type</param>
        /// <returns></returns>
        [ResponseType(typeof(List<DeviceViewModel>))]
        [Route("components/{type}")]
        public IHttpActionResult GetComponents(string type)
        {
            try
            {
                var devices = db.Devices.Include(x => x.Type);
                var meta = db.DeviceMeta.Include(x => x.Component);
                var component = db.Components.Include(x => x.DeviceType);
                List<object> response = new List<object>();

                var brands = devices
                    .Where(i => i.Type.Description.ToLower() == type.ToLower())
                    .Select(i => i.Brand)
                    .Distinct()
                    .ToList();

                brands.Sort();

            IDictionary<string, object> brandList = new Dictionary<string, object>();
            brandList.Add("component", "Marke");
            brandList.Add("values", brands);

                response.Add(brandList);


                var deviceComponents = component
                        .Where(i => i.DeviceType.Description.ToLower() == type.ToLower())
                        .Select(i => i.Name)
                        .ToList();

                deviceComponents.Sort();


                foreach (var c in deviceComponents)
                {
                    var componentValues = meta
                        .Where(i => i.Component.DeviceType.Description.ToLower() == type.ToLower())
                        .Where(i => i.Component.Name.ToLower() == c.ToLower())
                        .OrderBy(i => i.MetaValue)
                        .Select(i => i.MetaValue)
                        .Distinct()
                        .ToList();

                    componentValues.Sort();
                    IDictionary<string, object> componentList = new Dictionary<string, object>();
                    componentList.Add("component", c);
                    componentList.Add("values", componentValues);
                    response.Add(componentList);

                }

                if(response.Count < 2)
                {
                    return NotFound();
                }

                return Ok(response);
            } catch
            {
                return InternalServerError();
            }
           
        }

        /// <summary>
        /// Returns component values filtered by device type, component and user input
        /// </summary>
        /// <param name="type">Device Type</param>
        /// <param name="component">Device Component (e.g. Marke, Name, Prozessor, etc)</param>
        /// <param name="input">Input string</param>
        /// <returns></returns>
        [ResponseType(typeof(IQueryable<string>))]
        [Route("components/{type}/{component}/{input}")]
        public IHttpActionResult GetComponentValues(string type, string component, string input)
        {
            try
            {
                var devices = db.Devices.Include(x => x.Type);
                var meta = db.DeviceMeta.Include(x => x.Component);

                if (component.ToLower() == "name")
                {
                    var componentNameValues = devices
                        .Where(i => i.Type.Description.ToLower() == type.ToLower())
                        .Where(i => i.Name.ToLower().Contains(input.ToLower()))
                        .OrderBy(i => i.Name)
                        .Select(i => i.Name)
                        .Distinct();

                    return Ok(componentNameValues);

                } else if (component.ToLower() == "marke" || component.ToLower() == "brand") {

                    var componentBrandValues = devices
                        .Where(i => i.Type.Description.ToLower() == type.ToLower())
                        .Where(i => i.Brand.ToLower().Contains(input.ToLower()))
                        .OrderBy(i => i.Brand)
                        .Select(i => i.Brand)
                        .Distinct();

                    return Ok(componentBrandValues);

                } else
                {
                    var componentMetaValues = meta
                        .Where(i => i.Component.DeviceType.Description.ToLower() == type.ToLower())
                        .Where(i => i.Component.Name.ToLower() == component.ToLower())
                        .Where(i => i.MetaValue.ToLower().Contains(input.ToLower()))
                        .OrderBy(i => i.MetaValue)
                        .Select(i => i.MetaValue)
                        .Distinct();

                    return Ok(componentMetaValues);
                }


            }
            catch
            {
                return InternalServerError();
            }

        }


        // GET: api/devices/types/type
        /// <summary>
        /// Get all component fields from a device type
        /// </summary>
        /// <param name="type">Type Name</param>
        /// <returns></returns>
        //[Authorize]
        [Authorize(Roles = "Admin")]
        [Route("types/{type}")]
        [ResponseType(typeof(int))]
        public IHttpActionResult GetTypeComponents(string type)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var componetns = db.Components.Include("DeviceType");

            var fields = componetns.Where(i => i.DeviceType.Description.ToLower() == type.ToLower()).ToDictionary(i => i.Name, i => i.FieldType);

            return Ok(fields);
        }


        /// <summary>
        /// Returns DeviceStatus
        /// </summary>
        /// <returns></returns>
        [Route("status")]
        public IHttpActionResult GetStatus()
        {
            return Ok(db.DeviceStatus.ToList());
        }

        // POST: api/devices/create
        /// <summary>
        /// Creates a new device
        /// </summary>
        /// <param name="vmdl">Name, Marke, InvNum, TypeId, StatusId, RoomId, OwnerUid, DeviceMetaData</param>
        /// <returns></returns>
        //[Authorize]
        [Route("create")]
        [ResponseType(typeof(int))]
        public IHttpActionResult PostDevice([FromBody]DeviceViewModel vmdl)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(String.IsNullOrWhiteSpace(vmdl.Name))
            {
                return BadRequest("Bitte einen Namen für das Gerät angeben.");
            }

            if (String.IsNullOrWhiteSpace(vmdl.Marke))
            {
                return BadRequest("Bitte eine Marke für das Gerät angeben.");
            }

            if (db.Devices.Count(i => i.InvNum == vmdl.InvNum) > 0)
            {
                return BadRequest("Es existiert bereits ein Gerät mit dieser Inventarnummer.");
            }

            if(db.DeviceTypes.Count(i => i.TypeId == vmdl.TypeId) == 0)
            {
                return BadRequest("Type nicht vorhanden.");
            }

            if (db.DeviceStatus.Count(i => i.StatusId == vmdl.StatusId) == 0)
            {
                return BadRequest("Status nicht vorhanden.");
            }

            if(db.Persons.Count(i => i.uid == vmdl.OwnerUid) == 0)
            {
                return BadRequest("Person nicht vorhanden.");
            }

            
            Device device = vmdl.CreateDevice(_bl);

            return Ok(device.DeviceId);
        }

        // POST: api/devices/createdevicetype
        /// <summary>
        /// Creates a new device type
        /// </summary>
        /// <param name="vmdl">Type Name, Fields</param>
        /// <returns></returns>
        //[Authorize]
        [Authorize(Roles="Admin")]
        [Route("types/create")]
        [ResponseType(typeof(int))]
        public IHttpActionResult PostCreateDeviceType([FromBody]DeviceTypeViewModel vmdl)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
      
            DeviceType deviceType = vmdl.CreateDeviceType(_bl);

            return Ok(deviceType.TypeId);
        }

        // DELETE: api/devicee/{id}
        /// <summary>
        /// Deletes the device with the given id
        /// </summary>
        /// <param name="id">Device ID</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteDevice(int id)
        {
            if(!_bl.DeviceExists(id))
            {
                return NotFound();
            }
            else
            {
                _bl.DeleteDevice(id);
            }

            return Ok();
        }


        // PUT: api/Devicee/5
        /// <summary>
        /// NOT IMPLEMENTED!
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Dev"></param>
        /// <returns></returns>
        //[Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDevice(int id, Device Dev)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Dev.DeviceId)
            {
                return BadRequest();
            }

            try
            {
                _bl.UpdateDevice(Dev);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_bl.DeviceExists(id))
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}