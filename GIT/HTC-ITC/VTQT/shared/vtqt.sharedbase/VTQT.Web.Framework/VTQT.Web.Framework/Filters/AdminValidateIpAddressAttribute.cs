﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using VTQT.Core;
using VTQT.Core.Domain.Security;

namespace VTQT.Web.Framework.Filters
{
    // Note: Có thể dùng cho mô hình triển khai đóng gói 1 khối hoặc tạm thời.
    // Còn trong mô hình tổng thể thì việc validate IP sẽ config ở Firewall
    /// <summary>
    /// Represents filter attribute that validates IP address
    /// </summary>
    public sealed class AdminValidateIpAddressAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public AdminValidateIpAddressAttribute() : base(typeof(ValidateIpAddressFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that validates IP address
        /// </summary>
        private class ValidateIpAddressFilter : IAsyncActionFilter
        {
            #region Fields

            private readonly IWebHelper _webHelper;
            private readonly SecuritySettings _securitySettings;

            #endregion

            #region Ctor

            public ValidateIpAddressFilter(IWebHelper webHelper,
                SecuritySettings securitySettings)
            {
                _webHelper = webHelper;
                _securitySettings = securitySettings;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task ValidateIpAddress(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //get action and controller names
                var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                var actionName = actionDescriptor?.ActionName;
                var controllerName = actionDescriptor?.ControllerName;

                if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                    return;

                //don't validate on the 'Access denied' page
                if (controllerName.Equals("Security", StringComparison.InvariantCultureIgnoreCase) &&
                    actionName.Equals("AccessDenied", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                //get allowed IP addresses
                var ipAddresses = _securitySettings.AdminAreaAllowedIpAddresses;

                //there are no restrictions
                if (ipAddresses == null || !ipAddresses.Any())
                    return;

                //whether current IP is allowed
                var currentIp = _webHelper.GetCurrentIpAddress();

                if (ipAddresses.Any(ip => ip.Equals(currentIp, StringComparison.InvariantCultureIgnoreCase)))
                    return;

                //redirect to 'Access denied' page
                context.Result = new RedirectToActionResult("AccessDenied", "Security", context.RouteData.Values);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called asynchronously before the action, after model binding is complete.
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                await ValidateIpAddress(context);
                if (context.Result == null)
                    await next();
            }

            #endregion
        }

        #endregion
    }
}
