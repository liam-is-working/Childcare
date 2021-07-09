using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Childcare.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ChildCareUser> _userManager;
        private readonly SignInManager<ChildCareUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ChildCareContext _db;

        public IndexModel(
            UserManager<ChildCareUser> userManager,
            SignInManager<ChildCareUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ChildCareContext db) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db =db;
        }

        public string Username { get; set; }

        public List<Specialty> Specialties {get;set;}

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name ="Full name")]
            public string FullName {get;set;}

            [Required]
            [DataType(DataType.Date)]
            [Display(Name ="DOB")]
            public DateTime DOB {get;set;}

            [Required]
            [DataType(DataType.Text)]
            [Display(Name ="Address")]
            public string Address{get;set;}

            [Required]
            [DataType(DataType.Text)]
            [Display(Name ="Citizen ID")]
            public string CitizenID{get;set;}

            [DataType(DataType.Text)]
            [Display(Name ="Role")]
            public string Role{get;set;}

            [Display(Name ="Specialty ID")]
            public int? SpecialtyID{get;set;}
        }

        private async Task LoadAsync(ChildCareUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                DOB = user.DOB,
                FullName = user.FullName,
                CitizenID = user.CitizenID,
                Address = user.Address,               
            };

            if(User.IsInRole("Staff")){
                Specialties = await _db.Specialties.ToListAsync();
                var staffInfo = await _db.Staffs.FirstOrDefaultAsync(s => s.ChildcareUserId == user.Id);
                if(staffInfo!=null){
                    Input.SpecialtyID = staffInfo.SpecialtyID;
                }
                
            }

            
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            var isCustomer = true;
            
            if(Input.Role == "Staff"){
                isCustomer = false;
                await _userManager.AddToRoleAsync(user, "Staff");
                var newStaff = new Staff{
                    SpecialtyID = new Random().Next(1,6),
                    ChildcareUserId = await _userManager.GetUserIdAsync(user),
                };
                await _db.Staffs.AddAsync(newStaff);
                await _db.SaveChangesAsync();
            }
                
            if(Input.Role == "Manager"){
                isCustomer = false;
                await _userManager.AddToRoleAsync(user, "Manager");
                var newManager = new Manager{
                    ChildcareUserId = await _userManager.GetUserIdAsync(user),
                };
                await _db.Managers.AddAsync(newManager);
                await _db.SaveChangesAsync();
            }

            if(Input.Role == null)
                isCustomer =false;

            if(isCustomer){
                var newCustomer = new Customer{
                    ChildcareUserId = await _userManager.GetUserIdAsync(user),
                };
                await _db.Customers.AddAsync(newCustomer);
                await _db.SaveChangesAsync();
            }
                

            if(!user.FullName.Equals(Input.FullName))
                user.FullName = Input.FullName;
            
            if(!user.DOB.Equals(Input.DOB))
                user.DOB = Input.DOB;

            if(!user.CitizenID.Equals(Input.CitizenID))
                user.CitizenID = Input.CitizenID;

            if(!user.Address.Equals(Input.Address))
                user.Address = Input.Address;

                

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
