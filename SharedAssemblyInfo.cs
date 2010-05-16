#if !NOT_CLSCOMPLIANT
using System;
#endif
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

[assembly: AssemblyProduct("NHaml")]
[assembly: AssemblyCopyright("MIT License")]
[assembly: ComVisible(true)]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.0.0")]
[assembly: NeutralResourcesLanguage("")]
[assembly: AllowPartiallyTrustedCallers]
#if !NET4
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#endif
#if !NOT_CLSCOMPLIANT
[assembly: CLSCompliant(true)]
#endif
