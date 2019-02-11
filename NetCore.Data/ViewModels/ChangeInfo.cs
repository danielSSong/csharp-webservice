using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NetCore.Data.ViewModels
{
    public class ChangeInfo
    {
        
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        /// <summary>
        /// Compare 
        /// true: All the same info
        /// false: Not the same info
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(UserInfo other)
        {
            if (!string.Equals(UserName, other.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (!string.Equals(UserEmail, other.UserEmail, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true; 
        }
    }
}
