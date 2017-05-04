﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KiCadDoxer.Renderer.Tests.Schematic
{
    public class SchematicTestRenderContext : RenderContext
    {
        private string fileContent;
        private StringWriter outputWriter = new StringWriter();
        private Lazy<XDocument> result;

        public SchematicTestRenderContext(string fileContent)
        {
            this.fileContent = fileContent;
            result = new Lazy<XDocument>(() => XDocument.Parse(outputWriter.ToString()));
        }

        public XDocument Result => result.Value;

        public override Task<LineSource> CreateLibraryLineSource(string libraryName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<LineSource> CreateLineSource(CancellationToken cancellationToken)
        {
            return Task.FromResult((LineSource)new StringLineSource(fileContent));
        }

        public override Task<TextWriter> CreateOutputWriter(CancellationToken cancellationToken)
        {
            return Task.FromResult((TextWriter)outputWriter);
        }

        protected override SchematicRenderSettings CreateSchematicRenderSettings()
        {
            return new SchematicRenderSettings();
        }

        private class StringLineSource : LineSource
        {
            private string fileContent;

            public StringLineSource(string fileContent) : base(CancellationToken.None)
            {
                this.fileContent = fileContent;
            }

            protected override Task<TextReader> CreateReader(CancellationToken cancellationToken)
            {
                return Task.FromResult((TextReader)new StringReader(fileContent));
            }
        }
    }
}