﻿using KiCadDoxer.Renderer.Exceptions;
using KiCadDoxer.Renderer.Schematic;
using System.Threading.Tasks;
using Xunit;

namespace KiCadDoxer.Renderer.Tests.Schematic
{
    public class LineTests
    {
        [Theory]
        [InlineData("Wire", "bus")]
        [InlineData("Entry", "entry bus")]
        public async Task BussesAreBlueAndFat(string rootType, string expectedClass)
        {
            var testCase = new SchematicTestRenderContext("Bus Line\r\n1 2 3 4\r\n", false);
            var testWriter = new TestFragmentWriter();
            testCase.PushSvgWriter(testWriter);

            await (Line.Render(testCase, rootType));

            Assert.True(testWriter.ContainsLine(1, 2, 3, 4, ("stroke", "rgb(0,0,132)", true), ("stroke-width", "12", true), ("class", expectedClass, false)));
        }

        [Theory]
        [InlineData("", "Wire")]
        [InlineData("", "Bus")]
        [InlineData("", "Notes")]
        [InlineData("FAIL", "Bus")]
        [InlineData("Wire", "Line")]
        [InlineData("Wire FAIL", "Line")]
        [InlineData("Wire Line", "LineBreak")]
        [InlineData("Wire Line FAIL", "LineBreak")]
        [InlineData("Wire Line\n1", "integer")]
        [InlineData("Wire Line\n1 FAIL", "integer")]
        [InlineData("Wire Line\n1 2", "integer")]
        [InlineData("Wire Line\n1 2 FAIL", "integer")]
        [InlineData("Wire Line\n1 2 3", "integer")]
        [InlineData("Wire Line\n1 2 3 FAIL", "integer")]
        [InlineData("Wire Line\n1 2 3 4 FAIL", "LineBreak")]
        public async Task IncompleteLineOrWrongTokensThrows(string line, string expectedInException)
        {
            var testCase = new SchematicTestRenderContext(line, false);
            var ex = await Assert.ThrowsAsync<KiCadFileFormatException>(async () => await Line.Render(testCase, "Wire"));
            Assert.Contains(expectedInException, ex.Message);
        }

        [Fact]
        public async Task EntryCantHaveNotesType()
        {
            var testCase = new SchematicTestRenderContext("Notes", false);
            var ex = await Assert.ThrowsAsync<KiCadFileFormatException>(async () => await Line.Render(testCase, "Entry"));
            Assert.Contains("Notes", ex.Message);
        }

        [Fact]
        public async Task NotesAreBlueThinAndDashed()
        {
            var testCase = new SchematicTestRenderContext("Notes Line\r\n1 2 3 4\r\n", false);
            var testWriter = new TestFragmentWriter();
            testCase.PushSvgWriter(testWriter);

            await (Line.Render(testCase, "Wire"));

            Assert.True(testWriter.ContainsLine(1, 2, 3, 4,
                ("stroke", "rgb(0,0,132)", true),
                ("stroke-width", "6", true),
                ("class", "notes", false),
                ("stroke-dasharray", "13.685,15.8425", true)));
        }

        [Theory]
        [InlineData("Wire", "wire")]
        [InlineData("Entry", "entry wire")]
        public async Task WiresAreGreenAndThin(string rootType, string expectedClass)
        {
            var testCase = new SchematicTestRenderContext("Wire Line\r\n1 2 3 4\r\n", false);
            var testWriter = new TestFragmentWriter();
            testCase.PushSvgWriter(testWriter);

            await (Line.Render(testCase, rootType));

            Assert.True(testWriter.ContainsLine(1, 2, 3, 4, ("stroke", "rgb(0,132,0)", true), ("stroke-width", "6", true), ("class", expectedClass, false)));
        }
    }
}