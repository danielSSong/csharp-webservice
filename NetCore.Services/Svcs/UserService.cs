﻿//using NetCore.Data.DataModels;
using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {
        private DBFirstDBContext _context;
        private IPasswordHasher _hasher;
        public UserService(DBFirstDBContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }
        #region private method
        private IEnumerable<User> GetUsersInfos()
        {
            return _context.Users.ToList();
            /*
            return new List<User>()
            {
                new User()
                {
                    UserId = "daniel",
                    UserName = "daniel Song",
                    UserEmail = "danielzzang5517@gmail.com",
                    Password = "123456",
                }
            };
            */
        }
        
        private User GetUserInfo(string userId, string password)
        {
            User user;
            /*Lambda
            user = _context.Users.Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();
            */

            //FromSQL
            /*Table
            user = _context.Users.FromSql("SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate FROM dbo.[User]")
                .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();
            */
            /*View
            user = _context.Users.FromSql("SELECT UserId, UserName, UserEmail, Password, IsMembershipWithrawn, JoinedUtcDate FROM dbo.uvwUser")
                .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();
            */
            /*Function
            user = _context.Users.FromSql($"SELECT UserId, UserName, UserEmail, Password, IsMembershipWithrawn, JoinedUtcDate FROM dbo.ufnUser({userId, password})")
                .FirstOrDefault();
            */
            //Stored Procedure
            user = _context.Users.FromSql("dbo.uspCheckLoginByUserId @p0, @p1", new[] { userId, password })
                                 .FirstOrDefault();
            if (user == null)
            {
                //the count of connection fail
                int rowAffected;
                /*SQL
                rowAffected = _context.Database.ExecuteSqlCommand($"Update dbo.[User] SET AccessFailedCount += 1 WHERE UserId={userId} AND Password={password}");
                */
                //Stored Procedure
                rowAffected = _context.Database.ExecuteSqlCommand("dbo.FailedLoginByUserId @p0", parameters:new[] { userId });
            }
            
            return user;
            
        }

        private bool checkTheUserInfo(string userId, string password)
        {
            //return GetUsersInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();
            return GetUserInfo(userId, password) != null ? true : false;
        }

        private User GetUserInfo(string userId)
        {
            return _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault();
        }
        private IEnumerable<UserRolesByUser> GetUserRolesByUserInfos(string userId)
        {
            var userRolesByUserInfos = _context.UserRolesByUsers.Where(uru => uru.UserId.Equals(userId)).ToList();
            foreach(var role in userRolesByUserInfos)
            {
                role.UserRole = GetUserRole(role.RoleId);
            }
            return userRolesByUserInfos.OrderByDescending(uru => uru.UserRole.RolePriority);
        }

        private UserRole GetUserRole(string roleId)
        {
            return _context.UserRoles.Where(ur => ur.RoleId.Equals(roleId)).FirstOrDefault();
        }

        private int RegisterUser(RegisterInfo register)
        {
            var utcNow = DateTime.UtcNow;
            var passwordInfo = _hasher.SetPasswordInfo(register.UserId, register.Password);
            var user = new User()
            {
                UserId = register.UserId.ToLower(),
                UserName = register.UserName,
                UserEmail = register.UserEmail,
                GUIDSalt = passwordInfo.GUIDSalt,
                RNGSalt = passwordInfo.RNGSalt,
                PasswordHash = passwordInfo.PasswordHash,
                AccessFailedCount = 0, 
                IsMembershipWithrawn = false,
                JoinedUtcDate = utcNow
            };

            var userRolesByUser = new UserRolesByUser()
            {
                UserId = register.UserId.ToLower(),
                RoleId = "AssociateUser",
                OwnedUtcDate = utcNow
            };
            _context.Add(user);
            _context.Add(userRolesByUser);
            return _context.SaveChanges();
        }

        private UserInfo GetUserInfoForUpdate(string userId)
        {
            var user = GetUserInfo(userId);
            var userInfo = new UserInfo()
            {
                UserId = null,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                ChangeInfo = new ChangeInfo()
                {
                    UserName = user.UserName,
                    UserEmail = user.UserEmail
                }
            };
            return userInfo;
        }

        private int UpdateUser(UserInfo user)
        {
            var userInfo = _context.Users.Where(u => u.UserId.Equals(user.UserId)).FirstOrDefault();

            if (userInfo == null)
            {
                return 0;
            }
            bool check = _hasher.CheckThePasswordInfo(user.UserId, user.Password, userInfo.GUIDSalt, userInfo.RNGSalt, userInfo.PasswordHash);
            int rowAffected = 0;
            if (check)
            {
                _context.Update(userInfo);

                userInfo.UserName = user.UserName;
                userInfo.UserEmail = user.UserEmail;
                rowAffected = _context.SaveChanges();
            }
            return rowAffected;
        }

        private bool MatchTheUserInfo(LoginInfo login)
        {
            var user = _context.Users.Where(u => u.UserId.Equals(login.UserId)).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            return _hasher.CheckThePasswordInfo(login.UserId, login.Password, user.GUIDSalt, user.RNGSalt, user.PasswordHash);
        }

        private bool CompareInfo(UserInfo user)
        {
            return user.ChangeInfo.Equals(user);
        }

        private int WithdrawnUser(WithdrawnInfo user)
        {
            var userInfo = _context.Users.Where(u => u.UserId.Equals(user.UserId)).FirstOrDefault();
            if (userInfo == null)
            {
                return 0;
            }
            bool check = _hasher.CheckThePasswordInfo(user.UserId, user.Password, userInfo.GUIDSalt, userInfo.RNGSalt, userInfo.PasswordHash);
            int rowAffected = 0;
            if (check)
            {
                _context.Remove(userInfo);

                rowAffected = _context.SaveChanges();
            }
            return rowAffected;
        }
        #endregion

        bool IUser.MatchTheUserInfo(LoginInfo login)
        {

            //return checkTheUserInfo(login.UserId, login.Password);
            return MatchTheUserInfo(login);
        }

        User IUser.GetUserInfo(string userId)
        {
            return GetUserInfo(userId);
        }

        public IEnumerable<UserRolesByUser> GetRolesOwendByUser(string userId)
        {
            return GetUserRolesByUserInfos(userId);
        }

        int IUser.RegisterUser(RegisterInfo register)
        {
            return RegisterUser(register);
        }

        UserInfo IUser.GetUserInfoForUpdate(string userId)
        {
            return GetUserInfoForUpdate(userId);
        }

        int IUser.UpdateUser(UserInfo user)
        {
            return UpdateUser(user);
        }

        bool IUser.CompareInfo(UserInfo user)
        {
            return CompareInfo(user);
        }

        int IUser.WithdrawnUser(WithdrawnInfo user)
        {
            return WithdrawnUser(user);
        }
    }
}
