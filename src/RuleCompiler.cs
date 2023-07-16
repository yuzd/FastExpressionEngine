// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FastExpressionEngine.Exceptions;
using FastExpressionEngine.ExpressionBuilders;
using FastExpressionEngine.HelperFunctions;
using FastExpressionEngine.Models;

namespace FastExpressionEngine
{
    /// <summary>
    /// Rule compilers
    /// </summary>
    internal class RuleCompiler
    {
        /// <summary>
        /// The expression builder factory
        /// </summary>
        private readonly RuleExpressionBuilderFactory _expressionBuilderFactory;


        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCompiler"/> class.
        /// </summary>
        /// <param name="expressionBuilderFactory">The expression builder factory.</param>
        /// <exception cref="ArgumentNullException">expressionBuilderFactory</exception>
        internal RuleCompiler(RuleExpressionBuilderFactory expressionBuilderFactory)
        {
            _expressionBuilderFactory = expressionBuilderFactory ??
                                        throw new ArgumentNullException(
                                            $"{nameof(expressionBuilderFactory)} can't be null.");
        }

        /// <summary>
        /// Compiles the rule
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rule"></param>
        /// <param name="input"></param>
        /// <param name="ruleParam"></param>
        /// <returns>Compiled func delegate</returns>
        internal RuleFunc<RuleResultTree> CompileRule(Rule rule, RuleExpressionType ruleExpressionType,
            RuleParameter[] ruleParams, Lazy<RuleExpressionParameter[]> globalParams)
        {
            if (rule == null)
            {
                var ex = new ArgumentNullException(nameof(rule));
                throw ex;
            }

            try
            {
                var globalParamExp = globalParams.Value;
                var extendedRuleParams = ruleParams.Concat(globalParamExp.Select(c =>
                        new RuleParameter(c.ParameterExpression.Name, c.ParameterExpression.Type)))
                    .ToArray();
                var ruleExpression = GetDelegateForRule(rule, extendedRuleParams);


                return GetWrappedRuleFunc(rule, ruleExpression, ruleParams, globalParamExp);
            }
            catch (Exception ex)
            {
                var message = $"Error while compiling rule `{rule.RuleName}`: {ex.Message}";
                return Helpers.ToRuleExceptionResult(rule, new RuleException(message, ex));
            }
        }


        /// <summary>
        /// Gets the expression for rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="typeParameterExpressions">The type parameter expressions.</param>
        /// <param name="ruleInputExp">The rule input exp.</param>
        /// <returns></returns>
        private RuleFunc<RuleResultTree> GetDelegateForRule(Rule rule, RuleParameter[] ruleParams)
        {
            var scopedParamList = GetRuleExpressionParameters(rule.RuleExpressionType, rule?.LocalParams, ruleParams);

            var extendedRuleParams = ruleParams.Concat(scopedParamList.Select(c =>
                    new RuleParameter(c.ParameterExpression.Name, c.ParameterExpression.Type)))
                .ToArray();

            RuleFunc<RuleResultTree> ruleFn = BuildRuleFunc(rule, extendedRuleParams);
            ;

            return GetWrappedRuleFunc(rule, ruleFn, ruleParams, scopedParamList);
        }

        internal RuleExpressionParameter[] GetRuleExpressionParameters(RuleExpressionType ruleExpressionType,
            IEnumerable<ScopedParam> localParams, RuleParameter[] ruleParams)
        {
            var ruleExpParams = new List<RuleExpressionParameter>();

            if (localParams?.Any() == true)
            {
                var parameters = ruleParams.Select(c => c.ParameterExpression)
                    .ToList();

                var expressionBuilder = GetExpressionBuilder(ruleExpressionType);

                foreach (var lp in localParams)
                {
                    try
                    {
                        var lpExpression = expressionBuilder.Parse(lp.Expression, parameters.ToArray(), null);
                        var ruleExpParam = new RuleExpressionParameter()
                        {
                            ParameterExpression = Expression.Parameter(lpExpression.Type, lp.Name),
                            ValueExpression = lpExpression
                        };
                        parameters.Add(ruleExpParam.ParameterExpression);
                        ruleExpParams.Add(ruleExpParam);
                    }
                    catch (Exception ex)
                    {
                        var message = $"{ex.Message}, in ScopedParam: {lp.Name}";
                        throw new RuleException(message);
                    }
                }
            }

            return ruleExpParams.ToArray();
        }

        /// <summary>
        /// Builds the expression.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="typeParameterExpressions">The type parameter expressions.</param>
        /// <param name="ruleInputExp">The rule input exp.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private RuleFunc<RuleResultTree> BuildRuleFunc(Rule rule, RuleParameter[] ruleParams)
        {
            var ruleExpressionBuilder = GetExpressionBuilder(rule.RuleExpressionType);

            var ruleFunc = ruleExpressionBuilder.BuildDelegateForRule(rule, ruleParams);

            return ruleFunc;
        }


        internal Func<object[], Dictionary<string, object>> CompileScopedParams(RuleExpressionType ruleExpressionType,
            RuleParameter[] ruleParameters, RuleExpressionParameter[] ruleExpParams)
        {
            return GetExpressionBuilder(ruleExpressionType).CompileScopedParams(ruleParameters, ruleExpParams);
        }

        private RuleFunc<RuleResultTree> GetWrappedRuleFunc(Rule rule, RuleFunc<RuleResultTree> ruleFunc,
            RuleParameter[] ruleParameters, RuleExpressionParameter[] ruleExpParams)
        {
            if (ruleExpParams.Length == 0)
            {
                return ruleFunc;
            }

            var paramDelegate = CompileScopedParams(rule.RuleExpressionType, ruleParameters, ruleExpParams);

            return (ruleParams) =>
            {
                var inputs = ruleParams.Select(c => c.Value).ToArray();
                IEnumerable<RuleParameter> scopedParams;
                try
                {
                    var scopedParamsDict = paramDelegate(inputs);
                    scopedParams = scopedParamsDict.Select(c => new RuleParameter(c.Key, c.Value));
                }
                catch (Exception ex)
                {
                    var message = $"Error while executing scoped params for rule `{rule.RuleName}` - {ex}";
                    var resultFn = Helpers.ToRuleExceptionResult(rule, new RuleException(message, ex));
                    return resultFn(ruleParams);
                }

                var extendedInputs = ruleParams.Concat(scopedParams);
                var result = ruleFunc(extendedInputs.ToArray());
                return result;
            };
        }

        private RuleExpressionBuilderBase GetExpressionBuilder(RuleExpressionType expressionType)
        {
            return _expressionBuilderFactory.RuleGetExpressionBuilder(expressionType);
        }
    }
}