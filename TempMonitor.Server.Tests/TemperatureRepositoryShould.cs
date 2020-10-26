using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using TempMonitor.Server.Repository;
using TempMonitor.Server.Settings;
using TempMonitor.Shared;

namespace TempMonitor.Server.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task SaveTemperaturesToANewFile()
        {
            //Arrange
            var basePath = "/Users/rossellerington/Projects/TempMonitor/TempMonitor/Server/Data";
            var options = new OptionsWrapper<TemperatureSettings>(new TemperatureSettings
            {
                BasePath = basePath
            });
            var repository = new TemperatureRepository(options, NullLogger<TemperatureRepository>.Instance);
            var temperatureToSave = new Temperature
            {
                dateTime = new DateTime(2020, 01, 01, 0, 0, 0),
                Humidity = 45.6,
                InsideTemp = 21.2,
                OutsideTemp = 14.3,
                WeatherDescription = "Sunny but cold"
            };
            var expectedFilePath =  basePath + "/01-01-20_temps.csv";
            if (File.Exists(expectedFilePath))
                File.Delete(expectedFilePath);
            
            //Act
            await repository.SaveTemperature(temperatureToSave);
            
            //Assert
            Assert.IsTrue(File.Exists(expectedFilePath));
            var temperatureFile = File.ReadLines(expectedFilePath);
            var temperatureLine = temperatureFile.First();
            var expectedLine = $"{temperatureToSave.dateTime:dd/MM/yy HH:mm:ss},{temperatureToSave.InsideTemp},{temperatureToSave.Humidity},{temperatureToSave.OutsideTemp},{temperatureToSave.WeatherDescription}";
            Assert.AreEqual(expectedLine, temperatureLine);
            
            //Teardown
            File.Delete(expectedFilePath);
        }
    }
}