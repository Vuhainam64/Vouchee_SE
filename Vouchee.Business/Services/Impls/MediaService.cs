using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class MediaService : IMediaService
    {
        private readonly IBaseRepository<Media> _mediaRepository;
        private readonly IMapper _mapper;

        public MediaService(IBaseRepository<Media> mediaRepository,
                            IMapper mapper)
        {
            _mediaRepository = mediaRepository;
            _mapper = mapper;
        }
    }
}
