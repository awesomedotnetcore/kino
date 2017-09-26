﻿#if NET47
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace kino.Core.Diagnostics.Performance
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Field)]
    public class PerformanceCounterDefinitionAttribute : Attribute
    {
        public PerformanceCounterDefinitionAttribute(string counterName, PerformanceCounterType type)
        {
            if (string.IsNullOrWhiteSpace(counterName))
            {
                throw new ArgumentNullException(nameof(counterName));
            }

            Name = counterName;
            Type = type;
        }

        public string Name { get; }

        public string Description { get; set; }

        public PerformanceCounterType Type { get; }
    }
}
#endif