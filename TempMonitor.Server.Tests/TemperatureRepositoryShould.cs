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
        private string  basePath = "/Users/rossellerington/Projects/TempMonitor/TempMonitor/Server/Data";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task SaveTemperaturesToANewFile()
        {
            //Arrange
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
        
        [Test]
        public async Task SaveTemperaturesToAExistingFile()
        {
            //Arrange
            var options = new OptionsWrapper<TemperatureSettings>(new TemperatureSettings
            {
                BasePath = basePath
            });
            var repository = new TemperatureRepository(options, NullLogger<TemperatureRepository>.Instance);
            var temperatureToSave = new Temperature
            {
                dateTime = new DateTime(2020, 01, 02, 0, 10, 0),
                Humidity = 45.6,
                InsideTemp = 21.2,
                OutsideTemp = 14.3,
                WeatherDescription = "Sunny but cold"
            };
            var expectedFilePath =  basePath + "/02-01-20_temps.csv";
            var expectedLine1 = "02/01/20 00:05:00,15.8,51.2,7.05,few clouds";
            if (File.Exists(expectedFilePath))
            {
                File.Delete(expectedFilePath);
            }
            await File.WriteAllLinesAsync(expectedFilePath, new[]{expectedLine1} );

            //Act
            await repository.SaveTemperature(temperatureToSave);
            
            //Assert
            Assert.IsTrue(File.Exists(expectedFilePath));
            var temperatureFile = File.ReadLines(expectedFilePath);
            var temperatureLine1 = temperatureFile.Skip(0).Take(1).Single();
            Assert.AreEqual(expectedLine1, temperatureLine1);
            var temperatureLine2 = temperatureFile.Skip(1).Take(1).Single();
            var expectedLine2 = $"{temperatureToSave.dateTime:dd/MM/yy HH:mm:ss},{temperatureToSave.InsideTemp},{temperatureToSave.Humidity},{temperatureToSave.OutsideTemp},{temperatureToSave.WeatherDescription}";
            Assert.AreEqual(expectedLine2, temperatureLine2);
            
            //Teardown
            File.Delete(expectedFilePath);
        }

        [TearDown]
        public void Teardown()
        {
            File.Delete(basePath + "/02-01-20_temps.csv");
            File.Delete(basePath + "/01-01-20_temps.csv");

        }
    }
}