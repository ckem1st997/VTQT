using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Aspose.Cells;
using Aspose.Pdf.Generator;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RestSharp;
using VTQT.Core;
using VTQT.Core.Logging;
using VTQT.SharedMvc.Dashboard.Models;
using VTQT.SharedMvc.Master.Models;
using VTQT.Utilities;
using VTQT.Web.Dashboard.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;
using Color = System.Drawing.Color;

namespace VTQT.Web.Dashboard.Areas.Admin.Controllers
{
    [XBaseMvcAuthorize]
    public class StorageValueController : AdminMvcController
    {
        #region Fields

        private readonly ILogger<StorageValueController> _logger;

        private const string MasterVTCNTTController = "master-vtcntt";
        private const string MasterVTCNTTGetAll = "get-all";
        private const string MasterVTCNTTAddOrUpdate = "add-or-edit";
        private const string ExampleController = "example";
        private const string ContentTypeXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string FTTHController = "ftth";
        private const string MasterFileKH2022Controller = "master-file-kh-2022";
        private const string FTTH2022Controller = "FTTH2022";
        private const string FTTHMB3132022Controller = "ftth-mb-31-3-2022";
        private const string FTTHAddOrUpdate = "add-or-edit";
        private const string FTTHGetAll = "get-all";
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public StorageValueController(IWorkContext workContext, ILogger<StorageValueController> logger)
        {
            _workContext = workContext;
            _logger = logger;
        }

        #endregion

        // GET

        public async Task<IActionResult> AddImportAsync()
        {
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/create", null, Method.GET,
                ApiHosts.Dashboard);
            if (res == null || res.data == null)
                return View(new AddImportModel());
            var model = new AddImportModel();
            // if (string.IsNullOrEmpty(idTypeValue))
            //     model.TypeValueId = idTypeValue;
            model.AvailableNameTable = res.data.AvailableNameTable;
            model.AvailableTypeValue = res.data.AvailableTypeValue;
            return View(model);
        }

        public async Task<IActionResult> UpdateImportAsync()
        {
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/create", null, Method.GET,
                ApiHosts.Dashboard);
            if (res == null || res.data == null)
                return View(new UpdateImportModels());
            var model = new UpdateImportModels();
            // if (string.IsNullOrEmpty(idTypeValue))
            //     model.TypeValueId = idTypeValue;
            model.AvailableNameTable = res.data.AvailableNameTable;
            model.AvailableTypeValue = res.data.AvailableTypeValue;
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            // var checkRole = await ApiHelper.ExecuteAsync<List<TypeValueModel>>(
            //     "/dashboard-user/check-role-user?idUser=" + _workContext.UserId + "", null, Method.GET,
            //     ApiHosts.Dashboard);
            //
            // if (!checkRole.success)
            //     return RedirectToAction("AccessDeniedPath", "TypeValue");
            var searchModel = new StorageValueSearchModel();

            var resLastSelected = await ApiHelper.ExecuteAsync<string>(
                $"/type-value/get-last-selected/?appId=9&userId={_workContext.UserId}&path=/Admin/StorageValue", null,
                Method.GET, ApiHosts.Dashboard);

            if (!string.IsNullOrEmpty(resLastSelected.data))
            {
                searchModel.TypeValueId = resLastSelected.data;
            }

