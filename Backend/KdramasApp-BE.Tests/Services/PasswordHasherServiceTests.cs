using KdramasApp_BE.Services.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KdramasApp_BE.Tests.Services;

public class PasswordHasherServiceTests
{
    [Fact]
    public void Hash_ShouldReturnDifferentValueFromOriginalPassword()
    {
        // Arrange
        var hasher = new BCryptPasswordHasher();
        var password = "TestPassword123!";

        // Act
        var hash = hasher.Hash(password);

        // Assert -> we expect the password and the hash to not match
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void Verify_ShouldReturnTrue_WhenPasswordIsCorrect()
    {
        // Arrange
        var hasher = new BCryptPasswordHasher();
        var password = "TestPassword123!";
        var hash = hasher.Hash(password);

        // Act
        var result = hasher.Verify(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Verify_ShouldReturnFalse_WhenPasswordIsWrong()
    {
        // Arrange
        var hasher = new BCryptPasswordHasher();
        var correctPassword = "CorrectPassword123!";
        var wrongPassword = "WrongPassword123!";
        var hash = hasher.Hash(correctPassword);

        // Act
        var result = hasher.Verify(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Hash_ShouldGenerateDifferentHashes_ForSamePassword()
    {
        // Arrange
        var hasher = new BCryptPasswordHasher();
        var password = "TestPassword123!";

        // Act
        var firstHash = hasher.Hash(password);
        var secondHash = hasher.Hash(password);

        // Assert
        Assert.NotEqual(firstHash, secondHash);
    }
}
