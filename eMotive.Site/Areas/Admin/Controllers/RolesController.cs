﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eMotive.Managers.Interfaces;
using eMotive.Models.Objects.Roles;
using eMotive.Models.Validation.Role;
using eMotive.SCE.Common;
using eMotive.SCE.Common.ActionFilters;
using eMotive.Services.Interfaces;
using Extensions;

namespace eMotive.MMI.Areas.Admin.Controllers
{
    
    public class RolesController : Controller
    {
        private readonly IRoleManager roleManager;
        private readonly INotificationService notificationService;

        public RolesController(IRoleManager _roleManager, INotificationService _notificationService)
        {
            roleManager = _roleManager;
            notificationService = _notificationService;
        }
        [SCE.Common.ActionFilters.Authorize(Roles = "Super Admin")]
        public ActionResult Index()
        {
            return View();
        }
        [SCE.Common.ActionFilters.Authorize(Roles = "Super Admin")]
        public ActionResult List()
        {
            return View(roleManager.FetchAll());
        }

        [AjaxOnly]
        [SCE.Common.ActionFilters.Authorize(Roles = "Super Admin, Admin")]
        public CustomJsonResult GetAllRoles()
        {
            var roles = roleManager.FetchAll();

            return new CustomJsonResult
            {
                Data = new { success = roles.HasContent(), message = string.Empty, results = roles }
            };
        }

        [AjaxOnly]
        [SCE.Common.ActionFilters.Authorize(Roles = "Super Admin")]
        public CustomJsonResult CreateRole(Role role)
        {
            var validationErrors = Validate(role);

            if (validationErrors.HasContent())
            {
                return new CustomJsonResult
                {
                    Data = new { success = false, message = validationErrors, results = string.Empty }
                };
            }

            int newId;
            var success = roleManager.Create(role, out newId);

            var errors = !success ? notificationService.FetchIssues() : new string[] { };

            return new CustomJsonResult
            {
                Data = new { success = success, message = errors, results = newId }
            };
        }

        [AjaxOnly]
        [SCE.Common.ActionFilters.Authorize(Roles = "Super Admin")]
        public CustomJsonResult UpdateRole(Role role)
        {
            var validationErrors = Validate(role);

            if (validationErrors.HasContent())
            {
                return new CustomJsonResult
                {
                    Data = new { success = false, message = validationErrors, results = string.Empty }
                };
            }

            var success = roleManager.Update(role);

            var errors = !success ? notificationService.FetchIssues() : new string[] { };

            return new CustomJsonResult
            {
                Data = new { success = success, message = errors, results = string.Empty }
            };
        }

        [AjaxOnly]
        [SCE.Common.ActionFilters.Authorize(Roles = "Super Admin")]
        public CustomJsonResult DeleteRole(Role role)
        {
            var success = roleManager.Delete(role);

            var errors = !success ? notificationService.FetchIssues() : new string[] { };

            return new CustomJsonResult
            {
                Data = new { success = success, message = errors, results = string.Empty }
            };
        }

        private static IEnumerable<string> Validate(Role _role)
        {
            var validator = new RoleValidator();
            var result = validator.Validate(_role);

            return !result.IsValid ? result.Errors.Select(n => n.ErrorMessage) : null;
        }
    }
}
