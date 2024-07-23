using HONATIMEPIECES.Data;
using Microsoft.AspNetCore.Mvc;

namespace HONATIMEPIECES.Helpers
{
    public static class StatusCodeResponse
    {
        public static IActionResult CreateResponse<T>(int statusCode, string message, T result, string error = null)
        {
            var response = new ResponseAPI<T>
            {
                Message = message,
                Result = result,
                Error = error,
                StatusCode = statusCode
            };
            return new ObjectResult(response) { StatusCode = statusCode };
        }

        public static IActionResult SuccessResponse<T>(T result, string message = "Success")
        {
            return CreateResponse(200, message, result);
        }

        public static IActionResult BadRequestResponse(string message, string error)
        {
            return CreateResponse<object>(400, message, null, error);
        }

        public static IActionResult NotFoundResponse(string message, string error)
        {
            return CreateResponse<object>(404, message, null, error);
        }

        public static IActionResult InternalServerErrorResponse(string message, string error)
        {
            return CreateResponse<object>(500, message, null, error);
        }

        public static IActionResult CreatedResponse<T>(T result, string message = "Resource created successfully")
        {
            return CreateResponse(201, message, result);
        }

        public static IActionResult NoContentResponse(string message = "Delete successfully / No Content")
        {
            return CreateResponse<object>(204, message, null);
        }
    }
}
