using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Data.ViewModels
{
    /// <summary>
    /// Item info class
    /// </summary>
    public class ItemInfo
    {
        /// <summary>
        /// Item number
        /// </summary>
        public Guid ItemNo { get; set; }
        /// <summary>
        /// Item Name
        /// </summary>
        public string ItemName { get; set; }
    }
}
