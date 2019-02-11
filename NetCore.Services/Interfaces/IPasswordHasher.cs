using NetCore.Services.Bridges;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string GetGUIDSalt();
        string GetRNGSalt();
        string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt);
        bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash);
        /// <summary>
        /// This method is for membership register [password indication]
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        PasswordHashInfo SetPasswordInfo(string userId, string password);
    }
}
