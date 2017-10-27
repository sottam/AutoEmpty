using System;
using ICities;

namespace AutoEmpty
{
    public class AutoEmpty : IUserMod
    {
        public String Name
        {
            get { return "Automatic Empty"; }
        }

        public String Description
        {
            get { return "Automatically starts emptying and stops whenever it finishes. Works for landfills and cemeteries"; }
        }
    }
}
