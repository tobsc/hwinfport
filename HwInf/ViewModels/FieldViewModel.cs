﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HwInf.Common.BL;
using HwInf.Common.Models;

namespace HwInf.ViewModels
{
    public class FieldViewModel
    {

        public int FieldId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }


        public FieldViewModel()
        {
            
        }

        public FieldViewModel(Field fg)
        {
            Refresh(fg);
        }

        public void Refresh(Field fg)
        {
            var target = this;
            var source = fg;

            target.Name = source.Name;
            target.Slug = source.Slug;
            target.FieldId = source.FieldId;
        }

        public void ApplyChanges(Field fg, BL bl)
        {
            var target = fg;
            var source = this;

            target.Name = source.Name;
            target.Slug = SlugGenerator.GenerateSlug(source.Name);
        }

        public static implicit operator Field(FieldViewModel vmdl)
        {
            return new Field
            {
                Name = vmdl.Name,
                Slug = SlugGenerator.GenerateSlug(vmdl.Name)
            };
        }
    }
}