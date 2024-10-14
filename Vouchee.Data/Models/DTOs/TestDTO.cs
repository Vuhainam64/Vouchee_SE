using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.DTOs
{
    public class TestCreateVoucherDTO
    {
        public TestCreateVoucherDTO()
        {
            address = new List<TestCreateAddressDTO>();
        }

        public string title { get; set; }
        public IList<TestCreateAddressDTO> address { get; set; }
    }

    public class TestCreateAddressDTO
    {
        public string? address_name { get; set; }
        public decimal? lon { get; set; }
        public decimal? lat { get; set; }
    }
}
