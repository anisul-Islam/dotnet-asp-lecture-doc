using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_db_api.Utilities
{
    public class ApiResponse
    {
        // Central method to handle the creation of ApiResponseTemplate with ObjectResult
        private static IActionResult CreateApiResponse<T>(T? data, string message, int statusCode, bool success)
        {
            var response = new ApiResponseTemplate<T>(success, data, message, statusCode);
            return new ObjectResult(response)
            {
                StatusCode = statusCode
            };
        }

        public static IActionResult Success<T>(T data, string message = "Success")
        {
            return CreateApiResponse(data, message, StatusCodes.Status200OK, true);
        }

        public static IActionResult Created<T>(T data, string message = "Resource Created")
        {
            return CreateApiResponse(data, message, StatusCodes.Status201Created, true);
        }

        public static IActionResult NotFound(string message = "Resource not found")
        {
            return CreateApiResponse<object>(null, message, StatusCodes.Status404NotFound, false);
        }

        public static IActionResult Conflict(string message = "Conflict Detected")
        {
            return CreateApiResponse<object>(null, message, StatusCodes.Status409Conflict, false);
        }

        public static IActionResult BadRequest(string message = "Bad request")
        {
            return CreateApiResponse<object>(null, message, StatusCodes.Status400BadRequest, false);
        }

        public static IActionResult Unauthorized(string message = "Unauthorized access")
        {
            return CreateApiResponse<object>(null, message, StatusCodes.Status401Unauthorized, false);
        }

        public static IActionResult Forbidden(string message = "Forbidden access")
        {
            return CreateApiResponse<object>(null, message, StatusCodes.Status403Forbidden, false);
        }

        public static IActionResult ServerError(string message = "Internal server error")
        {
            return CreateApiResponse<object>(null, message, StatusCodes.Status500InternalServerError, false);
        }
    }

    public class ApiResponseTemplate<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ApiResponseTemplate(bool success, T? data, string message, int statusCode)
        {
            Success = success;
            Data = data;
            Message = message;
            StatusCode = statusCode;
        }
    }
}

