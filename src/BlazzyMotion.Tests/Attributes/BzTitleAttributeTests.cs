namespace BlazzyMotion.Tests.Attributes;

public class BzTitleAttributeTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var attribute = new BzTitleAttribute();

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeAssignableTo<Attribute>();
    }

    [Fact]
    public void Attribute_CanBeAppliedToProperty()
    {
        // Arrange
        var propertyInfo = typeof(TestModel).GetProperty(nameof(TestModel.Title));

        // Act
        var attribute = propertyInfo?.GetCustomAttributes(typeof(BzTitleAttribute), false).FirstOrDefault();

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<BzTitleAttribute>();
    }

    [Fact]
    public void Attribute_ShouldBeInheritedAttribute()
    {
        // Arrange
        var attribute = new BzTitleAttribute();

        // Act
        var attributeUsage = typeof(BzTitleAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), false)
            .FirstOrDefault() as AttributeUsageAttribute;

        // Assert
        attributeUsage.Should().NotBeNull();
        attributeUsage!.ValidOn.Should().HaveFlag(AttributeTargets.Property);
    }

    private class TestModel
    {
        [BzTitle]
        public string? Title { get; set; }
    }
}
