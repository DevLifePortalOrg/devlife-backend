// DevLife.Infrastructure/Mappers/EntityToDtoMapper.cs
using DevLife.Api.Models; // DTOs
using DevLife.Domain.Entities;
using System.ComponentModel.DataAnnotations;
namespace DevLife.Infrastructure.Mappers
{
    public static class EntityToDtoMapper
    {
        public static UserProfile MapUserProfile(UserEntity userEntity)
        {
            if (userEntity == null) return null;
            return new UserProfile
            {
                UserId = userEntity.Id,
                Username = userEntity.Username,
                Email = userEntity.Email,
                Bio = userEntity.Bio,
                GitHubUsername = userEntity.GitHubUsername,
                ReputationPoints = userEntity.ReputationPoints,
                BirthDate = userEntity.BirthDate // Include birth date if needed for horoscope display
            };
        }

        public static UserEntity MapUserEntity(UserProfile userProfileDto)
        {
            if (userProfileDto == null) return null;
            return new UserEntity
            {
                Id = userProfileDto.UserId,
                Username = userProfileDto.Username,
                Email = userProfileDto.Email,
                Bio = userProfileDto.Bio,
                GitHubUsername = userProfileDto.GitHubUsername,
                ReputationPoints = userProfileDto.ReputationPoints,
                BirthDate = userProfileDto.BirthDate
                // PasswordHash, CreatedAt, LastLogin would be handled by AuthService/UserRepository
            };
        }

        public static AuthResponseDto MapAuthResponse(UserEntity userEntity, string token)
        {
            if (userEntity == null) return null;
            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Authentication successful.",
                Token = token,
                UserId = userEntity.Id,
                Username = userEntity.Username,
                Email = userEntity.Email
            };
        }

        public static DatingProfile MapDatingProfile(DatingProfileEntity entity)
        {
            if (entity == null) return null;
            return new DatingProfile
            {
                UserId = entity.UserId,
                Bio = entity.Bio,
                LookingFor = entity.LookingFor,
                Interests = entity.Interests,
                ProfilePictureUrl = entity.ProfilePictureUrl
            };
        }

        public static DatingProfileEntity MapDatingProfileEntity(DatingProfile dto, Guid userId)
        {
            if (dto == null) return null;
            return new DatingProfileEntity
            {
                UserId = userId,
                Bio = dto.Bio,
                LookingFor = dto.LookingFor,
                Interests = dto.Interests,
                ProfilePictureUrl = dto.ProfilePictureUrl,
                CreatedAt = dto.CreatedAt == default ? System.DateTime.UtcNow : dto.CreatedAt,
                LastUpdatedAt = System.DateTime.UtcNow,
                LikedUserIds = dto.LikedUserIds,
                MatchedUserIds = dto.MatchedUserIds
            };
        }

        public static CodeSnippet MapCodeSnippetDto(CodeSnippetEntity entity)
        {
            if (entity == null) return null;
            return new CodeSnippet
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CodeContent = entity.CodeContent,
                SubmissionDate = entity.SubmissionDate,
                Type = entity.Type,
                Comments = entity.Comments,
                Language = entity.Language,
                Title = entity.Title,
                Likes = entity.Likes,
                Dislikes = entity.Dislikes
            };
        }

        public static CodeChallenge MapCodeChallengeDto(CodeChallengeEntity entity)
        {
            if (entity == null) return null;
            return new CodeChallenge
            {
                Id = entity.Id,
                Title = entity.Title,
                ProblemDescription = entity.ProblemDescription,
                InitialCode = entity.InitialCode,
                Language = entity.Language,
                TestCasesJson = entity.TestCasesJson,
                Difficulty = entity.Difficulty,
                CreatedDate = entity.CreatedDate,
                Tags = entity.Tags
            };
        }

        public static LeaderboardEntry MapLeaderboardEntry(ScoreEntity score, UserEntity user)
        {
            if (score == null || user == null) return null;
            return new LeaderboardEntry
            {
                UserId = user.Id,
                Username = user.Username,
                GameType = score.GameType,
                Score = score.Points,
                AchievedAt = score.AchievedAt
            };
        }

        // Add more mapping methods as needed for other DTOs/Entities
    }
}