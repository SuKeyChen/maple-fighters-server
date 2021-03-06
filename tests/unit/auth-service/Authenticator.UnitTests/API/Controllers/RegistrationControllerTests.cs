﻿using Authenticator.API.Controllers;
using Authenticator.API.Datas;
using Authenticator.Domain.Aggregates.User;
using Authenticator.Domain.Aggregates.User.Services;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Authenticator.UnitTests.API.Controllers
{
    public class RegistrationControllerTests
    {
        [Theory]
        [InlineData("", "benzuk", "Ben", "Ukhanov")]
        [InlineData("benzuk@gmail.com", "", "Ben", "Ukhanov")]
        [InlineData("benzuk@gmail.com", "benzuk", "", "Ukhanov")]
        [InlineData("benzuk@gmail.com", "benzuk", "Ben", "")]
        public void Register_Returns_Failed(
            string email,
            string password,
            string firstName,
            string lastName)
        {
            // Arrange
            var registrationService = Substitute.For<IRegistrationService>();
            var registrationController =
                new RegistrationController(registrationService);
            var registrationData = 
                new RegistrationData(email, password, firstName, lastName);

            // Act
            var authenticationStatus =
                registrationController.Register(registrationData);

            // Assert
            authenticationStatus
                .Equals(AccountCreationStatus.Failed)
                .ShouldBeTrue();
        }

        [Fact]
        public void Register_Returns_Succeed()
        {
            // Arrange
            const string Email = "benzuk@gmail.com";
            const string Password = "benzuk";
            const string FirstName = "Ben";
            const string LastName = "Ukhanov";

            var registrationService = Substitute.For<IRegistrationService>();
            registrationService
                .CreateAccount(Arg.Any<Account>())
                .Returns(AccountCreationStatus.Succeed);

            var registrationController =
                new RegistrationController(registrationService);
            var registrationData =
                new RegistrationData(Email, Password, FirstName, LastName);

            // Act
            var authenticationStatus =
                registrationController.Register(registrationData);

            // Assert
            authenticationStatus
                .Equals(AccountCreationStatus.Succeed)
                .ShouldBeTrue();
        }
    }
}