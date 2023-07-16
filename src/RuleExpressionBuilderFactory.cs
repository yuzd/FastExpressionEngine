// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using FastExpressionEngine.ExpressionBuilders;
using FastExpressionEngine.Models;

namespace FastExpressionEngine
{
    internal class RuleExpressionBuilderFactory
    {
        private readonly LambdaExpressionBuilder _lambdaExpressionBuilder;

        public RuleExpressionBuilderFactory(RuleExpressionParser expressionParser)
        {
            _lambdaExpressionBuilder = new LambdaExpressionBuilder(expressionParser);
        }

        public RuleExpressionBuilderBase RuleGetExpressionBuilder(RuleExpressionType ruleExpressionType)
        {
            switch (ruleExpressionType)
            {
                case RuleExpressionType.LambdaExpression:
                    return _lambdaExpressionBuilder;
                default:
                    throw new InvalidOperationException($"{nameof(ruleExpressionType)} has not been supported yet.");
            }
        }
    }
}