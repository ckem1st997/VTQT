using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTQT.Core;
using VTQT.Services.Ticket;
using VTQT.SharedMvc.Ticket;
using VTQT.SharedMvc.Ticket.Models;
using VTQT.Web.Framework.Controllers;
using VTQT.Web.Framework.Security;

namespace VTQT.Api.Ticket.Controllers
{
    [Route("comment")]
    [ApiController]
    [XBaseApiAuthorize]
    [AppApiController("Ticket.Controllers.Comment")]
    public class CommentController : AdminApiController
    {
        #region Fields

        private readonly ICommentService _commentService;

        #endregion

        #region Ctor

        public CommentController(
            ICommentService commentService)
        {
            _commentService = commentService;
        }

        #endregion

        #region Methods

        [Route("index")]
        [HttpGet]
        [AppApiAction("Ticket.AppActions.Comment.Index")]
        public async Task<IActionResult> Index()
        {
            return Ok(new XBaseResult());
        }

        [Route("create")]
        [HttpPost]
        [AppApiAction("Ticket.AppActions.Comment.Create")]
        public async Task<IActionResult> Create(CommentModel model)
        {
            if (!ModelState.IsValid)
                return InvalidModelResult();

            var entity = model.ToEntity();
            entity.CreatedDate = DateTime.UtcNow;

            await _commentService.InsertAsync(entity);

            return Ok(new XBaseResult
            {
                message = string.Format(T("Common.Notify.Added"), T("Common.Comment"))
            });
        }
        #endregion Lists

        #region List
        [Route("get")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> Get(string ticketId)
        {
            var results = new List<CommentModel>();

            var comments = _commentService.GetByTicketIdAsync(ticketId);
            if (comments?.Count > 0)
            {
                foreach(var cmt in comments)
                {
                    var model = cmt.ToModel();
                    model.CreatedDate = cmt.CreatedDate.ToLocalTime();
                    model.StrCreatedDate = model.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                    model.TicketId = ticketId;
                    results.Add(model);
                }
            }

            return Ok(new XBaseResult
            {
                data = results,
                success = true
            });
        }

        [Route("get-CrMx")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetCrMx(string CrMxId)
        {
            var results = new List<CommentModel>();

            var comments = _commentService.GetByCRMxIdAsync(CrMxId);
            if (comments?.Count > 0)
            {
                foreach (var cmt in comments)
                {
                    var model = cmt.ToModel();
                    model.CreatedDate = cmt.CreatedDate.ToLocalTime();
                    model.StrCreatedDate = model.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                    model.CrMxId = CrMxId;
                    results.Add(model);
                }
            }

            return Ok(new XBaseResult
            {
                data = results,
                success = true
            });
        }

        [Route("get-Cr")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetCr(string CrId)
        {
            var results = new List<CommentModel>();

            var comments = _commentService.GetByCRIdAsync(CrId);
            if (comments?.Count > 0)
            {
                foreach (var cmt in comments)
                {
                    var model = cmt.ToModel();
                    model.CreatedDate = cmt.CreatedDate.ToLocalTime();
                    model.StrCreatedDate = model.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                    model.CrId = CrId;
                    results.Add(model);
                }
            }

            return Ok(new XBaseResult
            {
                data = results,
                success = true
            });
        }

        [Route("get-CrPartner")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetCrPartner(string CrPartnerId)
        {
            var results = new List<CommentModel>();

            var comments = _commentService.GetByCRPartnerIdAsync(CrPartnerId);
            if (comments?.Count > 0)
            {
                foreach (var cmt in comments)
                {
                    var model = cmt.ToModel();
                    model.CreatedDate = cmt.CreatedDate.ToLocalTime();
                    model.StrCreatedDate = model.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                    model.CrPartnerId = CrPartnerId;
                    results.Add(model);
                }
            }

            return Ok(new XBaseResult
            {
                data = results,
                success = true
            });
        }

        [Route("get-Ftth")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetFtth(string FtthId)
        {
            var results = new List<CommentModel>();

            var comments = _commentService.GetByFTTHIdAsync(FtthId);
            if (comments?.Count > 0)
            {
                foreach (var cmt in comments)
                {
                    var model = cmt.ToModel();
                    model.CreatedDate = cmt.CreatedDate.ToLocalTime();
                    model.StrCreatedDate = model.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                    model.FtthId = FtthId;
                    results.Add(model);
                }
            }

            return Ok(new XBaseResult
            {
                data = results,
                success = true
            });
        }

        [Route("get-wide-ftth")]
        [HttpGet]
        [MapAppApiAction(nameof(Index))]
        public async Task<IActionResult> GetWideFtth(string wideFtthId)
        {
            var results = new List<CommentModel>();

            var comments = _commentService.GetByWideFtthIdAsync(wideFtthId);
            if (comments?.Count > 0)
            {
                foreach (var cmt in comments)
                {
                    var model = cmt.ToModel();
                    model.CreatedDate = cmt.CreatedDate.ToLocalTime();
                    model.StrCreatedDate = model.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
                    model.WideFtthId = wideFtthId;
                    results.Add(model);
                }
            }

            return Ok(new XBaseResult
            {
                data = results,
                success = true
            });
        }

        #endregion

        #region Utilities


        #endregion
    }
}
