using AutoMapper;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.API.AppStarts
{
    public class AutoMapperResolver : Profile
    {
        public AutoMapperResolver()
        {
            CreateMap<Voucher, VoucherDTO>().ReverseMap();
            CreateMap<Voucher, CreateVoucherDTO>().ReverseMap();
            CreateMap<Voucher, UpdateVoucherDTO>().ReverseMap();
            CreateMap<Voucher, GetVoucherDTO>().ReverseMap();
            CreateMap<GetVoucherDTO, VoucherFiler>().ReverseMap();
        }
    }
}
