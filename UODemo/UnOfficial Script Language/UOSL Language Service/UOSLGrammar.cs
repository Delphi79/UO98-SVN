using System;
using System.Collections.Generic;
using Irony.Parsing;
using JoinUO.UOdemoSDK;
using JoinUO.UOSL.Service.ASTNodes;
using System.Linq;


namespace JoinUO.UOSL.Service
{
    // Based on Microsost Visual Studio SDK MyC 
    [Language("UOSL", "2.0", "UnOfficial Script Language")]
    [CLSCompliant(false)]
    public partial class UOSLGrammar : UOSLBase
    {
        public void ScriptNodeCreator(ParsingContext context, ParseTreeNode parseNode)
        {
            ScopedNode node = new ScopedNode();
            node.Init(context, parseNode);
            parseNode.AstNode = node;

            foreach (ParseTreeNode cnode in parseNode.ChildNodes)
                if (cnode.Term.Name == "Declarations")
                {
                    node.ChildNodes.Add((ScopedNode)cnode.AstNode);
                    ((ScopedNode)cnode.AstNode).Parent = node;

                    node.m_LocalVars = GetMembers(context);
                }

            // Initialize a check of types and scope on the script tree
            node.CheckTree(context);
        }

        public void DeclarationsNodeCreator(ParsingContext context, ParseTreeNode parseNode)
        {
            ScopedNode node = new DeclarationsNode(this);
            node.Init(context, parseNode);
            parseNode.AstNode = node;
        }

        protected override bool CanCache { get { return true; } }

