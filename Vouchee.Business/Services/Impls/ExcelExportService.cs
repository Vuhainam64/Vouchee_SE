using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly IBaseRepository<WalletTransaction> _walletTransactionRepository;

        public ExcelExportService(IBaseRepository<WalletTransaction> walletTransactionRepository)
        {
            _walletTransactionRepository = walletTransactionRepository;
        }

        public async Task<byte[]> GenerateStatementExcel(ThisUserObj thisUserObj, DateTime startTime, DateTime endTime)
        {
            // Execute the query and materialize the results into a list
            var transactions = await _walletTransactionRepository.GetTable()
                                .Where(x => x.CreateDate >= startTime && x.CreateDate <= endTime)
                                .Where(x => x.SellerWallet.SellerId == thisUserObj.userId)
                                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Seller Wallet Statement");

                // Add Header Row
                worksheet.Cells[1, 1].Value = "Transaction ID";
                worksheet.Cells[1, 2].Value = "Order ID";
                worksheet.Cells[1, 3].Value = "Amount";
                worksheet.Cells[1, 4].Value = "Status";
                worksheet.Cells[1, 5].Value = "Created Date";
                worksheet.Cells[1, 6].Value = "Updated Date";

                // Apply styles to the header row
                using (var headerRange = worksheet.Cells[1, 1, 1, 6])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Populate Data Rows
                for (int i = 0; i < transactions.Count; i++)
                {
                    var transaction = transactions[i];
                    var row = i + 2; // Start from row 2 (below the header)

                    worksheet.Cells[row, 1].Value = transaction.Id.ToString();
                    worksheet.Cells[row, 2].Value = transaction.OrderId;
                    worksheet.Cells[row, 3].Value = transaction.Amount;
                    worksheet.Cells[row, 4].Value = transaction.Status;
                    worksheet.Cells[row, 5].Value = transaction.CreateDate?.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 6].Value = transaction.UpdateDate?.ToString("yyyy-MM-dd HH:mm:ss");
                }

                // Auto-fit columns for better readability
                worksheet.Cells[1, 1, transactions.Count + 1, 6].AutoFitColumns();

                // Return the Excel file as a byte array
                return package.GetAsByteArray();
            }
        }
    }
}