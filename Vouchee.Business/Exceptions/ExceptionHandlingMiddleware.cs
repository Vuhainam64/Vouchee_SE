﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Vouchee.Business.Models;

namespace Vouchee.Business.Exceptions
{
    public class ExceptionHandlingMiddleware : AuthorizeAttribute
    {
        public RequestDelegate requestDelegate;
        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }
        private static Task HandleException(HttpContext context, Exception ex)
        {
            var errorMessageObject = new ErrorResponse { Message = ex.Message, Code = "500" };
            var statusCode = (int)HttpStatusCode.InternalServerError;
            switch (ex)
            {
                case UnauthorizedException:
                    errorMessageObject.Code = "401";
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case RegisterException:
                    errorMessageObject.Code = "R001";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case UnauthorizedAccessException:
                    errorMessageObject.Code = "U001";
                    statusCode = (int)HttpStatusCode.ServiceUnavailable;
                    break;
                case NotFoundException:
                    errorMessageObject.Code = "N001";
                    statusCode = (int)HttpStatusCode.NotFound;
                    break;
                case FileException:
                    errorMessageObject.Code = "F001";
                    statusCode = (int)HttpStatusCode.Conflict;
                    break;
                case InUseException:
                    errorMessageObject.Code = "I001";
                    statusCode = (int)HttpStatusCode.Conflict;
                    break;
                case WalletBalanceException:
                    errorMessageObject.Code = "W001";
                    statusCode = (int)HttpStatusCode.Conflict;
                    break;
                case LoginException:
                    errorMessageObject.Code = "L001";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case AmountExcessException:
                    errorMessageObject.Code = "A001";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case FormatException:
                    errorMessageObject.Code = "F002";
                    statusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case LoadException:
                    errorMessageObject.Code = "L001";
                    statusCode =(int)HttpStatusCode.ServiceUnavailable;
                    break;
            }

            var errorMessage = JsonConvert.SerializeObject(errorMessageObject);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(errorMessage);
        }
    }
}
