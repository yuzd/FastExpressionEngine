// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastExpressionEngine.Exceptions;
using FastExpressionEngine.Models;

namespace FastExpressionEngine.HelperFunctions
{
    /// <summary>
    /// Helpers
    /// </summary>
    internal static class Helpers
    {
        internal static RuleFunc<RuleResultTree> ToResultTree(Rule rule,
            IEnumerable<RuleResultTree> childRuleResults, Func<object[], bool> isSuccessFunc,
            string exceptionMessage = "")
        {
            return (inputs) =>
            {
                var isSuccess = false;
                var inputsDict = new Dictionary<string, object>();
                try
                {
                    inputsDict = inputs.ToDictionary(c => c.Name, c => c.Value);
                    isSuccess = isSuccessFunc(inputs.Select(c => c.Value).ToArray());
                }
                catch (Exception ex)
                {
                    exceptionMessage = $"Error while executing rule : {rule?.RuleName} - {ex.Message}";
                    HandleRuleException(new RuleException(exceptionMessage, ex), rule);
                    isSuccess = false;
                }

                return new RuleResultTree
                {
                    Rule = rule,
                    Inputs = inputsDict,
                    IsSuccess = isSuccess,
                    ExceptionMessage = exceptionMessage
                };
            };
        }

        internal static RuleFunc<RuleResultTree> ToRuleExceptionResult(Rule rule, Exception ex)
        {
            HandleRuleException(ex, rule);
            return ToResultTree(rule, null, (args) => false, ex.Message);
        }

        internal static void HandleRuleException(Exception ex, Rule rule)
        {
            ex.Data.Add(nameof(rule.RuleName), rule.RuleName);
            ex.Data.Add(nameof(rule.Expression), rule.Expression);
        }
    }
}