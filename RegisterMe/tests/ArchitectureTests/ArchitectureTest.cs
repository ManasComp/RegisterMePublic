#region

using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using RegisterMe.Domain;

#endregion

namespace ArchitectureTests;

public class ArchitectureTests
{
    private const string DomainNamespace = "Domain";
    private const string ApplicationNamespace = "Application";
    private const string InfrastructureNamespace = "Infrastructure";
    private const string WebGuiNamespace = "WebGui";
    private const string WebApiNamespace = "WebApi";

    [Test]
    public void DomainShouldNotHaveDependency()
    {
        // Arrange
        Assembly assembly = typeof(AssemblyReference).Assembly;

        string[] otherProjects =
        [
            DomainNamespace, ApplicationNamespace, InfrastructureNamespace, WebGuiNamespace, WebApiNamespace
        ];
        // Act
        TestResult? testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Test]
    public void ApplicationShouldNotHaveDependencyOnOtherProjects()
    {
        // Arrange
        Assembly assembly = typeof(RegisterMe.Application.AssemblyReference).Assembly;

        string[] otherProjects = [InfrastructureNamespace, WebGuiNamespace, WebApiNamespace];

        // Act
        TestResult? testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }


    [Test]
    public void InfrastructureShouldNotHaveDependencyOnOtherProjects()
    {
        // Arrange
        Assembly assembly = typeof(RegisterMe.Infrastructure.AssemblyReference).Assembly;

        string[] otherProjects = [WebGuiNamespace, WebApiNamespace];

        // Act
        TestResult? testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Test]
    public void GUIShouldNotHaveDependencyOnOtherProjects()
    {
        // Arrange
        Assembly assembly = typeof(WebGui.AssemblyReference).Assembly;

        string[] otherProjects = [InfrastructureNamespace, WebApiNamespace];

        // Act
        TestResult? testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }


    [Test]
    public void APIShouldNotHaveDependencyOnOtherProjects()
    {
        // Arrange
        Assembly assembly = typeof(WebApi.AssemblyReference).Assembly;

        string[] otherProjects = [InfrastructureNamespace, WebGuiNamespace];

        // Act
        TestResult? testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }
}