            return View(searchModel);
        }


        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> Read([DataSourceRequest] DataSourceRequest request,
            StorageValueSearchModel searchModel)
        {
            searchModel.BindRequest(request);

            var res = await ApiHelper.ExecuteAsync<List<StorageValueModel>>("/storage-value/get", searchModel,
                Method.GET, ApiHosts.Dashboard);
            await ApiHelper.ExecuteAsync<string>(
                $"/type-value/update-last-selected/?appId=9&userId={_workContext.UserId}&path=/Admin/StorageValue&typeValueId={searchModel.TypeValueId}",
                null, Method.POST, ApiHosts.Dashboard);

            var data = res?.data;
            var result = new DataSourceResult
            {
                Data = data,
                Total = res.totalCount == 0 ? res.data.Count() : res.totalCount
            };
            return Ok(result);
        }


        public async Task<IActionResult> AddItem()
        {
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/create", null, Method.GET,
                ApiHosts.Dashboard);
            if (res == null || res.data == null)
                return View(new StorageValueModel());
            var model = res.data;
            var time = DateTime.UtcNow;
            model.TimeYear = time.Year;
            model.TimeMouth = time.Month;
            model.TimeDay = time.Day;
            return View(model);
        }


        public IActionResult AddTable()
        {
            return View(new AddTableModel());
        }


        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> AddItem(StorageValueModel model)
        {
            if (!ModelState.IsValid)
            {
                NotifyError(ModelState.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var resUser =
                await ApiHelper.ExecuteAsync<List<UserModel>>("/user/get-available", null, Method.GET, ApiHosts.Master);
            if (resUser is { data: { } } && !string.IsNullOrEmpty(model.VoucherBy))
                model.VoucherByName = resUser.data.FirstOrDefault(x => x.Id.Equals(model.VoucherBy))?.FullName;
            var res = await ApiHelper.ExecuteAsync("/storage-value/create", model, Method.POST, ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }


        public async Task<IActionResult> Details(string id)
        {
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/details", new { id = id },
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/edits", new { id = id },
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success || res.data == null)
            {
                NotifyError(res.message);
                return Ok(new XBaseResult { success = false });
            }

            // var checkRole = await ApiHelper.ExecuteAsync<List<TypeValueModel>>(
            //     "/dashboard-user/check?idUser=" + _workContext.UserId + "&idTypeValue=" + res.data.TypeValueId + "",
            //     null, Method.GET, ApiHosts.Dashboard);
            //
            // if (checkRole.success == false)
            // {
            //     NotifyError("Bạn không có quyền thực hiện thao tác này !");
            //     return Ok(new XBaseResult { success = false });
            // }


            var model = res.data;
            if (model.NameTableRefense != null && model.NameTableRefense.Split(',').Length > 0)
            {
                var tem = "[";
                for (int i = 0; i < model.NameTableRefense.Split(',').Length; i++)
                {
                    tem = tem + "'" + model.NameTableRefense.Split(',')[i] + "'";
                    if (i == model.NameTableRefense.Split(',').Length - 1)
                        tem = tem + "]";
                    else
                        tem = tem + ",";
                }

                model.NameTableRefense = tem;
            }
            else
            {
                model.NameTableRefense = "[]";
            }

            if (model.OptionSelectColumn != null && model.OptionSelectColumn.Split(',').Length > 0)
            {
                var tem = "[";
                for (int i = 0; i < model.OptionSelectColumn.Split(',').Length; i++)
                {
                    tem = tem + "'" + model.OptionSelectColumn.Split(',')[i].Replace("\n", "+") + "'";
                    if (i == model.OptionSelectColumn.Split(',').Length - 1)
                        tem = tem + "]";
                    else
                        tem = tem + ",";
                }

                model.OptionSelectColumn = tem;
            }
            else
            {
                model.OptionSelectColumn = "[]";
            }

            return View(model);
        }

        public async Task<IActionResult> EditFileToExcel(string id)
        {
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/edits", new { id = id },
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success || res.data == null)
            {
                NotifyError(res.message);
                return RedirectToAction("AccessDeniedPath", "TypeValue");
            }

            // var checkRole = await ApiHelper.ExecuteAsync<List<TypeValueModel>>(
            //     "/dashboard-user/check?idUser=" + _workContext.UserId + "&idTypeValue=" + res.data.TypeValueId + "",
            //     null, Method.GET, ApiHosts.Dashboard);
            //
            // if (checkRole.success == false)
            // {
            //     NotifyError("Bạn không có quyền thực hiện thao tác này !");
            //     return RedirectToAction("AccessDeniedPath", "TypeValue");
            // }

            var model = res.data;
            return View(model);
        }

        public async Task<IActionResult> GetTypeValue(string id)
        {
            var res = await ApiHelper.ExecuteAsync<TypeValueModel>("/type-value/details", new { id = id },
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            if (res.data == null)
                return Ok();
            var model = res.data;
            return Ok(new
            {
                data = model
            });
        }

        public async Task<IActionResult> GetExcelToMasterVTCNTT()
        {
            var res = await ApiHelper.ExecuteAsync<List<MasterVTCNTTModel>>(
                $"{MasterVTCNTTController}/{MasterVTCNTTGetAll}",
                null,
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            return Ok(new
            {
                start = 0,
                count = model.Count,
                total = model.Count,
                data = model,
            });
        }

        public async Task<IActionResult> GetExcelToFTTH()
        {
            var res = await ApiHelper.ExecuteAsync<List<FTTHModel>>(
                "/ftth/get-object",
                null,
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            return Ok(new
            {
                start = 0,
                count = res.totalCount,
                total = res.totalCount,
                data = model,
            });
        }

        public async Task<IActionResult> GetObject(string nameCOLUMN)
        {
            var modelquery = new SelectListItem();
            modelquery.Value = "select * from " + nameCOLUMN + " LIMIT 1 OFFSET 1 ";
            var res = ApiHelper.Execute<DataTable>("/storage-value/run-query", modelquery,
                Method.POST,
                ApiHosts.Dashboard);
            // var res = await ApiHelper.ExecuteAsync<List<FTTHModel>>(
            //     "/ftth/get-object",
            //     null,
            //     Method.GET,
            //     ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            return Ok(new
            {
                start = 0,
                count = res.totalCount,
                total = res.totalCount,
                data = model,
            });
        }

        public async Task<IActionResult> GetExcelToMasterFileKH2022()
        {
            var res = await ApiHelper.ExecuteAsync<List<MasterFileKH2022Model>>(
                "/master-file-kh-2022/get-object",
                null,
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            return Ok(new
            {
                start = 0,
                count = res.totalCount,
                total = res.totalCount,
                data = model,
            });
        }

        public async Task<IActionResult> GetExcelToFTTH2022()
        {
            var res = await ApiHelper.ExecuteAsync<List<FTTH2022Model>>(
                "/FTTH2022/get-object",
                null,
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            return Ok(new
            {
                start = 0,
                count = res.totalCount,
                total = res.totalCount,
                data = model,
            });
        }

        public async Task<IActionResult> GetExcelToFTTHMB3132022()
        {
            var res = await ApiHelper.ExecuteAsync<List<FTTHMB3132022Model>>(
                "/ftth-mb-31-3-2022/get-object",
                null,
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            return Ok(new
            {
                start = 0,
                count = res.totalCount,
                total = res.totalCount,
                data = model,
            });
        }

        public IActionResult Search()
        {
            return View();
        }

        public async Task<IActionResult> Download(string id)
        {
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/edits", new { id = id },
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            return File(model.DataSave, model.FileContent, model.FileName);
        }

        public void CreateFolder(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine("Path đã tồn tại !");
                }

                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }


        public async Task<IActionResult> DownloadToTemplate(string id)
        {
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/edits", new { id = id },
                Method.GET,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            var model = res.data;
            var momery = new MemoryStream(model.DataSave);
            Workbook workbook = new Workbook(momery);
            if (!string.IsNullOrEmpty(model.NameTableRefense) && model.NameTableRefense.Split(',').Length > 0)
                foreach (var item in model.NameTableRefense.Split(','))
                {
                    if (workbook.Worksheets[item] == null)
                        workbook.Worksheets.Add(item);
                    Worksheet worksheet = workbook.Worksheets[item];
                    dynamic data = null;
                    var resR = await ApiHelper.ExecuteAsync<object>(
                        "/storage-value/get-all-table?table=" + item + "",
                        null,
                        Method.GET, ApiHosts.Dashboard);
                    data = resR.data;
                    if (data != null)
                    {
                        var resm = data;

                        ImportTableOptions tableOptions = new ImportTableOptions();
                        tableOptions.IsFieldNameShown = true;
                        worksheet.Cells.ImportCustomObjects((ICollection)resm, 1, 0, tableOptions);
                        worksheet.AutoFitRows(true);
                        worksheet.AutoFitColumns(0, worksheet.Cells.Columns.Count);
                    }

                    worksheet.Protection.Password = "HTCITC";
                }

            return File(momery.ToArray(), model.FileContent, model.FileName);
        }

        private byte[] GetFileByPath(string pathFile)
        {
            string path = CommonHelper.MapPath(pathFile);
            var fileStream = System.IO.File.OpenRead(path);

            return fileStream.ToByteArray();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMasterVTCNTT(IEnumerable<MasterVTCNTTModel> models,
            IEnumerable<MasterVTCNTTModel> modelsAdd)
        {
            if (models == null && modelsAdd == null)
                return Ok(new XBaseResult() { success = false });
            var unused = new MasterVTCNTTAddOrUpdateModel()
            {
                models = models,
                modelsAdd = modelsAdd
            };
            var res = await ApiHelper.ExecuteAsync($"/{MasterVTCNTTController}/{MasterVTCNTTAddOrUpdate}", unused,
                Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult() { success = res.success });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateFTTH(IEnumerable<FTTHModel> models,
            IEnumerable<FTTHModel> modelsAdd)
        {
            if (models == null && modelsAdd == null)
                return Ok(new XBaseResult() { success = false });
            var unused = new FTTHAddOrUpdateModel()
            {
                models = models,
                modelsAdd = modelsAdd
            };
            var res = await ApiHelper.ExecuteAsync($"/{FTTHController}/{FTTHAddOrUpdate}", unused,
                Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult() { success = res.success });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateMasterFileKH2022(IEnumerable<MasterFileKH2022Model> models,
            IEnumerable<MasterFileKH2022Model> modelsAdd)
        {
            if (models == null && modelsAdd == null)
                return Ok(new XBaseResult() { success = false });
            var unused = new MasterFileKH2022AddOrUpdateModel()
            {
                models = models,
                modelsAdd = modelsAdd
            };
            var res = await ApiHelper.ExecuteAsync($"/{MasterFileKH2022Controller}/{FTTHAddOrUpdate}", unused,
                Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult() { success = res.success });
        }

        public async Task<IActionResult> UpdateFTTH2022(IEnumerable<FTTH2022Model> models,
            IEnumerable<FTTH2022Model> modelsAdd)
        {
            if (models == null && modelsAdd == null)
                return Ok(new XBaseResult() { success = false });
            var unused = new FTTH2022AddOrUpdateModel()
            {
                models = models,
                modelsAdd = modelsAdd
            };
            var res = await ApiHelper.ExecuteAsync($"/{FTTH2022Controller}/{FTTHAddOrUpdate}", unused,
                Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult() { success = res.success });
        }

        public async Task<IActionResult> UpdateFTTHMB3132022(IEnumerable<FTTHMB3132022Model> models,
            IEnumerable<FTTHMB3132022Model> modelsAdd)
        {
            if (models == null && modelsAdd == null)
                return Ok(new XBaseResult() { success = false });
            var unused = new FTTHMB3132022AddOrUpdateModel()
            {
                models = models,
                modelsAdd = modelsAdd
            };
            var res = await ApiHelper.ExecuteAsync($"/{FTTHMB3132022Controller}/{FTTHAddOrUpdate}", unused,
                Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult() { success = res.success });
        }


        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Edit(StorageValueModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();


            var res = await ApiHelper.ExecuteAsync("/storage-value/edit", model, Method.POST, ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult());
        }


        [HttpPost]
        public async Task<IActionResult> Deletes([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] IEnumerable<StorageValueModel> models)
        {
            if (models == null)
            {
                NotifyInfo(T("Notifies.NoItemsSelected"));
                return Ok(new XBaseResult { success = false });
            }

            // foreach (var item in models)
            // {
            //     var checkRole = await ApiHelper.ExecuteAsync<List<TypeValueModel>>(
            //         "/dashboard-user/check?idUser=" + _workContext.UserId + "&idTypeValue=" + item.TypeValueId + "",
            //         null, Method.GET, ApiHosts.Dashboard);
            //
            //     if (!checkRole.success)
            //     {
            //         NotifyError("Bạn không có quyền thực hiện thao tác này !");
            //         return Ok(new XBaseResult { success = false });
            //     }
            // }

            var ids = new List<string>();
            foreach (var item in models)
            {
                ids.Add(item.Id);
            }

            var res = await ApiHelper.ExecuteAsync("/storage-value/deletes", ids, Method.POST, ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.GetErrorsToHtml());
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess(res.message);
            return Ok(new XBaseResult { success = true });
        }

        /// <summary>
        /// Chuyển đổi file sang dạng byte
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> FileConvert(StorageValueModel model)
        {
            if (model.FormFile == null || !FormFileExtensions.IsExcel(model.FormFile))
            {
                NotifyError("Xin vui lòng chọn lại file !");
                return Ok(new XBaseResult { success = false });
            }

            await using (var ms = new MemoryStream())
            {
                await model.FormFile.CopyToAsync(ms);
                var fileBytes = ms.ToArray();
                var s = Convert.ToBase64String(fileBytes);
                model.DataSave = fileBytes;
                model.FileLength = model.FormFile.Length;
                model.FileName = model.FormFile.FileName;
                model.FileContent = model.FormFile.ContentType;
                model.FileType = GetExtension(Path.GetExtension(model.FormFile.FileName));
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                ms.Position = 0;
            }

            return Ok(model);
        }


        /// <summary>
        /// Chuyển đổi file sang dạng byte
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> FileGetListData(AddImportModel model)
        {
            if (model.NumberHeader < 1)
            {
                NotifyError("Xin vui lòng nhập dòng tiêu đề !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.StartWith < 1)
            {
                NotifyError("Xin vui lòng nhập dòng bắt đầu dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (string.IsNullOrEmpty(model.SheeActive))
            {
                NotifyError("Xin vui lòng nhập sheet dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.FormFile == null || !FormFileExtensions.IsExcel(model.FormFile))
            {
                NotifyError("Xin vui lòng chọn lại file !");
                return Ok(new XBaseResult { success = false });
            }

            using var mst = new MemoryStream(model.FormFile.OpenReadStream().ToByteArray());

            Workbook workbook = new Workbook(mst);

            Worksheet worksheet = workbook.Worksheets[model.SheeActive];
            if (worksheet == null)
            {
                NotifyError("Sheet dữ liệu không tồn tại !");
                return Ok(new XBaseResult { success = false });
            }

            int countEmty = 0;

            worksheet.Cells.StandardHeight = 55;
            var namecolumn = "";
            var value = "";
            var sqlToSelectWhere = $" select * from {model.NameColumn} where ";
            for (int i = 0; i < 500; i++)
            {
                if (string.IsNullOrEmpty(worksheet.Cells[model.NumberHeader - 1, i].StringValue))
                    break;
                if (!string.IsNullOrEmpty(namecolumn))
                    namecolumn = namecolumn + ",";
                namecolumn = namecolumn + "`" + worksheet.Cells[model.NumberHeader - 1, i].StringValue.Trim() + "`";
            }

            // lấy từng thuộc tính của models
            //
            for (int j = model.StartWith - 1; j < 30000; j++)
            {
                var insert = "(";
                if (j < 500)
                    sqlToSelectWhere = sqlToSelectWhere + "(";
                for (int i = 0; i < namecolumn.Split(',').Length; i++)
                {
                    var getValue = worksheet.Cells[j, i].StringValue;
                    if (string.IsNullOrEmpty(getValue))
                        countEmty++;
                    insert = insert + "'" + getValue.Replace("\n", " ").Replace(",", " ").Replace("'", " ") + "'" + ",";
                    if (j < 500)
                        sqlToSelectWhere = sqlToSelectWhere + namecolumn.Split(',')[i].Replace("\n", "") + "=" + "'" +
                                           getValue.Replace("\n", " ").Replace(",", " ").Replace("'", " ") + "' and ";
                }

                if (j < 500)
                    sqlToSelectWhere = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 4, 4) + ") or";


                insert = insert.Remove(insert.Length - 1) + ")";

                if (countEmty == namecolumn.Split(',').Length)
                {
                    break;
                }
                else
                {
                    if (j == model.StartWith - 1)
                        value = insert;
                    else
                        value = value + " , " + insert;
                }

                countEmty = 0;
            }

            namecolumn = namecolumn.Replace("\n", "");


            var countBefore = 0;
            var countAfter = 0;


            var modelqueryCount = new SelectListItem();
            modelqueryCount.Value = "select count(*) from " + model.NameColumn + " ";
            var dataTableCount = await ApiHelper.ExecuteDynamicAsync("/storage-value/count-table",
                modelqueryCount,
                Method.POST,
                ApiHosts.Dashboard);

            countBefore = (int)dataTableCount.data;


            var genSql = "INSERT INTO " + model.NameColumn + " ( " + namecolumn + " ) values " + value;
            if (string.IsNullOrEmpty(namecolumn) || string.IsNullOrEmpty(value))
            {
                NotifyError("Không có dữ liệu nào để import, xin vui lòng kiểm tra lại !");
                return Ok(new XBaseResult { success = false });
            }

            //  var genSql = "INSERT INTO " + model.NameColumn + " values " + value;
            var modelquery = new SelectListItem();
            modelquery.Value = genSql;
            //  model.Value = query.Replace("\\n", "\r\n");
            //   model.Value = query.Replace("\n", " ");
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/run-query", modelquery,
                Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.message);
                return Ok(new XBaseResult { success = false });
            }

            modelqueryCount.Value = "select count(*) from " + model.NameColumn + " ";
            var dataTableCountAfter = await ApiHelper.ExecuteDynamicAsync("/storage-value/count-table",
                modelqueryCount,
                Method.POST,
                ApiHosts.Dashboard);

            countAfter = (int)dataTableCountAfter.data;
            NotifySuccess("Thành công " + (countAfter - countBefore) + " hàng dữ liệu !");
            return Ok(new XBaseResult()
            {
                data = countAfter - countBefore,
                success = true
            });
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> GetResultPage(int start, string NameColumn, AddImportModel model)
        {
            if (model.NumberHeader < 1)
            {
                NotifyError("Xin vui lòng nhập dòng tiêu đề !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.StartWith < 1)
            {
                NotifyError("Xin vui lòng nhập dòng bắt đầu dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (string.IsNullOrEmpty(model.SheeActive))
            {
                NotifyError("Xin vui lòng nhập sheet dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.FormFile == null || !FormFileExtensions.IsExcel(model.FormFile))
            {
                NotifyError("Xin vui lòng chọn lại file !");
                return Ok(new XBaseResult { success = false });
            }


            using var mstgetvalue = new MemoryStream(await model.FormFile.OpenReadStream().ToByteArrayAsync());

            Workbook workbookgetvalue = new Workbook(mstgetvalue);

            Worksheet worksheetgetvalue = workbookgetvalue.Worksheets[model.SheeActive];
            if (worksheetgetvalue == null)
            {
                NotifyError("Sheet dữ liệu không tồn tại !");
                return Ok(new XBaseResult { success = false });
            }

            int countEmty = 0;

            worksheetgetvalue.Cells.StandardHeight = 55;
            var namecolumn = "";
            var value = "";
            var sqlToSelectWhere = $" select * from {model.NameColumn} where ";
            for (int i = 0; i < 500; i++)
            {
                if (string.IsNullOrEmpty(worksheetgetvalue.Cells[model.NumberHeader - 1, i].StringValue))
                    break;
                if (!string.IsNullOrEmpty(namecolumn))
                    namecolumn = namecolumn + ",";
                namecolumn = namecolumn + "`" + worksheetgetvalue.Cells[model.NumberHeader - 1, i].StringValue.Trim() +
                             "`";
            }

            // lấy từng thuộc tính của models
            //
            for (int j = (model.StartWith - 1) + (start * 100); j < (model.StartWith - 1) + (start * 100) + 100; j++)
            {
                sqlToSelectWhere = sqlToSelectWhere + "(";
                for (int i = 0; i < namecolumn.Split(',').Length; i++)
                {
                    var getValue = worksheetgetvalue.Cells[j, i].StringValue;
                    if (string.IsNullOrEmpty(getValue))
                        countEmty++;
                    sqlToSelectWhere = sqlToSelectWhere + namecolumn.Split(',')[i].Replace("\n", "") + "=" + "'" +
                                       getValue.Replace("\n", " ").Replace(",", " ").Replace("'", " ") + "' and ";
                }

                sqlToSelectWhere = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 4, 4) + ") or";


                if (countEmty == namecolumn.Split(',').Length)
                {
                    break;
                }

                countEmty = 0;
            }

            sqlToSelectWhere = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 2, 2);
            // var tmpPath = CommonHelper.MapPath("/wwwroot/Template/TeamplateResult.xlsx");
            // var file= new FileStream(tmpPath, FileMode.Open, FileAccess.Read);
            using var mst = new MemoryStream();
            Workbook workbook = new Workbook(mst);
            Worksheet worksheet = workbook.Worksheets[0];

            worksheet.Cells.StandardHeight = 55;
            worksheet.Cells.StandardWidth = 25;
            //This is your code
            worksheet.AutoFitColumns();

            ///
            var resListColumn = await ApiHelper.ExecuteAsync<List<SelectTableModel>>(
                $"/select-table/get-select-table?NameTable={NameColumn}", null, Method.GET,
                ApiHosts.Dashboard);
            // lấy từng thuộc tính của models
            if (resListColumn == null || resListColumn.data == null)
            {
                NotifyError("Bảng " + NameColumn + " không tồn tại !");
                return Ok(new XBaseResult { success = false });
            }

            for (int i = 0; i < resListColumn.data.Count; i++)
            {
                // nếu ô tiêu đề không có tên, tiến hành break
                if (string.IsNullOrEmpty(resListColumn.data[i].ValueShow))
                    break;
                var convertname = "&=Data." + resListColumn.data[i].TextShow;
                worksheet.Cells[0, i].PutValue(resListColumn.data[i].TextShow);
                //This Style object will make the fill color Red and font Bold


                if (i > 0)
                {
                    Style boldStyle1 = workbook.CreateStyle();
                    boldStyle1.Pattern = BackgroundType.Solid;
                    boldStyle1.Font.Size = 15;
                    boldStyle1.IsTextWrapped = true;
                    boldStyle1.VerticalAlignment = TextAlignmentType.Center;

                    StyleFlag boldStyleFlag1 = new StyleFlag();
                    boldStyleFlag1.HorizontalAlignment = true;
                    boldStyleFlag1.FontSize = true;
                    boldStyleFlag1.WrapText = true;
                    boldStyleFlag1.VerticalAlignment = true;
                    var rowChirldren = workbook.Worksheets[0].Cells.Rows[i];
                    rowChirldren.ApplyStyle(boldStyle1, boldStyleFlag1);
                }

                else
                {
                    Style boldStyle = workbook.CreateStyle();
                    boldStyle.ForegroundColor = Color.Red;
                    boldStyle.Pattern = BackgroundType.Solid;
                    boldStyle.BackgroundColor = Color.Red;
                    boldStyle.Font.IsBold = true;
                    boldStyle.Font.Size = 15;
                    boldStyle.IsTextWrapped = true;
                    boldStyle.VerticalAlignment = TextAlignmentType.Center;

                    StyleFlag boldStyleFlag = new StyleFlag();
                    boldStyleFlag.HorizontalAlignment = true;
                    boldStyleFlag.FontBold = true;
                    boldStyleFlag.FontSize = true;
                    boldStyleFlag.WrapText = true;
                    boldStyleFlag.VerticalAlignment = true;
                    var row1 = workbook.Worksheets[0].Cells.Rows[0];
                    row1.ApplyStyle(boldStyle, boldStyleFlag);
                }

                worksheet.Cells[1, i].PutValue(convertname);
                // var style = worksheet.Cells[0, i].GetStyle();
                // style.ForegroundColor = System.Drawing.Color.Gold;
                // style.Font.Color = Color.White;
                // worksheet.Cells.ApplyStyle(style, new StyleFlag());
            }

            var count = 0;
            var ds = new DataSet();

            // // excute query<DataTable>
            var modelquery = new SelectListItem();
            //   modelquery.Value = "select * from " + NameColumn + "  LIMIT 100 OFFSET " + start * 100 + " ";
            modelquery.Value = sqlToSelectWhere;
            var dataTable = ApiHelper.Execute<DataTable>("/storage-value/run-query-object", modelquery,
                Method.POST,
                ApiHosts.Dashboard);
            var listdataTable = dataTable.data;
            var dtDataName = "Data";
            listdataTable.TableName = dtDataName;
            ds.Tables.Add(listdataTable);
            var modelqueryCount = new SelectListItem();
            modelqueryCount.Value = sqlToSelectWhere.Replace("*", "count(*)");
            var dataTableCount = await ApiHelper.ExecuteDynamicAsync("/storage-value/count-table",
                modelqueryCount,
                Method.POST,
                ApiHosts.Dashboard);

            count = (int)dataTableCount.data;

            var wd = new WorkbookDesigner(workbook);
            wd.SetDataSource(dataSet: ds);
            wd.Process();
            wd.Workbook.CalculateFormula();
            var ms = new MemoryStream();
            workbook.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
            ms.Seek(0, SeekOrigin.Begin);
            ms.Position = 0;
            ms.Flush();
            ms.Close();
            mst.Seek(0, SeekOrigin.Begin);
            mst.Position = 0;
            mst.Flush();
            mst.Close();
            workbook.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var result = new ExtensionModel()
            {
                FileConvert = Convert.ToBase64String(await ms.ToByteArrayAsync()),
                TotalCount = count
            };
            return Ok(result);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> GetListDataResult(AddImportModel model, int start, int length)
        {
            start = start + 1;
            if (model.NumberHeader < 1)
            {
                NotifyError("Xin vui lòng nhập dòng tiêu đề !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.StartWith < 1)
            {
                NotifyError("Xin vui lòng nhập dòng bắt đầu dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (string.IsNullOrEmpty(model.SheeActive))
            {
                NotifyError("Xin vui lòng nhập sheet dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.FormFile == null || !FormFileExtensions.IsExcel(model.FormFile))
            {
                NotifyError("Xin vui lòng chọn lại file !");
                return Ok(new XBaseResult { success = false });
            }

            using var mst = new MemoryStream(model.FormFile.OpenReadStream().ToByteArray());

            Workbook workbook = new Workbook(mst);

            Worksheet worksheet = workbook.Worksheets[model.SheeActive];
            if (worksheet == null)
            {
                NotifyError("Sheet dữ liệu không tồn tại !");
                return Ok(new XBaseResult { success = false });
            }

            int countEmty = 0;

            worksheet.Cells.StandardHeight = 55;
            var namecolumn = "";
            var value = "";
            var sqlToSelectWhere = $" select * from {model.NameColumn} where ";
            for (int i = 0; i < 5000; i++)
            {
                if (string.IsNullOrEmpty(worksheet.Cells[model.NumberHeader - 1, i].StringValue))
                    break;
                if (!string.IsNullOrEmpty(namecolumn))
                    namecolumn = namecolumn + ",";
                namecolumn = namecolumn + "`" + worksheet.Cells[model.NumberHeader - 1, i].StringValue.Trim() + "`";
            }

            // lấy từng thuộc tính của models

            for (int j = (model.StartWith - 1) + start * length;
                j < (model.StartWith - 1) + start * length + length;
                j++)
            {
                sqlToSelectWhere = sqlToSelectWhere + "(";
                for (int i = 0; i < namecolumn.Split(',').Length; i++)
                {
                    var getValue = worksheet.Cells[j, i].StringValue;
                    if (string.IsNullOrEmpty(getValue))
                        countEmty++;
                    sqlToSelectWhere = sqlToSelectWhere + namecolumn.Split(',')[i].Replace("\n", "") + "=" + "'" +
                                       getValue.Replace("\n", " ").Replace(",", " ").Replace("'", " ") + "' and ";
                }

                sqlToSelectWhere = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 4, 4) + ") or";
                if (countEmty == namecolumn.Split(',').Length)
                {
                    break;
                }

                countEmty = 0;
            }

            namecolumn = namecolumn.Replace("\n", "");
            if (string.IsNullOrEmpty(namecolumn))
            {
                NotifyError("Không có dữ liệu nào để import, xin vui lòng kiểm tra lại !");
                return Ok(new XBaseResult { success = false });
            }

            return Ok(new XBaseResult()
            {
                data = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 2, 2),
                success = true
            });
        }


        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> FileGetListDataResult(AddImportModel model)
        {
            if (model.NumberHeader < 1)
            {
                NotifyError("Xin vui lòng nhập dòng tiêu đề !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.StartWith < 1)
            {
                NotifyError("Xin vui lòng nhập dòng bắt đầu dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (string.IsNullOrEmpty(model.SheeActive))
            {
                NotifyError("Xin vui lòng nhập sheet dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.FormFile == null || !FormFileExtensions.IsExcel(model.FormFile))
            {
                NotifyError("Xin vui lòng chọn lại file !");
                return Ok(new XBaseResult { success = false });
            }

            using var mst = new MemoryStream(model.FormFile.OpenReadStream().ToByteArray());

            Workbook workbook = new Workbook(mst);

            Worksheet worksheet = workbook.Worksheets[model.SheeActive];
            if (worksheet == null)
            {
                NotifyError("Sheet dữ liệu không tồn tại !");
                return Ok(new XBaseResult { success = false });
            }

            int countEmty = 0;

            worksheet.Cells.StandardHeight = 55;
            var namecolumn = "";
            var value = "";
            var sqlToSelectWhere = $" select * from {model.NameColumn} where ";
            for (int i = 0; i < 500; i++)
            {
                if (string.IsNullOrEmpty(worksheet.Cells[model.NumberHeader - 1, i].StringValue))
                    break;
                if (!string.IsNullOrEmpty(namecolumn))
                    namecolumn = namecolumn + ",";
                namecolumn = namecolumn + "`" + worksheet.Cells[model.NumberHeader - 1, i].StringValue.Trim() + "`";
            }

            // lấy từng thuộc tính của models
            //
            // int jjj = model.StartWith - 1;
            // var lengthCoulmn = namecolumn.Split(',').Length;
            // while (countEmty == namecolumn.Split(',').Length || jjj < model.StartWith + 9)
            // {
            //     sqlToSelectWhere = sqlToSelectWhere + "(";
            //     for (int i = 0; i < lengthCoulmn; i++)
            //     {
            //         var getValue = worksheet.Cells[jjj, i].StringValue;
            //         var name = namecolumn.Split(',')[i].Replace("\n", "");
            //         if (string.IsNullOrEmpty(getValue))
            //             countEmty++;
            //         sqlToSelectWhere = sqlToSelectWhere + name + "=" + "'" +
            //                            getValue.Replace("\n", " ").Replace(",", " ").Replace("'", " ") + "' and ";
            //     }
            //
            //     sqlToSelectWhere = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 4, 4) + ") or";
            //     if (countEmty == namecolumn.Split(',').Length)
            //     {
            //         break;
            //     }
            //
            //     countEmty = 0;
            //     jjj++;
            // }
            for (int j = model.StartWith - 1; j < 300000; j++)
            {
                sqlToSelectWhere = sqlToSelectWhere + "(";
                for (int i = 0; i < namecolumn.Split(',').Length; i++)
                {
                    var getValue = worksheet.Cells[j, i].StringValue;
                    if (string.IsNullOrEmpty(getValue))
                        countEmty++;
                    // sqlToSelectWhere = sqlToSelectWhere + namecolumn.Split(',')[i].Replace("\n", "") + "=" + "'" +
                    //                    getValue.Replace("\n", " ").Replace(",", " ").Replace("'", " ") + "' and ";
                }

                // sqlToSelectWhere = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 4, 4) + ") or";
                if (countEmty == namecolumn.Split(',').Length)
                {
                    break;
                }

                countEmty = 0;
                // Console.WriteLine(j);
            }

            namecolumn = namecolumn.Replace("\n", "");
            if (string.IsNullOrEmpty(namecolumn))
            {
                NotifyError("Không có dữ liệu nào để import, xin vui lòng kiểm tra lại !");
                return Ok(new XBaseResult { success = false });
            }

            return Ok(new XBaseResult()
            {
                data = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 2, 2),
                success = true
            });
        }


        /// <summary>
        /// trả về danh sách bảng có trong file excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetNameTableByExcel(AddTableModel model)
        {
            if (model.ActiveInput)
                return Ok(new XBaseResult { success = false });
            if (model.NumberHeader < 1)
            {
                NotifyError("Xin vui lòng nhập dòng tiêu đề !");
                return Ok(new XBaseResult { success = false });
            }


            if (string.IsNullOrEmpty(model.SheeActive))
            {
                NotifyError("Xin vui lòng nhập sheet dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.FormFile == null || !FormFileExtensions.IsExcel(model.FormFile) && !model.ActiveInput)
            {
                NotifyError("Xin vui lòng chọn lại file !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.ActiveInput && string.IsNullOrEmpty(model.Data))
            {
                NotifyError("Xin vui lòng nhập dãy cột muốn thêm vào, ngăn cách nhau bởi dấu phẩy ',' !");
                return Ok(new XBaseResult { success = false });
            }

            using var mst = new MemoryStream(model.FormFile.OpenReadStream().ToByteArray());

            Workbook workbook = new Workbook(mst);

            Worksheet worksheet = workbook.Worksheets[model.SheeActive];
            if (worksheet == null)
            {
                NotifyError("Sheet dữ liệu không tồn tại !");
                return Ok(new XBaseResult { success = false });
            }

            worksheet.Cells.StandardHeight = 55;
            var namecolumn = "";
            if (!model.ActiveInput)
            {
                for (int i = 0; i < 500; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[model.NumberHeader - 1, i].StringValue))
                        break;
                    if (!string.IsNullOrEmpty(namecolumn))
                        namecolumn = namecolumn + ",";
                    namecolumn = namecolumn + "`" + worksheet.Cells[model.NumberHeader - 1, i].StringValue + "`";
                }
            }
            else
            {
                namecolumn = model.Data;
            }


            if (string.IsNullOrEmpty(namecolumn) || namecolumn.Split(',').Length < 1)
            {
                NotifyError("Không có dữ liệu nào để import, xin vui lòng kiểm tra lại !");
                return Ok(new XBaseResult { success = false });
            }

            //  var genSql = "INSERT INTO " + model.NameColumn + " values " + value;
            return Ok(new XBaseResult()
            {
                data = namecolumn.Replace("\n", "")
            });
        }


        [HttpPost]
        public async Task<IActionResult> AddTable(AddTableModel model)
        {
            if (string.IsNullOrEmpty(model.NameTable))
            {
                NotifyError("Xin vui lòng nhập tên bảng !");
                return Ok(new XBaseResult { success = false });
            }

            var resss = await ApiHelper.ExecuteAsync<List<SelectTableModel>>(
                $"/select-table/get-select-table?NameTable={model.NameTable}", null, Method.GET, ApiHosts.Dashboard);
            if (resss.data != null)
            {
                NotifyError("Tên bảng đã tồn tại !");
                return Ok(new XBaseResult { success = false });
            }


            if (!model.ActiveInput)
            {
                if (model.Data.Split(',') != null && model.Data.Split(',').Length > 0)
                {
                    var sql = new StringBuilder();
                    sql.Append(" CREATE TABLE " + model.NameTable + " ( ");
                    for (int i = 0; i < model.Data.Split(',').Length; i++)
                    {
                        var tem = model.Data.Split(',')[i];
                        if (i == model.Data.Split(',').Length - 1)
                            sql.Append("" + tem + " varchar(50) DEFAULT NULL )");
                        else
                            sql.Append("" + tem + " varchar(50) DEFAULT NULL ,");
                    }

                    sql.Append(" ENGINE=InnoDB DEFAULT CHARSET=utf8mb4,COLLATE utf8mb4_unicode_ci;");
                    var modell = new SelectListItem();
                    modell.Value = sql.ToString();
                    //  model.Value = query.Replace("\\n", "\r\n");
                    //   model.Value = query.Replace("\n", " ");
                    var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/run-query", modell,
                        Method.POST,
                        ApiHosts.Dashboard);
                    if (!res.success)
                    {
                        NotifyError(res.message);
                        return Ok(new XBaseResult { success = false });
                    }

                    var addDes = new StringBuilder();

                    addDes.Append("INSERT INTO NameTableExist");
                    addDes.Append("(");
                    addDes.Append("  Id");
                    addDes.Append(" ,Name");
                    addDes.Append(" ,NameDes");
                    addDes.Append(" )");
                    addDes.Append("  VALUES");
                    addDes.Append("  (");
                    addDes.Append("     '" + Guid.NewGuid() + "' ");
                    addDes.Append("     ,'" + model.NameTable + "' ");
                    addDes.Append("      ,'" + model.Description + "' ");
                    addDes.Append(")");
                    var modelll = new SelectListItem();
                    modelll.Value = addDes.ToString();
                    var ress = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/run-query", modelll,
                        Method.POST,
                        ApiHosts.Dashboard);
                    if (!ress.success)
                    {
                        NotifyError("Tạo bảng thành công, tạo mô tả thất bại !");
                        return Ok(new XBaseResult { success = false });
                    }

                    NotifySuccess("Thành công !");
                    return Ok(new XBaseResult { success = true });
                }
                else
                {
                    NotifyError("Xin vui lòng nhập dãy cột muốn thêm vào !");
                    return Ok(new XBaseResult { success = false });
                }
            }


            return Ok(new XBaseResult()
            {
                data = model
            });
        }


        [HttpPost]
        // [DisableRequestSizeLimit]
        // [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        public async Task<IActionResult> RunQueryImport(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                NotifyError("Có lỗi xảy ra hoặc bạn chưa chọn file, xin vui lòng thực hiện lại !");
                return Ok(new XBaseResult { success = false });
            }

            var model = new SelectListItem();
            model.Value = query;
            //  model.Value = query.Replace("\\n", "\r\n");
            //   model.Value = query.Replace("\n", " ");
            var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/run-query", model,
                Method.POST,
                ApiHosts.Dashboard);
            if (!res.success)
            {
                NotifyError(res.message);
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess("Thành công !");
            return Ok(new XBaseResult { success = true });
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> GenSqlToUpdate(UpdateImportModels model)
        {
            if (model.NumberHeader < 1)
            {
                NotifyError("Xin vui lòng nhập dòng tiêu đề !");
                return Ok(new XBaseResult { success = false });
            }

            if (model.StartWith < 1)
            {
                NotifyError("Xin vui lòng nhập dòng bắt đầu dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            if (string.IsNullOrEmpty(model.SheeActive))
            {
                NotifyError("Xin vui lòng nhập sheet dữ liệu !");
                return Ok(new XBaseResult { success = false });
            }

            var listMapping = new List<SelectListItem>();
            if (model.CoulmnMapping.Split('|').Length > 1)
            {
                for (int i = 0; i < model.CoulmnMapping.Split('|')[0].Split(',').Length; i++)
                {
                    listMapping.Add(new SelectListItem()
                    {
                        // replace space with ""
                        Text = model.CoulmnMapping.Split('|')[1].Split(',')[i].Trim().Replace("\r\n", ""),
                        Value = model.CoulmnMapping.Split('|')[0].Split(',')[i]
                    });
                }
            }

            if (model.FormFile == null || !FormFileExtensions.IsExcel(model.FormFile))
            {
                NotifyError("Xin vui lòng chọn lại file !");
                return Ok(new XBaseResult { success = false });
            }

            var res = await ApiHelper.ExecuteAsync<List<SelectTableModel>>(
                $"/select-table/get-select-table?NameTable={model.NameColumn}", null, Method.GET,
                ApiHosts.Dashboard);
            var list = new List<SelectItem>();
            if (res.data != null)
            {
                foreach (var item in res.data)
                {
                    var tem = new SelectItem()
                    {
                        text = item.TextShow,
                        id = item.ValueShow
                    };
                    list.Add(tem);
                }
            }
            else
            {
                NotifyError("Bảng vừa chọn chưa đúng, xin vui lòng thử lại !");
                return Ok(new XBaseResult { success = false });
            }


            using var mst = new MemoryStream(model.FormFile.OpenReadStream().ToByteArray());
            Workbook workbook = new Workbook(mst);
            Worksheet worksheet = workbook.Worksheets[model.SheeActive];
            if (worksheet == null)
            {
                NotifyError("Sheet dữ liệu không tồn tại !");
                return Ok(new XBaseResult { success = false });
            }

            worksheet.Cells.StandardHeight = 55;

            var namecolumnlist = "";
            for (int i = 0; i < 50000; i++)
            {
                if (string.IsNullOrEmpty(worksheet.Cells[model.NumberHeader - 1, i].StringValue))
                    break;
                if (!string.IsNullOrEmpty(namecolumnlist))
                    namecolumnlist = namecolumnlist + ",";
                namecolumnlist = namecolumnlist +
                                 worksheet.Cells[model.NumberHeader - 1, i].StringValue.Trim().Replace("\n", "");
                ;
            }

            var value = "";
            var sqlToSelectWhere = $" select * from {model.NameColumn} where ";
            var genSql = "";
            int countEmty = 0;
            for (int i = model.StartWith - 1; i < model.StartWith + 100000; i++)
            {
                var updateSql = " Update " + model.NameColumn + " ";
                var set = " set ";
                var where = " where ";
                sqlToSelectWhere = sqlToSelectWhere + "(";

                for (int j = 0; j < namecolumnlist.Split(',').Length; j++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[model.NumberHeader - 1, j].StringValue))
                        break;
                    if (string.IsNullOrEmpty(worksheet.Cells[i, j].StringValue))
                        countEmty++;
                    var namecolumn = worksheet.Cells[model.NumberHeader - 1, j].StringValue;
                    namecolumn = namecolumn.Trim().Replace("\n", "");
                    ;
                    if (model.OptionSelectColumn.Contains(namecolumn) ||
                        model.UpdateSelectColumn.Contains(namecolumn) ||
                        model.CoulmnMapping.Contains(namecolumn))
                    {
                        var check = list.Where(x => x.id.Equals("`" + namecolumn + "`"));
                        if (model.OptionSelectColumn.Contains(namecolumn) ||
                            model.UpdateSelectColumn.Contains(namecolumn))
                        {
                            if (check != null && check.Any())
                                if (model.UpdateSelectColumn.Contains(namecolumn))
                                    set = set + " `" + namecolumn + "` " + " = " + " '" +
                                          worksheet.Cells[i, j].StringValue.Replace("\n", " ").Replace(",", " ")
                                              .Replace("'", " ") + "' " +
                                          " ,   ";

                                else if (model.OptionSelectColumn.Contains(namecolumn))
                                    where = where + " `" + namecolumn + "` " + "=" + " '" +
                                            worksheet.Cells[i, j].StringValue.Replace("\n", " ").Replace(",", " ")
                                                .Replace("'", " ") + "' " +
                                            " and ";
                        }
                        else
                        {
                            if (model.CoulmnMapping.Contains(namecolumn))
                            {
                                foreach (var item in listMapping)
                                {
                                    if (item.Text.Equals(namecolumn))
                                    {
                                        namecolumn = item.Value;
                                        break;
                                    }
                                }

                                if (check != null && check.Any())
                                    if (model.UpdateSelectColumn.Contains(namecolumn))

                                        set = set + namecolumn + " = " + " '" +
                                              worksheet.Cells[i, j].StringValue.Replace("\n", " ").Replace(",", " ")
                                                  .Replace("'", " ") + "' " + " ,   ";

                                    else if (model.OptionSelectColumn.Contains(namecolumn))

                                        where = where + namecolumn + " = " + "'" +
                                                worksheet.Cells[i, j].StringValue.Replace("\n", " ").Replace(",", " ")
                                                    .Replace("'", " ") +
                                                "'" + " and ";
                            }
                        }


                        sqlToSelectWhere = sqlToSelectWhere + "`" + namecolumn + "`" + "=" + "'" +
                                           worksheet.Cells[i, j].StringValue.Replace("\n", " ").Replace(",", " ")
                                               .Replace("'", " ") + "' and ";
                    }
                }


                if (countEmty == namecolumnlist.Split(',').Length)
                {
                    break;
                }

                sqlToSelectWhere = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 4, 4) + ") or";

                countEmty = 0;
                set = set.Remove(set.Length - 4, 4);
                where = where.Remove(where.Length - 4, 4);
                if (string.IsNullOrEmpty(where) || string.IsNullOrEmpty(set) || where.Length < 5 || set.Length < 5)
                {
                    NotifyError("Không có dữ liệu nào để import, xin vui lòng kiểm tra lại !");
                    return Ok(new XBaseResult { success = false });
                }

                updateSql = updateSql + set + where;
                if (string.IsNullOrEmpty(genSql))
                    genSql = updateSql;
                else
                    genSql = genSql + ";" + updateSql;
            }

            var modelquery = new SelectListItem();
            modelquery.Value = genSql;
            //  model.Value = query.Replace("\\n", "\r\n");
            //   model.Value = query.Replace("\n", " ");
            var ress = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/run-query", modelquery,
                Method.POST,
                ApiHosts.Dashboard);
            if (!ress.success)
            {
                NotifyError(res.message);
                return Ok(new XBaseResult { success = false });
            }

            NotifySuccess("Thành công !");
            return Ok(new XBaseResult()
            {
                data = sqlToSelectWhere.Remove(sqlToSelectWhere.Length - 4, 4) + ")",
                success = true
            });
        }

        public IActionResult ShowResult()
        {
            var modelquery = new SelectListItem();
            return View(modelquery);
        }


        [HttpPost]
        public IActionResult GetResult(string q)
        {
            if (string.IsNullOrEmpty(q))
                return Ok(new XBaseResult { success = false, data = null });

            var modelquery = new SelectListItem();
            modelquery.Value = q;
            var dataTable = ApiHelper.Execute("/storage-value/run-query-object", modelquery,
                Method.POST,
                ApiHosts.Dashboard);
            var listdataTable = dataTable.data;
            return Ok(new { data = listdataTable, recordsTotal = 100, recordsFiltered = 100 });
        }


        [HttpPost]
        public async Task<IActionResult> GetFileTemplateToAddOrUpdate(string id, string keySearch, int numberPage,
            IEnumerable<SelectListItem> listItem)
        {
            try
            {
                var res = await ApiHelper.ExecuteAsync<StorageValueModel>("/storage-value/edits", new { id = id },
                    Method.GET,
                    ApiHosts.Dashboard);

                var model = res.data;
                using var mst = new MemoryStream(model.DataSave);
                Workbook workbook = new Workbook(mst);
                Worksheet worksheet = workbook.Worksheets[model.SheeActive];
                if (worksheet == null)
                {
                    NotifyError("Sheet dữ liệu không tồn tại !");
                    return Ok(new XBaseResult { success = false });
                }

                worksheet.Cells.StandardHeight = 55;

                ///

                // lấy từng thuộc tính của models
                for (int i = 0; i < 200; i++)
                {
                    // nếu ô tiêu đề không có tên, tiến hành break
                    if (string.IsNullOrEmpty(worksheet.Cells[model.NumberHeader - 1, i].StringValue))
                        break;

                    // check xem ô có định dạng kiểu công thức
                    var check = worksheet.Cells[model.NumberHeader, i].IsFormula;

                    // lấy công thức của ô
                    var getValue = worksheet.Cells[model.NumberHeader, i].Formula;

                    var getname = ConvertStringToUpper(worksheet.Cells[model.NumberHeader - 1, i].StringValue);

                    var convertname = "&=Data." + getname.Replace("\n", "");

                    // gán xuống ô ở dưới của tên cột
                    worksheet.Cells[model.NumberHeader, i].PutValue(convertname);
                }

                // bind query where with key search by list column
                string where = " where 1=1 ";
                if (!string.IsNullOrEmpty(model.NameColumn))
                {
                    var resListColumn = await ApiHelper.ExecuteAsync<List<SelectTableModel>>(
                        $"/select-table/get-select-table?NameTable={model.NameColumn}", null, Method.GET,
                        ApiHosts.Dashboard);
                    int check = 0;
                    if (resListColumn.data != null)
                    {
                        if (!string.IsNullOrEmpty(model.NameColumn) && !string.IsNullOrEmpty(keySearch))
                            where = where + " and ( ";
                        foreach (var item in resListColumn.data)
                        {
                            if (!string.IsNullOrEmpty(item.ValueShow) && !string.IsNullOrEmpty(keySearch))
                                if (check > 0)
                                    where += $" or {item.ValueShow} like '%{keySearch}%'";
                                else
                                    where += $" {item.ValueShow} like '%{keySearch}%'";

                            check++;
                        }

                        if (!string.IsNullOrEmpty(model.NameColumn) && !string.IsNullOrEmpty(keySearch))

                            where += " ) ";
                    }
                }

                var count = 0;
                var ds = new DataSet();

                // excute query<DataTable>
                var modelquery = new SelectListItem();
                if (model.ActiveGetAllData == false && model.OptionSelectColumn != null &&
                    model.OptionSelectColumn != null && model.OptionSelectColumn.Split(',') != null &&
                    model.OptionSelectColumn.Split(',').Length > 0)
                    modelquery.Value = "select " + model.OptionSelectColumn + " from " + model.NameColumn + "  " +
                                       where + "  LIMIT 100 OFFSET " +
                                       numberPage * 100 + " ";
                else
                    modelquery.Value = "select * from " + model.NameColumn + "  " + where + "  LIMIT 100 OFFSET " +
                                       numberPage * 100 + " ";
                var dataTable = ApiHelper.Execute<DataTable>("/storage-value/run-query-object", modelquery,
                    Method.POST,
                    ApiHosts.Dashboard);
                var listdataTable = dataTable.data;
                count = dataTable.totalCount;
                var dtDataName = "Data";
                listdataTable.TableName = dtDataName;
                ds.Tables.Add(listdataTable);
                var modelqueryCount = new SelectListItem();
                modelqueryCount.Value = "select count(*) from " + model.NameColumn + " ";
                var dataTableCount = await ApiHelper.ExecuteDynamicAsync("/storage-value/count-table", modelqueryCount,
                    Method.POST,
                    ApiHosts.Dashboard);

                count = (int)dataTableCount.data;

                WorkbookDesigner report = new WorkbookDesigner();

                var wd = new WorkbookDesigner(workbook);
                wd.SetDataSource(dataSet: ds);
                wd.Process();
                wd.Workbook.CalculateFormula();
                var ms = new MemoryStream();
                workbook.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
                ms.Seek(0, SeekOrigin.Begin);
                ms.Position = 0;
                ms.Flush();
                ms.Close();
                mst.Seek(0, SeekOrigin.Begin);
                mst.Position = 0;
                mst.Flush();
                mst.Close();
                workbook.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
// trả về định dạng file tương ứng để hiển thị lên speadjs
                var result = new ExtensionModel()
                {
                    FileConvert = Convert.ToBase64String(ms.ToByteArray()),
                    TotalCount = count
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                NotifyWarning("Có lỗi xảy ra, vui lòng thử lại sau !");
                return Ok(new XBaseResult { success = false });
            }
        }


        private static string ConvertStringToUpper(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "";
            //khai báo một mảng kiểu char sau đó sử dụng phuonge thức ToCharArray() để chuyển đổi chuỗi message thành kiểu mảng char
            char[] charArray = message.ToCharArray();
            bool foundSpace = true;
            for (int i = 0; i < charArray.Length; i++)
            {
                //sử dụng phương thức IsLetter() để kiểm tra từng phần tử có phải là một chữ cái
                if (Char.IsLetter(charArray[i]))
                {
                    if (foundSpace)
                    {
                        //nếu phải thì sử dụng phương thức ToUpper() để in hoa ký tự đầu
                        charArray[i] = Char.ToUpper(charArray[i]);
                        foundSpace = false;
                    }
                }
                else
                {
                    foundSpace = true;
                }
            }

            //chuyển đổi kiểu mảng char thàng string
            message = new string(charArray);
            return message;
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectAsync(string nameTable)
        {
            if (string.IsNullOrEmpty(nameTable))
            {
                return null;
            }

            var res = await ApiHelper.ExecuteAsync<List<SelectTableModel>>(
                $"/select-table/get-select-table?NameTable={nameTable}", null, Method.GET, ApiHosts.Dashboard);
            if (res.data != null)
            {
                var list = new List<SelectItem>();
                foreach (var item in res.data)
                {
                    var tem = new SelectItem()
                    {
                        text = item.TextShow,
                        id = item.ValueShow
                    };
                    list.Add(tem);
                }

                return Ok(list);
            }

            return Ok(new List<SelectItem>());
        }

        public IActionResult GetFileTemplateToAddOrUpdateToFTTH()
        {
            string path = CommonHelper.MapPath("/wwwroot/Template/TemplateMasterVTCNTT.xlsx");
            var fileStream = System.IO.File.OpenRead(path);
            //   return File(fileStream, ContentTypeXlsx);

            return Ok(Convert.ToBase64String(fileStream.ToByteArray()));
        }


        private static string GetExtension(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Unknown";
            var Cut = name.Split(".");
            return Cut[Cut.Length - 1];
        }


        /// <summary>
        /// Gọi Api lấy cấu trúc cây 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetTree()
        {
            var res = await ApiHelper.ExecuteAsync<List<TypeValuesTreeModel>>("/type-value/get-tree?showHidden=true",
                null, Method.POST, ApiHosts.Dashboard);

            var data = res.data;
            IList<TypeValuesTreeModel> cg = new List<TypeValuesTreeModel>();
            foreach (var item in data)
            {
                cg.Add(item);
            }

            var all = new TypeValuesTreeModel()
            {
                Name = "Tất cả",
                key = "",
                title = "Tất cả",
                tooltip = "Tất cả",
                children = cg,
                level = 1,
                expanded = true
            };
            res.data.Clear();
            res.data.Add(all);

            return Ok(res);
        }

        [HttpGet]
        public IActionResult GetGuid(int Count = 1)
        {
            var ListGuid = new List<string>();

            for (int i = 0; i < Count; i++)
            {
                ListGuid.Add(Guid.NewGuid().ToString());
            }

            return Ok(new XBaseResult
            {
                success = ListGuid.Count > 0,
                data = ListGuid
            });
        }


        [HttpGet]
        public IActionResult CheckRole()
        {
            return Ok(true);
        }
    }
}