using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.SharedMvc.Warehouse.Models;
using VTQT.Utilities;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using VTQT.Web.Warehouse.Models;
using System.ComponentModel;
using System.Reflection;
using Aspose.Cells;
using VTQT.Core.Domain.Warehouse.Enum;
using Newtonsoft.Json;
using RestSharp.Authenticators;

namespace VTQT.Web.Warehouse.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class InwardController : AdminMvcController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private static readonly string jiraUrl = CommonHelper.GetAppSetting<string>("Jira:Url");
        private static readonly string jiraUserName = CommonHelper.GetAppSetting<string>("Jira:UserName");
        private static readonly string jiraPassword = CommonHelper.GetAppSetting<string>("Jira:Password");

        #endregion Fields

        #region Ctor

        public InwardController(IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion Ctor

        #region Methods

        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<InwardModel>("/inward/details", new { id = id }, Method.GET,
                ApiHosts.Warehouse);

            var model = res.data;
            if (GetEnumDescription((InwardReason)model.Reason) != null)
                model.ReasonDescription = GetEnumDescription((InwardReason)model.Reason);

            return View(model);
        }

        public async Task<IActionResult> Create(string IdWareHouse, string keyJira, string linkJira)
        {
            var res = await ApiHelper.ExecuteAsync<InwardModel>("/inward/create", null, Method.GET, ApiHosts.Warehouse);
            var code = ApiHelper.Execute("/generate-code/get?tableName=Inward", null, Method.GET, ApiHosts.Master);

            var model = res.data;

            if (model != null)
            {
                model.VoucherDate = DateTime.Now;
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = _workContext.UserId;
                model.VoucherCode = code.data;
                if (!string.IsNullOrEmpty(IdWareHouse))
                    model.WareHouseID = IdWareHouse;

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
        public async Task<IActionResult> CreateSave(InwardModel model, IEnumerable<InwardDetailModel> modelDetails)
        {
            foreach (var item in modelDetails)
            {
                item.AccountMore = "1541.66";
                item.AccountYes = "1561";
                item.Amount = item.UIQuantity * item.UIPrice;
                var convertRate = await ApiHelper.ExecuteAsync(
                    $"/warehouse-item-unit/get-convert-rate?itemId={item.ItemId}&unitId={item.UnitId}", null,
                    Method.GET, ApiHosts.Warehouse);
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

            model.InwardDetails = modelDetails.ToList();
            var res = await ApiHelper.ExecuteAsync("/inward/create", model, Method.POST, ApiHosts.Warehouse);
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
                        var request =
                            new RestRequest($"/rest/api/2/issue/{r.Text}/transitions?expand=transitions.fields",
                                Method.POST);
                        request.AddParameter("application/json", jsonBodyUpdateStatusJira,
                            RestSharp.ParameterType.RequestBody);
                        updateStatusJira.Add(client.ExecuteAsync(request));
                    });

                    if (updateStatusJira?.Count > 0)
                    {
                        await Task.WhenAll(updateStatusJira);
                    }
                }
            }

            return Ok(new XBaseResult
            {
                data = res.data
            });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<InwardModel>("/inward/edit", new { id = id }, Method.GET,
                ApiHosts.Warehouse);

            var model = res.data;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditSave(InwardModel model)
        {
            model.ModifiedBy = _workContext.UserId;
            var res = await ApiHelper.ExecuteAsync("/inward/edit", model, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/inward/deletes", ids, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> AddItem(string AccObjectId)
        {
            var res = await ApiHelper.ExecuteAsync<InwardDetailModel>("/inward/detail-create", null, Method.GET,
                ApiHosts.Warehouse);

            var model = res.data;
            model.AvailableUnits.Clear();
            model.AvailableAccountMores = GetListAccountIdentifier();
            model.AvailableAccountYes = GetListAccountIdentifier();
            model.AccObjectId = AccObjectId;
            model.AccountMore = "1541.66";
            model.AccountYes = "1561";
            return View(model);
        }

        public async Task<IActionResult> CreateItem(string inwardId, string AccObjectId)
        {
            var res = await ApiHelper.ExecuteAsync<InwardDetailModel>("/inward/detail-create", null, Method.GET,
                ApiHosts.Warehouse);

            var model = res.data;
            model.InwardId = inwardId;
            model.AvailableAccountMores = GetListAccountIdentifier();
            model.AvailableAccountYes = GetListAccountIdentifier();
            model.AccObjectId = AccObjectId;
            model.AccountMore = "1541.66";
            model.AccountYes = "1561";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(InwardDetailModel model)
        {
            model.AccountMore = "1541.66";
            model.AccountYes = "1561";
            model.Amount = model.UIQuantity * model.UIPrice;
            var convertRate = await ApiHelper.ExecuteAsync(
                $"/warehouse-item-unit/get-convert-rate?itemId={model.ItemId}&unitId={model.UnitId}", null, Method.GET,
                ApiHosts.Warehouse);
            model.Quantity = convertRate.totalCount * model.UIQuantity;
            model.Price = model.Amount;

            var res = await ApiHelper.ExecuteAsync("/inward/detail-create", model, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        public async Task<IActionResult> EditItem(string id, string AccObjectId)
        {
            var res = await ApiHelper.ExecuteAsync<InwardDetailModel>("/inward/detail-edit", new { id = id },
                Method.GET, ApiHosts.Warehouse);

            var model = res.data;
            model.AvailableAccountMores = GetListAccountIdentifier();
            model.AvailableAccountYes = GetListAccountIdentifier();
            model.AccObjectId = AccObjectId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(InwardDetailModel model)
        {
            model.AccountMore = "1541.66";
            model.AccountYes = "1561";
            model.Amount = model.UIQuantity * model.UIPrice;
            var convertRate = await ApiHelper.ExecuteAsync(
                $"/warehouse-item-unit/get-convert-rate?itemId={model.ItemId}&unitId={model.UnitId}", null, Method.GET,
                ApiHosts.Warehouse);
            model.Quantity = convertRate.totalCount * model.UIQuantity;
            model.Price = model.Amount;

            var res = await ApiHelper.ExecuteAsync("/inward/detail-edit", model, Method.POST, ApiHosts.Warehouse);
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

            var res = await ApiHelper.ExecuteAsync("/inward/detail-deletes", ids, Method.POST, ApiHosts.Warehouse);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }

        #endregion Methods

        #region Lists

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Details_Read([DataSourceRequest] DataSourceRequest request,
            InwardDetailSearchModel searchModel)
        {
            var res = await ApiHelper.ExecuteAsync<List<InwardDetailModel>>("/inward/detail-get", searchModel,
                Method.GET, ApiHosts.Warehouse);
            var result = new DataSourceResult();
            if (res.data == null)
            {
                return Ok(result);
            }

            var data = res.data;
            result.Data = data;
            result.Total = res.totalCount == 0 ? res.data.Count : res.totalCount;
            return Ok(result);
        }

        public async Task<IActionResult> GetUnitByItemId(string id)
        {
            var result = new DataSourceResult
            {
                Data = null
            };
            if (string.IsNullOrEmpty(id))
            {
                return Ok(result);
            }

            var idUnit = await ApiHelper.ExecuteAsync<WareHouseItemModel>($"/warehouse-item/get-by-id?id={id}", null,
                Method.GET, ApiHosts.Warehouse);
            result.Data = idUnit.data.UnitId;
            return Ok(result);
        }


        public async Task<IActionResult> GetWareHouseItemUnitByItemId(string id)
        {
            var getUnitItem = new WareHouseItemUnitSearchModel();
            getUnitItem.ItemId = id;
            var listItem = await ApiHelper.ExecuteAsync<List<WareHouseItemUnitModel>>("/warehouse-item-unit/get",
                getUnitItem, Method.GET, ApiHosts.Warehouse);
            var getItem = await ApiHelper.ExecuteAsync<WareHouseItemModel>($"/warehouse-item/get-by-id?id={id}", null,
                Method.GET, ApiHosts.Warehouse);
            var model = new List<SelectItem>();
            if (listItem.data.Any())
                foreach (var item in listItem.data)
                {
                    var tem = new SelectItem
                    {
                        text = item.UnitName,
                        id = item.UnitId
                    };
                    if (getItem.data != null && getItem.data.UnitId.Equals(item.UnitId))
                        tem.selected = true;
                    model.Add(tem);
                }

            return Ok(model);
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        #endregion Lists


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
    }
}