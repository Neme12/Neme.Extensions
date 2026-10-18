using System.Runtime.CompilerServices;

namespace Neme.Extensions.SourceGenerators.Internal;

internal static class DefaultInterpolatedStringHandlerExtensions
{
    extension(DefaultInterpolatedStringHandler handler)
    {
        public void AppendLine()
        {
            handler.AppendLiteral(Environment.NewLine);
        }

        public void AppendLineLiteral(string value)
        {
            handler.AppendLiteral(value);
            handler.AppendLiteral(Environment.NewLine);
        }

        #region AppendLineFormatted
        #region AppendLineFormatted T
        public void AppendLineFormatted<T>(T value)
        {
            handler.AppendLineFormatted(value);
            handler.AppendLiteral(Environment.NewLine);
        }

        public void AppendLineFormatted<T>(T value, string? format)
        {
            handler.AppendLineFormatted(value, format);
            handler.AppendLiteral(Environment.NewLine);
        }

        public void AppendLineFormatted<T>(T value, int alignment)
        {
            handler.AppendLineFormatted(value, alignment);
            handler.AppendLiteral(Environment.NewLine);
        }

        public void AppendLineFormatted<T>(T value, int alignment, string? format)
        {
            handler.AppendLineFormatted(value, alignment, format);
            handler.AppendLiteral(Environment.NewLine);
        }
        #endregion

        #region AppendLineFormatted ReadOnlySpan<char>
        public void AppendLineFormatted(scoped ReadOnlySpan<char> value)
        {
            handler.AppendLineFormatted(value);
            handler.AppendLiteral(Environment.NewLine);
        }

        public void AppendLineFormatted(scoped ReadOnlySpan<char> value, int alignment = 0, string? format = null)
        {
            handler.AppendLineFormatted(value, alignment, format);
            handler.AppendLiteral(Environment.NewLine);
        }
        #endregion

        #region AppendLineFormatted string
        public void AppendLineFormatted(string? value)
        {
            handler.AppendLineFormatted(value);
            handler.AppendLiteral(Environment.NewLine);
        }

        public void AppendLineFormatted(string? value, int alignment = 0, string? format = null)
        {
            handler.AppendLineFormatted(value, alignment, format);
            handler.AppendLiteral(Environment.NewLine);
        }
        #endregion

        #region AppendLineFormatted object
        public void AppendLineFormatted(object? value, int alignment = 0, string? format = null)
        {
            handler.AppendLineFormatted(value, alignment, format);
            handler.AppendLiteral(Environment.NewLine);
        }
        #endregion
        #endregion
    }
}
