//-----------------------------------------------------------------------
// <copyright file="EngineSetting .cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <create>$Date$</create>
// <summary></summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FastExpressionEngine.Models;

namespace FastExpressionEngine
{
    [ExcludeFromCodeCoverage]
    public class EngineSetting
    {
        public EngineSetting()
        {
        }

        // create a copy of settings
        internal EngineSetting(EngineSetting reSettings)
        {
            CustomTypes = reSettings.CustomTypes;
            EnableScopedParams = reSettings.EnableScopedParams;
            AutoRegisterInputType = reSettings.AutoRegisterInputType;
        }

        /// <summary>
        /// Gets or Sets the global params which will be applicable to all rules
        /// </summary>
        public IEnumerable<ScopedParam> GlobalParams { get; set; }


        /// <summary>
        /// Get/Set the custom types to be used in Rule expressions
        /// </summary>
        public Type[] CustomTypes { get; set; }


        /// <summary>
        /// Enables Global params and local params for rules
        /// </summary>
        public bool EnableScopedParams { get; set; } = true;


        /// <summary>
        /// Auto Registers input type in Custom Type to allow calling method on type.
        /// Default : true
        /// </summary>
        public bool AutoRegisterInputType { get; set; } = true;

        /// <summary>
        /// Sets the mode for Nested rule execution, Default: All
        /// </summary>
        public int CacheSize { get; set; } = 1000;
    }
}