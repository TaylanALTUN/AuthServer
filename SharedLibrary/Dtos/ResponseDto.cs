using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public  class ResponseDto<T> where T : class
    {
        public T Data { get; private set; }

        public int StatusCode { get; private set; }

        public ErrorDto Errors { get; private set; }

        [JsonIgnore]
        public bool IsSuccessful { get; private set; }

        //Static factory design patern. Nesne oluştyurmayı clientlardan almak amacı ile yapıldı. Nesne oluşturma işlemini kontrol altına alınıyor
        public static ResponseDto<T> Success(int statusCode, T data)
        {
            return new ResponseDto<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };
        }

        public static ResponseDto<T> Success(int statusCode)
        {
            return new ResponseDto<T> { StatusCode = statusCode, IsSuccessful = true };
        }
        
        public static ResponseDto<T> Fail(int statusCode, ErrorDto error)
        {
            return new ResponseDto<T> { StatusCode = statusCode, Errors = error };
        }
        
        public static ResponseDto<T> Fail(int statusCode, string error, bool isShow)
        {
            return new ResponseDto<T> { StatusCode = statusCode, Errors = new ErrorDto(error, isShow) };
        }
    }
}
