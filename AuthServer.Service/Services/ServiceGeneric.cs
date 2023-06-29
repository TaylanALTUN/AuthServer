using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class ServiceGeneric<Entity, Dto> : IServiceGeneric<Entity, Dto> where Entity : class where Dto : class
    {
        private readonly IGenericRepository<Entity> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceGeneric(IGenericRepository<Entity> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDto<Dto>> AddAsync(Dto dto)
        {
            var newEntity=ObjectMapper.Mapper.Map<Entity>(dto);
            await _repository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();
            var newDto = ObjectMapper.Mapper.Map<Dto>(newEntity);
            return ResponseDto<Dto>.Success(StatusCodes.Status200OK, newDto);
        }

        public async Task<ResponseDto<IEnumerable<Dto>>> GetAllAsync()
        {
            var entities = ObjectMapper.Mapper.Map<List<Dto>>(await _repository.GetAllAsync());

            return ResponseDto<IEnumerable<Dto>>.Success(StatusCodes.Status200OK, entities);
        }

        public async Task<ResponseDto<Dto>> GetByIdAsync(int id)
        {
            var entity = ObjectMapper.Mapper.Map<Dto>(await _repository.GetByIdAsync(id));

            if(entity == null)
            {
                return ResponseDto<Dto>.Fail(StatusCodes.Status404NotFound, "Id not found", true);
            }
            return ResponseDto<Dto>.Success(StatusCodes.Status200OK, entity);
        }

        public async Task<ResponseDto<NoDataDto>> Remove(int id)
        {
            var isExistEntity = await _repository.GetByIdAsync(id);

            if(isExistEntity == null)
                return ResponseDto<NoDataDto>.Fail(StatusCodes.Status404NotFound, "Id not found", true);

            _repository.Remove(isExistEntity);
            await _unitOfWork.CommitAsync();

            return ResponseDto<NoDataDto>.Success(StatusCodes.Status204NoContent);
        }

        public async Task<ResponseDto<NoDataDto>> Update(Dto dto, int id)
        {
            var isExistEntity = await _repository.GetByIdAsync(id);

            if (isExistEntity == null)
                return ResponseDto<NoDataDto>.Fail(StatusCodes.Status404NotFound, "Id not found", true);

            var updateEntity= ObjectMapper.Mapper.Map<Entity>(dto);
            _repository.Update(updateEntity);
            await _unitOfWork.CommitAsync();

            return ResponseDto<NoDataDto>.Success(StatusCodes.Status204NoContent);
        }

        public async Task<ResponseDto<IEnumerable<Dto>>> Where(Expression<Func<Entity, bool>> expression)
        {
            var entities= _repository.Where(expression);

            return ResponseDto<IEnumerable<Dto>>.Success(StatusCodes.Status200OK,ObjectMapper.Mapper.Map<IEnumerable<Dto>>(await entities.ToListAsync()));
        }
    }
}
