using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Warehouse;
using VTQT.SharedMvc.Warehouse;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Warehouse.Controllers
{
    [Route("print")]
    [ApiController]
    //[XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("WareHouse.Controllers.Print")]
    public class PrintController : AdminApiController
    {

        private readonly IPrint _print;
        public PrintController(IPrint print) { _print = print; }

        [Route("index")]
        [HttpGet]
        [AppApiAction("WareHouse.AppActions.Prints.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu phiếu nhập theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-id-to-in")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByToWordIn(string id)
        {
            var entity = await _print.GetByIdToWordInAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Inward"))
                });

            var model = entity.ToModel();
            model.VoucherDate = entity.VoucherDate.ToLocalTime();
            model.CreatedBy = entity.CreatedBy;
            model.Voucher = entity.Voucher;
            model.ModifiedBy = entity.ModifiedBy;
            model.CreatedDate = entity.CreatedDate.ToLocalTime();
            model.ModifiedDate = entity.ModifiedDate.ToLocalTime();            
            model.WareHouse = entity.WareHouse == null ? new WareHouseModel
            {
                Name = ""
            } :
           new WareHouseModel
           {
               Id = entity.WareHouse.Id,
               Name = entity.WareHouse.Name,
               Address = entity.WareHouse.Address
           };
            model.Vendor = entity.Vendor == null ? new VendorModel
            {
                Name = ""
            } :
            new VendorModel
            {
                Id = entity.Vendor.Id,
                Name = entity.Vendor.Name
            };
            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        [Route("get-by-id-to-audit")]
        [HttpGet]
        //[MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByToWordAudit(string id)
        {
            var entity = await _print.GetByIdToWordAuditAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Audit"))
                });

            var model = entity.ToModel();
            model.VoucherDate = entity.VoucherDate.ToLocalTime();
            model.CreatedBy = entity.CreatedBy;
            model.ModifiedBy = entity.ModifiedBy;
            model.StringVoucherDate=entity.VoucherDate.ToLocalTime().ToString();
            model.CreatedDate = entity.CreatedDate.ToLocalTime();
            model.ModifiedDate = entity.ModifiedDate.ToLocalTime();
            model.WareHouse = entity.WareHouse == null ? new WareHouseModel
            {
                Name = ""
            } :
           new WareHouseModel
           {
               Id = entity.WareHouse.Id,
               Name = entity.WareHouse.Name,
               Address = entity.WareHouse.Address
           };
            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }

        /// Lấy dữ liệu phiếu nhập theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Got successfully</response>
        [Route("get-by-id-to-out")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetByToWordOut(string id)
        {
            var entity = await _print.GetByIdToWordOutAsync(id);
            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.Inward"))
                });

            var model = entity.ToModel();
            model.VoucherDate = entity.VoucherDate.ToLocalTime();
            model.CreatedBy = entity.CreatedBy;
            model.VoucherCodeReality = entity.VoucherCodeReality;
            model.ModifiedBy = entity.ModifiedBy;
            model.CreatedDate = entity.CreatedDate.ToLocalTime();
            model.ModifiedDate = entity.ModifiedDate.ToLocalTime();            
            model.WareHouse = entity.WareHouse == null ? new WareHouseModel
            {
                Name = ""
            } :
           new WareHouseModel
           {
               Id = entity.WareHouse.Id,
               Name = entity.WareHouse.Name,
               Address = entity.WareHouse.Address
           };
            model.ToWareHouse = entity.ToWareHouse == null ? new WareHouseModel
            {
                Name = ""
            } :
           new WareHouseModel
           {
               Id = entity.ToWareHouse.Id,
               Name = entity.ToWareHouse.Name,
               Address = entity.ToWareHouse.Address
           };
            return Ok(new XBaseResult
            {
                success = true,
                data = model
            });
        }


        [Route("get-list-out-details-by-id")]
        [HttpGet]
        [ProducesResponseType(typeof(XBaseResult), StatusCodes.Status200OK)]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _print.GetByIdOutDetailsAsync(id);

            if (entity == null)
                return Ok(new XBaseResult
                {
                    success = false,
                    message = string.Format(
                        T("Common.Notify.DoesNotExist"),
                        T("Common.WareHouseLimit"))
                });
            var models = new List<OutwardDetailModel>();
            var entities = await _print.GetByIdOutDetailsAsync(id);
            foreach (var e in entities)
            {
                var m = e.ToModel();
               // m.ItemName = e.ItemId;
               // m.UnitName = e.UnitId;
                //m.Quantity = e.Quantity;
                models.Add(m);
            }
            return Ok(new XBaseResult
            {
                success = true,
                data = models
            });
        }

    }
}
