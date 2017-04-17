﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HwInf.Common.DAL;
using HwInf.Common.BL;
using HwInf.Common.Models;
using HwInf.ViewModels;
using log4net;
using WebGrease.Css.Extensions;

namespace HwInf.Controllers
{
    [RoutePrefix("api/customfields")]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public class CustomFieldsController : ApiController
    {
        private readonly IDAL _db;
        private readonly BL _bl;

        public CustomFieldsController()
        {
            _db = new HwInfContext();
            _bl = new BL(_db);
        }

        // GET: api/admin/devices/groups
        /// <summary>
        /// Returns all Field Groups
        /// </summary>
        /// <param name="input">String</param>
        /// <param name="entity">Entity Name (deviceType, fieldGroup)</param>
        /// <returns></returns>
        [Route("slug")]
        public IHttpActionResult GetSlug(string input, string entity)
        {
            return Ok(SlugGenerator.GenerateSlug(_bl, input, entity));
        }

        // GET: api/admin/devices/groups
        /// <summary>
        /// Returns all Field Groups
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(IQueryable<FieldGroupViewModel>))]
        [Route("fieldgroups")]
        public IHttpActionResult GetGroups()
        {

            var vmdl = _bl.GetFieldGroups()
                .ToList()
                .Select(i => new FieldGroupViewModel(i));

            return Ok(vmdl);
        }


        // POST: api/admin/devices/types
        /// <summary>
        /// Return FieldGroups of a DeviceType
        /// </summary>
        /// <returns>List of FieldGroups</returns>
        [ResponseType(typeof(FieldGroupViewModel))]
        [Route("fieldgroups/{typeSlug}")]
        public IHttpActionResult GetGroupsOfDeviceType(string typeSlug)
        {
            var dt = _bl.GetDeviceType(typeSlug);

            var vmdl = _bl.GetFieldGroups()
                .ToList()
                .Where(i => i.DeviceTypes.Contains(dt))
                .Select(i => new FieldGroupViewModel(i));

            return Ok(vmdl);
        }

        // POST: api/admin/devices/groups
        /// <summary>
        /// Add new FieldGroup
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(DeviceViewModel))]
        [Route("fieldgroups")]
        public IHttpActionResult PostGroup(FieldGroupViewModel vmdl)
        {
            var obj = _bl.CreateFieldGroup();
            vmdl.ApplyChanges(obj, _bl);
            _bl.SaveChanges();
            return Ok(vmdl);
        }

        // POST: api/admin/devices/groups/fields
        /// <summary>
        /// Add new Field to FieldGroup
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(DeviceViewModel))]
        [Route("fields")]
        public IHttpActionResult PostField(string groupSlug, FieldViewModel vmdl)
        {
            var obj = _bl.GetFieldGroups(groupSlug);
            _bl.UpdateFieldGroup(obj);

            var field =_bl.CreateField();
            vmdl.ApplyChanges(field, _bl);
            obj.Fields.Add(field);

            _bl.SaveChanges();
            vmdl.Refresh(field);

            return Ok(vmdl);
        }

        // POST: api/admin/devices/groups/types
        /// <summary>
        /// Add DeviceType to FieldGroup
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(DeviceViewModel))]
        [Route("fieldgroups/types")]
        public IHttpActionResult PostGroupType(string typeSlug, string groupSlug)
        {

            var fg = _bl.GetFieldGroups(groupSlug);
            var dt = _bl.GetDeviceType(typeSlug);

            _bl.UpdateFieldGroup(fg);
            fg.DeviceTypes.Add(dt);
            _bl.SaveChanges();

            return Ok();
        }


        // PUT: api/Devicee/5
        /// <summary>
        /// Update a Devices
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="vmdl"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut]
        [Route("fieldgroups/{slug}")]
        public IHttpActionResult PutFieldGroups(string slug, FieldGroupViewModel vmdl)
        {

            if (slug != vmdl.Slug)
            {
                return BadRequest();
            }

            try
            {
                var fg = _bl.GetFieldGroups(vmdl.Slug);
                _bl.UpdateFieldGroup(fg);
                var f = fg.Fields.ToList();
                f.ForEach(i => _bl.DeleteField(i));
                fg.Fields.Clear();
                vmdl.ApplyChanges(fg, _bl); 

                _bl.SaveChanges();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_bl.FieldGroupExists(vmdl.Slug))
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
        // GET: 
        /// <summary>
        /// Returns fieldgroups with used fields from type
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(FieldGroupViewModel))]
        [Route("filter/fieldgroups/")]
        public IHttpActionResult GetFieldGroupsUsedFields()
        {

            var vmdl = _bl.GetFieldGroups()
                .ToList()
                .Select(i => new FieldGroupViewModel(i))
                .ToList();


            var meta = _bl.GetFilteredDevices(null)
                .ToList()
                .Select(i => new DeviceViewModel(i).LoadMeta(i))
                .ToList()
                .SelectMany(i => i.DeviceMeta)
                .ToList()
                .Select(i => i.FieldSlug)
                .ToList();


            // Bad?
            var res = new List<FieldGroupViewModel>();

            vmdl.ForEach(i =>
            {
                var fieldGroup = new FieldGroupViewModel
                {
                    Name = i.Name,
                    Slug = i.Slug,
                    Fields = new List<FieldViewModel>()
                };

                i.Fields.ForEach(y =>
                {
                    if (!meta.Contains(y.Slug)) return;
                    fieldGroup.Fields.Add(y);
                });

                res.Add(fieldGroup);
            });

            return Ok(res);



        }

        // GET: 
        /// <summary>
        /// Returns fieldgroups with used fields from type
        /// </summary>
        /// <param name="typeSlug">DeviceType Slug</param>
        /// <returns></returns>
        [ResponseType(typeof(FieldGroupViewModel))]
        [Route("filter/fieldgroups/{typeslug}")]
        public IHttpActionResult GetFieldGroupsUsedFieldsType(string typeSlug)
        {
            var dt = _bl.GetDeviceType(typeSlug);

            var vmdl = _bl.GetFieldGroups()
                .ToList()
                .Where(i => i.DeviceTypes.Contains(dt))
                .Select(i => new FieldGroupViewModel(i))
                .ToList();


            var meta = _bl.GetFilteredDevices(null, typeSlug)
                .ToList()
                .Select(i => new DeviceViewModel(i).LoadMeta(i))
                .ToList()
                .SelectMany(i => i.DeviceMeta)
                .ToList()
                .Select(i => i.FieldSlug)
                .ToList();


            // Bad?
            var res = new List<FieldGroupViewModel>();

            vmdl.ForEach(i =>
            {
                var fieldGroup = new FieldGroupViewModel
                {
                    Name = i.Name,
                    Slug = i.Slug,
                    Fields = new List<FieldViewModel>()
                };

                i.Fields.ForEach(y =>
                {
                    if (!meta.Contains(y.Slug)) return;
                    fieldGroup.Fields.Add(y);
                });

                res.Add(fieldGroup);
            });

            return Ok(res);
        }

        // DELETE: 
        /// <summary>
        /// Delete Fieldgroup
        /// </summary>
        /// <param name="slug">DeviceType Slug</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin, Verwalter")]
        [ResponseType(typeof(FieldGroupViewModel))]
        [Route("fieldgroups/{slug}")]
        public IHttpActionResult DeleteFieldGroup(string slug)
        {
            try
            {
                var obj = _bl.GetFieldGroups(slug);
                _bl.DeleteFieldGroup(obj);
                _bl.SaveChanges();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_bl.FieldGroupExists(slug))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}