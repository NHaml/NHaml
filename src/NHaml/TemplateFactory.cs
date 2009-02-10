using System;
using System.Reflection.Emit;

namespace NHaml
{
    public class TemplateFactory
    {
        private delegate Template FastActivator();

        private readonly FastActivator _fastActivator;

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

        private static FastActivator CreateFastActivator( Type type )
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

            return (FastActivator)dynamicMethod.CreateDelegate( typeof( FastActivator ) );
        }
    }
}