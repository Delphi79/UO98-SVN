using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Irony.Parsing;
using JoinUO.UOdemoSDK;
using JoinUO.UOSL.Service.ASTNodes;

namespace JoinUO.UOSL.Service
{
    [CLSCompliant(false)]
    public interface Tokenizer
    {
        UoToken Tokenize(object value = null);
        UoToken TokenType { get; }
    }

    [Flags]
    public enum LanguageOption
    {
        Native = 0x00,
        /// <summary>
        /// Use enhanced parameterized trigger declarations
        /// </summary>
        UOSLTriggers = 0x01,
        /// <summary>
        /// Adds extra spaces and other punctuation for clarity.
        /// </summary>
        ExtraPunct = 0x02,
        /// <summary>
        /// Allow constants.
        /// </summary>
        Constants = 0x04,
        /// <summary>
        /// Allows If's, For's, and While's to contained a single statement, not enclosed in a block
        /// </summary>
        UnBracedLoopsIfs=0x08,
        /// <summary>
        /// Inserts and enforces : after case and default labels in switch statement.
        /// </summary>
        ColonAfterSwitchCase = 0x10,

        Enhanced = UOSLTriggers | ExtraPunct | ColonAfterSwitchCase,
        Extended = UOSLTriggers | ExtraPunct | Constants | UnBracedLoopsIfs | ColonAfterSwitchCase,
    }

    static class CodeFlagExtentions
    {
        public static bool HasOption(this LanguageOption options, LanguageOption option)
        {
            return (options & option) == option;
        }

        public static LanguageOption GetOptions(this ParsingContext context)
        {
            object options;
            if (context.Values.TryGetValue("UOSLOptions", out options) && options is LanguageOption)
                return (LanguageOption)options;
            else
                return LanguageOption.Native;
        }

        public static bool TryParseHex(this string value, out int result)
        {
            NumberStyles style = value.StartsWith("0x") ? NumberStyles.HexNumber : NumberStyles.Integer;
            value = value.Replace("0x", "");
            return int.TryParse(value, style, CultureInfo.InvariantCulture, out result);
        }
    }

    [CLSCompliant(false)]
    public static class Types
    {
        public static readonly UoToken Int = UOSLBase.KeyToken<UoToken.UoTypeName_int>.Token;
        public static readonly UoToken String = UOSLBase.KeyToken<UoToken.UoTypeName_string>.Token;
        public static readonly UoToken Char = UOSLBase.KeyToken<UoToken.UoTypeName_string>.Token;
        public static readonly UoToken UString = UOSLBase.KeyToken<UoToken.UoTypeName_ustring>.Token;
        public static readonly UoToken Loc = UOSLBase.KeyToken<UoToken.UoTypeName_loc>.Token;
        public static readonly UoToken Obj = UOSLBase.KeyToken<UoToken.UoTypeName_object>.Token;
        public static readonly UoToken List = UOSLBase.KeyToken<UoToken.UoTypeName_list>.Token;
        public static readonly UoToken Void = UOSLBase.KeyToken<UoToken.UoTypeName_void>.Token;
    }

    [CLSCompliant(false)]
    public static class Punct
    {
        public static readonly UoToken LBrace = UOSLBase.KeyToken<UoToken.UoPunctuator_OpenBrace>.Token;
        public static readonly UoToken RBrace = UOSLBase.KeyToken<UoToken.UoPunctuator_CloseBrace>.Token;
        public static readonly UoToken LPara = UOSLBase.KeyToken<UoToken.UoPunctuator_OpenParenthesis>.Token;
        public static readonly UoToken RPara = UOSLBase.KeyToken<UoToken.UoPunctuator_CloseParenthesis>.Token;
        public static readonly UoToken LBrack = UOSLBase.KeyToken<UoToken.UoOperator_OpenBracket>.Token;
        public static readonly UoToken RBrack = UOSLBase.KeyToken<UoToken.UoOperator_CloseBracket>.Token;
        public static readonly UoToken LChev = UOSLBase.KeyToken<UoToken.UoPunctuator_OpenChevron>.Token;
        public static readonly UoToken RChev = UOSLBase.KeyToken<UoToken.UoPunctuator_CloseChevron>.Token;
        public static readonly UoToken Comma = UOSLBase.KeyToken<UoToken.UoPunctuator_Comma>.Token;
        public static readonly UoToken Colon = UOSLBase.KeyToken<UoToken.UoColon>.Token;
        public static readonly UoToken Semi = UOSLBase.KeyToken<UoToken.UoSemicolon>.Token;
    }

