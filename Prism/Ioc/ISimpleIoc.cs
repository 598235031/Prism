using System;
using System.Diagnostics.CodeAnalysis;
#if !NETSTANDARD1_0
#if NEWLOCATOR
using CommonServiceLocator;
#else
using Microsoft.Practices.ServiceLocation;
#endif
#endif
namespace Wanghzh.Prism.Ioc
{
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Ioc")]
    public interface ISimpleIoc
#if !NETSTANDARD1_0
        : IServiceLocator
#endif
    {
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        bool ContainsCreated<TClass>();
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        bool ContainsCreated<TClass>(string key);
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        bool IsRegistered<T>();
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        bool IsRegistered<T>(string key);
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        void Register<TInterface, TClass>()
            where TInterface : class
            where TClass : class, TInterface;
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        void Register<TInterface, TClass>(bool createInstanceImmediately)
            where TInterface : class
            where TClass : class, TInterface;




        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        void Register<TClass>()
            where TClass : class;
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        void Register<TClass>(bool createInstanceImmediately)
            where TClass : class;
        void Register<TClass>(Func<TClass> factory)
            where TClass : class;
        void Register<TClass>(Func<TClass> factory, bool createInstanceImmediately)
            where TClass : class;
        void Register<TClass>(Func<TClass> factory, string key)
            where TClass : class;
        void Register<TClass>(
            Func<TClass> factory,
            string key,
            bool createInstanceImmediately)
            where TClass : class;

        void Register<TInterface>(TInterface instance, string key = "") where TInterface : class;

        void Reset();
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        void Unregister<TClass>()
            where TClass : class;
        void Unregister<TClass>(TClass instance)
            where TClass : class;
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        void Unregister<TClass>(string key)
            where TClass : class;
    }
}
