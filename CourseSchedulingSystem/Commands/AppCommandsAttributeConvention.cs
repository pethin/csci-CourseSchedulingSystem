using System;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Microsoft.Extensions.DependencyInjection;

namespace CourseSchedulingSystem.Commands
{
    /// <summary>
    /// CommandLineUtils Convention to automatically add commands that implement the
    /// ICommand interface in the CourseSchedulingSystem.Commands namespace.
    /// </summary>
    public class AppCommandsAttributeConvention : IConvention
    {
        private static Type[] _types;

        public AppCommandsAttributeConvention()
        {
            // Get all Types of the classes in CourseSchedulingSystem.Commands namespace
            // that implement ICommand
            _types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.IsClass && type.Namespace == typeof(ICommand).Namespace)
                .Where(type => type.GetInterfaces().Contains(typeof(ICommand)))
                .ToArray();
        }

        /// <summary>
        /// Applies this convention.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="Exception"></exception>
        public void Apply(ConventionContext context)
        {
            if (context.ModelType == null)
            {
                return;
            }

            if (context.ModelType.Namespace == typeof(ICommand).Namespace)
            {
                return;
            }

            foreach (var type in _types)
            {
                var impl = AddSubCommandMethod.MakeGenericMethod(type);
                var contextArgs = new object[] {context};

                try
                {
                    impl.Invoke(this, contextArgs);
                }
                catch (TargetInvocationException ex)
                {
                    // unwrap
                    throw ex.InnerException ?? ex;
                }
            }
        }

        /// <summary>
        /// MethodInfo of the AddSubCommandImpl method.
        /// </summary>
        private static readonly MethodInfo AddSubCommandMethod
            = typeof(AppCommandsAttributeConvention).GetRuntimeMethods()
                .Single(m => m.Name == nameof(AddSubCommandImpl));

        /// <summary>
        /// Adds the sub command of type TSubCommand to the application.
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="TSubCommand"></typeparam>
        private void AddSubCommandImpl<TSubCommand>(ConventionContext context)
            where TSubCommand : class
        {
            context.Application.Command<TSubCommand>(null, application => { });
        }
    }
}