using System;
using VTQT.Web.Framework.Modelling;

namespace VTQT.SharedMvc.Dashboard.Models
{
    public class ExampleModel : BaseEntityModel
    {
        public string Name { get; set; }
        public int? NumOne { get; set; }
        public int? NumTwo { get; set; }
        public int? NumSum { get; set; }
        public DateTime? DateExample { get; set; }
    }
}