    [CLSCompliant(false)]
    public static class Keyword
    {
        public static readonly UoToken Break = UOSLBase.KeyToken<UoToken.UoKeyword_break>.Token;
        public static readonly UoToken Case = UOSLBase.KeyToken<UoToken.UoKeyword_case>.Token;
        public static readonly UoToken Continue = UOSLBase.KeyToken<UoToken.UoKeyword_continue>.Token;
        public static readonly UoToken Default = UOSLBase.KeyToken<UoToken.UoKeyword_default>.Token;
        public static readonly UoToken Else = UOSLBase.KeyToken<UoToken.UoKeyword_else>.Token;
        public static readonly UoToken For = UOSLBase.KeyToken<UoToken.UoKeyword_for>.Token;
        public static readonly UoToken Forward = UOSLBase.KeyToken<UoToken.UoKeyword_forward>.Token;
        public static readonly UoToken Function = UOSLBase.KeyToken<UoToken.UoKeyword_function>.Token;
        public static readonly UoToken If = UOSLBase.KeyToken<UoToken.UoKeyword_if>.Token;
        public static readonly UoToken Inherits = UOSLBase.KeyToken<UoToken.UoKeyword_inherits>.Token;
        public static readonly UoToken Member = UOSLBase.KeyToken<UoToken.UoKeyword_member>.Token;
        public static readonly UoToken Return = UOSLBase.KeyToken<UoToken.UoKeyword_return>.Token;
        public static readonly UoToken Switch = UOSLBase.KeyToken<UoToken.UoKeyword_switch>.Token;
        public static readonly UoToken This = UOSLBase.KeyToken<UoToken.UoKeyword_this>.Token;
        public static readonly UoToken Trigger = UOSLBase.KeyToken<UoToken.UoKeyword_trigger>.Token;
        public static readonly UoToken While = UOSLBase.KeyToken<UoToken.UoKeyword_while>.Token;
    }

    [CLSCompliant(false)]
    public class UOSLBase : Irony.Parsing.Grammar
    {
        public static Dictionary<string, KeyTerm> Keywords = new Dictionary<string, KeyTerm>();

        public class KeyToken<TUoToken> : KeyTerm, Tokenizer where TUoToken : UoToken, new()
        {
            public static UoToken Token = new TUoToken();
            public UoToken Tokenize(object value = null) { return Token; }
            public virtual UoToken TokenType { get { return null; } }

            public KeyToken(TermFlags flags = TermFlags.None) : base(Token.Value, Token.Value) 
            {
                this.Flags |= flags;

                if (this.FlagIsSet(TermFlags.IsKeyword) && !Keywords.ContainsKey(this.Text))
                    Keywords[this.Text] = this;
            }

        }

        public class This : KeyToken<UoToken.UoKeyword_this>
        {
            public This() : base(TermFlags.IsKeyword | TermFlags.IsReservedWord) { }
            public override UoToken TokenType { get { return Types.Obj; } }
        }

        static StringOptions escapeoptions = StringOptions.AllowsAllEscapes & ~(StringOptions.AllowsOctalEscapes | StringOptions.AllowsUEscapes | StringOptions.AllowsXEscapes | StringOptions.AllowsDoubledQuote);

        public class StringLiteralToken : StringLiteral, Tokenizer
        {
            public UoToken Tokenize(object value = null) { return new UoToken.UoConstantString("\"" + value as string + "\""); }
            public UoToken TokenType { get { return KeyToken<UoToken.UoTypeName_string>.Token; } }
            public StringLiteralToken(string name) : base(name, "\"", escapeoptions, typeof(StringNode)) { }
        }

        public class UStringLiteralToken : StringLiteral, Tokenizer
        {
            public UoToken Tokenize(object value = null) { return new UoToken.UoConstantString("L\"" + value as string + "\""); }
            public UoToken TokenType { get { return KeyToken<UoToken.UoTypeName_ustring>.Token; } }
            public UStringLiteralToken(string name) : base(name, "\"", escapeoptions, typeof(StringNode)) { }
        }

        public class CharLiteralToken : StringLiteral, Tokenizer
        {
            public UoToken Tokenize(object value = null) { return new UoToken.UoConstantCharacter((char)value); }
            public UoToken TokenType { get { return KeyToken<UoToken.UoTypeName_string>.Token; } }
            public CharLiteralToken(string name) : base(name, "'", escapeoptions | StringOptions.IsChar, typeof(CharNode)) { }
        }

