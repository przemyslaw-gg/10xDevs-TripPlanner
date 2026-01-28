using TripPlanner.Application.Locations.Queries.GetLocations;

namespace TripPlanner.Application.UnitTests.Locations.Queries;

/// <summary>
/// Unit tests for <see cref="GetLocationsQueryValidator"/>.
/// </summary>
[TestClass]
[TestCategory("Unit")]
public class GetLocationsQueryValidatorTests
{
    private GetLocationsQueryValidator _validator = null!;

    [TestInitialize]
    public void Setup()
    {
        _validator = new GetLocationsQueryValidator();
    }

    [TestMethod]
    public void Validate_WithDefaultValues_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery();

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.IsEmpty(result.Errors);
    }

    [TestMethod]
    public void Validate_WithValidSearchTerm_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery(Search: "Athens");

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_WithSearchTermExceeding100Characters_ShouldFail()
    {
        // Arrange
        var longSearchTerm = new string('a', 101);
        var query = new GetLocationsQuery(Search: longSearchTerm);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.HasCount(1, result.Errors);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Search"));
    }

    [TestMethod]
    public void Validate_WithSearchTermExactly100Characters_ShouldPass()
    {
        // Arrange
        var exactLengthSearchTerm = new string('a', 100);
        var query = new GetLocationsQuery(Search: exactLengthSearchTerm);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_WithPageZero_ShouldFail()
    {
        // Arrange
        var query = new GetLocationsQuery(Page: 0);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.HasCount(1, result.Errors);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Page"));
    }

    [TestMethod]
    public void Validate_WithNegativePage_ShouldFail()
    {
        // Arrange
        var query = new GetLocationsQuery(Page: -1);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Page"));
    }

    [TestMethod]
    public void Validate_WithPageOne_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery(Page: 1);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_WithLargePage_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery(Page: 1000);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_WithPageSizeZero_ShouldFail()
    {
        // Arrange
        var query = new GetLocationsQuery(PageSize: 0);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "PageSize"));
    }

    [TestMethod]
    public void Validate_WithPageSizeExceeding100_ShouldFail()
    {
        // Arrange
        var query = new GetLocationsQuery(PageSize: 101);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.HasCount(1, result.Errors);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "PageSize"));
    }

    [TestMethod]
    public void Validate_WithPageSizeOne_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery(PageSize: 1);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_WithPageSize100_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery(PageSize: 100);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_WithAllValidParameters_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery(
            Search: "Greece",
            Page: 5,
            PageSize: 50
        );

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_WithMultipleInvalidParameters_ShouldReturnMultipleErrors()
    {
        // Arrange
        var query = new GetLocationsQuery(
            Search: new string('a', 101),
            Page: 0,
            PageSize: 101
        );

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.HasCount(3, result.Errors);
    }

    [TestMethod]
    public void Validate_WithNullSearch_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery(Search: null);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_WithEmptySearch_ShouldPass()
    {
        // Arrange
        var query = new GetLocationsQuery(Search: "");

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.IsTrue(result.IsValid);
    }
}
