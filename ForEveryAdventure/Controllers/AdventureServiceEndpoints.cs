using ForEveryAdventure.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;

namespace ForEveryAdventure.Services
{
    public class AdventureAPIService
    {
        private readonly HttpClient _client = new HttpClient();
        private const string BaseUrl = "http://localhost:5034/api";

        // Fixes CS0721 and IDE0060: Remove JsonSerializer parameter, use static JsonSerializer directly
        public async Task<AssetTag> CreateAssetTagAsync(Guid userId)
        {
            var content = new StringContent(JsonSerializer.Serialize(userId), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}/MakeAssetTag", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var assetTag = JsonSerializer.Deserialize<AssetTag>(json);
            if (assetTag is null)
                throw new InvalidOperationException("Failed to deserialize AssetTag from API response.");
            return assetTag;
        }

        public async Task<TripPlan> AddTripPlanAsync(TripPlan plan)
        {
            var content = new StringContent(JsonSerializer.Serialize(plan), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}/AddPlanLogistics", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var tripPlan = JsonSerializer.Deserialize<TripPlan>(json) ?? throw new InvalidOperationException("Failed to deserialize TripPlan from API response.");
            return tripPlan!;
        }

        public async Task<AssetTag> GetAssetTagAsync(string tagCode)
        {
            var response = await _client.GetAsync($"{BaseUrl}/ShareAssetTag/{tagCode}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var assetTag = JsonSerializer.Deserialize<AssetTag>(json);
            if (assetTag is null)
                throw new InvalidOperationException("Failed to deserialize AssetTag from API response.");
            return assetTag;
        }

        public async Task<string> SendEmergencyAlertAsync(string tagCode)
        {
            var response = await _client.PostAsync($"{BaseUrl}/Emergency/{tagCode}/alert", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }


public static class AssetTagEndpoints
{
	public static void MapAssetTagEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/AssetTag").WithTags(nameof(AssetTag));

        group.MapGet("/", () =>
        {
            return new [] { new AssetTag() };
        })
        .WithName("GetAllAssetTags")
        .WithOpenApi();

        group.MapGet("/{id}", (int id) =>
        {
            //Replace with actual lookup logic
            return Results.Ok(new AssetTag { Id = Guid.NewGuid(), TagCode = "Sample" });
        })
        .WithName("GetAssetTagById")
        .WithOpenApi();

        group.MapPut("/{id}", (int id, AssetTag input) =>
        {
            return TypedResults.NoContent();
        })
        .WithName("UpdateAssetTag")
        .WithOpenApi();

        group.MapPost("/", (AssetTag model) =>
        {
            // Example return logic for the CreateAssetTag endpoint
            group.MapPost("/", (AssetTag model) =>
            {
                // Simulate creating a new AssetTag with a generated Id and TagCode
                var createdTag = new AssetTag
                {
                    Id = Guid.NewGuid(),
                    TagCode = model.TagCode ?? $"TAG-{Guid.NewGuid().ToString().Substring(0, 8)}",
                    UserId = model.UserId,
                    EmergencyContacts = model.EmergencyContacts ?? new List<EmergencyContact>(),
                    TripPlans = model.TripPlans ?? new List<TripPlan>()
                };
                return TypedResults.Created($"/api/AssetTag/{createdTag.Id}", createdTag);
            })
            .WithName("CreateAssetTag")
            .WithOpenApi();
            //return TypedResults.Created($"/api/AssetTags/{model.ID}", model);
        })
        .WithName("CreateAssetTag")
        .WithOpenApi();

        group.MapDelete("/{id}", (int id) =>
        {
            return Results.Ok(new AssetTag
            {
                Id = Guid.NewGuid(),
                TagCode = $"TAG-{Guid.NewGuid().ToString().Substring(0, 8)}",
                UserId = Guid.NewGuid(),
                EmergencyContacts = new List<EmergencyContact>
                    {
                        new EmergencyContact
                        {
                            Id = Guid.NewGuid(),
                            Name = "John Doe",
                            Phone = "555-1234",
                            Email = "john.doe@example.com"
                        }
                    },
                TripPlans = new List<TripPlan>
                    {
                        new TripPlan
                        {
                            TripIdentifier = Guid.NewGuid(),
                            TripRoutePreference = "Scenic",
                            TripRoute = "Mountain Trail",
                            TripStartDate = DateTime.UtcNow,
                            TripEndDate = DateTime.UtcNow.AddDays(3),
                            TripDurationDays = 3,
                            TripLocationStart = new List<ForEveryAdventure.Models.LocationCoordinates>
                            {
                                new ForEveryAdventure.Models.LocationCoordinates
                                {
                                    LocationIdentifier = Guid.NewGuid(),
                                    LocationName = "Trailhead",
                                    LocationGPSformat01 = "40.7128,-74.0060"
                                }
                            },
                            TripLocationEnd = new List<ForEveryAdventure.Models.LocationCoordinates>
                            {
                                new ForEveryAdventure.Models.LocationCoordinates
                                {
                                    LocationIdentifier = Guid.NewGuid(),
                                    LocationName = "Summit",
                                    LocationGPSformat01 = "40.7138,-74.0070"
                                }
                            },
                            TripFeaturedLocation = new List<ForEveryAdventure.Models.LocationCoordinates>
                            {
                                new ForEveryAdventure.Models.LocationCoordinates
                                {
                                    LocationIdentifier = Guid.NewGuid(),
                                    LocationName = "Lake View",
                                    LocationGPSformat01 = "40.7148,-74.0080"
                                }
                            }
                        }
                    }
            });
            //return TypedResults.Ok(new AssetTag { ID = id });
        })
        .WithName("DeleteAssetTag")
        .WithOpenApi();
    }
}

public static class TripPlanEndpoints
{
	public static void MapTripPlanEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TripPlan").WithTags(nameof(TripPlan));

        group.MapGet("/", () =>
        {
            return new [] { new TripPlan() };
        })
        .WithName("GetAllTripPlans")
        .WithOpenApi();

        group.MapGet("/{id}", (int id) =>
        {
            //return new TripPlan { ID = id };
            group.MapGet("/{id}", (int id) =>
            {
                // Example return logic for GetTripPlanById
                var exampleTripPlan = new TripPlan
                {
                    TripIdentifier = Guid.NewGuid(),
                    TripRoutePreference = "Scenic",
                    TripRoute = "Mountain Trail",
                    TripStartDate = DateTime.UtcNow,
                    TripEndDate = DateTime.UtcNow.AddDays(3),
                    TripDurationDays = 3,
                    TripLocationStart = new List<ForEveryAdventure.Models.LocationCoordinates>
                    {
                            new ForEveryAdventure.Models.LocationCoordinates
                            {
                                LocationIdentifier = Guid.NewGuid(),
                                LocationName = "Trailhead",
                                LocationGPSformat01 = "40.7128,-74.0060"
                            }
                    },
                    TripLocationEnd = new List<ForEveryAdventure.Models.LocationCoordinates>
                    {
                            new ForEveryAdventure.Models.LocationCoordinates
                            {
                                LocationIdentifier = Guid.NewGuid(),
                                LocationName = "Summit",
                                LocationGPSformat01 = "40.7138,-74.0070"
                            }
                    },
                    TripFeaturedLocation = new List<ForEveryAdventure.Models.LocationCoordinates>
                    {
                            new ForEveryAdventure.Models.LocationCoordinates
                            {
                                LocationIdentifier = Guid.NewGuid(),
                                LocationName = "Lake View",
                                LocationGPSformat01 = "40.7148,-74.0080"
                            }
                    }
                };
                return Results.Ok(exampleTripPlan);
            });
        })
        .WithName("GetTripPlanById")
        .WithOpenApi();

        group.MapPut("/{id}", (int id, TripPlan input) =>
        {
            return TypedResults.NoContent();
        })
        .WithName("UpdateTripPlan")
        .WithOpenApi();

        group.MapPost("/", (TripPlan model) =>
        {
            //return TypedResults.Created($"/api/TripPlans/{model.ID}", model);
            // Example return logic for the CreateTripPlan endpoint
            var createdPlan = new TripPlan
            {
                TripIdentifier = Guid.NewGuid(),
                TripRoutePreference = model.TripRoutePreference ?? "Scenic",
                TripRoute = model.TripRoute ?? "Default Route",
                TripStartDate = model.TripStartDate != default ? model.TripStartDate : DateTime.UtcNow,
                TripEndDate = model.TripEndDate != default ? model.TripEndDate : DateTime.UtcNow.AddDays(3),
                TripDurationDays = model.TripDurationDays > 0 ? model.TripDurationDays : 3,
                TripLocationStart = model.TripLocationStart ?? new List<ForEveryAdventure.Models.LocationCoordinates>(),
                TripLocationEnd = model.TripLocationEnd ?? new List<ForEveryAdventure.Models.LocationCoordinates>(),
                TripFeaturedLocation = model.TripFeaturedLocation ?? new List<ForEveryAdventure.Models.LocationCoordinates>()
            };
            return TypedResults.Created($"/api/TripPlan/{createdPlan.TripIdentifier}", createdPlan);
        })
        .WithName("CreateTripPlan")
        .WithOpenApi();

        group.MapDelete("/{id}", (int id) =>
        {
            //return TypedResults.Ok(new TripPlan { ID = id });
            group.MapDelete("/{id}", (int id) =>
            {
                // Example return logic for DeleteTripPlan
                // Simulate deleting a TripPlan and returning the deleted object
                var deletedPlan = new TripPlan
                {
                    TripIdentifier = Guid.NewGuid(),
                    TripRoutePreference = "Deleted",
                    TripRoute = "Deleted Route",
                    TripStartDate = DateTime.UtcNow,
                    TripEndDate = DateTime.UtcNow.AddDays(1),
                    TripDurationDays = 1,
                    TripLocationStart = new List<ForEveryAdventure.Models.LocationCoordinates>
                    {
                            new ForEveryAdventure.Models.LocationCoordinates
                            {
                                LocationIdentifier = Guid.NewGuid(),
                                LocationName = "Deleted Start",
                                LocationGPSformat01 = "0.0000,0.0000"
                            }
                    },
                    TripLocationEnd = new List<ForEveryAdventure.Models.LocationCoordinates>
                    {
                            new ForEveryAdventure.Models.LocationCoordinates
                            {
                                LocationIdentifier = Guid.NewGuid(),
                                LocationName = "Deleted End",
                                LocationGPSformat01 = "0.0000,0.0000"
                            }
                    },
                    TripFeaturedLocation = new List<ForEveryAdventure.Models.LocationCoordinates>
                    {
                            new ForEveryAdventure.Models.LocationCoordinates
                            {
                                LocationIdentifier = Guid.NewGuid(),
                                LocationName = "Deleted Featured",
                                LocationGPSformat01 = "0.0000,0.0000"
                            }
                    }
                };
                return Results.Ok(deletedPlan);
            });
        })
        .WithName("DeleteTripPlan")
        .WithOpenApi();
    }
}}