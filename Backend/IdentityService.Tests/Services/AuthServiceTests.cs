using AutoMapper;
using IdentityService.API.Auth.Models;
using IdentityService.API.Auth.Services;
using IdentityService.API.Interfaces;
using IdentityService.API.Services;
using Microsoft.AspNetCore.Identity;
using IdentityService.API.Auth.DTOs;
using Moq;
using Xunit;

namespace IdentityService.Tests.Services;

public class AuthServiceTests
{
    protected readonly Mock<UserManager<ApplicationUser>>
        _userManagerMock;

    protected readonly Mock<IMapper>
        _mapperMock;

    protected readonly Mock<IUserRepository>
        _userRepositoryMock;

    protected readonly Mock<ITokenService>
        _tokenServiceMock;

    protected readonly AuthService
        _authService;

    public AuthServiceTests()
    {
        var store =
            new Mock<IUserStore<ApplicationUser>>();

        _userManagerMock =
            new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

        _mapperMock =
            new Mock<IMapper>();

        _userRepositoryMock =
            new Mock<IUserRepository>();

        _tokenServiceMock =
            new Mock<ITokenService>();

        _authService =
            new AuthService(
                _userManagerMock.Object,
                _tokenServiceMock.Object,
                _mapperMock.Object,
                _userRepositoryMock.Object);
    }
    [Fact]
    public async Task Register_UserAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "Password@123"
        };

        _userManagerMock
            .Setup(x =>
                x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(
                new ApplicationUser());

        // Act
        var act = async () =>
            await _authService.RegisterAsync(dto);

        // Assert
        await Assert.ThrowsAsync<Exception>(act);
    }

    [Fact]
    public async Task Register_Success_ShouldReturnMessage()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email = "newuser@test.com",
            Password = "Password@123"
        };

        var user = new ApplicationUser
        {
            Email = dto.Email,
            UserName = dto.Email
        };

        _userManagerMock
            .Setup(x =>
                x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        _mapperMock
            .Setup(x =>
                x.Map<ApplicationUser>(dto))
            .Returns(user);

        _userManagerMock
            .Setup(x =>
                x.CreateAsync(
                    user,
                    dto.Password))
            .ReturnsAsync(
                IdentityResult.Success);

        _userManagerMock
            .Setup(x =>
                x.AddToRoleAsync(
                    user,
                    "User"))
            .ReturnsAsync(
                IdentityResult.Success);

        // Act
        var result =
            await _authService.RegisterAsync(dto);

        // Assert
        Assert.Equal(
            "User Registered Successfully",
            result);
    }
    
    [Fact]
    public async Task Login_UserNotFound_ShouldThrowException()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password@123"
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var act = async () =>
            await _authService.LoginAsync(dto);

        await Assert.ThrowsAsync<Exception>(act);
    }

    [Fact]
    public async Task Login_InvalidPassword_ShouldThrowException()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password@123"
        };

        var user = new ApplicationUser
        {
            Email = dto.Email
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(false);

        var act = async () =>
            await _authService.LoginAsync(dto);

        await Assert.ThrowsAsync<Exception>(act);
    }

    [Fact]
    public async Task Login_Success_ShouldReturnTokens()
    {
        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password@123"
        };

        var user = new ApplicationUser
        {
            Email = dto.Email
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, dto.Password))
            .ReturnsAsync(true);

        _tokenServiceMock
            .Setup(x => x.CreateToken(
                user,
                _userManagerMock.Object))
            .ReturnsAsync("jwt-token");

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        _userManagerMock
            .Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result =
            await _authService.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.Token);
        Assert.Equal("refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ShouldThrowException()
    {
        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "invalid-token"
        };

        _userRepositoryMock
            .Setup(x =>
                x.GetByRefreshTokenAsync(dto.RefreshToken))
            .ReturnsAsync((ApplicationUser?)null);

        var act = async () =>
            await _authService.RefreshTokenAsync(dto);

        await Assert.ThrowsAsync<Exception>(act);
    }

    [Fact]
    public async Task RefreshToken_ExpiredToken_ShouldThrowException()
    {
        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "expired-token"
        };

        var user = new ApplicationUser
        {
            RefreshToken = dto.RefreshToken,
            RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(-1)
        };

        _userRepositoryMock
            .Setup(x =>
                x.GetByRefreshTokenAsync(dto.RefreshToken))
            .ReturnsAsync(user);

        var act = async () =>
            await _authService.RefreshTokenAsync(dto);

        await Assert.ThrowsAsync<Exception>(act);
    }

    [Fact]
    public async Task RefreshToken_Success_ShouldReturnNewTokens()
    {
        var dto = new RefreshTokenRequestDto
        {
            RefreshToken = "old-refresh-token"
        };

        var user = new ApplicationUser
        {
            Email = "test@test.com",
            RefreshToken = dto.RefreshToken,
            RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(7)
        };

        _userRepositoryMock
            .Setup(x =>
                x.GetByRefreshTokenAsync(dto.RefreshToken))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(x =>
                x.CreateToken(
                    user,
                    _userManagerMock.Object))
            .ReturnsAsync("new-jwt-token");

        _tokenServiceMock
            .Setup(x =>
                x.GenerateRefreshToken())
            .Returns("new-refresh-token");

        _userManagerMock
            .Setup(x =>
                x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        var result =
            await _authService.RefreshTokenAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(
            "new-jwt-token",
            result.Token);

        Assert.Equal(
            "new-refresh-token",
            result.RefreshToken);
    }

}