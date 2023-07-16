// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FastExpressionEngine.Models
{
    /// <summary>
    /// Rule class
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Rule
    {
        /// <summary>
        /// Rule name for the Rule
        /// </summary>
        public string RuleName { get; set; }

        internal RuleExpressionType RuleExpressionType { get; set; } = RuleExpressionType.LambdaExpression;
        public IEnumerable<ScopedParam> LocalParams { get; set; }
        public string Expression { get; set; }
    }
}