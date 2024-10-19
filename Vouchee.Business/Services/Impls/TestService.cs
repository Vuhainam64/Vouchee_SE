﻿using AutoMapper;
using Google.Api;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class TestService : ITestService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public TestService(ISupplierRepository supplierRepository,
                            IUserRepository userRepository,
                            IVoucherRepository voucherRepository, 
                            IAddressRepository addressRepository,
                            ICategoryRepository categoryRepository,
                            IBrandRepository brandRepository,
                            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _userRepository = userRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _voucherRepository = voucherRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<Brand> CreateBrand(TestCreateVoucherDTO testCreateVoucherDTO)
        {
            Brand brand = new()
            {
                //PercentShow = 1,
                VerifiedDate = DateTime.Now,
                Status = ObjectStatusEnum.ACTIVE.ToString(),
                VerifiedBy = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                CreateBy = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                CreateDate = DateTime.Now,
                //Description = "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of \"de Finibus Bonorum et Malorum\" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, \"Lorem ipsum dolor sit amet..\", comes from a line in section 1.10.32.",
                Image = "https://firebasestorage.googleapis.com/v0/b/vouchee-504da.appspot.com/o/IMAGE%2FBRAND%2F50819.jpg?alt=media&token=d5d9b951-ec08-409f-a440-bcdc5ed1aa38",
                IsVerfied = true,
                Name = testCreateVoucherDTO.title,
            };

            foreach (var address in testCreateVoucherDTO.address)
            {
                if (address.lon != null && address.lat != null)
                {
                    var trackedAddress = _brandRepository.GetAddress((decimal)address.lon, (decimal)address.lat);

                    if (trackedAddress == null)
                    {
                        trackedAddress = await _addressRepository.GetFirstOrDefaultAsync(x => x.Lon == address.lon && x.Lat == address.lat);
                    }

                    if (trackedAddress != null)
                    {
                        brand.Addresses.Add(trackedAddress);
                    }
                    else
                    {
                        Address newAddress = new()
                        {
                            Id = Guid.Empty,
                            Name = address.address_name,
                            CreateBy = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                            CreateDate = DateTime.Now,
                            //Image = "https://firebasestorage.googleapis.com/v0/b/vouchee-504da.appspot.com/o/IMAGE%2FBRAND%2F50819.jpg?alt=media&token=d5d9b951-ec08-409f-a440-bcdc5ed1aa38",
                            Lat = address.lat,
                            Lon = address.lon,
                            Status = ObjectStatusEnum.ACTIVE.ToString(),
                            //PercentShow = 1,
                        };
                        brand.Addresses.Add(newAddress);
                    }
                }
            }
            return await _brandRepository.Add(brand);
        }

        public async Task<Voucher> CreateVoucher(TestCreateVoucherDTO createVoucherDTO, Guid voucherTypeId, Guid supplierID)
        {
            Random random = new Random();
            Voucher voucher = new();

            // Initialize addresses for the voucher
            //voucher.Addresses = new List<Address>();

            // Process addresses from DTO
            foreach (var address in createVoucherDTO.address)
            {
                // Check if the address already exists in the database
                var existedAddress = _addressRepository.GetAddress(address.lon, address.lat);
                if(existedAddress != null) 
                {
                    _voucherRepository.Attach(existedAddress);

                    //voucher.Addresses.Add(existedAddress);
                }
                else
                {
                    // Create a new address if it doesn't exist
                    Address newAddress = new()
                    {
                        Name = address.address_name,
                        CreateBy = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                        CreateDate = DateTime.Now,
                        Lat = address.lat,
                        Lon = address.lon,
                        //PercentShow = random.Next(1, 100),
                        Status = ObjectStatusEnum.ACTIVE.ToString(),
                        //Image = "https://firebasestorage.googleapis.com/v0/b/vouchee-504da.appspot.com/o/IMAGE%2FADDRESS%2Fstore-4156934.svg?alt=media&token=56725d16-70fa-42b9-bc90-5c6bc709e1fa",
                    };

                    //voucher.Addresses.Add(newAddress); // Add the newly created address to the voucher
                }
                
            }

            // Set voucher details
            voucher.Title = createVoucherDTO.title;
            voucher.Quantity = random.Next(100, 1000);
            DateTime today = DateTime.Now;

            // Generate a random number of days for StartDate and EndDate
            int randomStartDays = random.Next(0, 30); // Start date will be between today and 30 days from now
            //voucher.StartDate = today.AddDays(randomStartDays); // Set StartDate

            int randomEndDays = random.Next(1, 90); // End date will be 1 to 90 days after the StartDate
            //voucher.EndDate = voucher.StartDate.AddDays(randomEndDays); // Set EndDate
            voucher.Status = VoucherStatusEnum.SELLING.ToString();
            voucher.CreateBy = _userRepository.GetTable().FirstOrDefault(x => x.RoleId.Equals(Guid.Parse("2D80393A-3A3D-495D-8DD7-F9261F85CC8F"))).Id;
            voucher.CreateDate = DateTime.Now;
            voucher.Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry...";
            //voucher.Image = "https://firebasestorage.googleapis.com/v0/b/vouchee-504da.appspot.com/o/IMAGE%2FVOUCHER%2Ftest.png?alt=media&token=11650450-ca07-45b1-a5d5-abbd90495d7a";
            //voucher.PercentShow = random.Next(1, 100);
            //voucher.Policy = "Lorem Ipsum is simply dummy text of the printing and typesetting industry...";
            voucher.OriginalPrice = random.Next(50000, 1000000);
            voucher.SupplierId = supplierID;
            //voucher.VoucherTypeId = voucherTypeId;

            try
            {
                await _voucherRepository.AddAsync(voucher);
            }
            catch (DbUpdateException ex)
            {
                // Log or inspect the inner exception
                var innerException = ex.InnerException?.Message ?? "No inner exception";
                Console.WriteLine($"Error saving to database: {innerException}");
            }



            return voucher;
        }

    }
}
