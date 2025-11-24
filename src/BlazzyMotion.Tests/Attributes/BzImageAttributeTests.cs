namespace BlazzyMotion.Tests.Attributes;

public class BzImageAttributeTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var attribute = new BzImageAttribute();

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeAssignableTo<Attribute>();
    }

    [Fact]
    public void Attribute_CanBeAppliedToProperty()
    {
        // Arrange
        var propertyInfo = typeof(TestModel).GetProperty(nameof(TestModel.ImageUrl));

        // Act
        var attribute = propertyInfo?.GetCustomAttributes(typeof(BzImageAttribute), false).FirstOrDefault();

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<BzImageAttribute>();
    }

    [Fact]
    public void Attribute_ShouldBeInheritedAttribute()
    {
        // Arrange
        var attribute = new BzImageAttribute();

        // Act
        var attributeUsage = typeof(BzImageAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        attributeUsage.Should().NotBeNull();
        attributeUsage!.ValidOn.Should().HaveFlag(AttributeTargets.Property);
    }

    private class TestModel
    {
        [BzImage]
        public string? ImageUrl { get; set; }
    }
}
