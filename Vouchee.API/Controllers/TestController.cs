using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vouchee.API.Helpers;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/test")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        // CREATE
        [HttpPost("create_voucher_test")]
        public async Task<IActionResult> CreateVoucherTest([FromBody] TestCreateVoucherDTO createVoucherDTO,
                                                            [FromQuery] Guid supplierId,
                                                            [FromQuery] Guid voucherTypeId)
        {
            var result = await _testService.CreateVoucher(createVoucherDTO, voucherTypeId, supplierId);
            return Ok(result);
        }
    }
}
