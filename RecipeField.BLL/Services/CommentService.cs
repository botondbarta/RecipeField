using AutoMapper;
using RecipeField.BLL.Dto;
using RecipeField.BLL.ServiceInterfaces;
using RecipeField.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Services
{
    public class CommentService: ICommentService
    {
        private readonly RecipeFieldDbContext db;
        private readonly IMapper mapper;
        public CommentService(RecipeFieldDbContext _db, IMapper _mapper)
        {
            db = _db;
            mapper = _mapper;
        }

        public Task<CommentDto> CreateAsync(CommentDto comment)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CommentDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CommentDto> ModifyAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
