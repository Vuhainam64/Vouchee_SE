using System.ComponentModel.DataAnnotations.Schema;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Models.DTOs
{

    public class CreateSupplierDTO 
    {
        public string? name { get; set; }
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateSupplierDTO 
    {
        public string? name { get; set; }
        public string? contact { get; set; }
        public string? image { get; set; }

        public DateTime? updateDate = DateTime.Now;
    }

    public class UpdateBankSupplierDTO
    {
        public string? bankAccount { get; set; }
        public string? bankName { get; set; }
        public string? bankNumber { get; set; }

        public DateTime? updateDate = DateTime.Now;
    }

    public class SupplierDTO
    {
        public Guid? id { get; set; }

        public string? name { get; set; }
        public string? image { get; set; }
        public string? contact { get; set; }
        public string? bankName { get; set; }
        public string? bankAccount { get; set; }
        public string? bankNumber { get; set; }

        public bool? isVerified { get; set; }
    }

    public class GetSupplierDTO : SupplierDTO
    {
        public int totalQuantitySold { get; set; }

        public virtual GetWalletDTO? supplierWallet { get; set; }
    }

    public class GetDetailSupplierDTO : SupplierDTO
    {
        public GetDetailSupplierDTO()
        {
            vouchers = [];
        }

        public virtual ICollection<GetVoucherDTO>? vouchers { get; set; }
    }

    public class GetSupplierNameandMoney
    {
        public string name { get; set; }
        public int amountofyear { get; set; }
    }
}