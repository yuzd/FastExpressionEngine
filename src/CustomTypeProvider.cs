﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using FastExpressionEngine.HelperFunctions;

namespace FastExpressionEngine
{
    public class CustomTypeProvider : DefaultDynamicLinqCustomTypeProvider
    {
        private HashSet<Type> _types;

        public CustomTypeProvider(Type[] types) : base()
        {
            _types = new HashSet<Type>(types ?? new Type[] { });
            _types.Add(typeof(ExpressionUtils));
        }

        public override HashSet<Type> GetCustomTypes()
        {
            return _types;
        }
    }
}