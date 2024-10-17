using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Vouchee.API.Controllers
{
    [ApiController]
    [Route("api/subVoucher")]
    [EnableCors("MyAllowSpecificOrigins")]
    public class SubVoucherController : ControllerBase
    {
        
    }
}
