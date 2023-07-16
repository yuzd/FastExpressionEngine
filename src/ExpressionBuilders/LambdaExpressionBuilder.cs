// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Expressions;
using FastExpressionEngine.Exceptions;
using FastExpressionEngine.HelperFunctions;
using FastExpressionEngine.Models;

namespace FastExpressionEngine.ExpressionBuilders
{
    internal sealed class LambdaExpressionBuilder : RuleExpressionBuilderBase
    {
        private readonly RuleExpressionParser _ruleExpressionParser;

        internal LambdaExpressionBuilder(RuleExpressionParser ruleExpressionParser)
        {
            _ruleExpressionParser = ruleExpressionParser;
        }

        internal override RuleFunc<RuleResultTree> BuildDelegateForRule(Rule rule, RuleParameter[] ruleParams)
        {
            try
            {
                var ruleDelegate = _ruleExpressionParser.Compile<bool>(rule.Expression, ruleParams);
                return Helpers.ToResultTree(rule, null, ruleDelegate);
            }
            catch (Exception ex)
            {
                Helpers.HandleRuleException(ex, rule);

                var exceptionMessage = $"Exception while parsing expression `{rule?.Expression}` - {ex.Message}";

                bool func(object[] param) => false;

                return Helpers.ToResultTree(rule, null, func, exceptionMessage);
            }
        }

        internal override Expression Parse(string expression, ParameterExpression[] parameters, Type returnType)
        {
            try
            {
                return _ruleExpressionParser.Parse(expression, parameters, returnType);
            }
            catch (ParseException ex)
            {
                throw new ExpressionParserException(ex.Message, expression);
            }
        }

        internal override Func<object[], Dictionary<string, object>> CompileScopedParams(RuleParameter[] ruleParameters,
            RuleExpressionParameter[] scopedParameters)
        {
            return _ruleExpressionParser.CompileRuleExpressionParameters(ruleParameters, scopedParameters);
        }
    }
}