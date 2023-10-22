using System.Net;
using FluentValidation.Results;

namespace Core
{
	public class ApiResponse<Response> where Response : class
	{
		public HttpStatusCode StatusCode { get; set; }
		public IDictionary<string, List<string>>? Errors { get; set; }
		public Response? Data { get; set; }
		public bool Success => Errors == null && Data != null;
		
		public static ApiResponse<Response> FromApiResponse<T>(ApiResponse<T> apiResponse)
			where T : class
		{
			return new ApiResponse<Response>(apiResponse.StatusCode, apiResponse.Errors);
		}
		
		public ApiResponse(HttpStatusCode code, IDictionary<string, List<string>>? errors)
		{
			StatusCode = code;
			Errors = errors;
		}
		
		public ApiResponse(Response resp)
		{
			StatusCode = HttpStatusCode.OK;
			Data = resp;
		}

		public ApiResponse(IEnumerable<ValidationFailure> failures)
		{
			Errors = failures.GroupBy(x => x.PropertyName)
				.Aggregate(new Dictionary<string, List<string>>(), (a, b) =>
				{
					a.Add(LowerCaseOfFirstLetter(b.Key), b.Select(x => x.ErrorMessage).ToList());
					return a;
				});
			StatusCode = HttpStatusCode.BadRequest;
		}

		public ApiResponse(ValidationFailure failure): this(new List<ValidationFailure>() { failure })
		{
		}
		
		private static string LowerCaseOfFirstLetter(string key)
		{
			return !key.Any() || char.IsLower(key[0]) ?
				key :
				key.Replace(key[0], char.ToLower(key[0]));
		}
	}
}