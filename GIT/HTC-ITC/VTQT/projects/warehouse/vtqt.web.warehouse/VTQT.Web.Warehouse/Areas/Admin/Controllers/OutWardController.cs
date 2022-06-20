using Aspose.Cells;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Warehouse.Enum;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class OutWardController : AdminMvcController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private static readonly string jiraUrl = CommonHelper.GetAppSetting<string>("Jira:Url");
        private static readonly string jiraUserName = CommonHelper.GetAppSetting<string>("Jira:UserName");
        private static readonly string jiraPassword = CommonHelper.GetAppSetting<string>("Jira:Password");

        #endregion

        #region Ctor

        public OutWardController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<OutwardModel>("/outward/details", new { id = id }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            if (GetEnumDescription((OutwardReason)model.Reason) != null)
                model.ReasonDescription = GetEnumDescription((OutwardReason)model.Reason);
            return View(model);
        }

        public async Task<IActionResult> Create(string IdWareHouse, string keyJira, string linkJira)
        {
            var res = await ApiHelper.ExecuteAsync<OutwardModel>("/outward/create", null, Method.GET, ApiHosts.Warehouse);
            var code = ApiHelper.Execute("/generate-code/get?tableName=Outward", null, Method.GET, ApiHosts.Master);

            var model = res.data;

            if (model != null)
            {
                model.VoucherCode = code.data;
                model.VoucherDate = DateTime.Now;
                model.CreatedDate = DateTime.Now;
                if (!string.IsNullOrEmpty(IdWareHouse))
                    model.WareHouseID = IdWareHouse;

                model.CreatedBy = _workContext.UserId;

                if (!string.IsNullOrEmpty(keyJira) &&
                    !string.IsNullOrEmpty(linkJira) &&
                    keyJira != "undefined" &&
                    linkJira != "undefined")
                {
                    model.References = new List<Reference>
                    {
                        new Reference
                        {
                            Text = keyJira,
                            Link = new Uri(linkJira)
                        }
                    };
                    model.Reference = JsonConvert.SerializeObject(model.References);
                }
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSave(OutwardModel model, IEnumerable<OutwardDetailModel> modelDetails)
        {
            if (model.WareHouseID.Equals(model.ToWareHouseId))
            {
                NotifyError(string.Format(T("Common.Notify.AlreadyExist"), T("Common.Fields.ToWareHouseId")));
                return Ok(new XBaseResult { success = false });
            }

            foreach (var item in modelDetails)
            {
                item.AccountMore = "1541.66";
                item.AccountYes = "1561";
                item.Amount = item.UIQuantity * item.UIPrice;
                var convertRate = await ApiHelper.ExecuteAsync($"/warehouse-item-unit/get-convert-rate?itemId={item.ItemId}&unitId={item.UnitId}", null, Method.GET, ApiHosts.Warehouse);
                item.Quantity = convertRate.totalCount * item.UIQuantity;
                item.Price = item.Amount;

                if (!string.IsNullOrEmpty(item.Serial))
                {
                    var serials = item.Serial.Split(',');
                    foreach (var s in serials)
                    {
                        item.SerialWareHouses.Add(new SerialWareHouseModel
                        {
                            ItemId = item.ItemId,
                            Serial = s
                        });
                    }
                }
            }

            model.OutwardDetails = modelDetails.ToList();

            var res = await ApiHelper.ExecuteAsync("/outward/create", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            if (!string.IsNullOrEmpty(model.Reference))
            {
                model.References = JsonConvert.DeserializeObject<List<Reference>>(model.Reference);
                if (model.References?.Count > 0)
                {
                    var transition = new UpdateStatusJira
                    {
                        Transition = new Transition
                        {
                            Id = 41 // transition id of "Done" status jira is 41
                        }
                    };
                    var jsonBodyUpdateStatusJira = JsonConvert.SerializeObject(transition);
                    List<Task> updateStatusJira = new List<Task>();
                    var client = new RestClient(jiraUrl)
                    {
                        Authenticator = new HttpBasicAuthenticator(jiraUserName, jiraPassword)
                    };

                    model.References.ForEach(r =>
                    {
                        var request = new RestRequest($"/rest/api/2/issue/{r.Text}/transitions?expand=transitions.fields", Method.POST);
                        request.AddParameter("application/json", jsonBodyUpdateStatusJira, RestSharp.ParameterType.RequestBody);
                        updateStatusJira.Add(client.ExecuteAsync(request));
                    });

                    if (updateStatusJira?.Count > 0)
                    {
                        await Task.WhenAll(updateStatusJira);
                    }
                }
            }
            return Ok(new XBaseResult { 
                data = res.data
            });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<OutwardModel>("/outward/edit", new { id = id }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditSave(OutwardModel model)
        {
            model.ModifiedBy = _workContext.UserId;
            if (model.WareHouseID.Equals(model.ToWareHouseId))
            {
                NotifyError(string.Format(T("Common.Notify.AlreadyExist"), T("Common.Fields.ToWareHouseId")));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/outward/edit", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> Deletes(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/outward/deletes", ids, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }
        public List<SelectListItem> GetListAccountIdentifier()
        {
            var tmpPath = CommonHelper.MapPath("/wwwroot/Templates/Excel/He_thong_tai_khoan kế toán.xlsx");
            Workbook wb = new Workbook(tmpPath);
            //Get the first worksheet.
            Worksheet worksheet = wb.Worksheets[0];
            //Get the cells collection.
            Cells cells = worksheet.Cells;

            //Define the list.
            var list = new List<SelectListItem>(); //Get the AA column index. (Since "Status" is always @ AA column.
            int col = CellsHelper.ColumnNameToIndex("A");
            //  int col2 = CellsHelper.ColumnNameToIndex("B");

            //Get the last row index in AA column.
            int last_row = worksheet.Cells.GetLastDataRow(col);

            //Loop through the "Status" column while start collecting values from row 9
            //to save each value to List
            for (int i = 2; i < 259; i++)
            {
                //    myList.Add(cells[i, col].Value.ToString(), cells[i, col + 1].Value.ToString());
                var code = cells[i, col].Value.ToString() == null ? "" : cells[i, col].Value.ToString();
                var name = cells[i, col + 1].Value.ToString() == null ? "" : cells[i, col + 1].Value.ToString();
                var tem = new SelectListItem();
                tem.Text = $"[{code.Trim()}] {name.Trim()}";
                tem.Value = code.Trim();
                list.Add(tem);
            }

            return list;
        }
        public async Task<IActionResult> AddItem(string AccObjectId)
        {
            var res = await ApiHelper.ExecuteAsync<OutwardDetailModel>("/outward/detail-create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            model.AvailableUnits.Clear();
            model.AvailableAccountMores = GetListAccountIdentifier();
            model.AvailableAccountYes = GetListAccountIdentifier();
            model.AccountMore = "1541.66";
            model.AccountYes = "1561";
            model.AccObjectId = AccObjectId;
            return View(model);
        }

        public async Task<IActionResult> CreateItem(string outwardId,string AccObjectId)
        {
            var res = await ApiHelper.ExecuteAsync<OutwardDetailModel>("/outward/detail-create", null, Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            model.OutwardId = outwardId;
            model.AvailableAccountMores = GetListAccountIdentifier();
            model.AvailableAccountYes = GetListAccountIdentifier();
            model.AccountMore = "1541.66";
            model.AccountYes = "1561";
            model.AccObjectId = AccObjectId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(OutwardDetailModel model)
        {
            model.AccountMore = "1541.66";
            model.AccountYes = "1561";
            var convertRate = await ApiHelper.ExecuteAsync($"/warehouse-item-unit/get-convert-rate?itemId={model.ItemId}&unitId={model.UnitId}", null, Method.GET, ApiHosts.Warehouse);

            model.Amount = model.UIQuantity * model.UIPrice;
            model.Quantity = convertRate.totalCount * model.UIQuantity;
            model.Price = model.Amount;

            var res = await ApiHelper.ExecuteAsync("/outward/detail-create", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditItem(string id,string AccObjectId)
        {
            var res = await ApiHelper.ExecuteAsync<OutwardDetailModel>("/outward/detail-edit", new { id = id }, Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            model.AvailableAccountMores = GetListAccountIdentifier();
            model.AvailableAccountYes = GetListAccountIdentifier();
            model.AccObjectId = AccObjectId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(OutwardDetailModel model)
        {
            model.AccountMore = "1541.66";
            model.AccountYes = "1561";
            var convertRate = await ApiHelper.ExecuteAsync($"/warehouse-item-unit/get-convert-rate?itemId={model.ItemId}&unitId={model.UnitId}", null, Method.GET, ApiHosts.Warehouse);

            model.Amount = model.UIQuantity * model.UIPrice;
            model.Quantity = convertRate.totalCount * model.UIQuantity;
            model.Price = model.Amount;


            var res = await ApiHelper.ExecuteAsync("/outward/detail-edit", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItems(IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync("/Outward/detail-deletes", ids, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        [HttpPost]
        public async Task<IActionResult> CheckUIQuantity(string WareHouseId, string ItemId)
        {
            var res = await ApiHelper.ExecuteAsync($"/inward/check-ui-quantity?IdWareHouse={WareHouseId}&IdItem={ItemId}", null, Method.GET, ApiHosts.Warehouse);
           return Ok(new XBaseResult { success =true,data=res });
        }

        public async Task<IActionResult> CheckItemInwardDetail(string id)
        {
            var result = new DataSourceResult
            {
                Data = null
            };
            if (string.IsNullOrEmpty(id))
            {
                return Ok(result);
            }

            var idItem = await ApiHelper.ExecuteAsync<InwardDetailModel>($"/inward/detail-get?InwardId={id}", null, Method.GET, ApiHosts.Warehouse);
            result.Data = idItem.data.ItemId;
            return Ok(result);
        }
        #endregion

        #region Lists

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Details_Read([DataSourceRequest] DataSourceRequest request, OutwardDetailSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<OutwardDetailModel>>("/outward/detail-get", searchModel, Method.GET, ApiHosts.Warehouse);
            var data = res.data;

            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count : res.totalCount
            };
            return Ok(result);
        }

        public async Task<IActionResult> GetUnitByItemId(string itemId, string warehouseId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return Ok(new XBaseResult
                {
                    data = null,
                    success = false
                });
            }

            var resCheckItem = await ApiHelper
                .ExecuteAsync($"/inward/check-item-exist?itemId={itemId}&warehouseId={warehouseId}", null, Method.POST, ApiHosts.Warehouse);            

            bool checkItem = resCheckItem.data;

            if (checkItem) {                
                var idUnit = await ApiHelper.
                    ExecuteAsync<WareHouseItemModel>($"/warehouse-item/get-by-id?id={itemId}", null, Method.GET, ApiHosts.Warehouse);

                var unitId = idUnit.data.UnitId;
                return Ok(new XBaseResult{ 
                    data = unitId,
                    success = true
                });
            }

            return Ok(new XBaseResult
            {
                success = false,
                data = checkItem
            });
        }
        #endregion

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}