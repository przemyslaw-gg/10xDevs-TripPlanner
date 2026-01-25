using System.Net;
using System.Net.Http.Json;
using TripPlanner.Application.Locations.DTOs;
using TripPlanner.WebApi.Contracts;

namespace TripPlanner.WebApi.IntegrationTests.Endpoints;

/// <summary>
/// Integration tests for Location API endpoints.
/// </summary>
[TestClass]
[TestCategory("Integration")]
public class LocationEndpointsTests
{
    private static CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        _factory = new CustomWebApplicationFactory();
        // Seed data once for all tests
        _factory.SeedTestData();
    }

    [TestInitialize]
    public void Setup()
    {
        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _factory?.Dispose();
    }

    #region GET /api/locations Tests

    [TestMethod]
    public async Task GetLocations_WithNoParameters_ReturnsOkWithPaginatedResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/locations");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<LocationListItemDto>>();

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Items);
        Assert.IsNotNull(result.Pagination);
        Assert.AreEqual(3, result.Items.Count);
        Assert.AreEqual(1, result.Pagination.Page);
        Assert.AreEqual(20, result.Pagination.PageSize);
        Assert.AreEqual(3, result.Pagination.TotalItems);
    }

    [TestMethod]
    public async Task GetLocations_WithSearchParameter_ReturnsFilteredResults()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?search=Greece");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<LocationListItemDto>>();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual("Athens", result.Items[0].Name);
        Assert.AreEqual("Greece", result.Items[0].Country);
    }

    [TestMethod]
    public async Task GetLocations_WithSearchByName_ReturnsMatchingResults()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?search=Rome");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<LocationListItemDto>>();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual("Rome", result.Items[0].Name);
    }

    [TestMethod]
    public async Task GetLocations_WithPagination_ReturnsCorrectPage()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?page=1&pageSize=2");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<LocationListItemDto>>();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual(1, result.Pagination.Page);
        Assert.AreEqual(2, result.Pagination.PageSize);
        Assert.AreEqual(3, result.Pagination.TotalItems);
        Assert.AreEqual(2, result.Pagination.TotalPages);
        Assert.IsTrue(result.Pagination.HasNextPage);
        Assert.IsFalse(result.Pagination.HasPreviousPage);
    }

    [TestMethod]
    public async Task GetLocations_WithSecondPage_ReturnsRemainingItems()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?page=2&pageSize=2");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<LocationListItemDto>>();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(2, result.Pagination.Page);
        Assert.IsFalse(result.Pagination.HasNextPage);
        Assert.IsTrue(result.Pagination.HasPreviousPage);
    }

    [TestMethod]
    public async Task GetLocations_WithInvalidPageZero_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?page=0");

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task GetLocations_WithInvalidPageSizeOver100_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?pageSize=101");

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task GetLocations_WithInvalidPageSizeZero_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?pageSize=0");

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task GetLocations_WithSearchTermTooLong_ReturnsBadRequest()
    {
        // Arrange
        var longSearchTerm = new string('a', 101);

        // Act
        var response = await _client.GetAsync($"/api/locations?search={longSearchTerm}");

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task GetLocations_WithNoMatchingSearch_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/locations?search=NonExistentLocation");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<LocationListItemDto>>();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Items.Count);
        Assert.AreEqual(0, result.Pagination.TotalItems);
    }

    [TestMethod]
    public async Task GetLocations_ResultsAreOrderedByName()
    {
        // Act
        var response = await _client.GetAsync("/api/locations");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<LocationListItemDto>>();

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Items.Count);

        // Verify alphabetical order: Athens, Paris, Rome
        Assert.AreEqual("Athens", result.Items[0].Name);
        Assert.AreEqual("Paris", result.Items[1].Name);
        Assert.AreEqual("Rome", result.Items[2].Name);
    }

    #endregion

    #region GET /api/locations/{id} Tests

    [TestMethod]
    public async Task GetLocationById_WithExistingId_ReturnsOkWithLocation()
    {
        // Arrange
        var existingId = "11111111-1111-1111-1111-111111111111";

        // Act
        var response = await _client.GetAsync($"/api/locations/{existingId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<LocationDto>();

        Assert.IsNotNull(result);
        Assert.AreEqual(Guid.Parse(existingId), result.Id);
        Assert.AreEqual("Athens", result.Name);
        Assert.AreEqual("Greece", result.Country);
        Assert.AreEqual("Europe/Athens", result.Timezone);
        Assert.IsTrue(result.CreatedAt > DateTime.MinValue);
        Assert.IsTrue(result.UpdatedAt > DateTime.MinValue);
    }

    [TestMethod]
    public async Task GetLocationById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/locations/{nonExistentId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetLocationById_WithInvalidGuidFormat_ReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/locations/not-a-valid-guid");

        // Assert
        // Invalid GUID format typically returns 400 or 404 depending on routing
        Assert.IsTrue(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetLocationById_ReturnsCorrectContentType()
    {
        // Arrange
        var existingId = "22222222-2222-2222-2222-222222222222";

        // Act
        var response = await _client.GetAsync($"/api/locations/{existingId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [TestMethod]
    public async Task GetLocationById_WithDifferentExistingIds_ReturnsCorrectLocations()
    {
        // Test Rome
        var romeResponse = await _client.GetAsync("/api/locations/22222222-2222-2222-2222-222222222222");
        Assert.AreEqual(HttpStatusCode.OK, romeResponse.StatusCode);
        var rome = await romeResponse.Content.ReadFromJsonAsync<LocationDto>();
        Assert.IsNotNull(rome);
        Assert.AreEqual("Rome", rome.Name);
        Assert.AreEqual("Italy", rome.Country);

        // Test Paris
        var parisResponse = await _client.GetAsync("/api/locations/33333333-3333-3333-3333-333333333333");
        Assert.AreEqual(HttpStatusCode.OK, parisResponse.StatusCode);
        var paris = await parisResponse.Content.ReadFromJsonAsync<LocationDto>();
        Assert.IsNotNull(paris);
        Assert.AreEqual("Paris", paris.Name);
        Assert.AreEqual("France", paris.Country);
    }

    #endregion
}
