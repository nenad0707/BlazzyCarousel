namespace BlazzyMotion.Tests.Models;

public class BzCarouselOptionsTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var options = new BzCarouselOptions();

        // Assert
        options.Effect.Should().Be("coverflow");
        options.SlidesPerView.Should().Be("auto");
        options.InitialSlide.Should().Be(0);
        options.CenteredSlides.Should().BeTrue();
        options.Loop.Should().BeTrue();
        options.SpaceBetween.Should().Be(0);
        options.Speed.Should().Be(300);
        options.GrabCursor.Should().BeTrue();
        options.RotateDegree.Should().Be(50);
        options.Depth.Should().Be(150);
        options.Stretch.Should().Be(0);
        options.Modifier.Should().Be(1.5);
        options.SlideShadows.Should().BeTrue();
    }

    [Fact]
    public void Effect_CanBeSet()
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.Effect = "fade";

        // Assert
        options.Effect.Should().Be("fade");
    }

    [Fact]
    public void SlidesPerView_CanBeSetToString()
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.SlidesPerView = "3";

        // Assert
        options.SlidesPerView.Should().Be("3");
    }

    [Fact]
    public void SlidesPerView_CanBeSetToAuto()
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.SlidesPerView = "auto";

        // Assert
        options.SlidesPerView.Should().Be("auto");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void InitialSlide_CanBeSet(int slideIndex)
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.InitialSlide = slideIndex;

        // Assert
        options.InitialSlide.Should().Be(slideIndex);
    }

    [Fact]
    public void CenteredSlides_CanBeToggled()
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.CenteredSlides = false;

        // Assert
        options.CenteredSlides.Should().BeFalse();
    }

    [Fact]
    public void Loop_CanBeToggled()
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.Loop = false;

        // Assert
        options.Loop.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    public void SpaceBetween_CanBeSet(int space)
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.SpaceBetween = space;

        // Assert
        options.SpaceBetween.Should().Be(space);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(300)]
    [InlineData(500)]
    [InlineData(1000)]
    public void Speed_CanBeSet(int speed)
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.Speed = speed;

        // Assert
        options.Speed.Should().Be(speed);
    }

    [Fact]
    public void GrabCursor_CanBeToggled()
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.GrabCursor = false;

        // Assert
        options.GrabCursor.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(75)]
    [InlineData(100)]
    public void RotateDegree_CanBeSet(int rotate)
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.RotateDegree = rotate;

        // Assert
        options.RotateDegree.Should().Be(rotate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(150)]
    [InlineData(200)]
    [InlineData(300)]
    public void Depth_CanBeSet(int depth)
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.Depth = depth;

        // Assert
        options.Depth.Should().Be(depth);
    }

    [Theory]
    [InlineData(-50)]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void Stretch_CanBeSet(int stretch)
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.Stretch = stretch;

        // Assert
        options.Stretch.Should().Be(stretch);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(3.0)]
    public void Modifier_CanBeSet(double modifier)
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.Modifier = modifier;

        // Assert
        options.Modifier.Should().Be(modifier);
    }

    [Fact]
    public void SlideShadows_CanBeToggled()
    {
        // Arrange
        var options = new BzCarouselOptions();

        // Act
        options.SlideShadows = false;

        // Assert
        options.SlideShadows.Should().BeFalse();
    }

    [Fact]
    public void AllProperties_CanBeSetTogether()
    {
        // Arrange & Act
        var options = new BzCarouselOptions
        {
            Effect = "slide",
            SlidesPerView = "3",
            InitialSlide = 2,
            CenteredSlides = false,
            Loop = false,
            SpaceBetween = 30,
            Speed = 500,
            GrabCursor = false,
            RotateDegree = 45,
            Depth = 200,
            Stretch = 10,
            Modifier = 2.0,
            SlideShadows = false
        };

        // Assert
        options.Effect.Should().Be("slide");
        options.SlidesPerView.Should().Be("3");
        options.InitialSlide.Should().Be(2);
        options.CenteredSlides.Should().BeFalse();
        options.Loop.Should().BeFalse();
        options.SpaceBetween.Should().Be(30);
        options.Speed.Should().Be(500);
        options.GrabCursor.Should().BeFalse();
        options.RotateDegree.Should().Be(45);
        options.Depth.Should().Be(200);
        options.Stretch.Should().Be(10);
        options.Modifier.Should().Be(2.0);
        options.SlideShadows.Should().BeFalse();
    }
}
