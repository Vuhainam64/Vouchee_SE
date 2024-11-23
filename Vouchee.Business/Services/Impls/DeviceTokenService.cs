using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class DeviceTokenService : IDeviceTokenService
    {
        private readonly IBaseRepository<DeviceToken> _devicetokenRepository;
        private readonly IMapper _mapper;

        public async Task<IActionResult> RegisterDevice([FromBody] CreateDeviceTokenDTO dto)
        {
            if (dto.Token == null || dto.UserId == Guid.Empty)
            {
                throw new Exception ("Token and UserId are required.");
            }

            /*var existingToken = await _devicetokenRepository.GetTable()
                .Where(d => d.Token == dto.Token);*/
            var existingToken = await _devicetokenRepository.GetTable().ToListAsync() ;
            if (existingToken == null)
            {
                var newToken = new DeviceToken
                {
                    UserId = dto.UserId,
                    Token = dto.Token,
                    Platform = dto.Platform.ToString(),
                    CreateDate = DateTime.UtcNow
                };

                _devicetokenRepository.AddAsync(newToken);
                await _devicetokenRepository.SaveChanges();
            }

            return Ok(new { Message = "Device registered successfully." });
        }
}
