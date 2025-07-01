// DevLife.Api/Services/DevDatingService.cs
using DevLife.Api.Models;
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using DevLife.Infrastructure.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevLife.Api.Services
{
    public class DevDatingService
    {
        private readonly IMongoRepository<DatingProfileEntity> _datingProfileRepository;
        private readonly IUserRepository _userRepository; // To get usernames for matched profiles

        public DevDatingService(IMongoRepository<DatingProfileEntity> datingProfileRepository, IUserRepository userRepository)
        {
            _datingProfileRepository = datingProfileRepository;
            _userRepository = userRepository;
        }

        public async Task<DatingProfile> CreateOrUpdateDatingProfile(Guid userId, CreateUpdateDatingProfileDto profileDto)
        {
            var existingProfile = await _datingProfileRepository.GetByIdAsync(userId.ToString()); // MongoDB ID is string

            if (existingProfile == null)
            {
                var newProfile = new DatingProfileEntity
                {
                    Id = Guid.NewGuid().ToString(), // Generate a new string ID for MongoDB
                    UserId = userId,
                    Bio = profileDto.Bio,
                    LookingFor = profileDto.LookingFor,
                    Interests = profileDto.Interests,
                    ProfilePictureUrl = profileDto.ProfilePictureUrl,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow
                };
                await _datingProfileRepository.AddAsync(newProfile);
                return EntityToDtoMapper.MapDatingProfile(newProfile);
            }
            else
            {
                existingProfile.Bio = profileDto.Bio ?? existingProfile.Bio;
                existingProfile.LookingFor = profileDto.LookingFor ?? existingProfile.LookingFor;
                existingProfile.Interests = profileDto.Interests ?? existingProfile.Interests;
                existingProfile.ProfilePictureUrl = profileDto.ProfilePictureUrl ?? existingProfile.ProfilePictureUrl;
                existingProfile.LastUpdatedAt = DateTime.UtcNow;

                await _datingProfileRepository.UpdateAsync(existingProfile);
                return EntityToDtoMapper.MapDatingProfile(existingProfile);
            }
        }

        public async Task<DatingProfile> GetDatingProfile(Guid userId)
        {
            var profile = await _datingProfileRepository.GetByIdAsync(userId.ToString());
            return EntityToDtoMapper.MapDatingProfile(profile);
        }

        public async Task<IEnumerable<DatingMatchDto>> FindMatches(Guid userId)
        {
            var currentUserProfile = await _datingProfileRepository.GetByIdAsync(userId.ToString());
            if (currentUserProfile == null)
            {
                return new List<DatingMatchDto>();
            }

            var allProfiles = await _datingProfileRepository.GetAllAsync();
            var potentialMatches = new List<DatingMatchDto>();

            foreach (var profile in allProfiles)
            {
                if (profile.UserId == userId) continue; // Don't match with self
                if (currentUserProfile.LikedUserIds.Contains(profile.UserId)) continue; // Already liked/interacted

                // Simple matching algorithm:
                // 1. Check if looking for matches current profile's "looking for" (e.g., if A is looking for B, and B is looking for A)
                bool lookingForMatch = profile.LookingFor == currentUserProfile.LookingFor;
                // 2. Check for common interests (at least one common interest)
                bool hasCommonInterests = profile.Interests.Any(i => currentUserProfile.Interests.Contains(i));

                if (lookingForMatch && hasCommonInterests)
                {
                    var matchedUser = await _userRepository.GetUserByIdAsync(profile.UserId);
                    if (matchedUser != null)
                    {
                        potentialMatches.Add(new DatingMatchDto
                        {
                            MatchedUserId = profile.UserId,
                            MatchedUsername = matchedUser.Username,
                            MatchedUserBio = profile.Bio,
                            CommonInterests = profile.Interests.Intersect(currentUserProfile.Interests).ToList(),
                            ProfilePictureUrl = profile.ProfilePictureUrl
                        });
                    }
                }
            }
            return potentialMatches;
        }

        public async Task<bool> RecordLike(Guid currentUserId, Guid likedUserId)
        {
            var currentUserProfile = await _datingProfileRepository.GetByIdAsync(currentUserId.ToString());
            var likedUserProfile = await _datingProfileRepository.GetByIdAsync(likedUserId.ToString());

            if (currentUserProfile == null || likedUserProfile == null) return false;

            if (!currentUserProfile.LikedUserIds.Contains(likedUserId))
            {
                currentUserProfile.LikedUserIds.Add(likedUserId);
                await _datingProfileRepository.UpdateAsync(currentUserProfile);
            }

            // Check for a mutual match
            if (likedUserProfile.LikedUserIds.Contains(currentUserId) && !currentUserProfile.MatchedUserIds.Contains(likedUserId))
            {
                currentUserProfile.MatchedUserIds.Add(likedUserId);
                likedUserProfile.MatchedUserIds.Add(currentUserId);
                currentUserProfile.MatchesCount++;
                likedUserProfile.MatchesCount++;

                await _datingProfileRepository.UpdateAsync(currentUserProfile);
                await _datingProfileRepository.UpdateAsync(likedUserProfile);

                // Notify users of a new match via SignalR
                // _gameHubContext.Clients.User(currentUserId.ToString()).SendAsync("NewMatch", likedUserId);
                // _gameHubContext.Clients.User(likedUserId.ToString()).SendAsync("NewMatch", currentUserId);

                return true; // Indicates a match
            }
            return false; // No match yet
        }
    }
}