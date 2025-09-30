using Grpc.Core;
using TelemetryService.Protos;
using TelemetryService.Data;
using TelemetryService.Models;

namespace TelemetryService.Endpoints;

public class TelemetryGrpcService : TelemetryCollector.TelemetryCollectorBase
{
    private readonly TelemetryDbContext _db;
    private readonly ILogger<TelemetryGrpcService> _logger;

    public TelemetryGrpcService(TelemetryDbContext db, ILogger<TelemetryGrpcService> logger)
    {
        _db = db; _logger = logger;
    }

    public override async Task<TelemetryAck> StreamTelemetry(IAsyncStreamReader<TelemetryPoint> requestStream, ServerCallContext context)
    {
        var count = 0;
        await foreach (var p in requestStream.ReadAllAsync())
        {
            var record = new TelemetryRecord
            {
                VehicleId = p.VehicleId,
                Speed = p.Speed,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                FuelLevel = p.FuelLevel,
                Timestamp = DateTime.Parse(p.Timestamp)
            };
            _db.Telemetry.Add(record);
            count++;
            if (count % 50 == 0) await _db.SaveChangesAsync();
            _logger.LogInformation("Telemetry ingested for {v}: speed={s} fuel={f}", record.VehicleId, record.Speed, record.FuelLevel);
        }
        await _db.SaveChangesAsync();
        return new TelemetryAck { Message = $"Received {count} points" };
    }
}