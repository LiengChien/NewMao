using Microsoft.Extensions.Configuration;
using NewMao.Entity.Portal;
using NewMao.Interface.BaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace NewMao.Service.BaseModel
{
    public class Permission : IPermission
    {
        IConfiguration _configuration;
        IEnumerable<ApiPermission> _apiPermissions;

        public Permission(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public IEnumerable<ApiPermission> ApiPermissionList
        {
            get
            {
                if (_apiPermissions == null)
                {
                    _apiPermissions = this.GetApiPermissionList();
                }

                return _apiPermissions;
            }
        }

        private IEnumerable<ApiPermission> GetApiPermissionList()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                string uri = _configuration["ConnectionStrings:PortalWebApi"] + "Permission/ApiPermission?clientId=scerp";

                HttpResponseMessage response = client.GetAsync(uri).Result;

                var strResult = response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Result<IEnumerable<ApiPermission>>>(strResult.Result);
                return result.Data;
            }
        }
    }
}
