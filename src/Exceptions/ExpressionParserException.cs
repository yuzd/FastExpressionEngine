// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace FastExpressionEngine.Exceptions
{
    public class ExpressionParserException : Exception
    {
        public ExpressionParserException(string message, string expression) : base(message)
        {
            Data.Add("Expression", expression);
        }
    }
}