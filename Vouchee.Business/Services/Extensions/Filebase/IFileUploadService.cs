using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.Business.Services.Extensions.Filebase
{
    public interface IFileUploadService
    {
        public Task<string> UploadImageToFirebase(IFormFile file, string uid, StoragePathEnum storagePathEnum);
    }
}
