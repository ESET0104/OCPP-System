using Chargersimulator.State;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;


namespace Chargersimulator
{
    public class CarApi
    {
        public static void Start(WebSocketClient charger)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
            var app = builder.Build();

            app.MapPost("/plug", async (PlugDto dto) =>
            {
                //Console.WriteLine($"CAR PLUG REQUEST: {dto.vin}, SOC={dto.soc}%");
                //var authorized = await charger.AuthorizeVinAsync(dto.vin);
                //if (!authorized)
                //{
                //    Console.WriteLine("❌ VIN REJECTED");
                //    return Results.Unauthorized();
                //}
                //charger.SetCar(dto.vin, dto.soc);
                //return Results.Ok();
                Console.WriteLine($"CAR PLUG REQUEST: {dto.vin}, SOC={dto.soc}%");

                var authorized = await charger.AuthorizeVinAsync(dto.vin);

                if (authorized)
                {
                    charger.SetCar(dto.vin, dto.soc);
                }

                return Results.Ok(new
                {
                    Authorized = authorized
                });
            });

            app.MapGet("/status", () =>
            {
                return Results.Ok(new
                {
                    status = charger.State.Status.ToString()
                });
            });

            app.MapPost("/soc", (SocDto dto) =>
            {

                if (!charger.CanAcceptSoc)
                    return Results.BadRequest("Charging not started");

                charger.SetSoc(dto.soc);
                return Results.Ok();
            });

            app.MapPost("/unplug", () =>
            {
                charger.RemoveCar();
                return "OK";
            });

            app.RunAsync("http://localhost:7001");
        }

        public record PlugDto(string vin, double soc);
        public record SocDto(double soc);
    }
}
