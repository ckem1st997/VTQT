using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("file")]
    [ApiController]
    public class FileController : AdminApiController
    {
        #region Fields

        private readonly IFileService _fileService;

        #endregion

        #region Ctor

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(IEnumerable<FileModel> fileUploads)
        {
            if (fileUploads == null || !fileUploads.Any())
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            var fileEntities = fileUploads.Select(file => file.ToEntity()).ToList();

            await _fileService.InsertsAsync(fileEntities);

            return Ok(new XBaseResult
            {
                success = true,
                message = $"{fileEntities.Count} file uploaded!"
            });
        } 

        #endregion

        #region List

        [Route("get-list-by-ticket-id")]
        [HttpGet]
        public async Task<IActionResult> GetListByTicketId(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                return Ok(new XBaseResult
                {
                    success = false,
                    message = T("Common.Notify.NoItemsSelected")
                });
            }

            var results = new List<FileModel>();
            var fileEntities = _fileService.GetByTicketIdAsync(ticketId);
            if (fileEntities?.Count > 0)
            {
                results.AddRange(fileEntities.Select(f => f.ToModel()));
            }

            return Ok(new XBaseResult
            {
                success = true,
                data = results
            });
        }

        #endregion
    }
}