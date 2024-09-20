using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Services.Extensions.Filebase
{
    public interface IFileUploadService
    {
        public Task<string> UploadImageToFirebaseVoucher(IFormFile file, string uid);
    }
}
