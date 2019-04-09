using System;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Microsoft.Extensions.DependencyInjection;

namespace CourseSchedulingSystem.Commands
{
    public class AppCommandsAttributeConvention : IConvention
    {
        private static Type[] _types;

        public AppCommandsAttributeConvention()
        {
            _types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => type.IsClass && type.Namespace == typeof(ICommand).Namespace)
                .Where(type => type.GetInterfaces().Contains(typeof(ICommand)))
                .ToArray();
        }

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

        private static readonly MethodInfo AddSubCommandMethod
            = typeof(AppCommandsAttributeConvention).GetRuntimeMethods()
                .Single(m => m.Name == nameof(AddSubCommandImpl));

        private void AddSubCommandImpl<TSubCommand>(ConventionContext context)
            where TSubCommand : class
        {
            context.Application.Command<TSubCommand>(null, application => { });
        }
    }
}