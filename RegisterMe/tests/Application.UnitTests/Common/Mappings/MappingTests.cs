#region

using System.Reflection;
using System.Runtime.Serialization;
using AutoMapper;
using NUnit.Framework;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.UnitTests.Common.Mappings;

public class MappingTests
{
    private readonly MapperConfiguration _configuration;
    private readonly IMapper _mapper;

    public MappingTests()
    {
        _configuration = new MapperConfiguration(config =>
            config.AddMaps(Assembly.GetAssembly(typeof(IApplicationDbContext))));

        _mapper = _configuration.CreateMapper();
    }

    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Test]
    [TestCase(typeof(Breeder), typeof(BreederDto))]
    [TestCase(typeof(CatRegistration), typeof(MiddleCatRegistrationDto))]
    [TestCase(typeof(MiddleCatRegistrationDto), typeof(CatRegistrationDto))]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        object instance = GetInstanceOf(source);

        _mapper.Map(instance, source, destination);
    }

    private object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
        {
            return Activator.CreateInstance(type)!;
        }

        // Type without parameterless constructor
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        return FormatterServices.GetUninitializedObject(type);
#pragma warning restore SYSLIB0050 // Type or member is obsolete
    }
}
