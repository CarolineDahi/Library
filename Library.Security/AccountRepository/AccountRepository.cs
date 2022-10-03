using Library.Base;
using Library.DataTransferObjects.User;
using Library.Models.Security;
using Library.SharedKernel.Enums;
using Library.SharedKernel.ExtensionMethods;
using Library.SharedKernel.OperationResult;
using Library.SQL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.Security.AccountRepository
{
    public class AccountRepository : LibraryRepository, IAccountRepository
    {
        private readonly LibraryDBContext context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IConfiguration configuration;

        public AccountRepository(LibraryDBContext context, 
                                 UserManager<User> userManager,
                                 SignInManager<User> signInManager, 
                                 IConfiguration configuration) : base(context)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        public async Task<OperationResult<IEnumerable<GetUserDto>>> GetByType(UserType type)
        {
            var operation = new OperationResult<IEnumerable<GetUserDto>>();
            try
            {
                var users = await Context.Users.Where(u => !u.DateDeleted.HasValue 
                                                        && u.UserType.Equals(type))
                                               .Select(u => new GetUserDto
                                               {
                                                   Id = u.Id,
                                                   FullName = u.FullName,
                                                   DateCreated = u.DateCreated,
                                                   Email = u.Email,
                                                   PhoneNumber = u.PhoneNumber,
                                                   UserName = u.UserName,
                                               }).ToListAsync();

                operation.SetSuccess(users);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<GetAccountDto>> Login(LoginDto loginDto)
        {
            var operation = new OperationResult<GetAccountDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var user = await userManager.FindByNameAsync(loginDto.UserName);

                    if (user is null)
                    {
                        return operation.SetFailed($"{loginDto.UserName} Not Found", OperationResultType.NotExist);
                    }
                    var loginResult = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                    if (loginResult == SignInResult.Success)
                    {
                        var account = await FillAccount(user, true);

                        AssignRefreshTokenIfRememberMe(loginDto.RememberMe, account);

                        operation.SetSuccess(account);
                    }
                    else
                    {
                        operation.SetFailed("Failed login.");
                    }
                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    operation.SetException(ex);
                    transaction.Rollback();
                }
            }
            return operation;
        }

        public async Task<OperationResult<GetAccountDto>> Create(SetAccountDto accountDto)
        {
            var operation = new OperationResult<GetAccountDto>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var user = new User
                    {
                        FullName = accountDto.FullName,
                        UserName = accountDto.UserName,
                        Email = accountDto.Email,
                        PhoneNumber = accountDto.PhoneNumber,
                        UserType = accountDto.UserType,
                        GenerationStamp = ""
                    };

                    var identityResult = await userManager.CreateAsync(user, accountDto.Password);

                    if (!identityResult.Succeeded)
                    {
                        operation.SetFailed(String.Join(",", identityResult.Errors.Select(error => error.Description)));
                        return operation;
                    }
                    //user.DeviceToken = accountDto.DeviceToken;

                    IdentityResult roleIdentityResult;
                    if (user.UserType is UserType.Admin)
                    {
                        roleIdentityResult = await userManager.AddToRoleAsync(user, LibraryRole.Admin.ToString());
                    }
                    else if (user.UserType is UserType.EntryData)
                    {
                        roleIdentityResult = await userManager.AddToRoleAsync(user, LibraryRole.EntryData.ToString());
                    }
                    else
                    {
                        roleIdentityResult = await userManager.AddToRoleAsync(user, LibraryRole.SallingBook.ToString());
                    }
                    if (!roleIdentityResult.Succeeded)
                    {
                        operation.SetFailed(String.Join(",", roleIdentityResult.Errors.Select(error => error.Description)));
                        return operation;
                    }
                    await context.SaveChangesAsync();

                    var account = await FillAccount(user, true);
                    operation.SetSuccess(account);
                    transaction.Commit();   
                }
                catch (Exception ex)
                {
                    operation.SetException(ex);
                    transaction.Rollback();
                }
            }
            return operation;
        }

        public async Task<OperationResult<GetAccountDto>> Update(UpdateAccountDto accountDto)
        {
            var operation = new OperationResult<GetAccountDto>();
            using(var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var user = await userManager.FindByIdAsync(accountDto.Id.ToString());

                    if (user is null)
                        return operation.SetFailed($"{accountDto.UserName} Not Found");

                    user.UserName = accountDto.UserName;
                    user.Email = accountDto.Email;
                    user.FullName = accountDto.FullName;
                    user.PhoneNumber = accountDto.PhoneNumber;

                    if (accountDto.Password is not null && accountDto.Password != "")
                        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, accountDto.Password);

                    IdentityResult identityResult = await userManager.UpdateAsync(user);
                    await Context.SaveChangesAsync();

                    if (!identityResult.Succeeded)
                        return operation.SetFailed(String.Join(",", identityResult.Errors.Select(error => error.Description)));

                    var accountRes = await FillAccount(user, true);
                    operation.SetSuccess(accountRes);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    operation.SetException(ex);
                    transaction.Rollback();
                }
            }
            return operation;
        }

        public async Task<OperationResult<bool>> Delete(Guid id)
        {
            var operation = new OperationResult<bool>();
            try
            {
                operation = await DeleteRange(new List<Guid> { id });
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids)
        {
            var operation = new OperationResult<bool>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var users = await Context.Users.Where(user => ids.Contains(user.Id)).ToListAsync();

                    users.ForEach(user =>
                    {
                        user.DateDeleted = DateTime.UtcNow;
                    });

                    await Context.SaveChangesAsync();
                    operation.SetSuccess(true);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    operation.SetException(ex);
                    transaction.Rollback();
                }
            }
            return operation;
        }


        #region - HelperMethods -
        private void AssignRefreshTokenIfRememberMe(bool rememberMe, GetAccountDto accountDto) =>
            _ = !rememberMe ? accountDto.RefreshToken = string.Empty : string.Empty;

        private async Task<GetAccountDto> FillAccount(User user, bool isNewGeneration = false)
        {
            var roles = await userManager.GetRolesAsync(user);
            var expierDate = DateTime.Now.AddMinutes(GlobalValues.DefaultExpireTokenMinut);

            if (isNewGeneration)
            {
                user.GenerationStamp = Guid.NewGuid().ToString();
                await userManager.UpdateAsync(user);
            }

            var account = new GetAccountDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType,
                DateCreated = user.DateCreated,
                Token = GenerateJwtToken(user, roles, expierDate),
                RefreshToken = user.PasswordHash,
            };

            return account;
        }

        private string GenerateJwtToken(User user, IList<string> roles, DateTime expierDate)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("generate-date", DateTime.Now.ToLocalTime().ToString()),
                new Claim("generation-stamp", user.GenerationStamp),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                                             configuration["Jwt:Issuer"],
                                             claims,
                                             expires: expierDate,
                                             signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}
