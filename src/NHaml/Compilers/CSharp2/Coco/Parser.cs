using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NHaml.Compilers.CSharp2.Coco
{
    [GeneratedCode( "Coco", "1.0.0.0" )]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    internal class Parser
    {
        private const int _EOF = 0;
        private const int _ident = 1;
        private const int _intCon = 2;
        private const int _realCon = 3;
        private const int _charCon = 4;
        private const int _stringCon = 5;
        private const int _abstract = 6;
        private const int _as = 7;
        private const int _base = 8;
        private const int _bool = 9;
        private const int _break = 10;
        private const int _byte = 11;
        private const int _case = 12;
        private const int _catch = 13;
        private const int _char = 14;
        private const int _checked = 15;
        private const int _class = 16;
        private const int _const = 17;
        private const int _continue = 18;
        private const int _decimal = 19;
        private const int _default = 20;
        private const int _delegate = 21;
        private const int _do = 22;
        private const int _double = 23;
        private const int _else = 24;
        private const int _enum = 25;
        private const int _event = 26;
        private const int _explicit = 27;
        private const int _extern = 28;
        private const int _false = 29;
        private const int _finally = 30;
        private const int _fixed = 31;
        private const int _float = 32;
        private const int _for = 33;
        private const int _foreach = 34;
        private const int _goto = 35;
        private const int _if = 36;
        private const int _implicit = 37;
        private const int _in = 38;
        private const int _int = 39;
        private const int _interface = 40;
        private const int _internal = 41;
        private const int _is = 42;
        private const int _lock = 43;
        private const int _long = 44;
        private const int _namespace = 45;
        private const int _new = 46;
        private const int _null = 47;
        private const int _object = 48;
        private const int _operator = 49;
        private const int _out = 50;
        private const int _override = 51;
        private const int _params = 52;
        private const int _private = 53;
        private const int _protected = 54;
        private const int _public = 55;
        private const int _readonly = 56;
        private const int _ref = 57;
        private const int _return = 58;
        private const int _sbyte = 59;
        private const int _sealed = 60;
        private const int _short = 61;
        private const int _sizeof = 62;
        private const int _stackalloc = 63;
        private const int _static = 64;
        private const int _string = 65;
        private const int _struct = 66;
        private const int _switch = 67;
        private const int _this = 68;
        private const int _throw = 69;
        private const int _true = 70;
        private const int _try = 71;
        private const int _typeof = 72;
        private const int _uint = 73;
        private const int _ulong = 74;
        private const int _unchecked = 75;
        private const int _unsafe = 76;
        private const int _ushort = 77;
        private const int _usingKW = 78;
        private const int _virtual = 79;
        private const int _void = 80;
        private const int _volatile = 81;
        private const int _while = 82;
        private const int _and = 83;
        private const int _andassgn = 84;
        private const int _assgn = 85;
        private const int _colon = 86;
        private const int _comma = 87;
        private const int _dec = 88;
        private const int _divassgn = 89;
        private const int _dot = 90;
        private const int _dblcolon = 91;
        private const int _eq = 92;
        private const int _gt = 93;
        private const int _gteq = 94;
        private const int _inc = 95;
        private const int _lbrace = 96;
        private const int _lbrack = 97;
        private const int _lpar = 98;
        private const int _lshassgn = 99;
        private const int _lt = 100;
        private const int _ltlt = 101;
        private const int _minus = 102;
        private const int _minusassgn = 103;
        private const int _modassgn = 104;
        private const int _neq = 105;
        private const int _not = 106;
        private const int _orassgn = 107;
        private const int _plus = 108;
        private const int _plusassgn = 109;
        private const int _question = 110;
        private const int _rbrace = 111;
        private const int _rbrack = 112;
        private const int _rpar = 113;
        private const int _scolon = 114;
        private const int _tilde = 115;
        private const int _times = 116;
        private const int _timesassgn = 117;
        private const int _xorassgn = 118;
        private const int maxT = 130;

        private const bool T = true;
        private const bool x = false;
        private const int minErrDist = 2;

        public Scanner scanner;
        public Errors errors;

        public IList<DictionaryEntry> variables = new List<DictionaryEntry>();

        public Token t; // last recognized token
        public Token la; // lookahead token
        private int errDist = minErrDist;

        private const int maxTerminals = 160; // set size

        private static BitArray NewSet( params int[] values )
        {
            var a = new BitArray( maxTerminals );
            foreach( var x in values )
            {
                a[x] = true;
            }
            return a;
        }

        private static BitArray
          unaryOp = NewSet( _plus, _minus, _not, _tilde, _inc, _dec, _true, _false ) /* rshassgn: ">" ">="  no whitespace allowed*/
          ;

        private static readonly BitArray
          typeKW = NewSet( _char, _bool, _object, _string, _sbyte, _byte, _short,
            _ushort, _int, _uint, _long, _ulong, _float, _double, _decimal ) /* rshassgn: ">" ">="  no whitespace allowed*/
          ;

        private static readonly BitArray
          unaryHead = NewSet( _plus, _minus, _not, _tilde, _times, _inc, _dec, _and ) /* rshassgn: ">" ">="  no whitespace allowed*/
          ;

        private static readonly BitArray
          assnStartOp = NewSet( _plus, _minus, _not, _tilde, _times ) /* rshassgn: ">" ">="  no whitespace allowed*/
          ;

        private static readonly BitArray
          castFollower = NewSet( _tilde, _not, _lpar, _ident,
            /* literals */
            _intCon, _realCon, _charCon, _stringCon,
            /* any keyword expect as and is */
            _abstract, _base, _bool, _break, _byte, _case, _catch,
            _char, _checked, _class, _const, _continue, _decimal, _default,
            _delegate, _do, _double, _else, _enum, _event, _explicit,
            _extern, _false, _finally, _fixed, _float, _for, _foreach,
            _goto, _if, _implicit, _in, _int, _interface, _internal,
            _lock, _long, _namespace, _new, _null, _object, _operator,
            _out, _override, _params, _private, _protected, _public,
            _readonly, _ref, _return, _sbyte, _sealed, _short, _sizeof,
            _stackalloc, _static, _string, _struct, _switch, _this, _throw,
            _true, _try, _typeof, _uint, _ulong, _unchecked, _unsafe,
            _ushort, _usingKW, _virtual, _void, _volatile, _while
            ) /* rshassgn: ">" ">="  no whitespace allowed*/
          ;

        private static readonly BitArray
          typArgLstFol = NewSet( _lpar, _rpar, _rbrack, _colon, _scolon, _comma, _dot,
            _question, _eq, _neq ) /* rshassgn: ">" ">="  no whitespace allowed*/
          ;

        private static readonly BitArray
          keyword = NewSet( _abstract, _as, _base, _bool, _break, _byte, _case, _catch,
            _char, _checked, _class, _const, _continue, _decimal, _default,
            _delegate, _do, _double, _else, _enum, _event, _explicit,
            _extern, _false, _finally, _fixed, _float, _for, _foreach,
            _goto, _if, _implicit, _in, _int, _interface, _internal,
            _is, _lock, _long, _namespace, _new, _null, _object, _operator,
            _out, _override, _params, _private, _protected, _public,
            _readonly, _ref, _return, _sbyte, _sealed, _short, _sizeof,
            _stackalloc, _static, _string, _struct, _switch, _this, _throw,
            _true, _try, _typeof, _uint, _ulong, _unchecked, _unsafe,
            _ushort, _usingKW, _virtual, _void, _volatile, _while ) /* rshassgn: ">" ">="  no whitespace allowed*/
          ;

        private static readonly BitArray
          assgnOps = NewSet( _assgn, _plusassgn, _minusassgn, _timesassgn, _divassgn,
            _modassgn, _andassgn, _orassgn, _xorassgn, _lshassgn ) /* rshassgn: ">" ">="  no whitespace allowed*/
          ;

        /*---------------------------- auxiliary methods ------------------------*/

        private void Error( string s )
        {
            if( errDist >= minErrDist )
            {
                errors.SemErr( la.line, la.col, s );
            }
            errDist = 0;
        }

        // Return the n-th token after the current lookahead token
        private Token Peek( int n )
        {
            scanner.ResetPeek();
            var x = la;
            while( n > 0 )
            {
                x = scanner.Peek();
                n--;
            }
            return x;
        }

        // ident "="
        private bool IsAssignment()
        {
            return la.kind == _ident && Peek( 1 ).kind == _assgn;
        }

        /* True, if the comma is not a trailing one, *
         * like the last one in: a, b, c,            */

        private bool NotFinalComma()
        {
            var peek = Peek( 1 ).kind;
            return la.kind == _comma && peek != _rbrace && peek != _rbrack;
        }

        /* Checks whether the next sequence of tokens is a qualident *
         * and returns the qualident string                          *
         * !!! Proceeds from current peek position !!!               */

        private bool IsQualident( ref Token pt, out string qualident )
        {
            qualident = "";
            if( pt.kind == _ident )
            {
                qualident = pt.val;
                pt = scanner.Peek();
                while( pt.kind == _dot )
                {
                    pt = scanner.Peek();
                    if( pt.kind != _ident )
                    {
                        return false;
                    }
                    qualident += "." + pt.val;
                    pt = scanner.Peek();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsGeneric()
        {
            scanner.ResetPeek();
            var pt = la;
            if( !IsTypeArgumentList( ref pt ) )
            {
                return false;
            }
            return typArgLstFol[pt.kind];
        }

        private bool IsTypeArgumentList( ref Token pt )
        {
            if( pt.kind == _lt )
            {
                pt = scanner.Peek();
                while( true )
                {
                    if( !IsType( ref pt ) )
                    {
                        return false;
                    }
                    if( pt.kind == _gt )
                    {
                        // list recognized
                        pt = scanner.Peek();
                        break;
                    }
                    else if( pt.kind == _comma )
                    {
                        // another argument
                        pt = scanner.Peek();
                    }
                    else
                    {
                        // error in type argument list
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        // Type
        private bool IsType( ref Token pt )
        {
            String dummyId;

            if( typeKW[pt.kind] )
            {
                pt = scanner.Peek();
            }
            else if( pt.kind == _void )
            {
                pt = scanner.Peek();
                if( pt.kind != _times )
                {
                    return false;
                }
                pt = scanner.Peek();
            }
            else if( pt.kind == _ident )
            {
                pt = scanner.Peek();
                if( pt.kind == _dblcolon || pt.kind == _dot )
                {
                    // either namespace alias qualifier "::" or first
                    // part of the qualident
                    pt = scanner.Peek();
                    if( !IsQualident( ref pt, out dummyId ) )
                    {
                        return false;
                    }
                }
                if( pt.kind == _lt && !IsTypeArgumentList( ref pt ) )
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            if( pt.kind == _question )
            {
                pt = scanner.Peek();
            }
            return SkipPointerOrDims( ref pt );
        }

        // Type ident
        // (Type can be void*)
        private bool IsLocalVarDecl()
        {
            var pt = la;
            scanner.ResetPeek();
            return IsType( ref pt ) && pt.kind == _ident;
        }

        // "[" ("," | "]")
        private bool IsDims()
        {
            var peek = Peek( 1 ).kind;
            return la.kind == _lbrack && (peek == _comma || peek == _rbrack);
        }

        // "*" | "[" ("," | "]")
        private bool IsPointerOrDims()
        {
            return la.kind == _times || IsDims();
        }

        /* skip: { "[" { "," } "]" | "*" }             */
        /* !!! Proceeds from current peek position !!! */

        private bool SkipPointerOrDims( ref Token pt )
        {
            for( ; ; )
            {
                if( pt.kind == _lbrack )
                {
                    do
                        pt = scanner.Peek();
                    while( pt.kind == _comma );
                    if( pt.kind != _rbrack )
                    {
                        return false;
                    }
                }
                else if( pt.kind != _times )
                {
                    break;
                }
                pt = scanner.Peek();
            }
            return true;
        }

        // Is attribute target specifier
        // (ident | keyword) ":"
        private bool IsAttrTargSpec()
        {
            return (la.kind == _ident || keyword[la.kind]) && Peek( 1 ).kind == _colon;
        }

        // ident ("," | "=" | ";")
        private bool IsFieldDecl()
        {
            var peek = Peek( 1 ).kind;
            return la.kind == _ident &&
              (peek == _comma || peek == _assgn || peek == _scolon);
        }

        private bool IsTypeCast()
        {
            if( la.kind != _lpar )
            {
                return false;
            }
            if( IsSimpleTypeCast() )
            {
                return true;
            }
            return GuessTypeCast();
        }

        // "(" typeKW ")"
        private bool IsSimpleTypeCast()
        {
            // assert: la.kind == _lpar
            scanner.ResetPeek();
            var pt1 = scanner.Peek();
            var pt2 = scanner.Peek();
            return typeKW[pt1.kind] &&
              (pt2.kind == _rpar ||
                (pt2.kind == _question && scanner.Peek().kind == _rpar));
        }

        // "(" Type ")" castFollower
        private bool GuessTypeCast()
        {
            // assert: la.kind == _lpar
            scanner.ResetPeek();
            var pt = scanner.Peek();
            if( !IsType( ref pt ) )
            {
                return false;
            }
            if( pt.kind != _rpar )
            {
                return false;
            }
            pt = scanner.Peek();
            return castFollower[pt.kind];
        }

        // "[" "assembly"
        private bool IsGlobalAttrTarget()
        {
            var pt = Peek( 1 );
            return la.kind == _lbrack && pt.kind == _ident && ("assembly".Equals( pt.val ) || "module".Equals( pt.val ));
        }

        // "extern" "alias"
        // where alias is an identifier, no keyword
        private bool IsExternAliasDirective()
        {
            return la.kind == _extern && "alias".Equals( Peek( 1 ).val );
        }

        // true: anyToken"<"
        // no whitespace between the token and the "<" allowed
        // anything else will return false.
        private bool IsLtNoWs()
        {
            return (la.kind == _lt) && ((t.pos + t.val.Length) == la.pos);
        }

        private bool IsNoSwitchLabelOrRBrace()
        {
            return (la.kind != _case && la.kind != _default && la.kind != _rbrace) ||
              (la.kind == _default && Peek( 1 ).kind != _colon);
        }

        private bool IsShift()
        {
            var pt = Peek( 1 );
            return (la.kind == _ltlt) ||
              (la.kind == _gt &&
                pt.kind == _gt &&
                  (la.pos + la.val.Length == pt.pos)
                );
        }

        // true: TypeArgumentList followed by anything but "("
        private bool IsPartOfMemberName()
        {
            scanner.ResetPeek();
            var pt = la;
            if( !IsTypeArgumentList( ref pt ) )
            {
                return false;
            }
            return pt.kind != _lpar;
        }

        private enum TypeKind
        {
            simple,
            array,
            pointer,
            @void
        }

        [Flags]
        private enum Operator
        {
            plus = 0x00000001,
            minus = 0x00000002,
            not = 0x00000004,
            tilde = 0x00000008,
            inc = 0x00000010,
            dec = 0x00000020,
            @true = 0x00000040,
            @false = 0x00000080,
            times = 0x00000100,
            div = 0x00000200,
            mod = 0x00000400,
            and = 0x00000800,
            or = 0x00001000,
            xor = 0x00002000,
            lshift = 0x00004000,
            rshift = 0x00008000,
            eq = 0x00010000,
            neq = 0x00020000,
            gt = 0x00040000,
            lt = 0x00080000,
            gte = 0x00100000,
            lte = 0x00200000,
            unary = plus | minus | not | tilde | inc | dec | @true | @false,
            binary = plus | minus | times | div | mod | and | or | xor | lshift | rshift | eq | neq | gt | lt | gte | lte
        }

        /*------------------------------------------------------------------------*
         *----- SCANNER DESCRIPTION ----------------------------------------------*
         *------------------------------------------------------------------------*/

        public Parser( Scanner scanner )
        {
            this.scanner = scanner;
            errors = new Errors();
        }

        private void SynErr( int n )
        {
            if( errDist >= minErrDist )
            {
                errors.SynErr( la.line, la.col, n );
            }
            errDist = 0;
        }

        public void SemErr( string msg )
        {
            if( errDist >= minErrDist )
            {
                errors.SemErr( t.line, t.col, msg );
            }
            errDist = 0;
        }

        private void Get()
        {
            for( ; ; )
            {
                t = la;
                la = scanner.Scan();
                if( la.kind <= maxT )
                {
                    ++errDist;
                    break;
                }

                la = t;
            }
        }

        private void Expect( int n )
        {
            if( la.kind == n )
            {
                Get();
            }
            else
            {
                SynErr( n );
            }
        }

        private bool StartOf( int s )
        {
            return set[s, la.kind];
        }

        private void CS2()
        {
            while( IsExternAliasDirective() )
            {
                ExternAliasDirective();
            }
            while( la.kind == 78 )
            {
                UsingDirective();
            }
            while( IsGlobalAttrTarget() )
            {
                GlobalAttributes();
            }
            while( StartOf( 1 ) )
            {
                NamespaceMemberDeclaration();
            }
        }

        private void ExternAliasDirective()
        {
            Expect( 28 );
            Expect( 1 );
            if( t.val != "alias" )
            {
                Error( "alias expected" );
            }

            Expect( 1 );
            Expect( 114 );
        }

        private void UsingDirective()
        {
            Expect( 78 );
            if( IsAssignment() )
            {
                Expect( 1 );
                Expect( 85 );
            }
            TypeName();
            Expect( 114 );
        }

        private void GlobalAttributes()
        {
            Expect( 97 );
            Expect( 1 );
            if( !"assembly".Equals( t.val ) && !"module".Equals( t.val ) )
            {
                Error( "global attribute target specifier \"assembly\" or \"module\" expected" );
            }

            Expect( 86 );
            Attribute();
            while( NotFinalComma() )
            {
                Expect( 87 );
                Attribute();
            }
            if( la.kind == 87 )
            {
                Get();
            }
            Expect( 112 );
        }

        private void NamespaceMemberDeclaration()
        {
            if( la.kind == 45 )
            {
                Get();
                Expect( 1 );
                while( la.kind == 90 )
                {
                    Get();
                    Expect( 1 );
                }
                Expect( 96 );
                while( IsExternAliasDirective() )
                {
                    ExternAliasDirective();
                }
                while( la.kind == 78 )
                {
                    UsingDirective();
                }
                while( StartOf( 1 ) )
                {
                    NamespaceMemberDeclaration();
                }
                Expect( 111 );
                if( la.kind == 114 )
                {
                    Get();
                }
            }
            else if( StartOf( 2 ) )
            {
                while( la.kind == 97 )
                {
                    Attributes();
                }
                ModifierList();
                TypeDeclaration();
            }
            else
            {
                SynErr( 131 );
            }
        }

        private void TypeName()
        {
            Expect( 1 );
            if( la.kind == 91 )
            {
                Get();
                Expect( 1 );
            }
            if( la.kind == 100 )
            {
                TypeArgumentList();
            }
            while( la.kind == 90 )
            {
                Get();
                Expect( 1 );
                if( la.kind == 100 )
                {
                    TypeArgumentList();
                }
            }
        }

        private void Attributes()
        {
            Expect( 97 );
            if( IsAttrTargSpec() )
            {
                if( la.kind == 1 )
                {
                    Get();
                }
                else if( StartOf( 3 ) )
                {
                    Keyword();
                }
                else
                {
                    SynErr( 132 );
                }
                Expect( 86 );
            }
            Attribute();
            while( la.kind == _comma && Peek( 1 ).kind != _rbrack )
            {
                Expect( 87 );
                Attribute();
            }
            if( la.kind == 87 )
            {
                Get();
            }
            Expect( 112 );
        }

        private void ModifierList()
        {
            while( StartOf( 4 ) )
            {
                switch( la.kind )
                {
                    case 46:
                    {
                        Get();
                        break;
                    }
                    case 55:
                    {
                        Get();
                        break;
                    }
                    case 54:
                    {
                        Get();
                        break;
                    }
                    case 41:
                    {
                        Get();
                        break;
                    }
                    case 53:
                    {
                        Get();
                        break;
                    }
                    case 76:
                    {
                        Get();
                        break;
                    }
                    case 64:
                    {
                        Get();
                        break;
                    }
                    case 56:
                    {
                        Get();
                        break;
                    }
                    case 81:
                    {
                        Get();
                        break;
                    }
                    case 79:
                    {
                        Get();
                        break;
                    }
                    case 60:
                    {
                        Get();
                        break;
                    }
                    case 51:
                    {
                        Get();
                        break;
                    }
                    case 6:
                    {
                        Get();
                        break;
                    }
                    case 28:
                    {
                        Get();
                        break;
                    }
                }
            }
        }

        private void TypeDeclaration()
        {
            TypeKind dummy;
            if( StartOf( 5 ) )
            {
                if( la.kind == 119 )
                {
                    Get();
                }
                if( la.kind == 16 )
                {
                    Get();
                    Expect( 1 );
                    if( la.kind == 100 )
                    {
                        TypeParameterList();
                    }
                    if( la.kind == 86 )
                    {
                        ClassBase();
                    }
                    while( la.kind == 1 )
                    {
                        TypeParameterConstraintsClause();
                    }
                    ClassBody();
                    if( la.kind == 114 )
                    {
                        Get();
                    }
                }
                else if( la.kind == 66 )
                {
                    Get();
                    Expect( 1 );
                    if( la.kind == 100 )
                    {
                        TypeParameterList();
                    }
                    if( la.kind == 86 )
                    {
                        Get();
                        TypeName();
                        while( la.kind == 87 )
                        {
                            Get();
                            TypeName();
                        }
                    }
                    while( la.kind == 1 )
                    {
                        TypeParameterConstraintsClause();
                    }
                    StructBody();
                    if( la.kind == 114 )
                    {
                        Get();
                    }
                }
                else if( la.kind == 40 )
                {
                    Get();
                    Expect( 1 );
                    if( la.kind == 100 )
                    {
                        TypeParameterList();
                    }
                    if( la.kind == 86 )
                    {
                        Get();
                        TypeName();
                        while( la.kind == 87 )
                        {
                            Get();
                            TypeName();
                        }
                    }
                    while( la.kind == 1 )
                    {
                        TypeParameterConstraintsClause();
                    }
                    Expect( 96 );
                    while( StartOf( 6 ) )
                    {
                        InterfaceMemberDeclaration();
                    }
                    Expect( 111 );
                    if( la.kind == 114 )
                    {
                        Get();
                    }
                }
                else
                {
                    SynErr( 133 );
                }
            }
            else if( la.kind == 25 )
            {
                Get();
                Expect( 1 );
                if( la.kind == 86 )
                {
                    Get();
                    IntegralType();
                }
                EnumBody();
                if( la.kind == 114 )
                {
                    Get();
                }
            }
            else if( la.kind == 21 )
            {
                Get();
                Type( out dummy, true );
                Expect( 1 );
                if( la.kind == 100 )
                {
                    TypeParameterList();
                }
                Expect( 98 );
                if( StartOf( 7 ) )
                {
                    FormalParameterList();
                }
                Expect( 113 );
                while( la.kind == 1 )
                {
                    TypeParameterConstraintsClause();
                }
                Expect( 114 );
            }
            else
            {
                SynErr( 134 );
            }
        }

        private void TypeParameterList()
        {
            Expect( 100 );
            while( la.kind == 97 )
            {
                Attributes();
            }
            Expect( 1 );
            while( la.kind == 87 )
            {
                Get();
                while( la.kind == 97 )
                {
                    Attributes();
                }
                Expect( 1 );
            }
            Expect( 93 );
        }

        private void ClassBase()
        {
            Expect( 86 );
            ClassType();
            while( la.kind == 87 )
            {
                Get();
                TypeName();
            }
        }

        private void TypeParameterConstraintsClause()
        {
            Expect( 1 );
            if( t.val != "where" )
            {
                Error( "type parameter constraints clause must start with: where" );
            }

            Expect( 1 );
            Expect( 86 );
            if( StartOf( 8 ) )
            {
                if( la.kind == 16 )
                {
                    Get();
                }
                else if( la.kind == 66 )
                {
                    Get();
                }
                else if( la.kind == 48 )
                {
                    Get();
                }
                else if( la.kind == 65 )
                {
                    Get();
                }
                else
                {
                    TypeName();
                }
                while( la.kind == _comma && Peek( 1 ).kind != _new )
                {
                    Expect( 87 );
                    TypeName();
                }
                if( la.kind == 87 )
                {
                    Get();
                    Expect( 46 );
                    Expect( 98 );
                    Expect( 113 );
                }
            }
            else if( la.kind == 46 )
            {
                Get();
                Expect( 98 );
                Expect( 113 );
            }
            else
            {
                SynErr( 135 );
            }
        }

        private void ClassBody()
        {
            Expect( 96 );
            while( StartOf( 9 ) )
            {
                while( la.kind == 97 )
                {
                    Attributes();
                }
                ModifierList();
                ClassMemberDeclaration();
            }
            Expect( 111 );
        }

        private void StructBody()
        {
            Expect( 96 );
            while( StartOf( 10 ) )
            {
                while( la.kind == 97 )
                {
                    Attributes();
                }
                ModifierList();
                StructMemberDeclaration();
            }
            Expect( 111 );
        }

        private void InterfaceMemberDeclaration()
        {
            TypeKind dummy;

            while( la.kind == 97 )
            {
                Attributes();
            }
            if( la.kind == 46 )
            {
                Get();
            }
            if( StartOf( 11 ) )
            {
                Type( out dummy, true );
                if( la.kind == 1 )
                {
                    Get();
                    if( la.kind == 98 || la.kind == 100 )
                    {
                        if( la.kind == 100 )
                        {
                            TypeParameterList();
                        }
                        Expect( 98 );
                        if( StartOf( 7 ) )
                        {
                            FormalParameterList();
                        }
                        Expect( 113 );
                        while( la.kind == 1 )
                        {
                            TypeParameterConstraintsClause();
                        }
                        Expect( 114 );
                    }
                    else if( la.kind == 96 )
                    {
                        Get();
                        InterfaceAccessors();
                        Expect( 111 );
                    }
                    else
                    {
                        SynErr( 136 );
                    }
                }
                else if( la.kind == 68 )
                {
                    Get();
                    Expect( 97 );
                    FormalParameterList();
                    Expect( 112 );
                    Expect( 96 );
                    InterfaceAccessors();
                    Expect( 111 );
                }
                else
                {
                    SynErr( 137 );
                }
            }
            else if( la.kind == 26 )
            {
                Get();
                Type( out dummy, false );
                Expect( 1 );
                Expect( 114 );
            }
            else
            {
                SynErr( 138 );
            }
        }

        private void IntegralType()
        {
            switch( la.kind )
            {
                case 59:
                {
                    Get();
                    break;
                }
                case 11:
                {
                    Get();
                    break;
                }
                case 61:
                {
                    Get();
                    break;
                }
                case 77:
                {
                    Get();
                    break;
                }
                case 39:
                {
                    Get();
                    break;
                }
                case 73:
                {
                    Get();
                    break;
                }
                case 44:
                {
                    Get();
                    break;
                }
                case 74:
                {
                    Get();
                    break;
                }
                case 14:
                {
                    Get();
                    break;
                }
                default:
                SynErr( 139 );
                break;
            }
        }

        private void EnumBody()
        {
            Expect( 96 );
            if( la.kind == 1 || la.kind == 97 )
            {
                EnumMemberDeclaration();
                while( NotFinalComma() )
                {
                    Expect( 87 );
                    EnumMemberDeclaration();
                }
                if( la.kind == 87 )
                {
                    Get();
                }
            }
            Expect( 111 );
        }

        private void Type( out TypeKind type, bool voidAllowed )
        {
            type = TypeKind.simple;
            if( StartOf( 12 ) )
            {
                PrimitiveType();
            }
            else if( la.kind == 1 || la.kind == 48 || la.kind == 65 )
            {
                ClassType();
            }
            else if( la.kind == 80 )
            {
                Get();
                type = TypeKind.@void;
            }
            else
            {
                SynErr( 140 );
            }
            if( la.kind == 110 )
            {
                Get();
                if( type == TypeKind.@void )
                {
                    Error( "Unexpected token ?, void must not be nullable." );
                }
            }
            PointerOrArray( ref type );
            if( type == TypeKind.@void && !voidAllowed )
            {
                Error( "type expected, void found, maybe you mean void*" );
            }
        }

        private void FormalParameterList()
        {
            TypeKind type;
            while( la.kind == 97 )
            {
                Attributes();
            }
            if( StartOf( 13 ) )
            {
                if( la.kind == 50 || la.kind == 57 )
                {
                    if( la.kind == 57 )
                    {
                        Get();
                    }
                    else
                    {
                        Get();
                    }
                }
                Type( out type, false );
                Expect( 1 );
                if( la.kind == 87 )
                {
                    Get();
                    FormalParameterList();
                }
            }
            else if( la.kind == 52 )
            {
                Get();
                Type( out type, false );
                if( type != TypeKind.array )
                {
                    Error( "params argument must be an array" );
                }
                Expect( 1 );
            }
            else
            {
                SynErr( 141 );
            }
        }

        private void ClassType()
        {
            if( la.kind == 1 )
            {
                TypeName();
            }
            else if( la.kind == 48 || la.kind == 65 )
            {
                InternalClassType();
            }
            else
            {
                SynErr( 142 );
            }
        }

        private void ClassMemberDeclaration()
        {
            if( StartOf( 14 ) )
            {
                StructMemberDeclaration();
            }
            else if( la.kind == 115 )
            {
                Get();
                Expect( 1 );
                Expect( 98 );
                Expect( 113 );
                if( la.kind == 96 )
                {
                    Block();
                }
                else if( la.kind == 114 )
                {
                    Get();
                }
                else
                {
                    SynErr( 143 );
                }
            }
            else
            {
                SynErr( 144 );
            }
        }

        private void StructMemberDeclaration()
        {
            TypeKind type;
            Operator op;
            if( la.kind == 17 )
            {
                Get();
                Type( out type, false );
                Expect( 1 );
                Expect( 85 );
                Expression();
                while( la.kind == 87 )
                {
                    Get();
                    Expect( 1 );
                    Expect( 85 );
                    Expression();
                }
                Expect( 114 );
            }
            else if( la.kind == 26 )
            {
                Get();
                Type( out type, false );
                if( IsFieldDecl() )
                {
                    VariableDeclarators();
                    Expect( 114 );
                }
                else if( la.kind == 1 )
                {
                    TypeName();
                    Expect( 96 );
                    EventAccessorDeclarations();
                    Expect( 111 );
                }
                else
                {
                    SynErr( 145 );
                }
            }
            else if( la.kind == _ident && Peek( 1 ).kind == _lpar )
            {
                Expect( 1 );
                Expect( 98 );
                if( StartOf( 7 ) )
                {
                    FormalParameterList();
                }
                Expect( 113 );
                if( la.kind == 86 )
                {
                    Get();
                    if( la.kind == 8 )
                    {
                        Get();
                    }
                    else if( la.kind == 68 )
                    {
                        Get();
                    }
                    else
                    {
                        SynErr( 146 );
                    }
                    Expect( 98 );
                    if( StartOf( 15 ) )
                    {
                        Argument();
                        while( la.kind == 87 )
                        {
                            Get();
                            Argument();
                        }
                    }
                    Expect( 113 );
                }
                if( la.kind == 96 )
                {
                    Block();
                }
                else if( la.kind == 114 )
                {
                    Get();
                }
                else
                {
                    SynErr( 147 );
                }
            }
            else if( StartOf( 11 ) )
            {
                Type( out type, true );
                if( la.kind == 49 )
                {
                    if( type == TypeKind.@void )
                    {
                        Error( "operator not allowed on void" );
                    }

                    Get();
                    OverloadableOp( out op );
                    Expect( 98 );
                    Type( out type, false );
                    Expect( 1 );
                    if( la.kind == 87 )
                    {
                        Get();
                        Type( out type, false );
                        Expect( 1 );
                        if( (op & Operator.binary) == 0 )
                        {
                            Error( "too many operands for unary operator" );
                        }
                    }
                    else if( la.kind == 113 )
                    {
                        if( (op & Operator.unary) == 0 )
                        {
                            Error( "too few operands for binary operator" );
                        }
                    }
                    else
                    {
                        SynErr( 148 );
                    }
                    Expect( 113 );
                    if( la.kind == 96 )
                    {
                        Block();
                    }
                    else if( la.kind == 114 )
                    {
                        Get();
                    }
                    else
                    {
                        SynErr( 149 );
                    }
                }
                else if( IsFieldDecl() )
                {
                    if( type == TypeKind.@void )
                    {
                        Error( "field type must not be void" );
                    }

                    VariableDeclarators();
                    Expect( 114 );
                }
                else if( la.kind == 1 )
                {
                    MemberName();
                    if( la.kind == 96 )
                    {
                        if( type == TypeKind.@void )
                        {
                            Error( "property type must not be void" );
                        }

                        Get();
                        AccessorDeclarations();
                        Expect( 111 );
                    }
                    else if( la.kind == 90 )
                    {
                        if( type == TypeKind.@void )
                        {
                            Error( "indexer type must not be void" );
                        }

                        Get();
                        Expect( 68 );
                        Expect( 97 );
                        FormalParameterList();
                        Expect( 112 );
                        Expect( 96 );
                        AccessorDeclarations();
                        Expect( 111 );
                    }
                    else if( la.kind == 98 || la.kind == 100 )
                    {
                        if( la.kind == 100 )
                        {
                            TypeParameterList();
                        }
                        Expect( 98 );
                        if( StartOf( 7 ) )
                        {
                            FormalParameterList();
                        }
                        Expect( 113 );
                        while( la.kind == 1 )
                        {
                            TypeParameterConstraintsClause();
                        }
                        if( la.kind == 96 )
                        {
                            Block();
                        }
                        else if( la.kind == 114 )
                        {
                            Get();
                        }
                        else
                        {
                            SynErr( 150 );
                        }
                    }
                    else
                    {
                        SynErr( 151 );
                    }
                }
                else if( la.kind == 68 )
                {
                    if( type == TypeKind.@void )
                    {
                        Error( "indexer type must not be void" );
                    }

                    Get();
                    Expect( 97 );
                    FormalParameterList();
                    Expect( 112 );
                    Expect( 96 );
                    AccessorDeclarations();
                    Expect( 111 );
                }
                else
                {
                    SynErr( 152 );
                }
            }
            else if( la.kind == 27 || la.kind == 37 )
            {
                if( la.kind == 37 )
                {
                    Get();
                }
                else
                {
                    Get();
                }
                Expect( 49 );
                Type( out type, false );
                if( type == TypeKind.@void )
                {
                    Error( "cast type must not be void" );
                }
                Expect( 98 );
                Type( out type, false );
                Expect( 1 );
                Expect( 113 );
                if( la.kind == 96 )
                {
                    Block();
                }
                else if( la.kind == 114 )
                {
                    Get();
                }
                else
                {
                    SynErr( 153 );
                }
            }
            else if( StartOf( 16 ) )
            {
                TypeDeclaration();
            }
            else
            {
                SynErr( 154 );
            }
        }

        private void EnumMemberDeclaration()
        {
            while( la.kind == 97 )
            {
                Attributes();
            }
            Expect( 1 );
            if( la.kind == 85 )
            {
                Get();
                Expression();
            }
        }

        private void Block()
        {
            Expect( 96 );
            while( StartOf( 17 ) )
            {
                Statement();
            }
            Expect( 111 );
        }

        private void Expression()
        {
            Unary();
            if( assgnOps[la.kind] || (la.kind == _gt && Peek( 1 ).kind == _gteq) )
            {
                AssignmentOperator();
                Expression();
            }
            else if( StartOf( 18 ) )
            {
                NullCoalescingExpr();
                if( la.kind == 110 )
                {
                    Get();
                    Expression();
                    Expect( 86 );
                    Expression();
                }
            }
            else
            {
                SynErr( 155 );
            }
        }

        private void VariableDeclarators()
        {
            int start;
            Expect( 1 );
            var name = t.val;
            if( la.kind == 85 )
            {
                Get();
                start = la.pos;
                VariableInitializer();
                variables.Add( new DictionaryEntry( name, scanner.buffer.GetString( start, la.pos ) ) );
            }
            while( la.kind == 87 )
            {
                Get();
                Expect( 1 );
                name = t.val;
                if( la.kind == 85 )
                {
                    Get();
                    start = la.pos;
                    VariableInitializer();
                    variables.Add( new DictionaryEntry( name, scanner.buffer.GetString( start, la.pos ) ) );
                }
            }
        }

        private void EventAccessorDeclarations()
        {
            bool addFound = false, remFound = false;
            while( la.kind == 97 )
            {
                Attributes();
            }
            if( "add".Equals( la.val ) )
            {
                Expect( 1 );
                addFound = true;
            }
            else if( "remove".Equals( la.val ) )
            {
                Expect( 1 );
                remFound = true;
            }
            else if( la.kind == 1 )
            {
                Get();
                Error( "add or remove expected" );
            }
            else
            {
                SynErr( 156 );
            }
            Block();
            if( la.kind == 1 || la.kind == 97 )
            {
                while( la.kind == 97 )
                {
                    Attributes();
                }
                if( "add".Equals( la.val ) )
                {
                    Expect( 1 );
                    if( addFound )
                    {
                        Error( "add already declared" );
                    }
                }
                else if( "remove".Equals( la.val ) )
                {
                    Expect( 1 );
                    if( remFound )
                    {
                        Error( "remove already declared" );
                    }
                }
                else if( la.kind == 1 )
                {
                    Get();
                    Error( "add or remove expected" );
                }
                else
                {
                    SynErr( 157 );
                }
                Block();
            }
        }

        private void Argument()
        {
            if( la.kind == 50 || la.kind == 57 )
            {
                if( la.kind == 57 )
                {
                    Get();
                }
                else
                {
                    Get();
                }
            }
            Expression();
        }

        private void OverloadableOp( out Operator op )
        {
            op = Operator.plus;
            switch( la.kind )
            {
                case 108:
                {
                    Get();
                    break;
                }
                case 102:
                {
                    Get();
                    op = Operator.minus;
                    break;
                }
                case 106:
                {
                    Get();
                    op = Operator.not;
                    break;
                }
                case 115:
                {
                    Get();
                    op = Operator.tilde;
                    break;
                }
                case 95:
                {
                    Get();
                    op = Operator.inc;
                    break;
                }
                case 88:
                {
                    Get();
                    op = Operator.dec;
                    break;
                }
                case 70:
                {
                    Get();
                    op = Operator.@true;
                    break;
                }
                case 29:
                {
                    Get();
                    op = Operator.@false;
                    break;
                }
                case 116:
                {
                    Get();
                    op = Operator.times;
                    break;
                }
                case 127:
                {
                    Get();
                    op = Operator.div;
                    break;
                }
                case 128:
                {
                    Get();
                    op = Operator.mod;
                    break;
                }
                case 83:
                {
                    Get();
                    op = Operator.and;
                    break;
                }
                case 124:
                {
                    Get();
                    op = Operator.or;
                    break;
                }
                case 125:
                {
                    Get();
                    op = Operator.xor;
                    break;
                }
                case 101:
                {
                    Get();
                    op = Operator.lshift;
                    break;
                }
                case 92:
                {
                    Get();
                    op = Operator.eq;
                    break;
                }
                case 105:
                {
                    Get();
                    op = Operator.neq;
                    break;
                }
                case 93:
                {
                    Get();
                    op = Operator.gt;
                    if( la.kind == 93 )
                    {
                        if( la.pos > t.pos + 1 )
                        {
                            Error( "no whitespace allowed in right shift operator" );
                        }
                        Get();
                        op = Operator.rshift;
                    }
                    break;
                }
                case 100:
                {
                    Get();
                    op = Operator.lt;
                    break;
                }
                case 94:
                {
                    Get();
                    op = Operator.gte;
                    break;
                }
                case 126:
                {
                    Get();
                    op = Operator.lte;
                    break;
                }
                default:
                SynErr( 158 );
                break;
            }
        }

        private void MemberName()
        {
            Expect( 1 );
            if( la.kind == 91 )
            {
                Get();
                Expect( 1 );
            }
            if( la.kind == _lt && IsPartOfMemberName() )
            {
                TypeArgumentList();
            }
            while( la.kind == _dot && Peek( 1 ).kind == _ident )
            {
                Expect( 90 );
                Expect( 1 );
                if( la.kind == _lt && IsPartOfMemberName() )
                {
                    TypeArgumentList();
                }
            }
        }

        private void AccessorDeclarations()
        {
            bool getFound = false, setFound = false;

            while( la.kind == 97 )
            {
                Attributes();
            }
            ModifierList();
            if( "get".Equals( la.val ) )
            {
                Expect( 1 );
                getFound = true;
            }
            else if( "set".Equals( la.val ) )
            {
                Expect( 1 );
                setFound = true;
            }
            else if( la.kind == 1 )
            {
                Get();
                Error( "set or get expected" );
            }
            else
            {
                SynErr( 159 );
            }
            if( la.kind == 96 )
            {
                Block();
            }
            else if( la.kind == 114 )
            {
                Get();
            }
            else
            {
                SynErr( 160 );
            }
            if( StartOf( 19 ) )
            {
                while( la.kind == 97 )
                {
                    Attributes();
                }
                ModifierList();
                if( "get".Equals( la.val ) )
                {
                    Expect( 1 );
                    if( getFound )
                    {
                        Error( "get already declared" );
                    }
                }
                else if( "set".Equals( la.val ) )
                {
                    Expect( 1 );
                    if( setFound )
                    {
                        Error( "set already declared" );
                    }
                }
                else if( la.kind == 1 )
                {
                    Get();
                    Error( "set or get expected" );
                }
                else
                {
                    SynErr( 161 );
                }
                if( la.kind == 96 )
                {
                    Block();
                }
                else if( la.kind == 114 )
                {
                    Get();
                }
                else
                {
                    SynErr( 162 );
                }
            }
        }

        private void InterfaceAccessors()
        {
            bool getFound = false, setFound = false;
            while( la.kind == 97 )
            {
                Attributes();
            }
            if( "get".Equals( la.val ) )
            {
                Expect( 1 );
                getFound = true;
            }
            else if( "set".Equals( la.val ) )
            {
                Expect( 1 );
                setFound = true;
            }
            else if( la.kind == 1 )
            {
                Get();
                Error( "set or get expected" );
            }
            else
            {
                SynErr( 163 );
            }
            Expect( 114 );
            if( la.kind == 1 || la.kind == 97 )
            {
                while( la.kind == 97 )
                {
                    Attributes();
                }
                if( "get".Equals( la.val ) )
                {
                    Expect( 1 );
                    if( getFound )
                    {
                        Error( "get already declared" );
                    }
                }
                else if( "set".Equals( la.val ) )
                {
                    Expect( 1 );
                    if( setFound )
                    {
                        Error( "set already declared" );
                    }
                }
                else if( la.kind == 1 )
                {
                    Get();
                    Error( "set or get expected" );
                }
                else
                {
                    SynErr( 164 );
                }
                Expect( 114 );
            }
        }

        private void LocalVariableDeclaration()
        {
            TypeKind dummy;
            Type( out dummy, false );
            LocalVariableDeclarator();
            while( la.kind == 87 )
            {
                Get();
                LocalVariableDeclarator();
            }
        }

        private void LocalVariableDeclarator()
        {
            TypeKind dummy;
            Expect( 1 );
            if( la.kind == 85 )
            {
                Get();
                if( StartOf( 20 ) )
                {
                    VariableInitializer();
                }
                else if( la.kind == 63 )
                {
                    Get();
                    Type( out dummy, false );
                    Expect( 97 );
                    Expression();
                    Expect( 112 );
                }
                else
                {
                    SynErr( 165 );
                }
            }
        }

        private void VariableInitializer()
        {
            if( StartOf( 21 ) )
            {
                Expression();
            }
            else if( la.kind == 96 )
            {
                ArrayInitializer();
            }
            else
            {
                SynErr( 166 );
            }
        }

        private void ArrayInitializer()
        {
            Expect( 96 );
            if( StartOf( 20 ) )
            {
                VariableInitializer();
                while( NotFinalComma() )
                {
                    Expect( 87 );
                    VariableInitializer();
                }
                if( la.kind == 87 )
                {
                    Get();
                }
            }
            Expect( 111 );
        }

        private void Attribute()
        {
            TypeName();
            if( la.kind == 98 )
            {
                AttributeArguments();
            }
        }

        private void Keyword()
        {
            switch( la.kind )
            {
                case 6:
                {
                    Get();
                    break;
                }
                case 7:
                {
                    Get();
                    break;
                }
                case 8:
                {
                    Get();
                    break;
                }
                case 9:
                {
                    Get();
                    break;
                }
                case 10:
                {
                    Get();
                    break;
                }
                case 11:
                {
                    Get();
                    break;
                }
                case 12:
                {
                    Get();
                    break;
                }
                case 13:
                {
                    Get();
                    break;
                }
                case 14:
                {
                    Get();
                    break;
                }
                case 15:
                {
                    Get();
                    break;
                }
                case 16:
                {
                    Get();
                    break;
                }
                case 17:
                {
                    Get();
                    break;
                }
                case 18:
                {
                    Get();
                    break;
                }
                case 19:
                {
                    Get();
                    break;
                }
                case 20:
                {
                    Get();
                    break;
                }
                case 21:
                {
                    Get();
                    break;
                }
                case 22:
                {
                    Get();
                    break;
                }
                case 23:
                {
                    Get();
                    break;
                }
                case 24:
                {
                    Get();
                    break;
                }
                case 25:
                {
                    Get();
                    break;
                }
                case 26:
                {
                    Get();
                    break;
                }
                case 27:
                {
                    Get();
                    break;
                }
                case 28:
                {
                    Get();
                    break;
                }
                case 29:
                {
                    Get();
                    break;
                }
                case 30:
                {
                    Get();
                    break;
                }
                case 31:
                {
                    Get();
                    break;
                }
                case 32:
                {
                    Get();
                    break;
                }
                case 33:
                {
                    Get();
                    break;
                }
                case 34:
                {
                    Get();
                    break;
                }
                case 35:
                {
                    Get();
                    break;
                }
                case 36:
                {
                    Get();
                    break;
                }
                case 37:
                {
                    Get();
                    break;
                }
                case 38:
                {
                    Get();
                    break;
                }
                case 39:
                {
                    Get();
                    break;
                }
                case 40:
                {
                    Get();
                    break;
                }
                case 41:
                {
                    Get();
                    break;
                }
                case 42:
                {
                    Get();
                    break;
                }
                case 43:
                {
                    Get();
                    break;
                }
                case 44:
                {
                    Get();
                    break;
                }
                case 45:
                {
                    Get();
                    break;
                }
                case 46:
                {
                    Get();
                    break;
                }
                case 47:
                {
                    Get();
                    break;
                }
                case 48:
                {
                    Get();
                    break;
                }
                case 49:
                {
                    Get();
                    break;
                }
                case 50:
                {
                    Get();
                    break;
                }
                case 51:
                {
                    Get();
                    break;
                }
                case 52:
                {
                    Get();
                    break;
                }
                case 53:
                {
                    Get();
                    break;
                }
                case 54:
                {
                    Get();
                    break;
                }
                case 55:
                {
                    Get();
                    break;
                }
                case 56:
                {
                    Get();
                    break;
                }
                case 57:
                {
                    Get();
                    break;
                }
                case 58:
                {
                    Get();
                    break;
                }
                case 59:
                {
                    Get();
                    break;
                }
                case 60:
                {
                    Get();
                    break;
                }
                case 61:
                {
                    Get();
                    break;
                }
                case 62:
                {
                    Get();
                    break;
                }
                case 63:
                {
                    Get();
                    break;
                }
                case 64:
                {
                    Get();
                    break;
                }
                case 65:
                {
                    Get();
                    break;
                }
                case 66:
                {
                    Get();
                    break;
                }
                case 67:
                {
                    Get();
                    break;
                }
                case 68:
                {
                    Get();
                    break;
                }
                case 69:
                {
                    Get();
                    break;
                }
                case 70:
                {
                    Get();
                    break;
                }
                case 71:
                {
                    Get();
                    break;
                }
                case 72:
                {
                    Get();
                    break;
                }
                case 73:
                {
                    Get();
                    break;
                }
                case 74:
                {
                    Get();
                    break;
                }
                case 75:
                {
                    Get();
                    break;
                }
                case 76:
                {
                    Get();
                    break;
                }
                case 77:
                {
                    Get();
                    break;
                }
                case 78:
                {
                    Get();
                    break;
                }
                case 79:
                {
                    Get();
                    break;
                }
                case 80:
                {
                    Get();
                    break;
                }
                case 81:
                {
                    Get();
                    break;
                }
                case 82:
                {
                    Get();
                    break;
                }
                default:
                SynErr( 167 );
                break;
            }
        }

        private void AttributeArguments()
        {
            var nameFound = false;
            Expect( 98 );
            if( StartOf( 21 ) )
            {
                if( IsAssignment() )
                {
                    nameFound = true;
                    Expect( 1 );
                    Expect( 85 );
                }
                Expression();
                while( la.kind == 87 )
                {
                    Get();
                    if( IsAssignment() )
                    {
                        nameFound = true;
                        Expect( 1 );
                        Expect( 85 );
                    }
                    else if( StartOf( 21 ) )
                    {
                        if( nameFound )
                        {
                            Error( "no positional argument after named arguments" );
                        }
                    }
                    else
                    {
                        SynErr( 168 );
                    }
                    Expression();
                }
            }
            Expect( 113 );
        }

        private void PrimitiveType()
        {
            if( StartOf( 22 ) )
            {
                IntegralType();
            }
            else if( la.kind == 32 )
            {
                Get();
            }
            else if( la.kind == 23 )
            {
                Get();
            }
            else if( la.kind == 19 )
            {
                Get();
            }
            else if( la.kind == 9 )
            {
                Get();
            }
            else
            {
                SynErr( 169 );
            }
        }

        private void PointerOrArray( ref TypeKind type )
        {
            while( IsPointerOrDims() )
            {
                if( la.kind == 116 )
                {
                    Get();
                    type = TypeKind.pointer;
                }
                else if( la.kind == 97 )
                {
                    Get();
                    while( la.kind == 87 )
                    {
                        Get();
                    }
                    Expect( 112 );
                    type = TypeKind.array;
                }
                else
                {
                    SynErr( 170 );
                }
            }
        }

        private void ResolvedType()
        {
            var type = TypeKind.simple;
            if( StartOf( 12 ) )
            {
                PrimitiveType();
            }
            else if( la.kind == 48 )
            {
                Get();
            }
            else if( la.kind == 65 )
            {
                Get();
            }
            else if( la.kind == 1 )
            {
                Get();
                if( la.kind == 91 )
                {
                    Get();
                    Expect( 1 );
                }
                if( IsGeneric() )
                {
                    TypeArgumentList();
                }
                while( la.kind == 90 )
                {
                    Get();
                    Expect( 1 );
                    if( IsGeneric() )
                    {
                        TypeArgumentList();
                    }
                }
            }
            else if( la.kind == 80 )
            {
                Get();
                type = TypeKind.@void;
            }
            else
            {
                SynErr( 171 );
            }
            PointerOrArray( ref type );
            if( type == TypeKind.@void )
            {
                Error( "type expected, void found, maybe you mean void*" );
            }
        }

        private void TypeArgumentList()
        {
            TypeKind dummy;
            Expect( 100 );
            if( StartOf( 11 ) )
            {
                Type( out dummy, false );
            }
            while( la.kind == 87 )
            {
                Get();
                if( StartOf( 11 ) )
                {
                    Type( out dummy, false );
                }
            }
            Expect( 93 );
        }

        private void InternalClassType()
        {
            if( la.kind == 48 )
            {
                Get();
            }
            else if( la.kind == 65 )
            {
                Get();
            }
            else
            {
                SynErr( 172 );
            }
        }

        private void Statement()
        {
            TypeKind dummy;
            if( la.kind == _ident && Peek( 1 ).kind == _colon )
            {
                Expect( 1 );
                Expect( 86 );
                Statement();
            }
            else if( la.kind == 17 )
            {
                Get();
                Type( out dummy, false );
                Expect( 1 );
                Expect( 85 );
                Expression();
                while( la.kind == 87 )
                {
                    Get();
                    Expect( 1 );
                    Expect( 85 );
                    Expression();
                }
                Expect( 114 );
            }
            else if( IsLocalVarDecl() )
            {
                LocalVariableDeclaration();
                Expect( 114 );
            }
            else if( StartOf( 23 ) )
            {
                EmbeddedStatement();
            }
            else
            {
                SynErr( 173 );
            }
        }

        private void EmbeddedStatement()
        {
            TypeKind type;
            if( la.kind == 96 )
            {
                Block();
            }
            else if( la.kind == 114 )
            {
                Get();
            }
            else if( la.kind == _checked && Peek( 1 ).kind == _lbrace )
            {
                Expect( 15 );
                Block();
            }
            else if( la.kind == _unchecked && Peek( 1 ).kind == _lbrace )
            {
                Expect( 75 );
                Block();
            }
            else if( StartOf( 21 ) )
            {
                StatementExpression();
                Expect( 114 );
            }
            else if( la.kind == 36 )
            {
                Get();
                Expect( 98 );
                Expression();
                Expect( 113 );
                EmbeddedStatement();
                if( la.kind == 24 )
                {
                    Get();
                    EmbeddedStatement();
                }
            }
            else if( la.kind == 67 )
            {
                Get();
                Expect( 98 );
                Expression();
                Expect( 113 );
                Expect( 96 );
                while( la.kind == 12 || la.kind == 20 )
                {
                    SwitchSection();
                }
                Expect( 111 );
            }
            else if( la.kind == 82 )
            {
                Get();
                Expect( 98 );
                Expression();
                Expect( 113 );
                EmbeddedStatement();
            }
            else if( la.kind == 22 )
            {
                Get();
                EmbeddedStatement();
                Expect( 82 );
                Expect( 98 );
                Expression();
                Expect( 113 );
                Expect( 114 );
            }
            else if( la.kind == 33 )
            {
                Get();
                Expect( 98 );
                if( StartOf( 24 ) )
                {
                    ForInitializer();
                }
                Expect( 114 );
                if( StartOf( 21 ) )
                {
                    Expression();
                }
                Expect( 114 );
                if( StartOf( 21 ) )
                {
                    ForIterator();
                }
                Expect( 113 );
                EmbeddedStatement();
            }
            else if( la.kind == 34 )
            {
                Get();
                Expect( 98 );
                Type( out type, false );
                Expect( 1 );
                Expect( 38 );
                Expression();
                Expect( 113 );
                EmbeddedStatement();
            }
            else if( la.kind == 10 )
            {
                Get();
                Expect( 114 );
            }
            else if( la.kind == 18 )
            {
                Get();
                Expect( 114 );
            }
            else if( la.kind == 35 )
            {
                Get();
                if( la.kind == 1 )
                {
                    Get();
                }
                else if( la.kind == 12 )
                {
                    Get();
                    Expression();
                }
                else if( la.kind == 20 )
                {
                    Get();
                }
                else
                {
                    SynErr( 174 );
                }
                Expect( 114 );
            }
            else if( la.kind == 58 )
            {
                Get();
                if( StartOf( 21 ) )
                {
                    Expression();
                }
                Expect( 114 );
            }
            else if( la.kind == 69 )
            {
                Get();
                if( StartOf( 21 ) )
                {
                    Expression();
                }
                Expect( 114 );
            }
            else if( la.kind == 71 )
            {
                Get();
                Block();
                if( la.kind == 13 )
                {
                    CatchClauses();
                    if( la.kind == 30 )
                    {
                        Get();
                        Block();
                    }
                }
                else if( la.kind == 30 )
                {
                    Get();
                    Block();
                }
                else
                {
                    SynErr( 175 );
                }
            }
            else if( la.kind == 43 )
            {
                Get();
                Expect( 98 );
                Expression();
                Expect( 113 );
                EmbeddedStatement();
            }
            else if( la.kind == 78 )
            {
                Get();
                Expect( 98 );
                ResourceAcquisition();
                Expect( 113 );
                EmbeddedStatement();
            }
            else if( la.kind == 120 )
            {
                Get();
                if( la.kind == 58 )
                {
                    Get();
                    Expression();
                }
                else if( la.kind == 10 )
                {
                    Get();
                }
                else
                {
                    SynErr( 176 );
                }
                Expect( 114 );
            }
            else if( la.kind == 76 )
            {
                Get();
                Block();
            }
            else if( la.kind == 31 )
            {
                Get();
                Expect( 98 );
                Type( out type, false );
                if( type != TypeKind.pointer )
                {
                    Error( "can only fix pointer types" );
                }
                Expect( 1 );
                Expect( 85 );
                Expression();
                while( la.kind == 87 )
                {
                    Get();
                    Expect( 1 );
                    Expect( 85 );
                    Expression();
                }
                Expect( 113 );
                EmbeddedStatement();
            }
            else
            {
                SynErr( 177 );
            }
        }

        private void StatementExpression()
        {
            var isAssignment = assnStartOp[la.kind] || IsTypeCast();
            Unary();
            if( StartOf( 25 ) )
            {
                AssignmentOperator();
                Expression();
            }
            else if( la.kind == 87 || la.kind == 113 || la.kind == 114 )
            {
                if( isAssignment )
                {
                    Error( "error in assignment." );
                }
            }
            else
            {
                SynErr( 178 );
            }
        }

        private void SwitchSection()
        {
            SwitchLabel();
            while( la.kind == _case || (la.kind == _default && Peek( 1 ).kind == _colon) )
            {
                SwitchLabel();
            }
            Statement();
            while( IsNoSwitchLabelOrRBrace() )
            {
                Statement();
            }
        }

        private void ForInitializer()
        {
            if( IsLocalVarDecl() )
            {
                LocalVariableDeclaration();
            }
            else if( StartOf( 21 ) )
            {
                StatementExpression();
                while( la.kind == 87 )
                {
                    Get();
                    StatementExpression();
                }
            }
            else
            {
                SynErr( 179 );
            }
        }

        private void ForIterator()
        {
            StatementExpression();
            while( la.kind == 87 )
            {
                Get();
                StatementExpression();
            }
        }

        private void CatchClauses()
        {
            Expect( 13 );
            if( la.kind == 96 )
            {
                Block();
            }
            else if( la.kind == 98 )
            {
                Get();
                ClassType();
                if( la.kind == 1 )
                {
                    Get();
                }
                Expect( 113 );
                Block();
                if( la.kind == 13 )
                {
                    CatchClauses();
                }
            }
            else
            {
                SynErr( 180 );
            }
        }

        private void ResourceAcquisition()
        {
            if( IsLocalVarDecl() )
            {
                LocalVariableDeclaration();
            }
            else if( StartOf( 21 ) )
            {
                Expression();
            }
            else
            {
                SynErr( 181 );
            }
        }

        private void Unary()
        {
            TypeKind dummy;
            while( unaryHead[la.kind] || IsTypeCast() )
            {
                switch( la.kind )
                {
                    case 108:
                    {
                        Get();
                        break;
                    }
                    case 102:
                    {
                        Get();
                        break;
                    }
                    case 106:
                    {
                        Get();
                        break;
                    }
                    case 115:
                    {
                        Get();
                        break;
                    }
                    case 95:
                    {
                        Get();
                        break;
                    }
                    case 88:
                    {
                        Get();
                        break;
                    }
                    case 116:
                    {
                        Get();
                        break;
                    }
                    case 83:
                    {
                        Get();
                        break;
                    }
                    case 98:
                    {
                        Get();
                        Type( out dummy, false );
                        Expect( 113 );
                        break;
                    }
                    default:
                    SynErr( 182 );
                    break;
                }
            }
            Primary();
        }

        private void AssignmentOperator()
        {
            switch( la.kind )
            {
                case 85:
                {
                    Get();
                    break;
                }
                case 109:
                {
                    Get();
                    break;
                }
                case 103:
                {
                    Get();
                    break;
                }
                case 117:
                {
                    Get();
                    break;
                }
                case 89:
                {
                    Get();
                    break;
                }
                case 104:
                {
                    Get();
                    break;
                }
                case 84:
                {
                    Get();
                    break;
                }
                case 107:
                {
                    Get();
                    break;
                }
                case 118:
                {
                    Get();
                    break;
                }
                case 99:
                {
                    Get();
                    break;
                }
                case 93:
                {
                    Get();
                    var pos = t.pos;
                    Expect( 94 );
                    if( pos + 1 < t.pos )
                    {
                        Error( "no whitespace allowed in right shift assignment" );
                    }
                    break;
                }
                default:
                SynErr( 183 );
                break;
            }
        }

        private void SwitchLabel()
        {
            if( la.kind == 12 )
            {
                Get();
                Expression();
                Expect( 86 );
            }
            else if( la.kind == 20 )
            {
                Get();
                Expect( 86 );
            }
            else
            {
                SynErr( 184 );
            }
        }

        private void NullCoalescingExpr()
        {
            OrExpr();
            while( la.kind == 121 )
            {
                Get();
                Unary();
                OrExpr();
            }
        }

        private void OrExpr()
        {
            AndExpr();
            while( la.kind == 122 )
            {
                Get();
                Unary();
                AndExpr();
            }
        }

        private void AndExpr()
        {
            BitOrExpr();
            while( la.kind == 123 )
            {
                Get();
                Unary();
                BitOrExpr();
            }
        }

        private void BitOrExpr()
        {
            BitXorExpr();
            while( la.kind == 124 )
            {
                Get();
                Unary();
                BitXorExpr();
            }
        }

        private void BitXorExpr()
        {
            BitAndExpr();
            while( la.kind == 125 )
            {
                Get();
                Unary();
                BitAndExpr();
            }
        }

        private void BitAndExpr()
        {
            EqlExpr();
            while( la.kind == 83 )
            {
                Get();
                Unary();
                EqlExpr();
            }
        }

        private void EqlExpr()
        {
            RelExpr();
            while( la.kind == 92 || la.kind == 105 )
            {
                if( la.kind == 105 )
                {
                    Get();
                }
                else
                {
                    Get();
                }
                Unary();
                RelExpr();
            }
        }

        private void RelExpr()
        {
            ShiftExpr();
            while( StartOf( 26 ) )
            {
                if( StartOf( 27 ) )
                {
                    if( la.kind == 100 )
                    {
                        Get();
                    }
                    else if( la.kind == 93 )
                    {
                        Get();
                    }
                    else if( la.kind == 126 )
                    {
                        Get();
                    }
                    else if( la.kind == 94 )
                    {
                        Get();
                    }
                    else
                    {
                        SynErr( 185 );
                    }
                    Unary();
                    ShiftExpr();
                }
                else
                {
                    if( la.kind == 42 )
                    {
                        Get();
                    }
                    else if( la.kind == 7 )
                    {
                        Get();
                    }
                    else
                    {
                        SynErr( 186 );
                    }
                    ResolvedType();
                }
            }
        }

        private void ShiftExpr()
        {
            AddExpr();
            while( IsShift() )
            {
                if( la.kind == 101 )
                {
                    Get();
                }
                else if( la.kind == 93 )
                {
                    Get();
                    Expect( 93 );
                }
                else
                {
                    SynErr( 187 );
                }
                Unary();
                AddExpr();
            }
        }

        private void AddExpr()
        {
            MulExpr();
            while( la.kind == 102 || la.kind == 108 )
            {
                if( la.kind == 108 )
                {
                    Get();
                }
                else
                {
                    Get();
                }
                Unary();
                MulExpr();
            }
        }

        private void MulExpr()
        {
            while( la.kind == 116 || la.kind == 127 || la.kind == 128 )
            {
                if( la.kind == 116 )
                {
                    Get();
                }
                else if( la.kind == 127 )
                {
                    Get();
                }
                else
                {
                    Get();
                }
                Unary();
            }
        }

        private void Primary()
        {
            TypeKind type;
            var isArrayCreation = false;
            switch( la.kind )
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 29:
                case 47:
                case 70:
                {
                    Literal();
                    break;
                }
                case 98:
                {
                    Get();
                    Expression();
                    Expect( 113 );
                    break;
                }
                case 9:
                case 11:
                case 14:
                case 19:
                case 23:
                case 32:
                case 39:
                case 44:
                case 48:
                case 59:
                case 61:
                case 65:
                case 73:
                case 74:
                case 77:
                {
                    switch( la.kind )
                    {
                        case 9:
                        {
                            Get();
                            break;
                        }
                        case 11:
                        {
                            Get();
                            break;
                        }
                        case 14:
                        {
                            Get();
                            break;
                        }
                        case 19:
                        {
                            Get();
                            break;
                        }
                        case 23:
                        {
                            Get();
                            break;
                        }
                        case 32:
                        {
                            Get();
                            break;
                        }
                        case 39:
                        {
                            Get();
                            break;
                        }
                        case 44:
                        {
                            Get();
                            break;
                        }
                        case 48:
                        {
                            Get();
                            break;
                        }
                        case 59:
                        {
                            Get();
                            break;
                        }
                        case 61:
                        {
                            Get();
                            break;
                        }
                        case 65:
                        {
                            Get();
                            break;
                        }
                        case 73:
                        {
                            Get();
                            break;
                        }
                        case 74:
                        {
                            Get();
                            break;
                        }
                        case 77:
                        {
                            Get();
                            break;
                        }
                    }
                    Expect( 90 );
                    Expect( 1 );
                    if( IsGeneric() )
                    {
                        TypeArgumentList();
                    }
                    break;
                }
                case 1:
                {
                    Get();
                    if( la.kind == 91 )
                    {
                        Get();
                        Expect( 1 );
                        if( la.kind == 100 )
                        {
                            TypeArgumentList();
                        }
                        Expect( 90 );
                        Expect( 1 );
                    }
                    if( IsGeneric() )
                    {
                        TypeArgumentList();
                    }
                    break;
                }
                case 68:
                {
                    Get();
                    break;
                }
                case 8:
                {
                    Get();
                    if( la.kind == 90 )
                    {
                        Get();
                        Expect( 1 );
                        if( IsGeneric() )
                        {
                            TypeArgumentList();
                        }
                    }
                    else if( la.kind == 97 )
                    {
                        Get();
                        Expression();
                        while( la.kind == 87 )
                        {
                            Get();
                            Expression();
                        }
                        Expect( 112 );
                    }
                    else
                    {
                        SynErr( 188 );
                    }
                    break;
                }
                case 46:
                {
                    Get();
                    Type( out type, false );
                    if( la.kind == 98 )
                    {
                        Get();
                        if( StartOf( 15 ) )
                        {
                            Argument();
                            while( la.kind == 87 )
                            {
                                Get();
                                Argument();
                            }
                        }
                        Expect( 113 );
                    }
                    else if( la.kind == 97 )
                    {
                        Get();
                        Expression();
                        while( la.kind == 87 )
                        {
                            Get();
                            Expression();
                        }
                        Expect( 112 );
                        while( IsDims() )
                        {
                            Expect( 97 );
                            while( la.kind == 87 )
                            {
                                Get();
                            }
                            Expect( 112 );
                        }
                        if( la.kind == 96 )
                        {
                            ArrayInitializer();
                        }
                        isArrayCreation = true;
                    }
                    else if( la.kind == 96 )
                    {
                        ArrayInitializer();
                        if( type != TypeKind.array )
                        {
                            Error( "array type expected" );
                        }
                        isArrayCreation = true;
                    }
                    else
                    {
                        SynErr( 189 );
                    }
                    break;
                }
                case 72:
                {
                    Get();
                    Expect( 98 );
                    Type( out type, true );
                    Expect( 113 );
                    break;
                }
                case 15:
                {
                    Get();
                    Expect( 98 );
                    Expression();
                    Expect( 113 );
                    break;
                }
                case 75:
                {
                    Get();
                    Expect( 98 );
                    Expression();
                    Expect( 113 );
                    break;
                }
                case 20:
                {
                    Get();
                    Expect( 98 );
                    Primary();
                    Expect( 113 );
                    break;
                }
                case 21:
                {
                    Get();
                    if( la.kind == 98 )
                    {
                        Get();
                        if( StartOf( 13 ) )
                        {
                            AnonymousMethodParameter();
                            while( la.kind == 87 )
                            {
                                Get();
                                AnonymousMethodParameter();
                            }
                        }
                        Expect( 113 );
                    }
                    Block();
                    break;
                }
                case 62:
                {
                    Get();
                    Expect( 98 );
                    Type( out type, false );
                    Expect( 113 );
                    break;
                }
                default:
                SynErr( 190 );
                break;
            }
            while( StartOf( 28 ) )
            {
                switch( la.kind )
                {
                    case 95:
                    {
                        Get();
                        break;
                    }
                    case 88:
                    {
                        Get();
                        break;
                    }
                    case 129:
                    {
                        Get();
                        Expect( 1 );
                        if( IsGeneric() )
                        {
                            TypeArgumentList();
                        }
                        break;
                    }
                    case 90:
                    {
                        Get();
                        Expect( 1 );
                        if( IsGeneric() )
                        {
                            TypeArgumentList();
                        }
                        break;
                    }
                    case 98:
                    {
                        Get();
                        if( StartOf( 15 ) )
                        {
                            Argument();
                            while( la.kind == 87 )
                            {
                                Get();
                                Argument();
                            }
                        }
                        Expect( 113 );
                        break;
                    }
                    case 97:
                    {
                        if( isArrayCreation )
                        {
                            Error( "element access not allow on array creation" );
                        }
                        Get();
                        Expression();
                        while( la.kind == 87 )
                        {
                            Get();
                            Expression();
                        }
                        Expect( 112 );
                        break;
                    }
                }
            }
        }

        private void Literal()
        {
            switch( la.kind )
            {
                case 2:
                {
                    Get();
                    break;
                }
                case 3:
                {
                    Get();
                    break;
                }
                case 4:
                {
                    Get();
                    break;
                }
                case 5:
                {
                    Get();
                    break;
                }
                case 70:
                {
                    Get();
                    break;
                }
                case 29:
                {
                    Get();
                    break;
                }
                case 47:
                {
                    Get();
                    break;
                }
                default:
                SynErr( 191 );
                break;
            }
        }

        private void AnonymousMethodParameter()
        {
            TypeKind dummy;
            if( la.kind == 50 || la.kind == 57 )
            {
                if( la.kind == 57 )
                {
                    Get();
                }
                else
                {
                    Get();
                }
            }
            Type( out dummy, false );
            Expect( 1 );
        }

        public void Parse()
        {
            la = new Token {val = ""};
            Get();
            CS2();

            Expect( 0 );
        }

        private static readonly bool[,] set = {
      {
        T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, T, x, x, T, x, x, x
        ,
        x, x, x, x, x, x, x, x, T, T, x, x, x, T, T, x, x, x, x, T, x, T, T, T, T, x, x, x, T, x, x, x
        ,
        T, x, T, x, x, x, x, x, x, x, x, x, T, x, x, T, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, T, x, x, T, x, x, x
        ,
        x, x, x, x, x, x, x, x, T, T, x, x, x, x, T, x, x, x, x, T, x, T, T, T, T, x, x, x, T, x, x, x
        ,
        T, x, T, x, x, x, x, x, x, x, x, x, T, x, x, T, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T
        ,
        T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T
        ,
        T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, T, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, x, T, x, T, T, T, T, x, x, x, T, x, x, x
        ,
        T, x, x, x, x, x, x, x, x, x, x, x, T, x, x, T, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, x, x, x, x, x, x, x, T, x, T, x, x, T, x, x, x, x, T, x, x, x, T, x, x, T, x, x, x, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, T, x, T, x, x, x, x, x, x, x, x, x, x, T, x, T, x, x
        ,
        x, T, x, x, x, x, x, x, x, T, T, x, x, T, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, x, x, x, x, x, x, x, T, x, T, x, x, T, x, x, x, x, T, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, T, x, T, x, T, x, x, x, x, T, x, T, x, T, x, x
        ,
        x, T, x, x, x, x, x, x, x, T, T, x, x, T, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, x, x, x, x, T, x, x, T, x, T, x, x, T, x, T, T, x, T, x, T, x, T, x, T, T, T, T, x, x, x
        ,
        T, x, x, x, x, T, x, T, T, T, x, x, T, x, T, x, T, x, x, T, x, T, T, T, T, x, x, T, T, T, x, x
        ,
        T, T, T, x, x, x, x, x, x, T, T, x, T, T, x, T, T, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, x, x, x, x, T, x, x, T, x, T, x, x, T, x, T, T, x, T, x, T, x, T, x, T, T, T, T, x, x, x
        ,
        T, x, x, x, x, T, x, T, T, T, x, x, T, x, T, x, T, x, x, T, x, T, T, T, T, x, x, T, T, T, x, x
        ,
        T, T, T, x, x, x, x, x, x, T, T, x, T, T, x, T, T, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, x, x, x, x, x, x, x, T, x, T, x, x, T, x, x, x, x, T, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, T, x, x, x, x, x, x, x, x, x, x, T, x, T, x, x
        ,
        x, T, x, x, x, x, x, x, x, T, T, x, x, T, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, x, x, T, x, T, x, x, T, x, x, x, x, T, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, T, x, x
        ,
        x, x, x, x, x, x, x, x, x, T, T, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, x, x, x, x, x, x, x, T, x, T, x, x, T, x, x, x, x, T, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, T, x, T, x, x, x, x, x, x, T, x, T, x, T, x, x
        ,
        x, T, x, x, x, x, x, x, x, T, T, x, x, T, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, x, x, x, x, x, x, x, T, x, T, x, x, T, x, T, T, x, T, x, T, x, T, x, T, T, T, x, x, x, x
        ,
        T, x, x, x, x, T, x, T, T, x, x, x, T, x, x, x, T, x, x, x, x, x, x, x, x, x, x, T, x, T, x, x
        ,
        x, T, T, x, x, x, x, x, x, T, T, x, x, T, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, T, T, T, T, x, x, T, T, x, T, x, x, T, T, x, x, x, T, T, T, x, T, x, x, x, x, x, T, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, T, T, T, x, T, x, x, x, x, x, x, T, x, T, x, T, T, x
        ,
        x, T, x, x, T, x, T, x, T, T, T, T, x, T, x, x, x, x, x, T, x, x, x, x, T, x, x, x, x, x, x, T
        ,
        x, x, T, x, x, x, T, x, x, x, T, x, T, x, x, x, x, x, x, T, T, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, T, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, T, T, T, T, x, x, T, T, T, T, x, x, T, T, x, T, T, T, T, T, T, T, x, x, x, x, x, T, x, T
        ,
        T, T, T, T, T, x, x, T, x, x, x, T, T, x, T, T, T, x, x, x, x, x, x, x, x, x, T, T, x, T, T, x
        ,
        x, T, x, T, T, T, T, T, T, T, T, T, T, T, T, x, T, x, T, T, x, x, x, x, T, x, x, x, x, x, x, T
        ,
        T, x, T, x, x, x, T, x, x, x, T, x, T, x, x, x, x, x, T, T, T, x, x, x, T, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, T, T, x, x, x, x, T, T, T, x
        ,
        x, x, x, x, T, T, T, x, x, T, x, x, T, x, T, T, T, T, T, x, T, x, x, x, x, T, T, T, T, T, T, T
        ,
        T, x, x, x
      },
      {
        x, T, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, x, T, x, T, T, T, T, x, x, x, T, x, x, x
        ,
        T, x, x, x, x, x, x, x, x, x, x, x, T, x, x, T, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, T, T, T, T, x, x, T, T, x, T, x, x, T, T, x, x, x, T, T, T, x, T, x, x, x, x, x, T, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, T, T, T, x, x, x, x, x, x, x, x, x, x, T, x, T, T, x
        ,
        x, T, x, x, T, x, T, x, T, T, T, T, x, T, x, x, x, x, x, T, x, x, x, x, T, x, x, x, x, x, x, T
        ,
        T, x, T, x, x, x, T, x, x, x, T, x, T, x, x, x, x, x, x, T, T, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, T, T, T, T, x, x, T, T, x, T, x, x, T, T, x, x, x, T, T, T, x, T, x, x, x, x, x, T, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, T, T, T, x, x, x, x, x, x, x, x, x, x, T, x, T, T, x
        ,
        x, T, x, x, T, x, T, x, T, T, T, T, x, T, x, x, x, x, x, T, x, x, x, x, T, x, x, x, x, x, x, T
        ,
        x, x, T, x, x, x, T, x, x, x, T, x, T, x, x, x, x, x, x, T, T, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, x, x, x, x, T, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, T, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, T, x, x
        ,
        x, x, x, x, x, x, x, x, x, T, T, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, T, T, T, T, x, x, T, T, T, T, x, x, T, T, x, x, T, T, T, T, T, T, x, x, x, x, x, T, x, T
        ,
        T, T, T, T, T, x, x, T, x, x, x, T, T, x, T, T, T, x, x, x, x, x, x, x, x, x, T, T, x, T, T, x
        ,
        x, T, x, T, T, T, T, T, T, T, T, T, T, T, T, x, x, x, T, T, x, x, x, x, T, x, x, x, x, x, x, T
        ,
        T, x, T, x, x, x, T, x, x, x, T, x, T, x, x, x, x, x, T, T, T, x, x, x, T, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, T, T, T, T, T, x, x, T, T, x, T, x, x, T, T, x, x, x, T, T, T, x, T, x, x, x, x, x, T, x, x
        ,
        T, x, x, x, x, x, x, T, x, x, x, x, T, x, T, T, T, x, x, x, x, x, x, x, x, x, x, T, x, T, T, x
        ,
        x, T, x, x, T, x, T, x, T, T, T, T, x, T, x, x, T, x, x, T, x, x, x, x, T, x, x, x, x, x, x, T
        ,
        x, x, T, x, x, x, T, x, x, x, T, x, T, x, x, x, x, x, x, T, T, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, T, x, x, x, T, x, x, x, T, x, x
        ,
        x, x, x, T, x, x, x, T, T, x, x, T, x, T, x, x, x, x, x, x, x, T, T, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, T, x
        ,
        x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, T, x
        ,
        x, x, x, x, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x
        ,
        x, x, x, x
      },
      {
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, T, x, T, x, x, x, x, T
        ,
        x, T, T, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x, x
        ,
        x, T, x, x
      }
    };
    } // end Parser

    [GeneratedCode( "Coco", "1.0.0.0" )]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    internal class Errors
    {
        public int count; // number of errors detected
        //public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
        public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

        public void SynErr( int line, int col, int n )
        {
            string s;
            switch( n )
            {
                case 0:
                s = "EOF expected";
                break;
                case 1:
                s = "ident expected";
                break;
                case 2:
                s = "intCon expected";
                break;
                case 3:
                s = "realCon expected";
                break;
                case 4:
                s = "charCon expected";
                break;
                case 5:
                s = "stringCon expected";
                break;
                case 6:
                s = "abstract expected";
                break;
                case 7:
                s = "as expected";
                break;
                case 8:
                s = "base expected";
                break;
                case 9:
                s = "bool expected";
                break;
                case 10:
                s = "break expected";
                break;
                case 11:
                s = "byte expected";
                break;
                case 12:
                s = "case expected";
                break;
                case 13:
                s = "catch expected";
                break;
                case 14:
                s = "char expected";
                break;
                case 15:
                s = "checked expected";
                break;
                case 16:
                s = "class expected";
                break;
                case 17:
                s = "const expected";
                break;
                case 18:
                s = "continue expected";
                break;
                case 19:
                s = "decimal expected";
                break;
                case 20:
                s = "default expected";
                break;
                case 21:
                s = "delegate expected";
                break;
                case 22:
                s = "do expected";
                break;
                case 23:
                s = "double expected";
                break;
                case 24:
                s = "else expected";
                break;
                case 25:
                s = "enum expected";
                break;
                case 26:
                s = "event expected";
                break;
                case 27:
                s = "explicit expected";
                break;
                case 28:
                s = "extern expected";
                break;
                case 29:
                s = "false expected";
                break;
                case 30:
                s = "finally expected";
                break;
                case 31:
                s = "fixed expected";
                break;
                case 32:
                s = "float expected";
                break;
                case 33:
                s = "for expected";
                break;
                case 34:
                s = "foreach expected";
                break;
                case 35:
                s = "goto expected";
                break;
                case 36:
                s = "if expected";
                break;
                case 37:
                s = "implicit expected";
                break;
                case 38:
                s = "in expected";
                break;
                case 39:
                s = "int expected";
                break;
                case 40:
                s = "interface expected";
                break;
                case 41:
                s = "internal expected";
                break;
                case 42:
                s = "is expected";
                break;
                case 43:
                s = "lock expected";
                break;
                case 44:
                s = "long expected";
                break;
                case 45:
                s = "namespace expected";
                break;
                case 46:
                s = "new expected";
                break;
                case 47:
                s = "null expected";
                break;
                case 48:
                s = "object expected";
                break;
                case 49:
                s = "operator expected";
                break;
                case 50:
                s = "out expected";
                break;
                case 51:
                s = "override expected";
                break;
                case 52:
                s = "params expected";
                break;
                case 53:
                s = "private expected";
                break;
                case 54:
                s = "protected expected";
                break;
                case 55:
                s = "public expected";
                break;
                case 56:
                s = "readonly expected";
                break;
                case 57:
                s = "ref expected";
                break;
                case 58:
                s = "return expected";
                break;
                case 59:
                s = "sbyte expected";
                break;
                case 60:
                s = "sealed expected";
                break;
                case 61:
                s = "short expected";
                break;
                case 62:
                s = "sizeof expected";
                break;
                case 63:
                s = "stackalloc expected";
                break;
                case 64:
                s = "static expected";
                break;
                case 65:
                s = "string expected";
                break;
                case 66:
                s = "struct expected";
                break;
                case 67:
                s = "switch expected";
                break;
                case 68:
                s = "this expected";
                break;
                case 69:
                s = "throw expected";
                break;
                case 70:
                s = "true expected";
                break;
                case 71:
                s = "try expected";
                break;
                case 72:
                s = "typeof expected";
                break;
                case 73:
                s = "uint expected";
                break;
                case 74:
                s = "ulong expected";
                break;
                case 75:
                s = "unchecked expected";
                break;
                case 76:
                s = "unsafe expected";
                break;
                case 77:
                s = "ushort expected";
                break;
                case 78:
                s = "usingKW expected";
                break;
                case 79:
                s = "virtual expected";
                break;
                case 80:
                s = "void expected";
                break;
                case 81:
                s = "volatile expected";
                break;
                case 82:
                s = "while expected";
                break;
                case 83:
                s = "and expected";
                break;
                case 84:
                s = "andassgn expected";
                break;
                case 85:
                s = "assgn expected";
                break;
                case 86:
                s = "colon expected";
                break;
                case 87:
                s = "comma expected";
                break;
                case 88:
                s = "dec expected";
                break;
                case 89:
                s = "divassgn expected";
                break;
                case 90:
                s = "dot expected";
                break;
                case 91:
                s = "dblcolon expected";
                break;
                case 92:
                s = "eq expected";
                break;
                case 93:
                s = "gt expected";
                break;
                case 94:
                s = "gteq expected";
                break;
                case 95:
                s = "inc expected";
                break;
                case 96:
                s = "lbrace expected";
                break;
                case 97:
                s = "lbrack expected";
                break;
                case 98:
                s = "lpar expected";
                break;
                case 99:
                s = "lshassgn expected";
                break;
                case 100:
                s = "lt expected";
                break;
                case 101:
                s = "ltlt expected";
                break;
                case 102:
                s = "minus expected";
                break;
                case 103:
                s = "minusassgn expected";
                break;
                case 104:
                s = "modassgn expected";
                break;
                case 105:
                s = "neq expected";
                break;
                case 106:
                s = "not expected";
                break;
                case 107:
                s = "orassgn expected";
                break;
                case 108:
                s = "plus expected";
                break;
                case 109:
                s = "plusassgn expected";
                break;
                case 110:
                s = "question expected";
                break;
                case 111:
                s = "rbrace expected";
                break;
                case 112:
                s = "rbrack expected";
                break;
                case 113:
                s = "rpar expected";
                break;
                case 114:
                s = "scolon expected";
                break;
                case 115:
                s = "tilde expected";
                break;
                case 116:
                s = "times expected";
                break;
                case 117:
                s = "timesassgn expected";
                break;
                case 118:
                s = "xorassgn expected";
                break;
                case 119:
                s = "\"partial\" expected";
                break;
                case 120:
                s = "\"yield\" expected";
                break;
                case 121:
                s = "\"??\" expected";
                break;
                case 122:
                s = "\"||\" expected";
                break;
                case 123:
                s = "\"&&\" expected";
                break;
                case 124:
                s = "\"|\" expected";
                break;
                case 125:
                s = "\"^\" expected";
                break;
                case 126:
                s = "\"<=\" expected";
                break;
                case 127:
                s = "\"/\" expected";
                break;
                case 128:
                s = "\"%\" expected";
                break;
                case 129:
                s = "\"->\" expected";
                break;
                case 130:
                s = "??? expected";
                break;
                case 131:
                s = "invalid NamespaceMemberDeclaration";
                break;
                case 132:
                s = "invalid Attributes";
                break;
                case 133:
                s = "invalid TypeDeclaration";
                break;
                case 134:
                s = "invalid TypeDeclaration";
                break;
                case 135:
                s = "invalid TypeParameterConstraintsClause";
                break;
                case 136:
                s = "invalid InterfaceMemberDeclaration";
                break;
                case 137:
                s = "invalid InterfaceMemberDeclaration";
                break;
                case 138:
                s = "invalid InterfaceMemberDeclaration";
                break;
                case 139:
                s = "invalid IntegralType";
                break;
                case 140:
                s = "invalid Type";
                break;
                case 141:
                s = "invalid FormalParameterList";
                break;
                case 142:
                s = "invalid ClassType";
                break;
                case 143:
                s = "invalid ClassMemberDeclaration";
                break;
                case 144:
                s = "invalid ClassMemberDeclaration";
                break;
                case 145:
                s = "invalid StructMemberDeclaration";
                break;
                case 146:
                s = "invalid StructMemberDeclaration";
                break;
                case 147:
                s = "invalid StructMemberDeclaration";
                break;
                case 148:
                s = "invalid StructMemberDeclaration";
                break;
                case 149:
                s = "invalid StructMemberDeclaration";
                break;
                case 150:
                s = "invalid StructMemberDeclaration";
                break;
                case 151:
                s = "invalid StructMemberDeclaration";
                break;
                case 152:
                s = "invalid StructMemberDeclaration";
                break;
                case 153:
                s = "invalid StructMemberDeclaration";
                break;
                case 154:
                s = "invalid StructMemberDeclaration";
                break;
                case 155:
                s = "invalid Expression";
                break;
                case 156:
                s = "invalid EventAccessorDeclarations";
                break;
                case 157:
                s = "invalid EventAccessorDeclarations";
                break;
                case 158:
                s = "invalid OverloadableOp";
                break;
                case 159:
                s = "invalid AccessorDeclarations";
                break;
                case 160:
                s = "invalid AccessorDeclarations";
                break;
                case 161:
                s = "invalid AccessorDeclarations";
                break;
                case 162:
                s = "invalid AccessorDeclarations";
                break;
                case 163:
                s = "invalid InterfaceAccessors";
                break;
                case 164:
                s = "invalid InterfaceAccessors";
                break;
                case 165:
                s = "invalid LocalVariableDeclarator";
                break;
                case 166:
                s = "invalid VariableInitializer";
                break;
                case 167:
                s = "invalid Keyword";
                break;
                case 168:
                s = "invalid AttributeArguments";
                break;
                case 169:
                s = "invalid PrimitiveType";
                break;
                case 170:
                s = "invalid PointerOrArray";
                break;
                case 171:
                s = "invalid ResolvedType";
                break;
                case 172:
                s = "invalid InternalClassType";
                break;
                case 173:
                s = "invalid Statement";
                break;
                case 174:
                s = "invalid EmbeddedStatement";
                break;
                case 175:
                s = "invalid EmbeddedStatement";
                break;
                case 176:
                s = "invalid EmbeddedStatement";
                break;
                case 177:
                s = "invalid EmbeddedStatement";
                break;
                case 178:
                s = "invalid StatementExpression";
                break;
                case 179:
                s = "invalid ForInitializer";
                break;
                case 180:
                s = "invalid CatchClauses";
                break;
                case 181:
                s = "invalid ResourceAcquisition";
                break;
                case 182:
                s = "invalid Unary";
                break;
                case 183:
                s = "invalid AssignmentOperator";
                break;
                case 184:
                s = "invalid SwitchLabel";
                break;
                case 185:
                s = "invalid RelExpr";
                break;
                case 186:
                s = "invalid RelExpr";
                break;
                case 187:
                s = "invalid ShiftExpr";
                break;
                case 188:
                s = "invalid Primary";
                break;
                case 189:
                s = "invalid Primary";
                break;
                case 190:
                s = "invalid Primary";
                break;
                case 191:
                s = "invalid Literal";
                break;

                default:
                s = "error " + n;
                break;
            }
            //errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public void SemErr( int line, int col, string s )
        {
            //errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public void SemErr( string s )
        {
            //errorStream.WriteLine(s);
            count++;
        }

        public void Warning( int line, int col, string s )
        {
            //errorStream.WriteLine(errMsgFormat, line, col, s);
        }

        public void Warning( string s )
        {
            //errorStream.WriteLine(s);
        }
    } // Errors

    [GeneratedCode( "Coco", "1.0.0.0" )]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    internal class FatalError : Exception
    {
        public FatalError( string m )
            : base( m )
        {
        }
    }
}