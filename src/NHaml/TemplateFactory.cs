using System;
using System.Reflection.Emit;
using NHaml.TemplateBase;

namespace NHaml
{
    [Serializable]
    public class InvalidTemplateTypeException : Exception
    {
        public InvalidTemplateTypeException(Type t)
            : base("Attempted to create a template factory using an invalid type " + t.FullName)
        { }
    }

    public class TemplateFactory
    {
        private readonly Func<Template> _fastActivator;

        public TemplateFactory( Type templateType )
        {
            _fastActivator = CreateFastActivator( templateType );
        }

        public Template CreateTemplate()
        {
            return _fastActivator();
        }

        private static Func<Template> CreateFastActivator(Type type)
        {
            var constructor = type.GetConstructor( new Type[] { } );
            if (constructor == null)
                throw new InvalidTemplateTypeException(type);

            var dynamicMethod = new DynamicMethod( "activatefast__", type, null, type );
            var ilGenerator = dynamicMethod.GetILGenerator();

            ilGenerator.Emit( OpCodes.Newobj, constructor );
            ilGenerator.Emit( OpCodes.Ret );

            return (Func<Template>)dynamicMethod.CreateDelegate(typeof(Func<Template>));
        }

        public object CreateTemplate<T1>()
        {
            throw new NotImplementedException();
        }
    }
}