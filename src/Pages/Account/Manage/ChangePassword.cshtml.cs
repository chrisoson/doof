// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace doof.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IStringLocalizer<ChangePasswordModel> _localizer;

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            IStringLocalizer<ChangePasswordModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(
                ErrorMessageResourceName = nameof(Resources.Pages.Account.Manage.ChangePasswordModel.old_password_required),
                ErrorMessageResourceType = typeof(Resources.Pages.Account.Manage.ChangePasswordModel))]
            [DataType(DataType.Password,
                ErrorMessageResourceName = nameof(Resources.Pages.Account.Manage.ChangePasswordModel.password_valid),
                ErrorMessageResourceType = typeof(Resources.Pages.Account.Manage.ChangePasswordModel))]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(
                ErrorMessageResourceName = nameof(Resources.Pages.Account.Manage.ChangePasswordModel.new_password_required),
                ErrorMessageResourceType = typeof(Resources.Pages.Account.Manage.ChangePasswordModel))]
            [StringLength(100,
                ErrorMessageResourceName = nameof(Resources.Pages.Account.Manage.ChangePasswordModel.new_password_length),
                ErrorMessageResourceType = typeof(Resources.Pages.Account.Manage.ChangePasswordModel))]
            [DataType(DataType.Password,
                ErrorMessageResourceName = nameof(Resources.Pages.Account.Manage.ChangePasswordModel.password_valid),
                ErrorMessageResourceType = typeof(Resources.Pages.Account.Manage.ChangePasswordModel))]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password,
                ErrorMessageResourceName = nameof(Resources.Pages.Account.Manage.ChangePasswordModel.password_valid),
                ErrorMessageResourceType = typeof(Resources.Pages.Account.Manage.ChangePasswordModel))]
            [Compare("NewPassword",
                ErrorMessageResourceName = nameof(Resources.Pages.Account.Manage.ChangePasswordModel.password_compare),
                ErrorMessageResourceType = typeof(Resources.Pages.Account.Manage.ChangePasswordModel))]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                //todo There is a problem here to localize.
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = _localizer["password-change-success"];

            return RedirectToPage();
        }
    }
}
