using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_PersonalTracking
{
    public static class DefinitionStatics
    {
        public static class StateTask
        {
            public static int OnEmployee = 1;
            public static int Delivered = 2;
            public static int Approved = 3;
        }
        public static class PermissionState
        {
            public static int OnEmployee = 1;
            public static int Approved = 2;
            public static int Disapproved = 3;
        }
    }
}
