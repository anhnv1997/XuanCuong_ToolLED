using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases
{
    public class ListItem
    {
        private string lname;
        private string lvalue;

        public string Name
        {
            set { lname = value; }
            get { return lname; }
        }
        public string Value
        {
            set { lvalue = value; }
            get { return lvalue; }
        }
    }
}
