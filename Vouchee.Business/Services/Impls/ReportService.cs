using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class ReportService : IReportService
    {
        private readonly IBaseRepository<Media> _mediaRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Report> _reportRepository;
        private readonly IMapper _mapper;

        public ReportService(IBaseRepository<Media> mediaRepository, IBaseRepository<User> userRepository, IBaseRepository<Report> reportRepository, IMapper mapper)
        {
            _mediaRepository = mediaRepository;
            _userRepository = userRepository;
            _reportRepository = reportRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateReportAsync(Guid userId, CreateReportDTO createReportDTO, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.GetByIdAsync(userId);
            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user");
            }

            var newReport = _mapper.Map<Report>(createReportDTO);
            newReport.UserId = userId;
            newReport.CreateBy = thisUserObj.userId;
            newReport.Medias = [];

            int index = 0;

            foreach (var image in createReportDTO.imageUrl)
            {
                newReport.Medias.Add(new Media()
                {
                    Url = image,
                    CreateBy = thisUserObj.userId,
                    CreateDate = DateTime.Now,
                    Index = index++,
                });
            }

            var result = await _reportRepository.AddAsync(newReport);

            return new ResponseMessage<Guid>()
            {
                message = "Tạo report thành công",
                result = true,
                value = (Guid) result
            };
        }

        public async Task<ResponseMessage<bool>> DeleteReportAsync(Guid id, ThisUserObj thisUserObj)
        {
            var existedReport = await _reportRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Medias), isTracking: true);
            if (existedReport == null)
            {
                throw new Exception("Không tìm thấy report này");
            }
            if (existedReport.Medias.Count != 0)
            {
                foreach (var media in existedReport.Medias)
                {
                    await _mediaRepository.DeleteAsync(media);
                }
            }

            await _reportRepository.DeleteAsync(existedReport);

            return new ResponseMessage<bool>()
            {
                message = "Xóa report thành công",
                result = true,
                value = true
            };
        }

        public async Task<GetReportDTO> GetReportByIdAsync(Guid id)
        {
            var existedReport = await _reportRepository.GetByIdAsync(id);
            if (existedReport == null)
            {
                throw new Exception("Không tìm thấy report này");
            }

            return _mapper.Map<GetReportDTO>(existedReport);
        }

        public async Task<DynamicResponseModel<GetReportDTO>> GetReportsAsync(PagingRequest pagingRequest, ReportFilter reportFilter)
        {
            (int, IQueryable<GetReportDTO>) result;
            try
            {
                result = _reportRepository.GetTable()
                            .ProjectTo<GetReportDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetReportDTO>(reportFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return new DynamicResponseModel<GetReportDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetReportDTO>> GetUserReportsAsync(PagingRequest pagingRequest, ReportFilter reportFilter, ThisUserObj thisUserObj)
        {
            (int, IQueryable<GetReportDTO>) result;
            try
            {
                result = _reportRepository.GetTable()
                            .Where(x => x.UserId == thisUserObj.userId)
                            .ProjectTo<GetReportDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetReportDTO>(reportFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
            return new DynamicResponseModel<GetReportDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<ResponseMessage<bool>> UpdateReportAsync(Guid id, UpdateReportDTO updateReportDTO, ThisUserObj thisUserObj)
        {
            var existedReport = await _reportRepository.GetByIdAsync(id, isTracking: true);
            if (existedReport == null)
            {
                throw new NotFoundException("Không tìm thấy report này");
            }

            existedReport = _mapper.Map(updateReportDTO, existedReport );
            existedReport.UpdateBy = thisUserObj.userId;

            await _reportRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật report thành công",
                result = true,
                value = true
            };
        }
    }
}
