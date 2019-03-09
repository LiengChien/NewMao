using System;
using System.Collections.Generic;
using System.Text;

namespace NewMao.Entity.Portal
{
    public class ApiPermission
    {
        public string ApipermissionId { get; set; }
        public string ClientId { get; set; }
        public int? RoleId { get; set; }
        public string HttpMethod { get; set; }
        public string ApiUri { get; set; }
        public int? DataLevel { get; set; }
    }
}
