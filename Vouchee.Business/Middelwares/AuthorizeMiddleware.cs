using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vouchee.Business.Models;

namespace Vouchee.Business.Middelwares
{
    public class AuthorizeMiddleware : ActionResult
    {
        private static async Task WriteErrorResponseAsync(HttpContext context, params string[] errors)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse()
            {
                result = false,
                code = "401",
                message = "Tài khoản chưa được cấp quyền"
            };

            await JsonSerializer.SerializeAsync(context.Response.Body, response);
            await context.Response.Body.FlushAsync();
        }

        private readonly RequestDelegate _request;

        public AuthorizeMiddleware(RequestDelegate requestDelegate)
        {
            _request = requestDelegate ?? throw new ArgumentNullException(nameof(requestDelegate), $"{nameof(requestDelegate)} is required");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), $"{nameof(context)} is required");
            }

            await _request(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                await WriteErrorResponseAsync(context, "Unauthorized");
            }
        }
    }
}
