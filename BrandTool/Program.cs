using Newtonsoft.Json;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.DTOs;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Vouchee.Data.Models.Entities;

internal class Program
{
    private static void Main(string[] args)
    {
        using (var context = new VoucheeContext())
        {
            string baseDirectory = Directory.GetCurrentDirectory();

            // Specify the file names (relative to the Data folder)
            string[] fileNames = {
                "dealtoday_e_voucher.json",
                "dealtoday_with_coordinates.json",
                "giftpop_with_coordinates.json",
                "gotit_with_coordinates.json",
                "urbox_with_coordinates.json"
            };

            var allData = new List<TestCreateVoucherDTO>();

            foreach (var file in fileNames)
            {
                string filePath = Path.Combine(baseDirectory, file); // No need for "Data" folder

                var jsonData = File.ReadAllText(filePath);

                var brand = JsonConvert.DeserializeObject<List<TestCreateVoucherDTO>>(jsonData);

                allData.AddRange(brand);
            }

            // 1. Address Deduplication
            var uniqueAddresses = new Dictionary<string, Address>();
            foreach (var voucherData in allData)
            {
                foreach (var addressData in voucherData.address)
                {
                    var addressHash = CalculateAddressHash(addressData);

                    if (!uniqueAddresses.ContainsKey(addressHash))
                    {
                        var address = new Address
                        {
                            CreateBy = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                            CreateDate = DateTime.Now,
                            Lon = addressData.lon,
                            Lat = addressData.lat,
                            IsVerfied = true,
                            Name = addressData.address_name,
                            Status = "ACTIVE",
                            VerifiedBy = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                            VerifiedDate = DateTime.Now,
                        };

                        uniqueAddresses.Add(addressHash, address);
                        context.Address.Add(address);
                    }
                }
            }

            // 2. Import Brands and Addresses
            var uniqueBrands = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Case-insensitive HashSet
            foreach (var voucherData in allData)
            {
                if (!uniqueBrands.Contains(voucherData.title))
                {
                    uniqueBrands.Add(voucherData.title);

                    // Create a new brand
                    var existingBrand = new Brand
                    {
                        Id = Guid.NewGuid(),
                        Name = voucherData.title,
                        Image = "https://firebasestorage.googleapis.com/v0/b/vouchee-504da.appspot.com/o/IMAGE%2FBRAND%2F50819.jpg?alt=media&token=d5d9b951-ec08-409f-a440-bcdc5ed1aa38",
                        Status = "ACTIVE",
                        VerifiedBy = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                        VerifiedDate = DateTime.Now,
                        CreateBy = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                        CreateDate = DateTime.Now,
                        IsVerfied = true
                    };

                    context.Brands.Add(existingBrand);

                    // Add addresses to the brand (without duplicates)
                    foreach (var addressData in voucherData.address)
                    {
                        var addressHash = CalculateAddressHash(addressData);

                        // Check if the address is already associated with the brand
                        if (!existingBrand.Addresses.Any(a => a.AddressHash == addressHash))
                        {
                            // Get the address from the uniqueAddresses dictionary
                            var address = uniqueAddresses[addressHash];

                            // Add the address to the brand's collection
                            existingBrand.Addresses.Add(address); // This is how you associate addresses with brands
                        }
                    }
                }
            }

            context.SaveChanges();
        }
    }

    // Address Hashing Function
    public static string CalculateAddressHash(TestCreateAddressDTO addressData)
    {
        // Combine key fields for hashing
        var hashString = $"{addressData.lon}_{addressData.lat}";

        // Use a hashing function to generate the hash
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(hashBytes); // Convert to a base64 string
        }
    }
}