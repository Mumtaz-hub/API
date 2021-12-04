using System;
using System.Collections.Generic;

namespace Common.Interface
{
    public interface IResult
    {
        bool IsSuccess { get; }
        bool IsFailure { get; }
        IEnumerable<string> Failures { get; }
        string HtmlFormattedFailures { get; }

        Exception Exception { get; set; }
        bool HasException { get; }

        void SetFailures(IEnumerable<string> failures);
    }
}
