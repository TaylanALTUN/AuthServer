using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    internal interface IServiceGeneric<Entity, Dto> where Entity : class  where Dto : class
    {
        Task<ResponseDto<Dto>> GetByIdAsync(int id);

        Task<ResponseDto<IEnumerable<Dto>>> GetAllAsync();

        Task<ResponseDto<IEnumerable<Dto>>> Where(Expression<Func<Entity, bool>> expression);
        Task<ResponseDto<Dto>> AddAsync(Dto dto);

        Task<ResponseDto<NoDataDto>> Update(Dto dto);

        Task<ResponseDto<NoDataDto>> Remove(int id);
    }
}