        public UOSLGrammar() : this(LanguageOption.Native) { }
        public UOSLGrammar(LanguageOption options) : base(options)
        {
            NonTerminal Expression = new NonTerminal("Expression", typeof(ExpressionNode));
            NonTerminal ExpressionPara = new NonTerminal("ExpressionPara", typeof(ExpressionNode));
            NonTerminal PrimaryExpression = new NonTerminal("PrimaryExpression", typeof(ExpressionNode));
            
            NonTerminal ExpressionOptPara = new NonTerminal("ExpressionOptPara");

            NonTerminal FunctionInvocation = new NonTerminal("FunctionInvocation", typeof(FunctionInvocationNode));

            NonTerminal ExpressionList = new NonTerminal("ExpressionList", typeof(ExpressionNode));

            NonTerminal Statement = new NonTerminal("Statement");

            NonTerminal Statements = new NonTerminal("Statements");

            NonTerminal Block = new NonTerminal("Block", typeof(BlockNode));

            NonTerminal Script = new NonTerminal("Script", ScriptNodeCreator);

            NonTerminal EventDeclaration = new NonTerminal("EventDeclaration", typeof(EventDefNode));
            NonTerminal EventDeclarationNative = new NonTerminal("EventDeclaration", typeof(EventDefNativeNode));

            NonTerminal ForwardDeclaration = new NonTerminal("ForwardDeclaration", typeof(FunctionProtoNode));
            NonTerminal FunctionDeclaration = new NonTerminal("FunctionDeclaration", "function", typeof(FunctionDefNode));

            NonTerminal ConstantDeclaration = new NonTerminal("ConstantDeclaration", typeof(ConstantDeclarationNode));

            NonTerminal VariableDeclarationSemi = new NonTerminal("VariableDeclarationSemi", typeof(VariableDeclarationNode));
            NonTerminal VariableDeclaration = new NonTerminal("VariableDeclaration", typeof(VariableDeclarationNode));

            NonTerminal InheritsDeclaration = new NonTerminal("InheritsDeclaration", typeof(InheritsNode));
            NonTerminal MemberDeclaration = new NonTerminal("MemberDeclaration", typeof(MemberDeclarationNode));

            NonTerminal SimpleDeclaration = new NonTerminal("SimpleDeclaration");

            NonTerminal SimpleDeclaration_int = new UoIdentifier<UoToken.UoTypeName_int>() { AstNodeType = typeof(DeclarationNode) };
            NonTerminal SimpleDeclaration_str = new UoIdentifier<UoToken.UoTypeName_string>() { AstNodeType = typeof(DeclarationNode) };
            NonTerminal SimpleDeclaration_ustr = new UoIdentifier<UoToken.UoTypeName_ustring>() { AstNodeType = typeof(DeclarationNode) };
            NonTerminal SimpleDeclaration_loc = new UoIdentifier<UoToken.UoTypeName_loc>() { AstNodeType = typeof(DeclarationNode) };
            NonTerminal SimpleDeclaration_obj = new UoIdentifier<UoToken.UoTypeName_object>() { AstNodeType = typeof(DeclarationNode) };
            NonTerminal SimpleDeclaration_list = new UoIdentifier<UoToken.UoTypeName_list>() { AstNodeType = typeof(DeclarationNode) };

            NonTerminal Declaration = new NonTerminal("Declaration");
            NonTerminal Declarations = new NonTerminal("Declarations", DeclarationsNodeCreator);

            NonTerminal Parameters = new NonTerminal("Parameters", typeof(ParametersNode));
            NonTerminal ParenParameters = new NonTerminal("ParenParameters"); // transitive

            NonTerminal Arguments = new NonTerminal("Arguments");
            NonTerminal ParenArguments = new NonTerminal("ParenArguments"); // transitive

            Root = Script;
            Script.Rule = Declarations;

            NonTerminal Assign = new NonTerminal("Assign", typeof(AssignNode));
            Assign.Rule = (_assignment | _isequal) + ExpressionList;

            // Saving this for version 2.0, it's just not implementable in this current grammar.
            //#region Pragma

            //NonTerminal Pragma = new NonTerminal("Pragma");
            //Terminal _pragma = new PragmaTerminal("pragma");
 
            //NonTerminal PragmaLanguage = new NonTerminal("PragmaLanguage");
            //Terminal _language = new PragmaTerminal("language");

            //Terminal _lang_native = new LanguagePragmaTerminal("native");
            //Terminal _lang_enhanced = new LanguagePragmaTerminal("enhanced");
            //Terminal _lang_extended = new LanguagePragmaTerminal("extended");
            //NonTerminal language_option = new NonTerminal("language_option");

            //language_option.Rule = _lang_native | _lang_enhanced | _lang_extended;
            //MarkTransient(language_option);

            //Pragma.Rule = _pragma + PragmaLanguage;
            //PragmaLanguage.Rule = _language + language_option;

            //#endregion

            #region Declarations

            Declarations.Rule = MakeStarRule(Declarations, null, Declaration);
            Declaration.Rule = /*Pragma |*/ InheritsDeclaration | MemberDeclaration | ForwardDeclaration | ExternalDeclaration | (options.HasOption(LanguageOption.UOSLTriggers) ? EventDeclaration : EventDeclarationNative) | FunctionDeclaration | ConstantDeclaration;

            InheritsDeclaration.Rule = _inherits + _scriptname + _semicolon;

            MemberDeclaration.Rule = _member + VariableDeclarationSemi + _semicolon;
            SimpleDeclaration.Rule = SimpleDeclaration_int | SimpleDeclaration_str | SimpleDeclaration_ustr | SimpleDeclaration_loc | SimpleDeclaration_obj | SimpleDeclaration_list;

            SimpleDeclaration_int.Rule = _int + _variablename_int;
            SimpleDeclaration_str.Rule = _string + _variablename_str;
            SimpleDeclaration_ustr.Rule = _ustring + _variablename_ustr;
            SimpleDeclaration_loc.Rule = _loc + _variablename_loc;
            SimpleDeclaration_obj.Rule = _obj + _variablename_obj;
            SimpleDeclaration_list.Rule = _list + _variablename_list;

            #region Functions

            // See UOSL grammar base also...

            NonTerminal ParametersIdentOpt = new NonTerminal("ParenParametersIdentOpt", ParametersNode.CreateFromForward);
            NonTerminal ParameterIdentOpt = new NonTerminal("ParenParametersIdentOpt");
            ParameterIdentOpt.Rule= (_int | _string | _ustring | _loc | _obj | _list) + Identifier.Q();
            ParametersIdentOpt.Rule = MakeStarRule(ParametersIdentOpt, _comma, ParameterIdentOpt, TermListOptions.None);
            
            ForwardDeclaration.Rule = _forward + TypedFunction + _openparen_punc + ParametersIdentOpt + _closeparen_punc + _semicolon;

            FunctionDeclaration.Rule = _function + TypedFunction + ParenParameters + Block;

            ParenParameters.Rule = _openparen_punc + Parameters + _closeparen_punc;

            Parameters.Rule = MakeStarRule(Parameters, _comma, SimpleDeclaration);

            ParenArguments.Rule = _openparen_punc + Arguments + _closeparen_punc;
            Arguments.Rule = MakeStarRule(Arguments, _comma, Expression, TermListOptions.AllowTrailingDelimiter);

            #endregion

            #region Constants

            Terminal _const = ToTerm("const");
            MarkReservedWords("const");
            ConstantDeclaration.Rule = _const + VariableDeclaration;

            #endregion

            #region Variables

            VariableDeclarationSemi.Rule = SimpleDeclaration | (SimpleDeclaration + Assign);
            VariableDeclaration.Rule = VariableDeclarationSemi + _semicolon;

            #endregion

            #region Events

            NonTerminal EventSimple = new NonTerminal("EventSimple", typeof(EventDecNode<UoToken.UoTypeName_void>));
            EventSimple.Rule = _eventname_sim;
 
            #region UOSL Triggers

            if (options.HasOption(LanguageOption.UOSLTriggers))
            {
                NonTerminal EventChance = new NonTerminal("EventChance", typeof(EventParamNode<UoToken.UoTypeName_int>));
                NonTerminal EventChanceOpt = new NonTerminal("EventChanceOpt");
                NonTerminal EventParam_int = new NonTerminal("EventParam_int", typeof(EventParamNode<UoToken.UoTypeName_int>));
                NonTerminal EventParam_str = new NonTerminal("EventParam_str", typeof(EventParamNode<UoToken.UoTypeName_string>));

                NonTerminal TypedEvent = new NonTerminal("TypedEvent");
                NonTerminal EventInt = new NonTerminal("EventInt", typeof(EventDecNode<UoToken.UoTypeName_int>));
                NonTerminal EventStr = new NonTerminal("EventStr", typeof(EventDecNode<UoToken.UoTypeName_string>));

                EventInt.Rule = _eventname_int + EventParam_int;
                EventStr.Rule = _eventname_str + EventParam_str;
                TypedEvent.Rule = EventSimple | EventStr | EventInt;

                EventChanceOpt.Rule = Empty | EventChance;

                EventChance.Rule = _openchevron + _eventchance + _closechevron;
                EventParam_int.Rule = _openchevron + _constantnumber + _closechevron;
                EventParam_str.Rule = _openchevron + _constantstring + _closechevron;
                EventDeclaration.Rule = _trigger + EventChanceOpt + TypedEvent + ParenParameters + Block;

                MarkTransient(TypedEvent);
            }
            #endregion
            else
            #region native triggers
            {
                NonTerminal TypedEventNative = new NonTerminal("TypedEventNative");
                NonTerminal EventChanceNative = new NonTerminal("EventChanceNative", typeof(EventParamNode<UoToken.UoTypeName_int>));
                NonTerminal EventChanceNativeOpt = new NonTerminal("EventChanceNativeOpt");
                NonTerminal EventParamNative_int = new NonTerminal("EventParamNative_int", typeof(EventParamNode<UoToken.UoTypeName_int>));
                NonTerminal EventParamNative_str = new NonTerminal("EventParamNative_str", typeof(EventParamNode<UoToken.UoTypeName_string>));
                NonTerminal EventNativeInt = new NonTerminal("EventNativeInt", typeof(EventDecNode<UoToken.UoTypeName_int>));
                NonTerminal EventNativeStr = new NonTerminal("EventNativeStr", typeof(EventDecNode<UoToken.UoTypeName_string>));

                EventNativeInt.Rule = _eventname_int + EventParamNative_int;
                EventNativeStr.Rule = _eventname_str + EventParamNative_str;
                TypedEventNative.Rule = EventSimple | EventNativeStr | EventNativeInt;

                EventChanceNativeOpt.Rule = Empty | EventChanceNative;
                EventChanceNative.Rule = _eventchance;
                EventParamNative_int.Rule = _openparen_punc + _constantnumber + _closeparen_punc;
                EventParamNative_str.Rule = _openparen_punc + _constantstring + _closeparen_punc;
                EventDeclarationNative.Rule = _trigger + EventChanceNativeOpt + TypedEventNative + Block;

                MarkTransient(TypedEventNative);

            }
            #endregion

            #endregion

            #endregion

            #region Statements

            NonTerminal SemiStatement = new NonTerminal("SemiStatement");
            NonTerminal Assignment = new NonTerminal("Assignment", typeof(AssignmentNode));

            // Switch
            NonTerminal Switch = new NonTerminal("Switch", typeof(SwitchNode));
            NonTerminal SwitchSections = new NonTerminal("SwitchSectionsOpt");
            NonTerminal SwitchSection = new NonTerminal("SwitchSection",typeof(SwitchSectionNode));
            NonTerminal SwitchLabels = new NonTerminal("SwitchLabels");
            NonTerminal SwitchLabel = new NonTerminal("SwitchLabel");

            Switch.Rule = _switch + ExpressionPara + _openbrace + SwitchSections + _closebrace;
            SwitchSection.Rule = SwitchLabels + Statements;
            SwitchSections.Rule = MakePlusRule(SwitchSections, null, SwitchSection);
            NonTerminal OptColon = new NonTerminal("OptColon");
            OptColon.Rule = _colon.Q(); // the colon is optional in UOSL Native
            SwitchLabel.Rule = _case + PrimaryExpression + OptColon | _default + OptColon;  
            SwitchLabels.Rule = MakePlusRule(SwitchLabels, null, SwitchLabel);

            // For
            NonTerminal For = new NonTerminal("For", typeof(ForNode));
            NonTerminal ForHeader = new NonTerminal("forHeader");
            NonTerminal ForBlock = new NonTerminal("forBlock");

            For.Rule=_for + ForHeader + Statement;
            ForHeader.Rule = _openparen_punc + ForBlock + _closeparen_punc;
            ForBlock.Rule = SemiStatement.Q() + _semicolon + Expression.Q() + _semicolon + SemiStatement.Q();

            // If
            NonTerminal If = new NonTerminal("If", typeof(IfNode));
            NonTerminal Else = new NonTerminal("Else");
            NonTerminal ElseOpt = new NonTerminal("Else");
            If.Rule = _if + ExpressionPara + Statement + ElseOpt;
            ElseOpt.Rule = Empty | PreferShiftHere() + Else;
            Else.Rule = _else + Statement;

            // While
            NonTerminal While = new NonTerminal("While", typeof(WhileNode));
            While.Rule = _while + ExpressionPara + Statement;

            // Jumps
            NonTerminal Return = new NonTerminal("Return", typeof(ReturnNode));
            NonTerminal Break = new NonTerminal("Break", typeof(JumpNode));
            NonTerminal Continue = new NonTerminal("Continue", typeof(JumpNode));

            Return.Rule = _return + (ExpressionOptPara.Q() | Expression.Q());
            Break.Rule = _break;
            Continue.Rule = _continue;

            Statements.Rule = MakePlusRule(Statements, null, Statement);
            Statement.ErrorRule = SyntaxError + _semicolon; //skip all until semicolon
            Statement.Rule
                = SemiStatement + _semicolon
                | _semicolon
                | MemberDeclaration             // Can be anywhere, variable is treated as global, but is initialized locally.
                | While
                | For
                | If
                | Switch
                | Block
                ;

            SemiStatement.Rule 
                = Return
                | Break
                | Expression    // error
                | Continue
                | VariableDeclarationSemi
                | Assignment
                | FunctionInvocation
                ;

            Block.Rule = _openbrace + Statements.Q() + _closebrace;

            #endregion

            #region Expressions

            NonTerminal BinaryExpression = new NonTerminal("Binary", typeof(BinaryNode));
            NonTerminal UnaryPrefixedExpression = new NonTerminal("UnaryPrefixed", typeof(UnaryPrefixedNode));

            NonTerminal IndexedExpression = new NonTerminal("Index", typeof(IndexedNode));

            NonTerminal UnaryOperator = new NonTerminal("UnaryOperator", typeof(ExpressionNode));
            NonTerminal UnaryAssignment = new NonTerminal("UnaryAssignment", typeof(UnaryAssign));
            NonTerminal UnaryAssignmentOperator = new NonTerminal("UnaryAssignmentOperator");
            NonTerminal indexer_specifier = new NonTerminal("indexer_specifier");
            NonTerminal indexer_specifier_opt = new NonTerminal("indexer_specifier_opt");

            indexer_specifier.Rule = _openbracket + Expression + _closebracket;
            indexer_specifier_opt.Rule = indexer_specifier.Q();

            Assignment.Rule = (Identifier + Assign) | UnaryAssignment;

            IndexedExpression.Rule = (Identifier | IndexedExpression) + indexer_specifier;
            UnaryPrefixedExpression.Rule = UnaryOperator + PrimaryExpression;

            // A regular expression is not naturally descendant, as this is not allowed for index specifiers.
            Expression.Rule
                = PrimaryExpression
                | IndexedExpression | BinaryExpression
                ;

            ExpressionPara.Rule = _openparen_punc + Expression + _closeparen_punc;
            ExpressionOptPara.Rule = _openparen_punc + Expression.Q() + _closeparen_punc;

            ExpressionList.Rule = MakePlusRule(ExpressionList, _comma, Expression);

            FunctionInvocation.Rule = Identifier + ParenArguments;

            #region Factors

            PrimaryExpression.Rule 
                = Identifier
                | FunctionInvocation
                | _constantnumber
                | _constantstring
                | _constantcharacter
                | (_L + _constantustring)
                //| _this
                | ExpressionPara
                | UnaryPrefixedExpression
                ;


            #endregion

            #region Unary

            UnaryOperator.Rule = _logicalnegation;

            UnaryAssignmentOperator.Rule = _increment | _decrement;
            UnaryAssignment.Rule = Identifier + UnaryAssignmentOperator;

            #endregion

            #region Binary
            NonTerminal BinaryOperator = new NonTerminal("BinaryOperator");
            BinaryOperator.Rule = _plus | _minus | _multiply | _divide | _remainder
                | _isequal | _isnotequal | _greaterthen | _lowerthen
                | _greaterthenorequal | _lowerthenorequal | _logicaland | _logicalor
                ;

            BinaryExpression.Rule = Expression + BinaryOperator + PrimaryExpression;

            #endregion

            #endregion

            MarkTransient(Declaration, Statement, SimpleDeclaration);
            MarkTransient(ParenParameters, ParenArguments);

            this.LineTerminators = "\r\n\u2085\u2028\u2029"; //CR, linefeed, nextLine, LineSeparator, paragraphSeparator
            this.WhitespaceChars = " \t\r\n\v\u2085\u2028\u2029"; //add extra line terminators
        }

    }
}
