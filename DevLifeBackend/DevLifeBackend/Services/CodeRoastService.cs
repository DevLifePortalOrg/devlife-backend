// DevLife.Api/Services/CodeRoastService.cs
using DevLife.Api.Models;
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevLife.Infrastructure.Mappers;

namespace DevLife.Api.Services
{
    public class CodeRoastService
    {
        private readonly ICodeSnippetRepository _codeSnippetRepository;
        private readonly IUserRepository _userRepository; // To get usernames for display

        public CodeRoastService(ICodeSnippetRepository codeSnippetRepository, IUserRepository userRepository)
        {
            _codeSnippetRepository = codeSnippetRepository;
            _userRepository = userRepository;
        }

        public async Task<CodeSnippet> SubmitCodeForRoast(Guid userId, SubmitCodeRoastDto submitDto)
        {
            var codeSnippet = new CodeSnippetEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CodeContent = submitDto.Code,
                SubmissionDate = DateTime.UtcNow,
                Type = "Roast", // Identify as a roast submission
                Language = submitDto.Language,
                Title = submitDto.Title,
                Comments = new List<string>() // Initialize empty comment list
            };
            await _codeSnippetRepository.AddCodeSnippetAsync(codeSnippet);

            return EntityToDtoMapper.MapCodeSnippetDto(codeSnippet);
        }

        public async Task<bool> AddRoastComment(Guid codeSnippetId, Guid userId, AddRoastCommentDto commentDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            string commentWithAuthor = $"{user.Username}: {commentDto.CommentText}";
            await _codeSnippetRepository.AddCommentToCodeSnippetAsync(codeSnippetId, commentWithAuthor);
            return true;
        }

        public async Task<IEnumerable<CodeSnippet>> GetAllRoastsAsync()
        {
            var roastSnippets = await _codeSnippetRepository.GetCodeSnippetsByTypeAsync("Roast");
            var result = new List<CodeSnippet>();

            foreach (var snippet in roastSnippets)
            {
                var user = await _userRepository.GetUserByIdAsync(snippet.UserId);
                var dto = EntityToDtoMapper.MapCodeSnippetDto(snippet);
                dto.Username = user?.Username ?? "Unknown User"; // Add username for display
                result.Add(dto);
            }
            return result;
        }

        public async Task<CodeSnippet> GetRoastByIdAsync(Guid id)
        {
            var snippet = await _codeSnippetRepository.GetCodeSnippetByIdAsync(id);
            if (snippet == null || snippet.Type != "Roast") return null;

            var user = await _userRepository.GetUserByIdAsync(snippet.UserId);
            var dto = EntityToDtoMapper.MapCodeSnippetDto(snippet);
            dto.Username = user?.Username ?? "Unknown User";
            return dto;
        }

        public async Task<bool> RecordCodeRoastInteraction(Guid codeSnippetId, bool isLike)
        {
            await _codeSnippetRepository.UpdateLikesDislikesAsync(codeSnippetId, isLike);
            // Optionally award/deduct points for liking/disliking a roast
            return true;
        }
    }
}