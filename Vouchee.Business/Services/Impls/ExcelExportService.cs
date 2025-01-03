﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly IBaseRepository<MoneyRequest> _moneyRequestRepository;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<WalletTransaction> _walletTransactionRepository;
        private readonly IBaseRepository<User> _userRepository;

        public ExcelExportService(IBaseRepository<MoneyRequest> moneyRequestRepository,
                                  IBaseRepository<VoucherCode> voucherCodeRepository,
                                  IBaseRepository<WalletTransaction> walletTransactionRepository,
                                  IBaseRepository<User> userRepository)
        {
            _moneyRequestRepository = moneyRequestRepository;
            _voucherCodeRepository = voucherCodeRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _userRepository = userRepository;
        }

        public async Task<byte[]> GenerateStatementExcel(ThisUserObj thisUserObj, DateTime startTime, DateTime endTime)
        {
            // Execute the query and materialize the results into a list
            var transactions = await _walletTransactionRepository.GetTable()
                                .Where(x => x.CreateDate >= startTime && x.CreateDate <= endTime)
                                .Where(x => x.SellerWallet.SellerId == thisUserObj.userId)
                                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

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

        public async Task<byte[]> GenerateVoucherCodeExcel(ThisUserObj thisUserObj, DateTime startTime, DateTime endTime)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.Supplier));

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user nào");
            }

            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("User này không thuộc supplier nào");
            }

            var voucherCodes = await _voucherCodeRepository.GetTable()
                .Where(x => x.Modal.Voucher.SupplierId == existedUser.SupplierId &&
                            x.UpdateDate >= startTime &&
                            x.UpdateDate <= endTime)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Voucher Codes");

                // Add Header Row
                worksheet.Cells[1, 1].Value = "Voucher Code ID";
                worksheet.Cells[1, 2].Value = "Code";
                worksheet.Cells[1, 3].Value = "New Code";
                worksheet.Cells[1, 4].Value = "Status";
                worksheet.Cells[1, 5].Value = "Update Date";
                worksheet.Cells[1, 6].Value = "Modal Name";
                worksheet.Cells[1, 7].Value = "Voucher Name";

                // Apply styles to the header row
                using (var headerRange = worksheet.Cells[1, 1, 1, 5])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Populate Data Rows
                for (int i = 0; i < voucherCodes.Count; i++)
                {
                    var voucherCode = voucherCodes[i];
                    var row = i + 2; // Start from row 2 (below the header)

                    worksheet.Cells[row, 1].Value = voucherCode.Id.ToString();
                    worksheet.Cells[row, 2].Value = voucherCode.Code;
                    worksheet.Cells[row, 3].Value = voucherCode.NewCode;
                    worksheet.Cells[row, 4].Value = voucherCode.Status;
                    worksheet.Cells[row, 5].Value = voucherCode.UpdateDate?.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 6].Value = voucherCode.Modal.Title;
                    worksheet.Cells[row, 7].Value = voucherCode.Modal.Voucher.Title;
                }

                // Auto-fit columns for better readability
                worksheet.Cells[1, 1, voucherCodes.Count + 1, 7].AutoFitColumns();

                // Return the Excel file as a byte array
                return package.GetAsByteArray();
            }
        }

        public async Task<byte[]> GenerateWithdrawRequestExcel()
        {
            var result = await _moneyRequestRepository.GetTable().AsTracking()
                .Where(x => x.WithdrawWalletTransaction != null && x.Status == WithdrawRequestStatusEnum.PENDING.ToString())
                .ToListAsync();

            foreach (var item in result.ToList())
            {
                item.Status = WithdrawRequestStatusEnum.TRANSFERING.ToString();
            }

            await _moneyRequestRepository.SaveChanges();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Withdraw Requests");

                // Set headers
                worksheet.Cells["A1"].Value = "DANH SÁCH GIAO DỊCH (LIST OF TRANSACTIONS)";
                worksheet.Cells["A1:F1"].Merge = true;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A2"].Value = "STT (Ord. No.)";
                worksheet.Cells["B2"].Value = "Số tài khoản (Account No.)";
                worksheet.Cells["C2"].Value = "Tên người thụ hưởng (Beneficiary)";
                worksheet.Cells["D2"].Value = "Ngân hàng thụ hưởng/Chi nhánh (Beneficiary Bank)";
                worksheet.Cells["E2"].Value = "Số tiền (Amount)";
                worksheet.Cells["F2"].Value = "Nội dung chuyển khoản (Payment Detail)";

                // Style header
                using (var range = worksheet.Cells["A2:F2"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // Populate rows
                int row = 3;
                foreach (var item in result)
                {
                    worksheet.Cells[row, 1].Value = row - 2; // Ord. No.

                    // Check for BuyerWallet, SellerWallet, or SupplierWallet
                    if (item.WithdrawWalletTransaction?.BuyerWallet != null)
                    {
                        worksheet.Cells[row, 2].Value = item.WithdrawWalletTransaction.BuyerWallet.Buyer.BuyerWallet.BankNumber; // Account No.
                        worksheet.Cells[row, 3].Value = item.WithdrawWalletTransaction.BuyerWallet.Buyer.BuyerWallet.BankAccount; // Beneficiary
                        worksheet.Cells[row, 4].Value = item.WithdrawWalletTransaction.BuyerWallet.Buyer.BuyerWallet.BankName; // Beneficiary Bank
                    }
                    else if (item.WithdrawWalletTransaction?.SellerWallet != null)
                    {
                        worksheet.Cells[row, 2].Value = item.WithdrawWalletTransaction.SellerWallet.Seller.SellerWallet.BankNumber; // Account No.
                        worksheet.Cells[row, 3].Value = item.WithdrawWalletTransaction.SellerWallet.Seller.SellerWallet.BankAccount; // Beneficiary
                        worksheet.Cells[row, 4].Value = item.WithdrawWalletTransaction.SellerWallet.Seller.SellerWallet.BankName; // Beneficiary Bank
                    }
                    else if (item.WithdrawWalletTransaction?.SupplierWallet != null)
                    {
                        worksheet.Cells[row, 2].Value = item.WithdrawWalletTransaction.SupplierWallet.Supplier.SupplierWallet.BankNumber; // Account No.
                        worksheet.Cells[row, 3].Value = item.WithdrawWalletTransaction.SupplierWallet.Supplier.SupplierWallet.BankAccount; // Beneficiary
                        worksheet.Cells[row, 4].Value = item.WithdrawWalletTransaction.SupplierWallet.Supplier.SupplierWallet.BankName; // Beneficiary Bank
                    }
                    else
                    {
                        worksheet.Cells[row, 2].Value = "N/A"; // Account No.
                        worksheet.Cells[row, 3].Value = "N/A"; // Beneficiary
                        worksheet.Cells[row, 4].Value = "N/A"; // Beneficiary Bank
                    }

                    worksheet.Cells[row, 5].Value = item.Amount; // Amount
                    worksheet.Cells[row, 6].Value = "Chuyển khoản"; // Payment Detail

                    // Format currency
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";

                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return Excel file as byte array
                return package.GetAsByteArray();
            }
        }
    }
}