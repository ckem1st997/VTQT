using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using VTQT.SharedMvc.Asset.Models;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Asset.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Web.Asset.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class HomeController : AdminMvcController
    {
        #region Fields



        #endregion

        #region Ctor

        public HomeController(
            )
        {
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region Lists


        [HttpGet]
        public IActionResult GetPie(int Type, string OrganizationId)
        {
            var res = ApiHelper.Execute<List<AssetModel>>("/asset-chart/get-chart-pie?type=" + Type + "&OrganizationId=" + OrganizationId + "", null, Method.GET, ApiHosts.Asset);
            var data = res.data;
            var result = new List<PieModel>();

            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    var tem = new PieModel();
                    tem.name = item.Description;
                    tem.y = item.OriginQuantity;
                    result.Add(tem);
                }
                return Ok(result);
            }
            var resultPie = new PieModel();
            resultPie.name = T("Common.Notication.PieNone").Text;
            result.Add(resultPie);
            return Ok(result);
        }


        [HttpGet]
        public IActionResult GetColumn(int Type,string OrganizationId)
        {
            var res = ApiHelper.Execute<List<AssetModel>>("/asset-chart/get-chart-column?type=" + Type + "&OrganizationId=" + OrganizationId + "", null, Method.GET, ApiHosts.Asset);
            var data = res.data;
            var result = new List<PieModel>();

            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    var tem = new PieModel();
                    tem.name = item.Name;
                    tem.y = item.OriginQuantity;
                    result.Add(tem);
                }
                return Ok(result);
            }
            var resultPie = new PieModel();
            resultPie.name = T("Common.Notication.PieNone").Text;
            result.Add(resultPie);
            return Ok(result);

        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetWarrantyDuration([DataSourceRequest] DataSourceRequest request, AssetSearchModel model)
        {
            model.BindRequest(request);
            var res = ApiHelper.Execute<List<AssetModel>>("/asset-chart/get-warranty-duration", model, Method.POST, ApiHosts.Asset);
            var data = res.data;
            var result = new List<WarrantyDurationModel>();

            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    var tem = new WarrantyDurationModel();
                    tem.Name = item.Name == null ? "" : item.Name;
                    tem.WhereAsset = item.OrganizationUnitName == null ? "" : item.OrganizationUnitName;
                    tem.LimitDate = item.CreatedBy.Split(" ") == null ? "" : item.CreatedBy.Split(" ")[0];
                    tem.BalanceQuantity = item.OriginQuantity;
                    tem.CategoryId = item.CategoryId;
                    tem.Code = item.Code;
                    result.Add(tem);
                }
            }
            var results = new DataSourceResult
            {
                Data = result,
                Total = res.totalCount
            };
            return Ok(results);
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetProjectBase([DataSourceRequest] DataSourceRequest request, AssetSearchModel model)
        {
            model.BindRequest(request);
            var res = ApiHelper.Execute<List<AssetModel>>("/asset-chart/get-project-base", model, Method.POST, ApiHosts.Asset);
            var data = res.data;
            var result = new List<ProjectBaseModel>();

            if (data?.Count > 0)
            {
                foreach (var item in data)
                {
                    var tem = new ProjectBaseModel();
                    tem.OrganizationUnitName = item.OrganizationUnitName == null ? "" : item.OrganizationUnitName;
                    tem.Quantity = item.OriginQuantity;
                    tem.BrokenQuantity = item.BrokenQuantity;
                    tem.EndWarrantyDuration = item.Status;
                    tem.SoldQuantity = item.SoldQuantity;
                    tem.RecallQuantity = item.RecallQuantity;
                    result.Add(tem);
                }
            }
            var results = new DataSourceResult
            {
                Data = result,
                Total = res.totalCount
            };
            return Ok(results);
        }

        [HttpGet]
        public IActionResult GetAssetCategory()
        {
            var list = new List<SelectItem>();
            var tem = new SelectItem();
            tem.text = "Hành chính";
            tem.id = 10;
            list.Add(tem);
            var tem2 = new SelectItem();
            tem2.text = "Hạ tầng";
            tem2.id = 20;
            list.Add(tem2);
            var tem3 = new SelectItem();
            tem3.text = "Dự án";
            tem3.id = 30;
            list.Add(tem3);
            return Ok(list);

        }


        [HttpGet]
        public IActionResult GetOrganization()
        {
            var organizations = ApiHelper.Execute<List<OrganizationModel>>("/organization/get-available", null, Method.GET, ApiHosts.Master);

            if (organizations.data != null)
            {
                var list = new List<SelectItem>();

                foreach (var item in organizations.data)
                {
                    var tem = new SelectItem();
                    tem.text = "[" + item.Code + "] " + item.Name + "";
                    tem.id = item.Id;
                    list.Add(tem);
                }
                return Ok(list);

            }


            return Ok(new List<SelectItem>());

        } 

        #endregion

        #region Helpers



        #endregion

        #region Utilities



        #endregion
    }
}
