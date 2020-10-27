using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
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
        private string  basePath = "/test";

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
            var expectedFilePath =  basePath + "/01-01-20_temps.csv";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                // { @"expectedFilePath", new MockFileData("Testing is meh.") },
                // { @"c:\demo\jQuery.js", new MockFileData("some js") },
                // { @"c:\demo\image.gif", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });
            mockFileSystem.Directory.CreateDirectory(basePath);
            var repository = new TemperatureRepository(options, mockFileSystem, NullLogger<TemperatureRepository>.Instance);
            var temperatureToSave = new Temperature
            {
                dateTime = new DateTime(2020, 01, 01, 0, 0, 0),
                Humidity = 45.6,
                InsideTemp = 21.2,
                OutsideTemp = 14.3,
                WeatherDescription = "Sunny but cold"
            };
            
            //Act
            await repository.SaveTemperature(temperatureToSave);
            
            //Assert
            Assert.IsTrue(mockFileSystem.FileExists(expectedFilePath));
            var temperatureFile = mockFileSystem.File.ReadLines(expectedFilePath);
            var temperatureLine = temperatureFile.First();
            var expectedLine = $"{temperatureToSave.dateTime:dd/MM/yy HH:mm:ss},{temperatureToSave.InsideTemp},{temperatureToSave.Humidity},{temperatureToSave.OutsideTemp},{temperatureToSave.WeatherDescription}";
            Assert.AreEqual(expectedLine, temperatureLine);
            
        }
        
        [Test]
        public async Task SaveTemperaturesToAExistingFile()
        {
            //Arrange
            var options = new OptionsWrapper<TemperatureSettings>(new TemperatureSettings
            {
                BasePath = basePath
            });
            var expectedFilePath =  basePath + "/02-01-20_temps.csv";
            var expectedLine1 = "02/01/20 00:05:00,15.8,51.2,7.05,few clouds";
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { expectedFilePath, new MockFileData(expectedLine1+"\n") },
                // { @"c:\demo\jQuery.js", new MockFileData("some js") },
                // { @"c:\demo\image.gif", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });
            var repository = new TemperatureRepository(options, mockFileSystem, NullLogger<TemperatureRepository>.Instance);
            var temperatureToSave = new Temperature
            {
                dateTime = new DateTime(2020, 01, 02, 0, 10, 0),
                Humidity = 45.6,
                InsideTemp = 21.2,
                OutsideTemp = 14.3,
                WeatherDescription = "Sunny but cold"
            };

            //Act
            await repository.SaveTemperature(temperatureToSave);
            
            //Assert
            Assert.IsTrue(mockFileSystem.FileExists(expectedFilePath));
            var temperatureFile = mockFileSystem.File.ReadLines(expectedFilePath);
            var temperatureLine1 = temperatureFile.Skip(0).Take(1).Single();
            Assert.AreEqual(expectedLine1, temperatureLine1);
            var temperatureLine2 = temperatureFile.Skip(1).Take(1).Single();
            var expectedLine2 = $"{temperatureToSave.dateTime:dd/MM/yy HH:mm:ss},{temperatureToSave.InsideTemp},{temperatureToSave.Humidity},{temperatureToSave.OutsideTemp},{temperatureToSave.WeatherDescription}";
            Assert.AreEqual(expectedLine2, temperatureLine2);
        }

        // [Test]
        // public async Task ReturnLatestTemperatureFromMostRecentlyUpdatedFile()
        // {
        //     //Arrange
        //     var expectedFilePath0101 =  basePath + "/01-01-20_temps.csv";
        //     var expectedFilePath0201 =  basePath + "/02-01-20_temps.csv";
        //
        //     var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        //     {
        //         { expectedFilePath0101, new MockFileData(expectedLine1+"\n") },
        //         { expectedFilePath0201, new MockFileData(expectedLine1+"\n") },
        //     });
        //     var options = new OptionsWrapper<TemperatureSettings>(new TemperatureSettings
        //     {
        //         BasePath = basePath
        //     });
        //     ITemperatureRepository repo = new TemperatureRepository(options, mockFileSystem, NullLogger<TemperatureRepository>.Instance );
        //     
        //     
        //     //Act
        //     var latestTemperature =  await repo.GetLatestTemperature();
        //
        //     //Assert
        //     
        //
        // }
    }
}