using Aspose.Cells;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Core.Domain.Master;
using VTQT.Services.Master;
using VTQT.SharedMvc.Master.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Master.Controllers
{
    [Route("location")]
    [ApiController]
    [XBaseApiAuthorize]
    [Produces("application/json")]
    [AppApiController("Master.Controllers.Location")]
    public class LocationController : AdminApiController
    {
        #region Fields
        private readonly List<Location> locations = new List<Location>();
        private readonly Workbook workbook;
        private readonly Worksheet worksheet;
        private const string FILE_PATH = "wwwroot\\_AppFiles\\ReportTemplates\\Common\\Localtion_Seed_Data.csv";
        #endregion

        #region Ctor
        public LocationController()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            LoadOptions loadOptions = new LoadOptions(LoadFormat.CSV);
            workbook = new Workbook(FILE_PATH, loadOptions);
            worksheet = workbook.Worksheets[workbook.Worksheets.ActiveSheetIndex];
            InitLocations();
        }
        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Master.AppActions.Locations.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        /// <summary>
        /// Lấy dữ liệu vị trí theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("get-by-id")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetById(string id)
        {
            if (locations.Count > 0)
            {
                var entity = locations.FirstOrDefault(x => x.LocationId == id);
                if (entity != null)
                {
                    var model = new LocationModel
                    {
                        LocationId = entity.LocationId,
                        Code = entity.Code,
                        Name = entity.Name
                    };

                    return Ok(new XBaseResult
                    {
                        data = model,
                        success = true
                    });
                }
            }

            return Ok(new XBaseResult
            {
                success = false,
                message = string.Format(
                    T("Common.Notify.DoesNotExist"),
                    T("Common.Location"))
            });
        }
        #endregion

        #region List
        /// <summary>
        /// Lấy danh sách vị trí phân trang
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get([FromQuery] LocationSearchModel searchModel)
        {
            var searchContext = new LocationSearchContext
            {
                Keywords = searchModel.Keywords?.Trim(),
                PageIndex = searchModel.PageIndex - 1,
                PageSize = searchModel.PageSize
            };

            var query = from p in locations select p;

            if (searchContext.Keywords != null && searchContext.Keywords.HasValue())
            {
                query = from p in query
                        where (p.Code != null && p.Code.Contains(searchContext.Keywords)) ||
                              (p.Name != null && p.Name.Contains(searchContext.Keywords))
                        select p;
            }

            query = from p in query orderby p.Code select p;

            var entities = new PagedList<Location>(query.ToList(), searchContext.PageIndex, searchContext.PageSize);
            var models = new List<LocationModel>();

            if (entities?.Count > 0)
            {
                foreach (var e in entities)
                {
                    var m = new LocationModel
                    {
                        LocationId = e.LocationId,
                        Code = e.Code,
                        Name = e.Name
                    };
                    models.Add(m);
                }
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = models,
                totalCount = entities.TotalCount
            });
        }
        #endregion

        #region Utilities
        private void InitLocations()
        {
            if (worksheet?.Cells?.MaxDataRow > 1)
            {
                for(int row = 0; row < worksheet.Cells.MaxDataRow; row++)
                {
                    int column = 0;
                    var entity = new Location
                    {
                        LocationId = worksheet.Cells.GetCell(row + 1, column++).StringValue,
                        Name = worksheet.Cells.GetCell(row + 1, column++).StringValue,
                        ParentId = worksheet.Cells.GetCell(row + 1, column++).StringValue,
                        Level = worksheet.Cells.GetCell(row + 1, column++).IntValue,
                        Active = worksheet.Cells.GetCell(row + 1, column++).IntValue != 0,
                        IsDeleted = worksheet.Cells.GetCell(row + 1, column++).IntValue != 0,
                        IsShow = worksheet.Cells.GetCell(row + 1, column++).IntValue != 0,
                        Code = worksheet.Cells.GetCell(row + 1, column++).StringValue,
                        ParentName = worksheet.Cells.GetCell(row + 1, column++).StringValue,
                        Path = worksheet.Cells.GetCell(row + 1, column++).StringValue
                    };
                    locations.Add(entity);
                }
            }
        }
        #endregion

    }
}
