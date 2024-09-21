using AutoMapper;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.AppStarts
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // VOUCHER
            CreateMap<Voucher, VoucherDTO>().ReverseMap();
            CreateMap<Voucher, CreateVoucherDTO>().ReverseMap();
            CreateMap<Voucher, UpdateVoucherDTO>().ReverseMap();
            CreateMap<Voucher, GetVoucherDTO>().ReverseMap();
            CreateMap<GetVoucherDTO, VoucherFiler>().ReverseMap();
        }
    }
}
