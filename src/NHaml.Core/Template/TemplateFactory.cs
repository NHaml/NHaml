using System;
using System.Reflection.Emit;

namespace NHaml.Core.Template
{

    public delegate T Func<T>();

    public delegate TResult Func<T, TResult>(T arg);

    public class TemplateFactory
    {

        private readonly Func<Template> _fastActivator;

        protected TemplateFactory()
        {
        }

        public TemplateFactory( Type templateType )
        {
            _fastActivator = CreateFastActivator( templateType );
        }

        public virtual Template CreateTemplate()
        {
            return _fastActivator();
        }

        private static Func<Template> CreateFastActivator(Type type)
        {
            var dynamicMethod = new DynamicMethod( "activatefast__", type, null, type );

            var ilGenerator = dynamicMethod.GetILGenerator();
            var constructor = type.GetConstructor( new Type[] { } );

            if( constructor == null )
            {
                return null;
            }

            ilGenerator.Emit( OpCodes.Newobj, constructor );
            ilGenerator.Emit( OpCodes.Ret );

            return (Func<Template>)dynamicMethod.CreateDelegate(typeof(Func<Template>));
        }
    }
}