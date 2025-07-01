// DevLife.Infrastructure/Repositories/CodeSnippetRepository.cs
using DevLife.Api.Data;
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Infrastructure.Repositories
{
    public class CodeSnippetRepository : ICodeSnippetRepository
    {
        private readonly AppDbContext _dbContext;

        public CodeSnippetRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CodeSnippetEntity> GetCodeSnippetByIdAsync(Guid id)
        {
            return await _dbContext.CodeSnippets.FindAsync(id);
        }

        public async Task<IEnumerable<CodeSnippetEntity>> GetCodeSnippetsByUserIdAsync(Guid userId)
        {
            return await _dbContext.CodeSnippets.Where(cs => cs.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<CodeSnippetEntity>> GetCodeSnippetsByTypeAsync(string type)
        {
            return await _dbContext.CodeSnippets.Where(cs => cs.Type == type).ToListAsync();
        }

        public async Task AddCodeSnippetAsync(CodeSnippetEntity codeSnippet)
        {
            await _dbContext.CodeSnippets.AddAsync(codeSnippet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCodeSnippetAsync(CodeSnippetEntity codeSnippet)
        {
            _dbContext.CodeSnippets.Update(codeSnippet);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCodeSnippetAsync(Guid id)
        {
            var snippet = await _dbContext.CodeSnippets.FindAsync(id);
            if (snippet != null)
            {
                _dbContext.CodeSnippets.Remove(snippet);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AddCommentToCodeSnippetAsync(Guid codeSnippetId, string comment)
        {
            var snippet = await _dbContext.CodeSnippets.FindAsync(codeSnippetId);
            if (snippet != null)
            {
                // Ensure the Comments list is initialized
                if (snippet.Comments == null)
                {
                    snippet.Comments = new List<string>();
                }
                snippet.Comments.Add(comment);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateLikesDislikesAsync(Guid codeSnippetId, bool isLike)
        {
            var snippet = await _dbContext.CodeSnippets.FindAsync(codeSnippetId);
            if (snippet != null)
            {
                if (isLike)
                {
                    snippet.Likes++;
                }
                else
                {
                    snippet.Dislikes++;
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}