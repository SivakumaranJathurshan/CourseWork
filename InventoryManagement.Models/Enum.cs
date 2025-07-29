using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models
{
    public enum OrderStatus
    {
        Pending = 10,
        Processing = 20,
        Shipped = 30,
        Delivered = 40,
        Cancelled = 50
    }
}