        public class NumberLiteralToken<TUoToken> : NumberLiteral, Tokenizer where TUoToken : UoToken
        {
            public UoToken Tokenize(object value = null) { return new UoToken.UoConstantNumber((int)value); }
            public UoToken TokenType { get { return Types.Int; } }
            public NumberLiteralToken(string name) : base(name, NumberOptions.IntOnly | NumberOptions.NoDotAfterInt) 
            {
                DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.UInt32 };
                AddPrefix("0d", NumberOptions.IntOnly);
                AddPrefix("0x", NumberOptions.Hex);
                AddPrefix("0o", NumberOptions.Octal);
                AddPrefix("0b", NumberOptions.Binary);
            }
        }

        public class IdentifierToken<TUoToken> : IdentifierTerminal, Tokenizer where TUoToken : UoToken
        {
            List<string> m_ValidValues = null;
            string m_ErrorMessage=null;

            public UoToken Tokenize(object value = null) { return new UoToken.UoConstantString(value as string); }
            public virtual UoToken TokenType { get { return null; } }
            public IdentifierToken(string name, List<string> ValidValues = null, string ErrorMessage = "Event is not a valid name.")
                : base(name, IdOptions.IsNotKeyword)
            {
                if (ValidValues != null)
                {
                    m_ValidValues = ValidValues;
                    m_ErrorMessage = ErrorMessage;
                    this.ValidateToken += new EventHandler<ParsingEventArgs>(IdentifierToken_ValidateToken);
                }
            }


            public override Token TryMatch(ParsingContext context, ISourceStream source)
            {
                if (m_ValidValues != null)
                {
                    bool found=false;
                    foreach (string name in m_ValidValues)
                        if (source.MatchSymbol(name, false))
                        {
                            found = true;
                            break;
                        }
                    if (!found) return null;
                }
                return base.TryMatch(context, source);
            }

            void IdentifierToken_ValidateToken(object sender, ParsingEventArgs e)
            {
                if (m_ValidValues != null && !m_ValidValues.Contains(e.Context.CurrentToken.ValueString))
                    e.Context.AddParserError(m_ErrorMessage);
            }
        }

        public class UoIdentifier<TUoToken> : NonTerminal, Tokenizer where TUoToken : UoToken.UoTypeName, new()
        {
            public UoToken Tokenize(object value = null) { return new UoToken.UoConstantString(value as string); }
            public UoToken TokenType { get { return KeyToken<TUoToken>.Token; } }
            public UoIdentifier() : base("SimpleDeclaration_" + typeof(TUoToken).Name) { }
        }

        public class IdentifierTypedToken<TUoToken, TType> : IdentifierToken<TUoToken> where TUoToken : UoToken where TType : UoToken, new()
        {
            public override UoToken TokenType { get { return KeyToken<TType>.Token; } }
            public IdentifierTypedToken(string name) : base(name) { }
        }

        public class MethodToken<TType> : IdentifierTypedToken<UoToken.UoFunctionName, TType> where TType : UoToken, new()
        {
            public MethodToken(string name) : base(name) 
            {
                ErrorAlias = "Function";
            }
        }

        public class FieldToken<TType> : IdentifierTypedToken<UoToken.UoIdentifier, TType> where TType : UoToken, new()
        {
            public FieldToken(string name) : base(name) 
            {
                ErrorAlias = "Identifier";
            }
        }

        #region Literals
        protected StringLiteral _constantstring = new StringLiteralToken("Constant String");
        protected StringLiteral _constantustring = new UStringLiteralToken("Constant UString");
        protected StringLiteral _constantcharacter = new CharLiteralToken("Constant Character");
        protected NumberLiteral _constantnumber = new NumberLiteralToken<UoToken.UoConstantNumber>("Constant Number");
        protected NumberLiteral _eventchance = new NumberLiteralToken<UoToken.UoConstantNumber>("Constant Number");
        #endregion

