using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TicketsSystem.Core.Validations;
using TicketsSystem.Data.DTOs;
using TicketsSystem_Data;
using TicketsSystem_Data.Repositories;

namespace TicketsSystem.Core.Services
{
    public interface IUserService
    {
        Task CreateNewUserAsync(UserDTO userDTO);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<string> LoginAsync(LoginRequest request);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDTOValidator _userValidation;
        private readonly LoginRequestValidation _loginRequestValidation;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService(
            IUserRepository userRepository,
            UserDTOValidator validationRules,
            IPasswordHasher<User> passwordHasher,
            LoginRequestValidation loginValidationsRules)
        {
            _userRepository = userRepository;
            _userValidation = validationRules;
            _passwordHasher = passwordHasher;
            _loginRequestValidation = loginValidationsRules;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsers();
            IEnumerable<UserDTO> userDTOs = users.Select(u => new UserDTO
            {
                FullName = u.FullName,
                Email = u.Email,
                Password = " ",
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });
            return userDTOs;
        }

        public async Task CreateNewUserAsync(UserDTO userDTO)
        {
            if (userDTO == null)
            {
                throw new ArgumentNullException("userDto is null");
            }
            
            var validationResult = await _userValidation.ValidateAsync(userDTO);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("One or more fields do not meet the requirements." + validationResult.Errors);
            }

            var newUser = new User
            {
                FullName = userDTO.FullName,
                Email = userDTO.Email,
                PasswordHash = userDTO.Password,
                Role = userDTO.Role,
                IsActive = userDTO.IsActive
            };

            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, userDTO.Password);

            await _userRepository.CreateNewUser(newUser);
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("Email or password are null");
            }

            var validationResult = await _loginRequestValidation.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Email or password are not in a valid format." + validationResult.Errors);
            }

            var user = await _userRepository.Login(request.Email);

            if (user == null)
            {
                return ("The email address does not exist.");
            }

            var verificationPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationPassword != PasswordVerificationResult.Success)
            {
                return ("Incorrect password");
            }

            return ("Login Success");
        }

    }
}
