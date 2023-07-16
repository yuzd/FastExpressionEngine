// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BitFaster.Caching.Lru;
using FastExpressionEngine.ExpressionBuilders;
using FastExpressionEngine.Extensions;
using FastExpressionEngine.HelperFunctions;
using FastExpressionEngine.Models;

namespace FastExpressionEngine
{
    /// <summary>
    ///  执行字符串表达式，结果为true/false
    /// </summary>
    public class ExpressionEngine
    {
        private readonly EngineSetting _reSettings;

        #region Variables

        private readonly RuleCompiler _ruleCompiler;

        private readonly ConcurrentLru<string, RuleFunc<RuleResultTree>> _cache;

        #endregion

        #region Constructor

        public ExpressionEngine(EngineSetting reSettings = null)
        {
            _reSettings = reSettings == null ? new EngineSetting() : new EngineSetting(reSettings);
            if (_reSettings.CacheSize < 1)
            {
                _reSettings.CacheSize = 1000;
            }

            _cache = new ConcurrentLru<string, RuleFunc<RuleResultTree>>(_reSettings.CacheSize);
            var ruleExpressionParser = new RuleExpressionParser(_reSettings);
            _ruleCompiler = new RuleCompiler(new RuleExpressionBuilderFactory(ruleExpressionParser));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This will execute all the rules of the specified workflow
        /// </summary>
        /// <param name="expression">The name of the workflow with rules to execute against the inputs</param>
        /// <param name="inputs">A variable number of inputs</param>
        /// <returns>List of rule results</returns>
        public RuleResultTree Execute(string expression, params object[] inputs)
        {
            var ruleParams = new List<RuleParameter>();

            for (var i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                ruleParams.Add(new RuleParameter($"input{i + 1}", input));
            }

            return Execute(expression, ruleParams.ToArray());
        }

        public RuleResultTree Execute(Rule rule, params object[] inputs)
        {
            var ruleParams = new List<RuleParameter>();

            for (var i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                ruleParams.Add(new RuleParameter($"input{i + 1}", input));
            }

            return Execute(rule, ruleParams.ToArray());
        }

        /// <summary>
        /// This will execute all the rules of the specified workflow
        /// </summary>
        /// <param name="expression">The name of the workflow with rules to execute against the inputs</param>
        /// <param name="ruleParams">A variable number of rule parameters</param>
        /// <returns>List of rule results</returns>
        public RuleResultTree Execute(string expression,
            params RuleParameter[] ruleParams)
        {
            var sortedRuleParams = ruleParams.ToList();
            sortedRuleParams.Sort((RuleParameter a, RuleParameter b) => string.Compare(a.Name, b.Name));
            var rule = new Rule
            {
                Expression = expression
            };
            var ruleResultList = executeRule(rule, sortedRuleParams.ToArray());
            return ruleResultList;
        }

        public RuleResultTree Execute(Rule rule,
            params RuleParameter[] ruleParams)
        {
            var sortedRuleParams = ruleParams.ToList();
            sortedRuleParams.Sort((RuleParameter a, RuleParameter b) => string.Compare(a.Name, b.Name));
            var ruleResultList = executeRule(rule, sortedRuleParams.ToArray());
            return ruleResultList;
        }

        #endregion

        #region Private Methods

        private RuleResultTree executeRule(Rule rule, RuleParameter[] ruleParams)
        {
            var compiledRule = getOrRegisterRule(rule, ruleParams);
            var result = compiledRule(ruleParams);
            return result;
        }

        /// <summary>
        /// This will compile the rules and store them to dictionary
        /// </summary>
        /// <param name="rule">workflow name</param>
        /// <param name="ruleParams">The rule parameters.</param>
        /// <returns>
        /// bool result
        /// </returns>
        private RuleFunc<RuleResultTree> getOrRegisterRule(Rule rule, params RuleParameter[] ruleParams)
        {
            if (rule == null || string.IsNullOrEmpty(rule.Expression))
            {
                throw new ArgumentNullException(nameof(rule.Expression));
            }

            var compileRulesKey = GetCompiledRulesKey(rule.Expression, ruleParams);
            if (string.IsNullOrEmpty(rule.RuleName))
            {
                rule.RuleName = compileRulesKey;
            }

            var ruleFunc = _cache.GetOrAdd(compileRulesKey, key =>
            {
                if (_reSettings.AutoRegisterInputType)
                {
                    _reSettings.CustomTypes =
                        _reSettings.CustomTypes.Safe().Union(ruleParams.Select(c => c.Type)).ToArray();
                }

                var globalParamExp = new Lazy<RuleExpressionParameter[]>(
                    () => _ruleCompiler.GetRuleExpressionParameters(RuleExpressionType.LambdaExpression,
                        _reSettings.GlobalParams,
                        ruleParams)
                );

                return CompileRule(rule, RuleExpressionType.LambdaExpression, ruleParams, globalParamExp);
            });
            return ruleFunc;
        }

        private RuleFunc<RuleResultTree> CompileRule(Rule rule, RuleExpressionType ruleExpressionType,
            RuleParameter[] ruleParams, Lazy<RuleExpressionParameter[]> scopedParams)
        {
            return _ruleCompiler.CompileRule(rule, ruleExpressionType, ruleParams, scopedParams);
        }


        private string GetCompiledRulesKey(string workflowName, RuleParameter[] ruleParams)
        {
            var ruleParamsKey = string.Join("-", ruleParams.Select(c => $"{c.Name}_{c.Type.Name}"));
            var key = $"{workflowName}-" + ruleParamsKey;
            return key.CalculateMD5Hash();
        }

        #endregion
    }
}