        #region Identifiers
        protected IdentifierTerminal _functionname_int = new MethodToken<UoToken.UoTypeName_int>("UoFunctionName_int");
        protected IdentifierTerminal _functionname_str = new MethodToken<UoToken.UoTypeName_string>("UoFunctionName_string");
        protected IdentifierTerminal _functionname_ustr = new MethodToken<UoToken.UoTypeName_ustring>("UoFunctionName_ustring");
        protected IdentifierTerminal _functionname_loc = new MethodToken<UoToken.UoTypeName_loc>("UoFunctionName_loc");
        protected IdentifierTerminal _functionname_obj = new MethodToken<UoToken.UoTypeName_object>("UoFunctionName_object");
        protected IdentifierTerminal _functionname_list = new MethodToken<UoToken.UoTypeName_list>("UoFunctionName_list");
        protected IdentifierTerminal _functionname_void = new MethodToken<UoToken.UoTypeName_void>("UoFunctionName_void");

        #region Events
        protected IdentifierTerminal _eventname_sim = new IdentifierToken<UoToken.UoEventName>("UoEventName_Simple", EventNamesSimple, "This is not a valid event name for this parameter set.");
        protected IdentifierTerminal _eventname_int = new IdentifierToken<UoToken.UoEventName>("UoEventName_Integer", EventNamesInteger, "This is not a valid event name for this parameter set.");
        protected IdentifierTerminal _eventname_str = new IdentifierToken<UoToken.UoEventName>("UoEventName_String", EventNamesString, "This is not a valid event name for this parameter set.");
        static List<string> EventNamesSimple = new List<string>
        {
            "use" , "lookedat" , "creation" , "targetloc" , "targetobj" , "multirecycle" , "ooruse" , "serverswitch" , "give" , "pkpost" , "oortargetobj",
            "objectloaded" , "online" , "sawdeath" , "wasdropped" , "equip" , "unequip" , "washit" , "decay" , "destroyed" , "death" , "ishitting" , "gotattacked" , "wasgotten",
            "foundfood" , "canbuy" , "killedtarget" , "ishealthy" , "isstackableon" , "seekfood" , "seekdesire" , "seekshelter" , "mobishitting" , "logout" , "shop",
            "acquiredesire" , "famechanged" , "murdercountchanged" , "transaccountcheck" , "transresponse"
        };
        static List<string> EventNamesInteger = new List<string>
        {
            "callback" , "enterrange" , "leaverange" , "textentry" , "typeselected" , "pathfound" , "pathnotfound" , "objaccess" , "genericgump" , "hueselected"       
        };
        static List<string> EventNamesString = new List<string> 
        {
            "message", "speech", "convofunc", "time"
        };
        #endregion

        protected IdentifierTerminal _scriptname = new IdentifierToken<UoToken.UoScriptName>("UoScriptName");

        protected IdentifierTerminal _variablename_int = new FieldToken<UoToken.UoTypeName_int>("UoVariableName_int");
        protected IdentifierTerminal _variablename_str = new FieldToken<UoToken.UoTypeName_string>("UoVariableName_string");
        protected IdentifierTerminal _variablename_ustr = new FieldToken<UoToken.UoTypeName_ustring>("UoVariableName_ustring");
        protected IdentifierTerminal _variablename_loc = new FieldToken<UoToken.UoTypeName_loc>("UoVariableName_loc");
        protected IdentifierTerminal _variablename_obj = new FieldToken<UoToken.UoTypeName_object>("UoVariableName_object");
        protected IdentifierTerminal _variablename_list = new FieldToken<UoToken.UoTypeName_list>("UoVariableName_list");
        protected IdentifierTerminal _variablename_void = new FieldToken<UoToken.UoTypeName_void>("UoVariableName_void");
        
        #endregion

        #region Keywords
        //protected KeyTerm _this = new This();
        protected KeyTerm _if = new KeyToken<UoToken.UoKeyword_if>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _else = new KeyToken<UoToken.UoKeyword_else>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _for = new KeyToken<UoToken.UoKeyword_for>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _while = new KeyToken<UoToken.UoKeyword_while>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _continue = new KeyToken<UoToken.UoKeyword_continue>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _break = new KeyToken<UoToken.UoKeyword_break>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _switch = new KeyToken<UoToken.UoKeyword_switch>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _case = new KeyToken<UoToken.UoKeyword_case>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _default = new KeyToken<UoToken.UoKeyword_default>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _return = new KeyToken<UoToken.UoKeyword_return>(TermFlags.IsKeyword | TermFlags.IsReservedWord);

        protected KeyTerm _function = new KeyToken<UoToken.UoKeyword_function>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _trigger = new KeyToken<UoToken.UoKeyword_trigger>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _member = new KeyToken<UoToken.UoKeyword_member>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _inherits = new KeyToken<UoToken.UoKeyword_inherits>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _forward = new KeyToken<UoToken.UoKeyword_forward>(TermFlags.IsKeyword | TermFlags.IsReservedWord);

