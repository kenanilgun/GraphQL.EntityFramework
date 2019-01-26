﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

static class PropertyAccessorBuilder<T>
{
    static ConcurrentDictionary<string, PropertyAccessor> funcs = new ConcurrentDictionary<string, PropertyAccessor>();

    public static PropertyAccessor GetPropertyFunc(string propertyPath)
    {
        return funcs.GetOrAdd(propertyPath, x =>
        {
            var parameter = Expression.Parameter(typeof(T));
            var aggregatePath = AggregatePath(x, parameter);
            return new PropertyAccessor
            {
                SourceParameter = parameter,
                Left = aggregatePath,
                Type = aggregatePath.Type
            };
        });
    }

    public static Expression AggregatePath(string path, Expression parameter)
    {
        try
        {
            return path.Split('.')
                .Aggregate(parameter, Expression.PropertyOrField);
        }
        catch (ArgumentException exception)
        {
            throw new Exception($"Failed to create a member expression. Type: {typeof(T).FullName}, Path: {path}. Error: {exception.Message}");
        }
    }
}