using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzDoGitPermissions
{

    public class PermissionsInfo
    {
        public Details[] Property1 { get; set; }
    }

    public class Details
    {
        public string Descriptor { get; set; }
        public string Id { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public Resource Resource { get; set; }
        public Permission[] Permissions { get; set; }
        public object Error { get; set; }
    }


    public class Permission
    {
        public string PermissionName { get; set; }
        public string EffectivePermission { get; set; }
        public bool IsPermissionInherited { get; set; }
    }

    internal class Class1
    {
    }
}
