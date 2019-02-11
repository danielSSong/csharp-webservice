using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Services.Interfaces
{
    public interface IUser
    {
        bool MatchTheUserInfo(LoginInfo user);
        User GetUserInfo(string userId);
        IEnumerable<UserRolesByUser> GetRolesOwendByUser(string userId);
        /// <summary>
        /// [User Register] 
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        int RegisterUser(RegisterInfo register);

        /// <summary>
        /// [Search For Update]
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserInfo GetUserInfoForUpdate(string userId);

        /// <summary>
        /// [User info update]
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        int UpdateUser(UserInfo user);
        /// <summary>
        /// [Compare Info in update]
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool CompareInfo(UserInfo user);
        /// <summary>
        /// [User Withdrawn Info]
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        int WithdrawnUser(WithdrawnInfo user);
    }
}
