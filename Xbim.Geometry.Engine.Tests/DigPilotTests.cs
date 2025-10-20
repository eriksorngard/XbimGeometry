namespace Xbim.Geometry.Engine.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using Xbim.Geometry.Abstractions;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;
using Xunit;

public class DigPilotTests
{
    private readonly ILoggerFactory _loggerFactory;

    public DigPilotTests(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    [Theory]
    ////[InlineData("Stømner.ifc", 3)]
    ////[InlineData("StikkrenneTestAH.ifc", 1)]
    [InlineData("C_OMS_FM_GEN_DRE_1100_Sporv prosjektert anlegg.ifc", 23)]
    public void GetPolylines(string fileName, int expectedCount)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 4, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var model = IfcStore.Open(stream, IO.StorageType.Ifc, IO.XbimModelType.MemoryModel);
        var context = new Xbim3DModelContext(model, _loggerFactory, XGeometryEngineVersion.V6);

        var result = context.CreateContext(generatePolylines: true);

        result.Should().Be(true);
        context.Polylines.Should().HaveCount(expectedCount);
    }
}