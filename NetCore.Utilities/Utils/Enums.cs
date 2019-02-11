using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Utilities.Utils
{
    // Encryption 
    public class Enums
    {
        public enum CryptoType
        {
            Unmanaged = 1,
            Managed = 2,
            CngCbc = 3,
            CngGcm = 4
        }
    }
}
