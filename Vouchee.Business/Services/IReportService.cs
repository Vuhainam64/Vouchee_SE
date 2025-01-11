using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IReportService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateUserReportAsync(Guid userId, CreateReportDTO createReportDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<Guid>> CreateRatingReportAsync(Guid ratingId, CreateReportDTO createReportDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetReportDTO> GetReportByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetReportDTO>> GetReportsAsync(PagingRequest pagingRequest, ReportFilter reportFilter, ReportTypeEnum? reportTypeEnum);
        public Task<DynamicResponseModel<GetReportDTO>> GetUserReportsAsync(PagingRequest pagingRequest, ReportFilter reportFilter, Guid userId);
        public Task<DynamicResponseModel<GetReportDTO>> GetRatingReportsAsync(PagingRequest pagingRequest, ReportFilter reportFilter, Guid ratingId);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdateReportAsync(Guid id, UpdateReportDTO updateReportDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteReportAsync(Guid id, ThisUserObj thisUserObj);
    }
}
