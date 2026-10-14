using System.Text;

namespace System;

public static partial class StringPolyfill
{
#if !NET9_0_OR_GREATER
    internal const int StackallocCharBufferSizeLimit = 256;
#endif

    extension(String @string)
    {
        public static string Join(string? separator, params ReadOnlySpan<string?> value)
        {
#if NET9_0_OR_GREATER
            return string.Join(separator, value);
#else
            return JoinCore(separator.AsSpan(), value);
#endif
        }

        public static string Join(string? separator, params object?[] values)
        {
#if NET9_0_OR_GREATER
            return string.Join(separator, values);
#else
            if (values == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.values);
            }

            return JoinCore(separator.AsSpan(), (ReadOnlySpan<object?>)values);
#endif
        }

#if !NET9_0_OR_GREATER
        private static unsafe string JoinCore(ReadOnlySpan<char> separator, ReadOnlySpan<object?> values)
        {
            if (values.IsEmpty)
            {
                return string.Empty;
            }

            string? firstString = values[0]?.ToString();

            if (values.Length == 1)
            {
                return firstString ?? string.Empty;
            }

            var result = new ValueStringBuilder(stackalloc char[StackallocCharBufferSizeLimit]);

            result.Append(firstString);

            for (int i = 1; i < values.Length; i++)
            {
                result.Append(separator);
                object? value = values[i];
                if (value != null)
                {
                    result.Append(value.ToString());
                }
            }

            return result.ToString();
        }

        private static string JoinCore(ReadOnlySpan<char> separator, ReadOnlySpan<string?> values)
        {
            if (values.Length <= 1)
            {
                return values.IsEmpty ?
                    string.Empty :
                    values[0] ?? string.Empty;
            }

            long totalSeparatorsLength = (long)(values.Length - 1) * separator.Length;
            if (totalSeparatorsLength > int.MaxValue)
            {
                ThrowHelper.ThrowOutOfMemoryException_StringTooLong();
            }
            int totalLength = (int)totalSeparatorsLength;

            // Calculate the length of the resultant string so we know how much space to allocate.
            foreach (string? value in values)
            {
                if (value != null)
                {
                    totalLength += value.Length;
                    if (totalLength < 0) // Check for overflow
                    {
                        ThrowHelper.ThrowOutOfMemoryException_StringTooLong();
                    }
                }
            }

            if (totalLength == 0)
            {
                return string.Empty;
            }

            // Copy each of the strings into the result buffer, interleaving with the separator.
            Span<char> resultSpan = new char[totalLength];
            Span<char> originalResultSpan = resultSpan;
            bool copyFailed = false;

            for (int i = 0; i < values.Length; i++)
            {
                // It's possible that another thread may have mutated the input array
                // such that our second read of an index will not be the same string
                // we got during the first read.

                // We range check each value and separator to avoid buffer overflows if this happens.

                if (values[i] is string value)
                {
                    if (!value.TryCopyTo(resultSpan))
                    {
                        copyFailed = true;
                        break;
                    }

                    resultSpan = resultSpan[value.Length..];
                }

                if (i < values.Length - 1)
                {
                    if (separator.Length > resultSpan.Length)
                    {
                        copyFailed = true;
                        break;
                    }

                    // Fill in the separator.
                    // Special-case length 1 to avoid additional overheads of CopyTo.
                    // This is common due to the char separator overload.

                    if (separator.Length == 1)
                    {
                        resultSpan[0] = separator[0];
                    }
                    else
                    {
                        separator.CopyTo(resultSpan);
                    }

                    resultSpan = resultSpan[separator.Length..];
                }
            }

            // If we copied exactly the right amount, return the new string.  Otherwise,
            // something changed concurrently to mutate the input array: fall back to
            // doing the concatenation again, but this time with a defensive copy. This
            // fall back should be extremely rare.
            return !copyFailed && resultSpan.IsEmpty ?
                originalResultSpan.ToString() :
                JoinCore(separator, values.ToArray().AsSpan());
        }

#endif
    }
}