        protected Terminal _L;

        #endregion

        #region Types
        protected KeyTerm _int = new KeyToken<UoToken.UoTypeName_int>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _string = new KeyToken<UoToken.UoTypeName_string>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _ustring = new KeyToken<UoToken.UoTypeName_ustring>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _loc = new KeyToken<UoToken.UoTypeName_loc>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _obj = new KeyToken<UoToken.UoTypeName_object>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _list = new KeyToken<UoToken.UoTypeName_list>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        protected KeyTerm _void = new KeyToken<UoToken.UoTypeName_void>(TermFlags.IsKeyword | TermFlags.IsReservedWord);
        #endregion

        #region Operators

        protected KeyTerm _assignment = new KeyToken<UoToken.UoOperator_Assignment>();

        protected KeyTerm _logicalnegation = new KeyToken<UoToken.UoOperator_LogicalNegation>();
        protected KeyTerm _increment = new KeyToken<UoToken.UoOperator_increment>();
        protected KeyTerm _decrement = new KeyToken<UoToken.UoOperator_decrement>();

        protected KeyTerm _plus = new KeyToken<UoToken.UoOperator_Plus>();
        protected KeyTerm _minus = new KeyToken<UoToken.UoOperator_Minus>();
        protected KeyTerm _multiply = new KeyToken<UoToken.UoOperator_Multiply>();
        protected KeyTerm _divide = new KeyToken<UoToken.UoOperator_Divide>();
        protected KeyTerm _remainder = new KeyToken<UoToken.UoOperator_Remainder>();

        protected KeyTerm _isequal = new KeyToken<UoToken.UoOperator_IsEqual>();
        protected KeyTerm _isnotequal = new KeyToken<UoToken.UoOperator_IsNotEqual>();
        protected KeyTerm _lowerthen = new KeyToken<UoToken.UoOperator_LowerThen>();
        protected KeyTerm _greaterthen = new KeyToken<UoToken.UoOperator_GreaterThen>();
        protected KeyTerm _lowerthenorequal = new KeyToken<UoToken.UoOperator_LowerThenOrEqual>();
        protected KeyTerm _greaterthenorequal = new KeyToken<UoToken.UoOperator_GreaterThenOrEqual>();

        protected KeyTerm _logicaland = new KeyToken<UoToken.UoOperator_LogicalAnd>();
        protected KeyTerm _logicalor = new KeyToken<UoToken.UoOperator_LogicalOr>();
        protected KeyTerm _bitwiseexclusiveor = new KeyToken<UoToken.UoOperator_BitwiseExclusiveOr>();

        #endregion

        #region Punctuation

        protected KeyTerm _semicolon = new KeyToken<UoToken.UoSemicolon>();
        protected KeyTerm _colon = new KeyToken<UoToken.UoColon>();
        protected KeyTerm _comma = new KeyToken<UoToken.UoPunctuator_Comma>();

        #endregion
 
        #region Braces

        protected KeyTerm _openbracket = new KeyToken<UoToken.UoOperator_OpenBracket>();
        protected KeyTerm _closebracket = new KeyToken<UoToken.UoOperator_CloseBracket>();
        protected KeyTerm _openparen_op = new KeyToken<UoToken.UoOperator_OpenParenthesis>();
        protected KeyTerm _closeparen_op = new KeyToken<UoToken.UoOperator_CloseParenthesis>();

        protected KeyTerm _openparen_punc = new KeyToken<UoToken.UoPunctuator_OpenParenthesis>();
        protected KeyTerm _closeparen_punc = new KeyToken<UoToken.UoPunctuator_CloseParenthesis>();

        protected KeyTerm _openbrace = new KeyToken<UoToken.UoPunctuator_OpenBrace>();
        protected KeyTerm _closebrace = new KeyToken<UoToken.UoPunctuator_CloseBrace>();

        protected KeyTerm _openchevron = new KeyToken<UoToken.UoPunctuator_OpenChevron>();
        protected KeyTerm _closechevron = new KeyToken<UoToken.UoPunctuator_CloseChevron>();

        #endregion

        private void BracePair(KeyTerm open, KeyTerm close) 
        { 
            open.IsPairFor = close; 
            close.IsPairFor = open;
            open.SetFlag(TermFlags.IsOpenBrace);
            close.SetFlag(TermFlags.IsCloseBrace);
        }

