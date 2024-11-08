using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Services
{
    public interface IBankingService
    {
        Task<string> LoginAsync(string username, string password, string captchaText);
        Task<string> GetBalanceAsync(string sessionId, string accountNumber);
        Task<string> GetTransactionHistoryAsync(string fromDate, string toDate, string sessionId, string accountNumber);
        Task<string> GetCaptchaAsync();
        Task<string> CreateTaskCaptchaAsync(string base64Image);
        Task<string> CheckProgressCaptchaAsync(string taskId);
    }
}
