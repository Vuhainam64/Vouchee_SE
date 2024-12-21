using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IExcelExportService
    {
        Task<byte[]> GenerateStatementExcel(ThisUserObj thisUserObj, DateTime startTime, DateTime endTime);
        Task<byte[]> GenerateVoucherCodeExcel(ThisUserObj thisUserObj, DateTime startTime, DateTime endTime);
        Task<byte[]> GenerateWithdrawRequestExcel();
    }
}
