namespace Xbim.Geometry.Engine.Tests;

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Xbim.Geometry.Abstractions;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
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
    [InlineData("C_OMS_FM_GEN_DRE_1100_Sporv prosjektert anlegg.ifc", 23, 6)]
    ////[InlineData("P7_V32_f_c-veg_Vegmodeller-ifc4x3.ifc", 286, 0)]
    ////[InlineData("20200270_Bygg21_LARK.ifc", 0)]
    ////[InlineData("Example files for IFC export\\Ifx4x3-case6-mapcoordinates.ifc", 3)]
    public void GetCurvesAndPoints_FromModel_CountsAreAsExpected(string fileName, int expectedCurveCount, int expectedPointCount)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 4, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var model = IfcStore.Open(stream, IO.StorageType.Ifc, IO.XbimModelType.MemoryModel);
        var context = new Xbim3DModelContext(model, _loggerFactory, XGeometryEngineVersion.V6);

        var result = context.CreateContext(generateCurvesAndPoints: true);

        Assert.True(result);
        Assert.Equal(expectedCurveCount, context.Curves.Count);
        Assert.Equal(expectedPointCount, context.Points.Count);
    }

    [Theory]
    [InlineData("C_OMS_FM_GEN_DRE_1100_Sporv prosjektert anlegg.ifc")]
    public void PresentationStyleAssignment_FromModel_StyleShouldNotBeNull(string fileName)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 4, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var model = IfcStore.Open(stream, IO.StorageType.Ifc, IO.XbimModelType.MemoryModel);

        var assignment = model.Instances.OfType<IIfcPresentationStyleAssignment>().FirstOrDefault(e => e.EntityLabel == 38876);

        foreach (var style in assignment.Styles)
        {
            Assert.IsNotType<Xbim.Ifc4.PresentationAppearanceResource.IfcNullStyle>(style);
        }

        assignment = model.Instances.OfType<IIfcPresentationStyleAssignment>().FirstOrDefault(e => e.EntityLabel == 42822);

        foreach (var style in assignment.Styles)
        {
            Assert.IsNotType<Xbim.Ifc4.PresentationAppearanceResource.IfcNullStyle>(style);
        }
    }
}