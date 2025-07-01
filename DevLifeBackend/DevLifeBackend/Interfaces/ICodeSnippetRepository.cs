// DevLife.Domain/Interfaces/ICodeSnippetRepository.cs
using DevLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevLife.Domain.Interfaces
{
    public interface ICodeSnippetRepository
    {
        Task<CodeSnippetEntity> GetCodeSnippetByIdAsync(Guid id);
        Task<IEnumerable<CodeSnippetEntity>> GetCodeSnippetsByUserIdAsync(Guid userId);
        Task<IEnumerable<CodeSnippetEntity>> GetCodeSnippetsByTypeAsync(string type);
        Task AddCodeSnippetAsync(CodeSnippetEntity codeSnippet);
        Task UpdateCodeSnippetAsync(CodeSnippetEntity codeSnippet);
        Task DeleteCodeSnippetAsync(Guid id);
        Task AddCommentToCodeSnippetAsync(Guid codeSnippetId, string comment); // For simplicity, comment as string
        Task UpdateLikesDislikesAsync(Guid codeSnippetId, bool isLike);
    }
}