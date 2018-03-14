using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace TT.Domain.Extensions
{
    public static class AutoMapperExtensions
    {
        public static Empty EmptyObject { get; } = new Empty();

        public class Empty
        {
        }

        public static IMappingExpression<TSource, TDestination> ResolveForTopLevel<TSource, TDestination, TResult>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Func<TSource, TResult> sourceMember)
        {
            return mappingExpression.ConstructUsing((compositeDetail, context) => {
                return context.Mapper.Map<TDestination>(sourceMember(compositeDetail));
            });
        }

        // add more overloads
        public static IMappingExpression<TSource, TDestination> ResolveForMember<TSource, TDestination, TResult, TDestinationMember>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, TDestinationMember>> destinationMember,
            Func<TSource, TResult> sourceMember)
            => ResolveForMember(mappingExpression, destinationMember, ((w, x, y, z) => sourceMember(w)));

        public static IMappingExpression<TSource, TDestination> ResolveForMember<TSource, TDestination, TResult, TDestinationMember>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, TDestinationMember>> destinationMember,
            Func<TSource, TDestination, TDestinationMember, ResolutionContext, TResult> sourceMember)
        {
            return mappingExpression.ForMember(destinationMember, src => src.ResolveUsing(sourceMember));
        }

        public static IMappingExpression<TSource, TDestination> MapForMember<TSource, TDestination, TResult, TDestinationMember>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, TDestinationMember>> destinationMember,
            Expression<Func<TSource, TResult>> sourceMember)
        {
            return mappingExpression.ForMember(destinationMember, src => src.MapFrom(sourceMember));
        }

        public static IMappingExpression<TSource, TDestination> MapForMember<TSource, TDestination, TResult, TDestinationMember>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, TDestinationMember>> destinationMember,
            string sourceMember)
        {
            return mappingExpression.ForMember(destinationMember, src => src.MapFrom(sourceMember));
        }

        public static IMappingExpression<TSource, TDestination> MapForPath<TSource, TDestination, TResult, TDestinationMember>(
            this IMappingExpression<TSource, TDestination> mappingExpression,
            Expression<Func<TDestination, TDestinationMember>> destinationMember,
            Expression<Func<TSource, TResult>> sourceMember)
        {
            if (destinationMember.Body is MemberExpression)
            {
                mappingExpression.ForPath(destinationMember, src => src.MapFrom(sourceMember));
            }

            return mappingExpression;
        }

        public static TSource Set<TSource>(this TSource input, Action<TSource> setterAction)
        {
            setterAction(input);
            return input;
        }
    }
}