        protected NonTerminal TypedFunction = new NonTerminal("TypedFunction");
        protected NonTerminal Function_void = new NonTerminal("Function_void");
        protected NonTerminal Function_int = new NonTerminal("Function_int");
        protected NonTerminal Function_string = new NonTerminal("Function_string");
        protected NonTerminal Function_ustring = new NonTerminal("Function_ustring");
        protected NonTerminal Function_loc = new NonTerminal("Function_loc");
        protected NonTerminal Function_obj = new NonTerminal("Function_obj");
        protected NonTerminal Function_list = new NonTerminal("Function_list");

        protected CommentTerminal blockComment = new CommentTerminal("block-comment", "/*", "*/");
        protected CommentTerminal lineComment = new CommentTerminal("line-comment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
        protected IdentifierTerminal Identifier = new IdentifierTerminal("Identifier");

        public override void CreateAstNode(ParsingContext context, ParseTreeNode nodeInfo)
        {
            // this only needs to happen once but i don't see another way to transfer the Language Options to the parse context
            if(!context.Values.ContainsKey("UOSLOptions"))
                context.Values["UOSLOptions"]=Options;
            base.CreateAstNode(context, nodeInfo);
        }

        private static Dictionary<LanguageOption, Grammar> grammars = new Dictionary<LanguageOption, Grammar>();
        public static Grammar GetGrammar(LanguageOption options)
        {
            return grammars.ContainsKey(options) ? grammars[options] : new UOSLGrammar(options);
        }

        protected LanguageOption Options { get; private set; }

        protected virtual bool CanCache { get { return false; } }


        #region Externals
        internal static KeyTerm _any = new KeyTerm("any", "any"); // This special type should only appear in functions imported from core
        internal NonTerminal ExternalParam = new NonTerminal("ExternalParam");
        internal NonTerminal ExternalParams = new NonTerminal("ExternalParams", ParametersNode.CreateFromExtern);
        internal NonTerminal Function_any = new NonTerminal("Function_any");
        internal NonTerminal ExternalDeclaration = new NonTerminal("ExternalDeclaration", typeof(UoCoreFunctionNode));
        #endregion

        internal UOSLBase(LanguageOption options=LanguageOption.Native)
        {
            if(CanCache) grammars[options] = this;

            Options = options;

            this.DefaultNodeType = typeof(ScopedNode);

            #region Declare Terminals Here

            NonGrammarTerminals.Add(blockComment);
            NonGrammarTerminals.Add(lineComment);

            Identifier.AstNodeType = typeof(UoFieldExpressionNode);

            _L = ToTerm("L");
            MarkReservedWords("L");

            #endregion

            #region Functions
            TypedFunction.Rule = Function_void | Function_int | Function_string | Function_ustring | Function_loc | Function_obj | Function_list;
            Function_void.Rule = _void + _functionname_void;
            Function_int.Rule = _int + _functionname_int;
            Function_string.Rule = _string + _functionname_str;
            Function_ustring.Rule = _ustring + _functionname_ustr;
            Function_loc.Rule = _loc + _functionname_loc;
            Function_obj.Rule = _obj + _functionname_obj;
            Function_list.Rule = _list + _functionname_list;
            #endregion

            #region Externals
            _any.SetFlag(TermFlags.IsKeyword);
            if (!Keywords.ContainsKey(_any.Text)) Keywords[_any.Text] = _any;
            ExternalParam.Rule = (_int | _string | _ustring | _loc | _obj | _list | _any | _void) + (ToTerm("&").Q()) + (Identifier.Q());
            ExternalParams.Rule = MakeStarRule(ExternalParams, _comma, ExternalParam);
            Function_any.Rule = _any + _functionname_void;  // only valid for Core Commands
            ExternalDeclaration.Rule = ToTerm("external") + (Function_any | TypedFunction) + _openparen_punc + ExternalParams + _closeparen_punc + _semicolon;
            #endregion


            BracePair(_openbracket, _closebracket);
            BracePair(_openparen_op, _closeparen_op);
            BracePair(_openparen_punc, _closeparen_punc);
            BracePair(_openbrace, _closebrace);
            BracePair(_openchevron, _closechevron);

            this.Delimiters = "{}[](),:;+-*/%&|^!~<>=";
            this.MarkPunctuation(_semicolon, _comma, _openparen_punc, _closeparen_punc, _openbrace, _closebrace, _openbracket, _closebracket, _colon);
            
            RegisterOperators(-1, _assignment);
            RegisterOperators(1, _logicalnegation, _increment, _decrement);
            RegisterOperators(0, _plus, _minus, _multiply, _divide, _remainder, _isequal, _isnotequal, _lowerthen, _greaterthen, _lowerthenorequal,
                                 _greaterthenorequal, _logicaland, _logicalor, _bitwiseexclusiveor);

            this.LanguageFlags = LanguageFlags.CreateAst;
        }

        protected struct ParseCache
        {
            public ParseCache(DateTime filedate)
            {
                FileDate = filedate;
                Tree = null;
            }
            public ParseCache(DateTime filedate, ParseTree tree)
            {
                FileDate = filedate;
                Tree = tree;
            }
            public DateTime FileDate;
            public ParseTree Tree;
        }

        internal static bool AreSameParams(IList<Parameter> a, IList<Parameter> b)
        {
            if ((a == null || a.Count == 0) != (b == null || b.Count == 0)) return false;
            if ((a == null || a.Count == 0)) return true;
            if (a.Count != b.Count) return false;

            for (int i = 0; i < a.Count; i++)
                if (a[i].UoTypeToken != b[i].UoTypeToken)
                    return false;
            return true;
        }

        public static List<Method> GetFuncs(ParsingContext context)
        {
            if (!context.Values.ContainsKey("Funcs"))
                context.Values["Funcs"] = new List<Method>();
            return (List<Method>)context.Values["Funcs"];
        }

        public static void AddFunc(Method method, ParsingContext context)
        {
            List<Method> ScopeFuncs = GetFuncs(context);

            Method found;

            if (UOSLGrammar.Keywords.ContainsKey(method.Name) && context.GetOptions()==LanguageOption.Extended)
                context.AddParserMessage(ParserErrorLevel.Error, (method.DefNode ?? method.ForwardNode).Span, "{0} is a keyword. It cannot be used as a Method name.", method.Name);

            if (ScopeFuncs != null && (found = ScopeFuncs.FirstOrDefault(func => func.Name == method.Name)) != null)
            {
                if (found.DefNode == null || found.DefFilename == context.CurrentParseTree.FileName)
                    context.AddParserMessage(ParserErrorLevel.Error, (method.DefNode ?? method.ForwardNode).Span, "Function {0} is already {1} in this file.", method.Name, method.DefNode == null ? "declared" : "defined");
                else
                {
                    if (AreSameParams(method.Parameters, found.Parameters))
                    {
                        string file;
                        if(!string.IsNullOrEmpty( found.DefFilename) && File.Exists(found.DefFilename))
                            file=Path.GetFileName(found.DefFilename);
                        else
                            file="Unknown";

                        context.AddParserMessage(ParserErrorLevel.Info, method.DefNode.Span, "Function {0} overrides function in {1}.", method.Name, file);
                        ScopeFuncs.Remove(found);
                        ScopeFuncs.Add(method);
                    }
                    else
                        context.AddParserMessage(ParserErrorLevel.Error, method.DefNode.Span, "Function {0}: cannot override from {1} with different parameters.", method.Name);

                }
            }
            else
                ScopeFuncs.Add(method);
        }

        public static List<Field> GetMembers(ParsingContext context)
        {
            if (!context.Values.ContainsKey("Members"))
            {
                List<Field> list=new List<Field>();
                list.Add(new Field("this", Types.Obj, null, context));
                context.Values["Members"] = list;
            }
            return (List<Field>)context.Values["Members"];
        }

        public static void AddMember(Field field, ParsingContext context)
        {
            List<Field> ScopeVars = GetMembers(context);

            LanguageOption option = context.GetOptions();

            if (UOSLGrammar.Keywords.ContainsKey(field.Name) && option == LanguageOption.Extended)
                context.AddParserMessage(ParserErrorLevel.Info, field.Node.Span, "{0} is a keyword. It cannot be used as a Field name.", field.Name);

            Field found;
            if (ScopeVars != null && (found = ScopeVars.FirstOrDefault(var => var.Name == field.Name)) != null)
            {
                if(found.Node==null)
                    context.AddParserMessage(ParserErrorLevel.Error, field.Node.Span, "Implicit Member Declaration {0} cannot be redefined.", found.Name);
                else if (found.UoTypeToken != field.UoTypeToken)
                    context.AddParserMessage(ParserErrorLevel.Error, found.Node.Span, "Member Declaration: Field {0} is already defined as a different type.", found.Name);
                else
                    context.AddParserMessage(option == LanguageOption.Extended ? ParserErrorLevel.Error : ParserErrorLevel.Info, found.Node.Span, "Member Redeclaration of Field {0}.", found.Name);
            }
            ScopeVars.Add(field);
        }

        protected static Dictionary<string, ParseCache> Inherits = new Dictionary<string, ParseCache>();

        /// <summary>
        /// Used to re-cache latest parse tree when the tree is active in a buffer. and the file is in the cached due to being inherited by other files.
        /// </summary>
        /// <param name="filepath">This is the filename of the file, and is used as the key for cashing the parse result</param>
        /// <param name="newTree">The updated parsetree</param>
        /// <remarks>The tree will not be updated if the existing tree does not have errors</remarks>
        public static void ReCache(string filepath, ParseTree newTree)
        {
            if (Inherits.ContainsKey(filepath) /*&& Inherits[filepath].Tree.ParserMessages.Where(error => error.Level == ParserErrorLevel.Error).Count() > 0*/)
                Inherits[filepath] = new ParseCache(DateTime.Now, newTree);
        }


        internal void LoadFile(string path, ScopedNode node, ParsingContext context, IDictionary<string,string> refDepends=null)
        {
            string curExt = context.CurrentParseTree.FileName == "<Source>" ? null : Path.GetExtension(context.CurrentParseTree.FileName);
            string fullpath
                = curExt == null ? null : Utils.FindFile(string.Format("{0}.{1}", path, curExt), context.CurrentParseTree.FileName)
                ?? Utils.FindFile(path, context.CurrentParseTree.FileName)
                ?? Utils.FindFile(string.Format("{0}.uosl", path), context.CurrentParseTree.FileName)
                ?? Utils.FindFile(string.Format("{0}.uosl.q", path), context.CurrentParseTree.FileName);
            
            if (fullpath!=null)
            {
                fullpath = (new FileInfo(fullpath)).FullName;

                FileInfo fi = new FileInfo(fullpath);

                ParseCache cache;

                if (!Inherits.TryGetValue(fullpath, out cache) || fi.LastWriteTime > cache.FileDate )
                {
                    Inherits[fullpath] = cache = new ParseCache(fi.LastWriteTime);

                    LanguageOption options = Utils.DetermineFileLanguage(fullpath, this.Options);

                    ParsingContext subcontext = new ParsingContext(new Parser(options == this.Options ? this : GetGrammar(options)));

                    using (StreamReader reader = new StreamReader(fullpath))
                    {
                        Inherits[fullpath] = cache = new ParseCache(fi.LastWriteTime, subcontext.Parser.Parse(reader.ReadToEnd(), fullpath));
                    }
                }

                if (refDepends != null)
                    refDepends.Add(path, fullpath);

                if (cache.Tree != null)
                {
                    if (cache.Tree.HasErrors())
                        foreach (ParserMessage error in cache.Tree.ParserMessages)
                        {
                            if (error is ExternalErrorMessage)
                                context.AddParserMessage(error);
                            else
                                context.AddParserMessage(new ExternalErrorMessage(fullpath, error.Level, error.Location, error.Message, error.ParserState));
                        }
                    if (cache.Tree.Root !=null && cache.Tree.Root.AstNode != null)
                    {
                        if (((ScopedNode)cache.Tree.Root.AstNode).ScopeVars != null)
                            foreach (Field field in ((ScopedNode)cache.Tree.Root.AstNode).ScopeVars)
                                node.AddVar(field, context);
                        if (((ScopedNode)cache.Tree.Root.AstNode).TreeFuncs != null)
                            foreach (Method method in ((ScopedNode)cache.Tree.Root.AstNode).TreeFuncs)
                                AddFunc(method, context);

                        if (refDepends != null && cache.Tree.Root.FirstChild.AstNode is DeclarationsNode)
                            foreach (var kvp in ((DeclarationsNode)cache.Tree.Root.FirstChild.AstNode).Depends)
                            {
                                if (refDepends.ContainsKey(kvp.Key))
                                    context.AddParserMessage(ParserErrorLevel.Error, node.Span, "A recursion of inheritance detected occurred when parsing {0}.", kvp.Key);
                                else
                                    refDepends.Add(kvp.Key, kvp.Value);
                            }
                    }
                }

            }
            else
                context.AddParserError("{0} Dependancy not found.", path);
        }

    }
}
