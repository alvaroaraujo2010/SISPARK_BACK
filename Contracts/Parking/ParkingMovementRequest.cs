using System.ComponentModel.DataAnnotations;

namespace sispark_api.Contracts.Parking;

public record ParkingMovementRequest([Required][MaxLength(15)] string Plate);
