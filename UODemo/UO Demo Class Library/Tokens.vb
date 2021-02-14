' UO Demo Class Library / UODemoSDK : A general class library for the UO Demo
' Copyright ©  2010-2011 JoinUO | batlin@joinuo.com 
' Licensed under the Open Software License version 3.0 (COPYING.txt).

Public Class UoToken
  Enum UoScriptByte
    OpenParenthesis = &H2
    CloseParenthesis = &H3
    Comma = &H4
    SemiColon = &H6
    OpenBrace = &H7
    CloseBrace = &H8
    OpenBracket = &H9
    CloseBracket = &HA
    LogicalNegation = &HB
    Plus = &HC
    Minus = &HD
    Multiply = &HE
    Divide = &HF
    Remainder = &H10
    IsEqual = &H11
    IsNotEqual = &H12
    LowerThen = &H13
    GreaterThen = &H14
    LowerThenOrEqual = &H15
    GreaterThenOrEQual = &H16
    Assigment = &H17
    LogicalAnd = &H18
    LogicalOr = &H19
    BitwiseExclusiveOr = &H1A
    PlusPlus = &H1B
    MinisMinus = &H1C
    IntegerType = &H1D
    StringType = &H1E
    UstType = &H1F
    LocationType = &H20
    ObjectType = &H21
    ListType = &H22
    VoidType = &H23
    IfKeyword = &H25
    ElseKeyword = &H26
    WhileKeyword = &H28
    ForKeyword = &H2A
    ContinueKeyword = &H2C
    BreakKeyword = &H2D
    SwitchKeyword = &H2F
    CaseKeyword = &H31
    DefaultKeyword = &H32
    ReturnKeyword = &H33
    FunctionKeyword = &H34
    TriggerKeyword = &H35
    MemberKeyword = &H36
    InheritsKeyword = &H37
    ForwardKeyword = &H38
    QuotedStringConstant = &H3A
    SingleByteConstant = &H3C
    DoubleByteConstant = &H3D
    QuadByteConstant = &H3E
    StringConstant = &H41
  End Enum

  Protected _value As String = Nothing
  Public Overridable Property Value As String
    Get
      Return _value
    End Get
    Set(ByVal value As String)
      Throw New NotImplementedException
    End Set
  End Property

  Public Overridable ReadOnly Property IsLiteral As Boolean
    Get
      Return True
    End Get
  End Property

  Protected _binary As Byte() = Nothing
  Public Overridable ReadOnly Property BinaryValue As Byte()
        Get
            Return _binary
        End Get
    End Property

  Public Class UoComment
    Inherits UoToken

    Public Sub New(ByVal v As String)
      Value = v
    End Sub

    Public Overrides Property Value As String
      Get
        Return _value
      End Get
      Set(ByVal value As String)
        ' 'comments' don't have a binary value
        _binary = Nothing

        ' Ensure that our 'comment' starts with "//"
        If Not value.StartsWith("//") Then
          If Not value.StartsWith("/*") Then
            Throw New InvalidCastException("A comment must begin with // or be enclosed between /* and */.")
          End If
          If Not value.EndsWith("*/") Then
            Throw New InvalidCastException("A comment beginning with /* must end with */.")
          End If
        End If
        _value = value
      End Set
    End Property
  End Class

  Public Class UoConstant
    Inherits UoToken
  End Class

  Public Class UoConstantCharacter
    Inherits UoConstant

    Public Sub New(ByVal v As Char)
      Value = String.Format("'{0}'", v.ToString)
    End Sub

    Public Overrides Property Value As String
      Get
        Return _value
      End Get
      Set(ByVal value As String)
        Dim v As Char

        ' The character must be valid
        If value Is Nothing OrElse value.Length <= 0 Then
          Throw New InvalidCastException("A character was expected.")
        End If
        If value.IndexOf("'"c) <> 0 Then
          Throw New InvalidCastException("A character should start with '-character.")
        End If
        If value.LastIndexOf("'"c) <> value.Length - 1 Then
          Throw New InvalidCastException("A character should end with '-character.")
        End If
        If value.Length <> 3 Then
          If value.Length <> 4 Then
            Throw New InvalidCastException("Invalid character length.")
          End If
          If value(1) <> "\"c Then
            If value(1) <> "\"c Then
              Throw New InvalidCastException("The \-character was expected.")
            End If
          End If
          Select Case value(2)
            Case "b"c
              value = vbBack
            Case "t"c
              value = vbTab
            Case "n"
              value = vbLf
            Case "r"c
              value = vbCr
            Case "'"c
              value = "'"c
            Case Else
              Throw New InvalidCastException("Only \b,\t,\n,\r,\'-character sequences are supported.")
          End Select
        Else
          v = value(1)
          If v = "'"c Then
            Throw New InvalidCastException("Use \'-character sequence if you want the '-character!")
          End If
          If Asc(v) < 32 Then
            Throw New InvalidCastException("Use \-character prefix for special characters!")
          End If
        End If

        ' Ensure that the character is in the valid range
        Dim av As Integer = Asc(v)
        Select Case av
          Case 8 ' backspace
            _value = "'\b'"
          Case 9 ' tab
            _value = "'\t'"
          Case 10 ' line feed
            _value = "'\n'"
          Case 13 ' carriage return
            _value = "'\r'"
          Case 39
            _value = "'\''"
          Case Else
            If av < 32 AndAlso av > 255 Then
              Throw New InvalidCastException
            End If
            _value = "'"c & IIf(v = "'"c, "\", "") & value & "'"c
        End Select

        ' Set-up the binary sequence
        _binary = New Byte() {&H0, av}
      End Set
    End Property
  End Class

  Public Class UoConstantString
    Inherits UoConstant

    Public Sub New(ByVal v As String)
      Value = v
    End Sub

    Public Overrides Property Value As String
      Get
        Return _value
      End Get
      Set(ByVal value As String)
        ' The string must be valid
        If value Is Nothing Then
          Throw New InvalidCastException("Invalid string content! Are all SdbIndexes valid?")
        End If
        If Not ((value.StartsWith("""") OrElse value.StartsWith("L""")) AndAlso value.EndsWith("""")) Then
          Throw New InvalidCastException("A string must be enclosed between ""'s")
        End If

        _value = value
      End Set
    End Property
  End Class

  Public Class UoConstantNumber
    Inherits UoConstant

    Public Sub New(ByVal v As Int32)
      Value = v.ToString
    End Sub

    Public Overrides Property Value As String
      Get
        Return _value
      End Get
      Set(ByVal value As String)
        Dim x As Int32 = Int32.Parse(value)
        If x < 256 Then
          _value = "0x" & x.ToString("X2")
        ElseIf x < 65536 Then
          _value = "0x" & x.ToString("X4")
        Else
          _value = "0x" & x.ToString("X8")
        End If
      End Set
    End Property
  End Class

  Public Class UoConstantEventChance
    Inherits UoConstantNumber

    Public Sub New(ByVal v As Int32)
      ' Forward
      MyBase.New(v)
    End Sub
  End Class

  Public Class UoIdentifier
    Inherits UoToken

    Public Overrides ReadOnly Property IsLiteral As Boolean
      Get
        Return False
      End Get
    End Property

    Public Shared Function IsValidName(ByVal name As String) As Boolean
      ' TODO: verify
      Return True
    End Function
  End Class

  Public Class UoFunctionName
    Inherits UoIdentifier

    Public Sub New(ByVal value As String)
      If Not IsValidName(value) Then
        Throw New InvalidCastException("You must supply a valid function name.")
      End If

      _value = value
      _binary = New Byte() {UoScriptByte.StringConstant}
    End Sub
  End Class

  Public Class UoEventName
    Inherits UoIdentifier

    Public Sub New(ByVal value As String)
      If Not IsValidName(value) Then
        Throw New InvalidCastException("You must supply a valid event name.")
      End If

      Dim ec As Byte = Events.GetEventCode(value)
      If ec < 0 Then
        Throw New InvalidCastException("You must supply a supported event name.")
      End If

      _value = value
      _binary = New Byte() {UoScriptByte.StringConstant}
    End Sub
  End Class

  Public Class UoScriptName
    Inherits UoIdentifier

    Public Sub New(ByVal value As String)
      If Not IsValidName(value) Then
        Throw New InvalidCastException("You must supply a valid script name.")
      End If

      _value = value
      _binary = New Byte() {UoScriptByte.StringConstant}
    End Sub
  End Class

  Public Class UoVariableName
    Inherits UoIdentifier

    Public Sub New(ByVal value As String)
      If Not IsValidName(value) Then
        Throw New InvalidCastException("You must supply a valid variable name.")
      End If

      _value = value
      _binary = New Byte() {UoScriptByte.StringConstant}
    End Sub
  End Class

  Public Class UoKeyword
    Inherits UoToken
  End Class

  Public Class UoKeyword_this
    Inherits UoKeyword

    ' NOTE: the "this" keyword is reserved and requires special treatment by a compiler
    Public Sub New()
      _value = "this"
      _binary = New Byte() {UoScriptByte.StringConstant}
    End Sub
  End Class

  Public Class UoKeyword_if
    Inherits UoKeyword

    Public Sub New()
      _value = "if"
      _binary = New Byte() {UoScriptByte.IfKeyword}
    End Sub
  End Class

  Public Class UoKeyword_else
    Inherits UoKeyword

    Public Sub New()
      _value = "else"
      _binary = New Byte() {UoScriptByte.ElseKeyword}
    End Sub
  End Class

  Public Class UoKeyword_for
    Inherits UoKeyword

    Public Sub New()
      _value = "for"
      _binary = New Byte() {UoScriptByte.ForKeyword}
    End Sub
  End Class

  Public Class UoKeyword_while
    Inherits UoKeyword

    Public Sub New()
      _value = "while"
      _binary = New Byte() {UoScriptByte.WhileKeyword}
    End Sub
  End Class

  Public Class UoKeyword_continue
    Inherits UoKeyword

    ' NOTE:
    ' - the "break" keyword should be followed by a UoSemiColon token
    ' - can only exist within a switch statement? (or for/while loop?)
    Public Sub New()
      _value = "continue"
      _binary = New Byte() {UoScriptByte.ContinueKeyword}
    End Sub
  End Class

  Public Class UoKeyword_break
    Inherits UoKeyword

    ' NOTE:
    ' - the "break" keyword should be followed by a UoSemiColon token
    ' - can only exist within a switch statement? (or for/while loop?)
    Public Sub New()
      _value = "break"
      _binary = New Byte() {UoScriptByte.BreakKeyword}
    End Sub
  End Class

  Public Class UoKeyword_switch
    Inherits UoKeyword

    Public Sub New()
      _value = "switch"
      _binary = New Byte() {UoScriptByte.SwitchKeyword}
    End Sub
  End Class

  Public Class UoKeyword_case
    Inherits UoKeyword

    ' NOTE:
    ' - the "case" keyword should be followed by a UoColon token
    ' - can only exist within a switch statement
    Public Sub New()
      _value = "case"
      _binary = New Byte() {UoScriptByte.CaseKeyword}
    End Sub
  End Class

  Public Class UoKeyword_default
    Inherits UoKeyword

    ' NOTE:
    ' - the "default" keyword should be followed by a UoColon token
    ' - can only exist within a switch statement
    Public Sub New()
      _value = "default"
      _binary = New Byte() {UoScriptByte.DefaultKeyword}
    End Sub
  End Class

  Public Class UoKeyword_return
    Inherits UoKeyword

    Public Sub New()
      _value = "return"
      _binary = New Byte() {UoScriptByte.ReturnKeyword}
    End Sub
  End Class

  Public Class UoKeyword_function
    Inherits UoKeyword

    Public Sub New()
      _value = "function"
      _binary = New Byte() {UoScriptByte.FunctionKeyword}
    End Sub
  End Class

  Public Class UoKeyword_trigger
    Inherits UoKeyword

    Public Sub New()
      _value = "trigger"
      _binary = New Byte() {UoScriptByte.TriggerKeyword}
    End Sub
  End Class

  Public Class UoKeyword_member
    Inherits UoKeyword

    Public Sub New()
      _value = "member"
      _binary = New Byte() {UoScriptByte.MemberKeyword}
    End Sub
  End Class

  Public Class UoKeyword_inherits
    Inherits UoKeyword

    Public Sub New()
      _value = "inherits"
      _binary = New Byte() {UoScriptByte.InheritsKeyword}
    End Sub
  End Class

  Public Class UoKeyword_forward
    Inherits UoKeyword

    Public Sub New()
      _value = "forward"
      _binary = New Byte() {UoScriptByte.ForwardKeyword}
    End Sub
  End Class

  Public Class UoTypeName
    Inherits UoKeyword
  End Class

  Public Class UoTypeName_int
    Inherits UoTypeName

    Public Sub New()
      _value = "int"
      _binary = New Byte() {UoScriptByte.IntegerType}
    End Sub
  End Class

  Public Class UoTypeName_string
    Inherits UoTypeName

    Public Sub New()
      _value = "string"
      _binary = New Byte() {UoScriptByte.StringType}
    End Sub
  End Class

  Public Class UoTypeName_ustring ' unicode-string
    Inherits UoTypeName

    Public Sub New()
      _value = "ustring"
      _binary = New Byte() {UoScriptByte.UstType}
    End Sub
  End Class

  Public Class UoTypeName_loc
    Inherits UoTypeName

    Public Sub New()
      _value = "loc"
      _binary = New Byte() {UoScriptByte.LocationType}
    End Sub
  End Class

  Public Class UoTypeName_object
    Inherits UoTypeName

    Public Sub New()
      _value = "obj"
      _binary = New Byte() {UoScriptByte.ObjectType}
    End Sub
  End Class

  Public Class UoTypeName_list
    Inherits UoTypeName

    Public Sub New()
      _value = "list"
      _binary = New Byte() {UoScriptByte.ListType}
    End Sub
  End Class

  Public Class UoTypeName_void
    Inherits UoTypeName

    Public Sub New()
      _value = "void"
      _binary = New Byte() {UoScriptByte.VoidType}
    End Sub
  End Class

  Public Class UoOperator
    Inherits UoToken
  End Class

  Public Class UoOperator_Bracket
    Inherits UoOperator
  End Class

  Public Class UoOperator_OpenBracket
    Inherits UoOperator_Bracket

    Public Sub New()
      _value = "["
      _binary = New Byte() {UoScriptByte.OpenBracket}
    End Sub
  End Class

  Public Class UoOperator_CloseBracket
    Inherits UoOperator_Bracket

    Public Sub New()
      _value = "]"
      _binary = New Byte() {UoScriptByte.CloseBracket}
    End Sub
  End Class

  Public Class UoOperator_Parenthesis
    Inherits UoOperator
  End Class

  Public Class UoOperator_OpenParenthesis
    Inherits UoOperator_Parenthesis

    Public Sub New()
      _value = "("
      _binary = New Byte() {UoScriptByte.OpenParenthesis}
    End Sub
  End Class

  Public Class UoOperator_CloseParenthesis
    Inherits UoOperator_Parenthesis

    Public Sub New()
      _value = ")"
      _binary = New Byte() {UoScriptByte.CloseParenthesis}
    End Sub
  End Class

  Public Class UoUnaryOperator
    Inherits UoOperator
  End Class

  Public Class UoOperator_LogicalNegation
    Inherits UoUnaryOperator

    Public Sub New()
      _value = "!"
      _binary = New Byte() {UoScriptByte.LogicalNegation}
    End Sub
  End Class

  Public Class UoOperator_increment
    Inherits UoUnaryOperator

    Public Sub New()
      _value = "++"
      _binary = New Byte() {UoScriptByte.PlusPlus}
    End Sub
  End Class

  Public Class UoOperator_decrement
    Inherits UoUnaryOperator

    Public Sub New()
      _value = "--"
      _binary = New Byte() {UoScriptByte.MinisMinus}
    End Sub
  End Class

  Public Class UoBinaryOperator
    Inherits UoOperator
  End Class

  Public Class UoOperator_Plus
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "+"
      _binary = New Byte() {UoScriptByte.Plus}
    End Sub
  End Class

  Public Class UoOperator_Minus
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "-"
      _binary = New Byte() {UoScriptByte.Minus}
    End Sub
  End Class

  Public Class UoOperator_Multiply
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "*"
      _binary = New Byte() {UoScriptByte.Multiply}
    End Sub
  End Class

  Public Class UoOperator_Divide
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "/"
      _binary = New Byte() {UoScriptByte.Divide}
    End Sub
  End Class

  Public Class UoOperator_Remainder
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "%"
      _binary = New Byte() {UoScriptByte.Remainder}
    End Sub
  End Class

  Public Class UoOperator_IsEqual
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "=="
      _binary = New Byte() {UoScriptByte.IsEqual}
    End Sub
  End Class

  Public Class UoOperator_IsNotEqual
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "!="
      _binary = New Byte() {UoScriptByte.IsNotEqual}
    End Sub
  End Class

  Public Class UoOperator_LowerThen
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "<"
      _binary = New Byte() {UoScriptByte.LowerThen}
    End Sub
  End Class

  Public Class UoOperator_GreaterThen
    Inherits UoBinaryOperator

    Public Sub New()
      _value = ">"
      _binary = New Byte() {UoScriptByte.GreaterThen}
    End Sub
  End Class

  Public Class UoOperator_LowerThenOrEqual
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "<="
      _binary = New Byte() {UoScriptByte.LowerThenOrEqual}
    End Sub
  End Class

  Public Class UoOperator_GreaterThenOrEqual
    Inherits UoBinaryOperator

    Public Sub New()
      _value = ">="
      _binary = New Byte() {UoScriptByte.GreaterThenOrEQual}
    End Sub
  End Class

  Public Class UoOperator_Assignment
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "="
      _binary = New Byte() {UoScriptByte.Assigment}
    End Sub
  End Class

  Public Class UoOperator_LogicalAnd
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "&&"
      _binary = New Byte() {UoScriptByte.LogicalAnd}
    End Sub
  End Class

  Public Class UoOperator_LogicalOr
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "||"
      _binary = New Byte() {UoScriptByte.LogicalOr}
    End Sub
  End Class

  Public Class UoOperator_BitwiseExclusiveOr
    Inherits UoBinaryOperator

    Public Sub New()
      _value = "^"
      _binary = New Byte() {UoScriptByte.BitwiseExclusiveOr}
    End Sub
  End Class

  Public Class UoPunctuator
    Inherits UoToken
  End Class

  Public Class UoSemicolon
    Inherits UoPunctuator

    ' NOTE:
    ' - is required after all statements that do not end with an UoPunctuator_CloseBrace token
    Public Sub New()
      _value = ";"
      _binary = New Byte() {UoScriptByte.SemiColon}
    End Sub
  End Class

  Public Class UoColon
    Inherits UoPunctuator

    ' NOTE:
    ' - is required after the UoCase and UoDefault tokens
    ' - is optional after the UoFunction and UoScriptVar tokens
    Public Sub New()
      _value = ":"
      _binary = Nothing ' NOTE: the colon does not really exist in the UO-binary language
    End Sub
  End Class

  Public Class UoPunctuator_Comma
    Inherits UoPunctuator

    Public Sub New()
      _value = ","
      _binary = New Byte() {UoScriptByte.Comma}
    End Sub
  End Class

  Public Class UoPunctuator_Parenthesis
    Inherits UoPunctuator
  End Class

  Public Class UoPunctuator_OpenParenthesis
    Inherits UoPunctuator_Parenthesis

    Public Sub New()
      _value = "("
      _binary = New Byte() {UoScriptByte.OpenParenthesis}
    End Sub
  End Class

  Public Class UoPunctuator_CloseParenthesis
    Inherits UoPunctuator_Parenthesis

    Public Sub New()
      _value = ")"
      _binary = New Byte() {UoScriptByte.CloseParenthesis}
    End Sub
  End Class

  Public Class UoPunctuator_Brace
    Inherits UoPunctuator
  End Class

  Public Class UoPunctuator_OpenBrace
    Inherits UoPunctuator_Brace

    Public Sub New()
      _value = "{"
      _binary = New Byte() {UoScriptByte.OpenBrace}
    End Sub
  End Class

  Public Class UoPunctuator_CloseBrace
    Inherits UoPunctuator_Brace

    Public Sub New()
      _value = "}"
      _binary = New Byte() {UoScriptByte.CloseBrace}
    End Sub
  End Class

  Public Class UoPunctuator_Chevron
    Inherits UoPunctuator
  End Class

  Public Class UoPunctuator_OpenChevron
    Inherits UoPunctuator_Chevron

    Public Sub New()
      _value = "<"
      _binary = Nothing
    End Sub
  End Class

  Public Class UoPunctuator_CloseChevron
    Inherits UoPunctuator_Chevron

    Public Sub New()
      _value = ">"
      _binary = Nothing
    End Sub
  End Class
End Class
