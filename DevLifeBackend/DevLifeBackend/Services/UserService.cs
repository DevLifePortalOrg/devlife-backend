using DevLife.Api.Models;
using DevLife.Domain.Interfaces;
using DevLife.Infrastructure.Mappers;
using System;
using System.Threading.Tasks;

namespace DevLife.Api.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly HoroscopeService _horoscopeService;

        public UserService(IUserRepository userRepository, HoroscopeService horoscopeService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _horoscopeService = horoscopeService ?? throw new ArgumentNullException(nameof(horoscopeService));
        }

        public async Task<UserProfile?> GetUserProfileAsync(Guid userId)
        {
            var userEntity = await _userRepository.GetUserByIdAsync(userId);
            if (userEntity == null)
            {
                return null;
            }

            var userProfile = EntityToDtoMapper.MapUserProfile(userEntity);

            if (userEntity.BirthDate.HasValue)
            {
                userProfile.HoroscopeToday = await _horoscopeService.GetDailyHoroscope(userEntity.BirthDate.Value);
            }

            return userProfile;
        }

        public async Task<bool> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto updatedProfileDto)
        {
            if (updatedProfileDto == null) throw new ArgumentNullException(nameof(updatedProfileDto));

            var userEntity = await _userRepository.GetUserByIdAsync(userId);
            if (userEntity == null)
            {
                return false;
            }

            userEntity.Bio = updatedProfileDto.Bio ?? userEntity.Bio;
            userEntity.GitHubUsername = updatedProfileDto.GitHubUsername ?? userEntity.GitHubUsername;
            userEntity.BirthDate = updatedProfileDto.BirthDate ?? userEntity.BirthDate;

            await _userRepository.UpdateUserAsync(userEntity);
            return true;
        }

        public async Task<bool> AddReputationPointsAsync(Guid userId, int points)
        {
            if (points <= 0) throw new ArgumentOutOfRangeException(nameof(points), "Points must be positive.");

            var userEntity = await _userRepository.GetUserByIdAsync(userId);
            if (userEntity == null)
            {
                return false;
            }

            userEntity.ReputationPoints += points;
            await _userRepository.UpdateUserAsync(userEntity);
            return true;
        }

        public async Task<bool> DeductReputationPointsAsync(Guid userId, int points)
        {
            if (points <= 0) throw new ArgumentOutOfRangeException(nameof(points), "Points must be positive.");

            var userEntity = await _userRepository.GetUserByIdAsync(userId);
            if (userEntity == null)
            {
                return false;
            }

            userEntity.ReputationPoints = Math.Max(0, userEntity.ReputationPoints - points);
            await _userRepository.UpdateUserAsync(userEntity);
            return true;
        }
    }
}
