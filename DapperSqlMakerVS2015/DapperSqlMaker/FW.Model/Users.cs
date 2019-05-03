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

        public decimal numx { get; set; }
        public decimal? numxx { get; set; }
    }
}
