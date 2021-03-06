using KiCadDoxer.Renderer.Exceptions;
using System.Threading.Tasks;
using Xunit;

namespace KiCadDoxer.Renderer.Tests
{
    public class LineSourceTests
    {
        [Fact]
        public async Task NoNonWhiteSpaceCharacterAfterQuotedText()
        {
            using (var source = new StringLineSource(TokenizerMode.SExpresionKiCad, "\"t\"est"))
            {
                await Assert.ThrowsAsync<KiCadFileFormatException>(async () => await source.Read());
            }
        }

        [Fact]
        public async Task ParenthesisAroundQuotedText()
        {
            using (var source = new StringLineSource(TokenizerMode.SExpresionKiCad, "(\"test\")"))
            {
                Assert.Equal(TokenType.ExpressionOpen, (await source.Read()).Type);
                Assert.Equal("test", await source.Read());
                Assert.Equal(TokenType.ExpressionClose, (await source.Read()).Type);
            }
        }

        [Fact]
        public async Task ParenthesisAroundText()
        {
            using (var source = new StringLineSource(TokenizerMode.SExpresionKiCad, "(test)"))
            {
                Assert.Equal(TokenType.ExpressionOpen, (await source.Read()).Type);
                Assert.Equal("test", await source.Read());
                Assert.Equal(TokenType.ExpressionClose, (await source.Read()).Type);
            }
        }

        [Fact]
        public async Task ReadEmptyQuotedString()
        {
            Assert.Equal("", await StringLineSource.GetUnescapedString(@""));
        }

        [Fact]
        public async Task ReadFullLineOfTextWithWhitespaces()
        {
            using (var source = new StringLineSource(TokenizerMode.SExpresionKiCad, " This is a test \nnot read"))
            {
                string text = await source.ReadTextWhileNot(TokenType.EndOfFile, TokenType.LineBreak);
                Assert.Equal(" This is a test ", text);
            }
        }

        [Fact]
        public async Task ReadQuotedEmptyStringFollowedBySpace()
        {
            using (var source = new StringLineSource(TokenizerMode.SExpresionKiCad, "\"\" "))
            {
                Assert.Equal("", await source.Read());
                Assert.Equal(TokenType.EndOfFile, (await source.Read()).Type);
            }
        }

        [Fact]
        public async Task ReadQuotedStringWithParenthesis()
        {
            Assert.Equal("test (1)", await StringLineSource.GetUnescapedString(@"test (1)"));
        }

        [Fact]
        public async Task ReadQuotedStringWithSpaces()
        {
            Assert.Equal("t t", await StringLineSource.GetUnescapedString(@"t t"));
        }

        [Fact]
        public async Task ReadStringWithoutQuotes()
        {
            using (var source = new StringLineSource(TokenizerMode.SExpresionKiCad, "NoQuotes"))
            {
                string token = await source.Read();
                Assert.Equal("NoQuotes", token);
            }
        }

        [Fact]
        public async Task ReadStringWithoutQuotesBetweenWhitespace()
        {
            using (var source = new StringLineSource(TokenizerMode.SExpresionKiCad, "\r \t NoQuotes \t"))
            {
                string token = await source.Read();
                Assert.Equal("NoQuotes", token);
            }
        }
    }
}