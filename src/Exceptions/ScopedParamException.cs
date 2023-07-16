// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Autofac.Annotation.FastExpressionCompiler.Exceptions
{
    public class ScopedParamException : Exception
    {
        public ScopedParamException(string message, Exception innerException, string scopedParamName) : base(message,
            innerException)
        {
            Data.Add("ScopedParamName", scopedParamName);
        }
    }
}