namespace BlazzyMotion.Tests.Attributes;

public class BzDescriptionAttributeTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var attribute = new BzDescriptionAttribute();

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeAssignableTo<Attribute>();
    }

    [Fact]
    public void Attribute_CanBeAppliedToProperty()
    {
        // Arrange
        var propertyInfo = typeof(TestModel).GetProperty(nameof(TestModel.Description));

        // Act
        var attribute = propertyInfo?.GetCustomAttributes(typeof(BzDescriptionAttribute), false).FirstOrDefault();

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<BzDescriptionAttribute>();
    }

    [Fact]
    public void Attribute_ShouldBeInheritedAttribute()
    {
        // Arrange
        var attribute = new BzDescriptionAttribute();

        // Act
        var attributeUsage = typeof(BzDescriptionAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        attributeUsage.Should().NotBeNull();
        attributeUsage!.ValidOn.Should().HaveFlag(AttributeTargets.Property);
    }

    private class TestModel
    {
        [BzDescription]
        public string? Description { get; set; }
    }
}
