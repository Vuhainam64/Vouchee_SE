using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IReportService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateReportAsync(Guid userId, CreateReportDTO createReportDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetReportDTO> GetReportByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetReportDTO>> GetReportsAsync(PagingRequest pagingRequest, ReportFilter reportFilter);
        public Task<DynamicResponseModel<GetReportDTO>> GetUserReportsAsync(PagingRequest pagingRequest, ReportFilter reportFilter, ThisUserObj thisUserObj);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdateReportAsync(Guid id, UpdateReportDTO updateReportDTO, ThisUserObj thisUserObj);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteReportAsync(Guid id, ThisUserObj thisUserObj);
    }
}
