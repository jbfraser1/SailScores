using AutoMapper;
using System;
using System.Threading.Tasks;
using SailScores.Core.Model;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using SailScores.Core.Model.OpenWeatherMap;
using System.Linq;
using SailScores.Api.Enumerations;

namespace SailScores.Core.Services
{
    public class ConversionService : IConversionService
    {
        public string Fahrenheit => "Fahrenheit";

        public string Celsius => "Celsius";

        public string Kelvin => "Kelvin";

        public string MeterPerSecond => "MetersPerSecond";

        public string MilesPerHour => "MPH";


        public decimal? Convert(string measure, string sourceUnits, string destinationUnits)
        {
            decimal decimalMeasure;
            if(decimal.TryParse(measure, out decimalMeasure))
            {
                return Convert(measure, sourceUnits, destinationUnits);
            }
            return null;
        }

        public decimal? Convert(decimal? measure, string sourceUnits, string destinationUnits)
        {
            if (!measure.HasValue)
            {
                return null;
            }
            return Convert(measure.Value, sourceUnits, destinationUnits);
        }
        public decimal Convert(decimal measure, string sourceUnits, string destinationUnits)
        {
            var sourceUnitEnum = GetUnit(sourceUnits);
            var destinationUnitEnum = GetUnit(destinationUnits);
            if(GetUnitType(sourceUnitEnum) != GetUnitType(destinationUnitEnum))
            {
                throw new InvalidOperationException(
                    $"Could not convert between {sourceUnits} and {destinationUnits}");
            }

            if(GetUnitType(sourceUnitEnum) == UnitType.Temperature)
            {
                return ConvertTemperature(measure, sourceUnitEnum, destinationUnitEnum);
            } else if (GetUnitType(sourceUnitEnum) == UnitType.Speed)
            {
                return ConvertSpeed(measure, sourceUnitEnum, destinationUnitEnum);
            }
            throw new InvalidOperationException("Could not complete conversion for the unit types.");
        }

        private decimal ConvertTemperature(
            decimal temp,
            Units sourceUnits,
            Units destinationUnits)
        {
            var tempInKelvin = ConvertToKelvin(temp, sourceUnits);
            return ConvertFromKelvin(tempInKelvin, destinationUnits);
        }

        private decimal ConvertFromKelvin(decimal temp, Units destinationUnits)
        {
            if (destinationUnits == Units.Fahrenheit)
            {
                return ((temp - 273.15m) * 9m / 5m) + 32m;
            }
            if (destinationUnits == Units.Celsius)
            {
                return temp - 273.15m;
            }
            return temp;
        }

        private decimal ConvertToKelvin(decimal temp, Units sourceUnits)
        {
            switch (sourceUnits)
            {
                case Units.Fahrenheit:
                    return (5m / 9m * (temp - 32m)) + 273.15m;
                case Units.Celsius:
                    return temp + 273.15m;
                default:
                    return temp;
            }
        }

        private decimal ConvertSpeed(
            decimal speed,
            Units sourceUnits,
            Units destinationUnits)
        {
            var speedInMeterPerSecond = ConvertToMeterPerSecond(speed, sourceUnits);
            return ConvertFromMeterPerSecond(speedInMeterPerSecond, destinationUnits);
        }

        private decimal ConvertFromMeterPerSecond(decimal speed, Units destinationUnits)
        {
            switch (destinationUnits)
            {
                case Units.MilesPerHour:
                    return speed * 2.237m;
                case Units.KilometersPerHour:
                    return speed * 3.6m;
                case Units.Knots:
                    return speed * 1.944m;
                default:
                    return speed;
            }    
        }

        private decimal ConvertToMeterPerSecond(decimal speed, Units sourceUnits)
        {
            switch (sourceUnits)
            {
                case Units.MilesPerHour:
                    return speed / 2.237m;
                case Units.KilometersPerHour:
                    return speed / 3.6m;
                case Units.Knots:
                    return speed / 1.944m;
                default:
                    return speed;
            }
        }

        private Units GetUnit(string units)
        {
            if (units.ToUpperInvariant() == "MPH")
            {
                return Units.MilesPerHour;
            }
            if (units.ToUpperInvariant() == "KM/H" || units.ToUpperInvariant() == "KPH")
            {
                return Units.KilometersPerHour;
            }
            if (units.ToUpperInvariant() == "KNOTS" || units.ToUpperInvariant().StartsWith("KNT"))
            {
                return Units.Knots;
            }
            if (units.ToUpperInvariant() == "M/S" || units.ToUpperInvariant().StartsWith("METER"))
            {
                return Units.MeterPerSecond;
            }
            if (units.ToUpperInvariant().StartsWith("F")
                || units.ToUpperInvariant().StartsWith("�F"))
            {
                return Units.Fahrenheit;
            }
            if (units.ToUpperInvariant().StartsWith("CE")
                || units.ToUpperInvariant().StartsWith("�C"))
            {
                return Units.Celsius;
            }
            if (units.ToUpperInvariant().StartsWith("KE")
                || units.ToUpperInvariant().StartsWith("�K"))
            {
                return Units.Kelvin;
            }
            throw new InvalidOperationException("Could not convert. Unknown units");
        }

        private UnitType GetUnitType(Units unitEnum)
        {
            switch (unitEnum)
            {
                case Units.Fahrenheit:
                case Units.Celsius:
                case Units.Kelvin:
                    return UnitType.Temperature;
                case Units.KilometersPerHour:
                case Units.Knots:
                case Units.MeterPerSecond:
                case Units.MilesPerHour:
                    return UnitType.Speed;

            }
            throw new InvalidOperationException("Unit Type is not defined for conversion");
        }

        private enum Units
        {
            Fahrenheit = 1,
            Celsius = 2,
            Kelvin = 3,
            MeterPerSecond = 100,
            MilesPerHour = 101,
            KilometersPerHour = 102,
            Knots = 103
        }

        private enum UnitType
        {
            Speed = 1,
            Temperature = 2
        }
    }
}
