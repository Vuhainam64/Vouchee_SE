using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vouchee.API.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services;
using Vouchee.Business.Services.Impls;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/v1/partnerTransaction")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class PartnerTransactionController : ControllerBase
    {
        private readonly IPartnerTransactionService _partnerTransactionService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public PartnerTransactionController(IPartnerTransactionService partnerTransactionService, 
                                                IUserService userService, 
                                                IRoleService roleService)
        {
            _partnerTransactionService = partnerTransactionService;
            _userService = userService;
            _roleService = roleService;
        }

        [HttpPost("create_partner_transaction")]
        public async Task<IActionResult> CreatePartnerTransaction([FromBody] CreateSePayPartnerInTransactionDTO createPartnerInTransactionDTO)
        {
            var result = await _partnerTransactionService.CreatePartnerTransactionAsync(createPartnerInTransactionDTO);
            return Ok(result);
        }
    }
}