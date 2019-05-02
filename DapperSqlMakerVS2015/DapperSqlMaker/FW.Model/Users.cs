using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FW.Model
{
    public partial class Users
    {
        //public Users() { this.LockPerss = new List<LockPers>(); }

        public List<LockPers> LockPerss { get; set; }
    }
}
