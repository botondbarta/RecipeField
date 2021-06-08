using RecipeField.BLL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.ServiceInterfaces
{
    public interface ICommentService
    {
        Task<CommentDto> GetByIdAsync(int id);
        Task<CommentDto> CreateAsync(CommentDto ingredient);
        Task DeleteAsync(int id);
        Task<CommentDto> ModifyAsync(int id);
    }
}
