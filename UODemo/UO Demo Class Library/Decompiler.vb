' UO Demo Class Library / UODemoSDK : A general class library for the UO Demo
' Copyright ©  2010-2011 JoinUO | batlin@joinuo.com 
' Licensed under the Open Software License version 3.0 (COPYING.txt).

Public Class Decompiler
  Public Enum TargetLanguage
    NativeUOSL = 0
    EnhancedUOSL = 1
    EnhancedUOC = 2
    RealC = 10
  End Enum

  Public Class Tokenizer
    Public UseObfuscation As Boolean = True
    Public SDB As SDB = Nothing

    Private _ParenthesisQueue As New Stack(Of UoToken)
    Private _LastToken As UoToken
    Private _SourceIndex As Integer
    Private _Source As Byte() = Nothing
    Public Property Source As Byte()
      Get
        Return _Source
      End Get
      Set(ByVal value As Byte())
        ' Assign the new source that the tokenizer will use
        If value Is Nothing Then
          _Source = Nothing
        Else
          _Source = value.Clone
          Reset()
        End If
      End Set
    End Property

    Public ReadOnly Property Address As Integer
      Get
        ' Return the current address of the tokenizer
        If _Source Is Nothing Then
          Return 0
        End If
        Return _SourceIndex
      End Get
    End Property

    Public Sub New()
    End Sub

    Public Sub New(ByVal Source As Byte())
      Me.Source = Source
    End Sub

    Public Sub Reset()
      ' Move the tokenizer to the beginning
      _SourceIndex = 0
      _LastToken = Nothing
      _ParenthesisQueue.Clear()
    End Sub

    Public Function IsEndReached() As Boolean
      ' Did we reach the end?
      ' NOTE: we can only reach an end if we have a source ;-)
      If _Source Is Nothing OrElse _SourceIndex >= _Source.Length Then
        Return True
      End If

      Return False
    End Function

    Public Function IsScriptByteAvailable() As Boolean
      ' We don't have a script-byte available if we have no source
      If _Source Is Nothing Then
        Return False
      End If

      ' The script-byte must be within the bounds
      If UseObfuscation Then
        If (_SourceIndex + 2) > _Source.Length Then
          Return False
        End If
      Else
        If (_SourceIndex + 1) > _Source.Length Then
          Return False
        End If
      End If

      Return True
    End Function

    Public Function IsTokenAvailable() As Boolean
      ' We don't have a token available if the current token is 0
      If GetTokenLengh() <= 0 Then
        Return False
      End If

      Return True
    End Function

    Public Function GetToken() As UoToken
      ' Do we have a token available?
      Dim TokenLength As Integer = GetTokenLengh()
      If TokenLength <= 0 Then
        Throw New OverflowException("No token available.")
      End If

      ' Return the new token
      ' On error: do not advance to the next token
      Dim _LastOnToken As UoToken = _LastToken
      _LastToken = PreviewToken()
      If _LastToken IsNot Nothing Then
        ' Move to the next token
        _SourceIndex += TokenLength
      End If
      Return _LastToken
    End Function

    Public Function PreviewToken() As UoToken
      If Not IsTokenAvailable() Then
        Throw New OverflowException("No token available.")
      End If

      Dim b As UoToken.UoScriptByte = PreviewScriptByte()
      Select Case b
        Case UoToken.UoScriptByte.OpenParenthesis
          Dim t As UoToken
          If TypeOf _LastToken Is UoToken.UoFunctionName OrElse TypeOf _LastToken Is UoToken.UoEventName OrElse TypeOf _LastToken Is UoToken.UoKeyword_for OrElse TypeOf _LastToken Is UoToken.UoKeyword_while OrElse TypeOf _LastToken Is UoToken.UoKeyword_if OrElse TypeOf _LastToken Is UoToken.UoKeyword_switch OrElse TypeOf _LastToken Is UoToken.UoKeyword_return Then
            t = New UoToken.UoPunctuator_OpenParenthesis
          Else
            t = New UoToken.UoOperator_OpenParenthesis
          End If
          _ParenthesisQueue.Push(t)
          Return t
        Case UoToken.UoScriptByte.CloseParenthesis
          If _ParenthesisQueue.Count = 0 Then
            Return New UoToken.UoComment("/* ) */")
          End If
          Dim t As UoToken = _ParenthesisQueue.Pop()
          If TypeOf t Is UoToken.UoPunctuator_OpenParenthesis Then
            Return New UoToken.UoPunctuator_CloseParenthesis
          Else
            Return New UoToken.UoOperator_CloseParenthesis
          End If
        Case UoToken.UoScriptByte.Comma
          Return New UoToken.UoPunctuator_Comma
        Case UoToken.UoScriptByte.SemiColon
          Return New UoToken.UoSemicolon
        Case UoToken.UoScriptByte.OpenBrace
          Return New UoToken.UoPunctuator_OpenBrace
        Case UoToken.UoScriptByte.CloseBrace
          Return New UoToken.UoPunctuator_CloseBrace
        Case UoToken.UoScriptByte.OpenBracket
          Return New UoToken.UoOperator_OpenBracket
        Case UoToken.UoScriptByte.CloseBracket
          Return New UoToken.UoOperator_CloseBracket
        Case UoToken.UoScriptByte.LogicalNegation
          Return New UoToken.UoOperator_LogicalNegation
        Case UoToken.UoScriptByte.Plus
          Return New UoToken.UoOperator_Plus
        Case UoToken.UoScriptByte.Minus
          Return New UoToken.UoOperator_Minus
        Case UoToken.UoScriptByte.Multiply
          Return New UoToken.UoOperator_Multiply
        Case UoToken.UoScriptByte.Divide
          Return New UoToken.UoOperator_Divide
        Case UoToken.UoScriptByte.Remainder
          Return New UoToken.UoOperator_Remainder
        Case UoToken.UoScriptByte.IsEqual
          Return New UoToken.UoOperator_IsEqual
        Case UoToken.UoScriptByte.IsNotEqual
          Return New UoToken.UoOperator_IsNotEqual
        Case UoToken.UoScriptByte.LowerThen
          Return New UoToken.UoOperator_LowerThen
        Case UoToken.UoScriptByte.GreaterThen
          Return New UoToken.UoOperator_GreaterThen
        Case UoToken.UoScriptByte.LowerThenOrEqual
          Return New UoToken.UoOperator_LowerThenOrEqual
        Case UoToken.UoScriptByte.GreaterThenOrEQual
          Return New UoToken.UoOperator_GreaterThenOrEqual
        Case UoToken.UoScriptByte.Assigment
          Return New UoToken.UoOperator_Assignment
        Case UoToken.UoScriptByte.LogicalAnd
          Return New UoToken.UoOperator_LogicalAnd
        Case UoToken.UoScriptByte.LogicalOr
          Return New UoToken.UoOperator_LogicalOr
        Case UoToken.UoScriptByte.BitwiseExclusiveOr
          Return New UoToken.UoOperator_BitwiseExclusiveOr
        Case UoToken.UoScriptByte.PlusPlus
          Return New UoToken.UoOperator_increment
        Case UoToken.UoScriptByte.MinisMinus
          Return New UoToken.UoOperator_decrement
        Case UoToken.UoScriptByte.IntegerType
          Return New UoToken.UoTypeName_int
        Case UoToken.UoScriptByte.StringType
          Return New UoToken.UoTypeName_string
        Case UoToken.UoScriptByte.UstType
          Return New UoToken.UoTypeName_ustring
        Case UoToken.UoScriptByte.LocationType
          Return New UoToken.UoTypeName_loc
        Case UoToken.UoScriptByte.ObjectType
          Return New UoToken.UoTypeName_object
        Case UoToken.UoScriptByte.ListType
          Return New UoToken.UoTypeName_list
        Case UoToken.UoScriptByte.VoidType
          Return New UoToken.UoTypeName_void
        Case UoToken.UoScriptByte.IfKeyword
          Return New UoToken.UoKeyword_if
        Case UoToken.UoScriptByte.ElseKeyword
          Return New UoToken.UoKeyword_else
        Case UoToken.UoScriptByte.WhileKeyword
          Return New UoToken.UoKeyword_while
        Case UoToken.UoScriptByte.ForKeyword
          Return New UoToken.UoKeyword_for
        Case UoToken.UoScriptByte.ContinueKeyword
          Return New UoToken.UoKeyword_continue
        Case UoToken.UoScriptByte.BreakKeyword
          Return New UoToken.UoKeyword_break
        Case UoToken.UoScriptByte.SwitchKeyword
          Return New UoToken.UoKeyword_switch
        Case UoToken.UoScriptByte.CaseKeyword
          Return New UoToken.UoKeyword_case
        Case UoToken.UoScriptByte.DefaultKeyword
          Return New UoToken.UoKeyword_default
        Case UoToken.UoScriptByte.ReturnKeyword
          Return New UoToken.UoKeyword_return
        Case UoToken.UoScriptByte.FunctionKeyword
          Return New UoToken.UoKeyword_function
        Case UoToken.UoScriptByte.TriggerKeyword
          Return New UoToken.UoKeyword_trigger
        Case UoToken.UoScriptByte.MemberKeyword
          Return New UoToken.UoKeyword_member
        Case UoToken.UoScriptByte.InheritsKeyword
          Return New UoToken.UoKeyword_inherits
        Case UoToken.UoScriptByte.ForwardKeyword
          Return New UoToken.UoKeyword_forward
        Case UoToken.UoScriptByte.QuotedStringConstant
          ' Get the SDB-index
          Dim i As Integer
          If UseObfuscation Then
            i = _Source(_SourceIndex + 3) * 256 + _Source(_SourceIndex + 2)
          Else
            i = _Source(_SourceIndex + 2) * 256 + _Source(_SourceIndex + 1)
          End If

          Dim c As String = SDB.GetByIndex(i)
          Return New UoToken.UoConstantString(c)
        Case UoToken.UoScriptByte.SingleByteConstant
          ' Get the constant byte
          Dim c As Byte
          If UseObfuscation Then
            c = _Source(_SourceIndex + 2)
          Else
            c = _Source(_SourceIndex + 1)
          End If
          If TypeOf _LastToken Is UoToken.UoKeyword_trigger Then
            Return New UoToken.UoConstantEventChance(c)
          Else
            Return New UoToken.UoConstantNumber(c)
          End If
        Case UoToken.UoScriptByte.DoubleByteConstant
          ' Get the constant word (double byte)
          Dim c As UInt16
          If UseObfuscation Then
            c = (CType(_Source(_SourceIndex + 3), UInt16) << 8) + _Source(_SourceIndex + 2)
          Else
            c = (CType(_Source(_SourceIndex + 2), UInt16) << 8) + _Source(_SourceIndex + 1)
          End If
          If TypeOf _LastToken Is UoToken.UoKeyword_trigger Then
            Return New UoToken.UoConstantEventChance(c)
          Else
            Return New UoToken.UoConstantNumber(c)
          End If
        Case UoToken.UoScriptByte.QuadByteConstant
          ' Get the constant dword (quad byte)
          Dim c As Int32
          If UseObfuscation Then
            c = (CType(_Source(_SourceIndex + 5), Int32) << 24) + (CType(_Source(_SourceIndex + 4), UInt32) << 16) + (CType(_Source(_SourceIndex + 3), UInt16) << 8) + _Source(_SourceIndex + 2)
          Else
            c = (CType(_Source(_SourceIndex + 4), Int32) << 24) + (CType(_Source(_SourceIndex + 3), UInt32) << 16) + (CType(_Source(_SourceIndex + 2), UInt16) << 8) + _Source(_SourceIndex + 1)
          End If
          If TypeOf _LastToken Is UoToken.UoKeyword_trigger Then
            Return New UoToken.UoConstantEventChance(c)
          Else
            Return New UoToken.UoConstantNumber(c)
          End If
        Case UoToken.UoScriptByte.StringConstant
          ' Get the SDB-index
          Dim i As Integer
          If UseObfuscation Then
            i = _Source(_SourceIndex + 3) * 256 + _Source(_SourceIndex + 2)
          Else
            i = _Source(_SourceIndex + 2) * 256 + _Source(_SourceIndex + 1)
          End If
          Dim c As String = SDB.GetByIndex(i)

          ' Detect certain specialities
          If c = "this" Then
            Return New UoToken.UoKeyword_this
          End If
          If TypeOf _LastToken Is UoToken.UoKeyword_trigger OrElse TypeOf _LastToken Is UoToken.UoConstantEventChance Then
            Return New UoToken.UoEventName(c)
          End If
          If TypeOf _LastToken Is UoToken.UoKeyword_inherits Then
            Return New UoToken.UoScriptName(c)
          End If
          If TypeOf _LastToken Is UoToken.UoKeyword_forward OrElse TypeOf _LastToken Is UoToken.UoKeyword_function Then
            Return New UoToken.UoFunctionName(c)
          End If
          Dim pb As UoToken.UoScriptByte = PreviewNextScriptByte()
          If pb = UoToken.UoScriptByte.OpenParenthesis Then
            Return New UoToken.UoFunctionName(c)
          End If

          ' Default to variable-usage
          Return New UoToken.UoVariableName(c)
      End Select

      ' Should not happen: unknown token!
      Return Nothing
    End Function

    Public Function GetTokenLengh() As Integer
      ' We can only have a token if we have script-byte
      If Not IsScriptByteAvailable() Then
        Return 0
      End If

      ' Identify the script-byte
      Dim b As Byte
      If UseObfuscation Then
        b = Obfuscator.Deobfuscate(_Source, _SourceIndex)
      Else
        b = _Source(_SourceIndex)
      End If

      Dim AddedLength As Integer = 0

      ' Modify the length of the token depending on the script-byte
      If b = UoToken.UoScriptByte.QuotedStringConstant Then
        If _SourceIndex + 2 >= _Source.Length Then
          Return 0
        End If
        AddedLength = 2
      End If
      If b = UoToken.UoScriptByte.StringConstant Then
        If _SourceIndex + 2 >= _Source.Length Then
          Return 0
        End If
        AddedLength = 2
      End If
      If b = UoToken.UoScriptByte.SingleByteConstant Then
        If _SourceIndex + 1 >= _Source.Length Then
          Return 0
        End If
        AddedLength = 1
      End If
      If b = UoToken.UoScriptByte.DoubleByteConstant Then
        If _SourceIndex + 2 >= _Source.Length Then
          Return 0
        End If
        AddedLength = 2
      End If
      If b = UoToken.UoScriptByte.QuadByteConstant Then
        If _SourceIndex + 4 >= _Source.Length Then
          Return 0
        End If
        AddedLength = 4
      End If

      ' Done
      If UseObfuscation Then
        Return 2 + AddedLength
      Else
        Return 1 + AddedLength
      End If
    End Function

    Public Function PreviewScriptByte() As UoToken.UoScriptByte
      If Not IsTokenAvailable() Then
        Throw New OverflowException("No token available.")
      End If

      ' Return the current script-byte
      Dim PreviewIndex = _SourceIndex + GetTokenLengh()
      If UseObfuscation Then
        Dim b As Byte = Obfuscator.Deobfuscate(_Source, _SourceIndex)
        Return b
      Else
        Dim b As Byte = _Source(_SourceIndex)
        Return b
      End If
    End Function

    Public Function PreviewNextScriptByte() As UoToken.UoScriptByte
      ' To know the location of the next script-byte we must detect the size of the current token
      Dim l As Integer = GetTokenLengh()
      If l = 0 Then
        Throw New OverflowException("No token available.")
      End If

      ' Return the next script-byte
      ' TODO: this may cause a crash if the next script-byte is not within the array bounds
      Dim PreviewIndex = _SourceIndex + GetTokenLengh()
      If UseObfuscation Then
        Dim b As Byte = Obfuscator.Deobfuscate(_Source, PreviewIndex)
        Return b
      Else
        Dim b As Byte = _Source(PreviewIndex)
        Return b
      End If
    End Function

    Public Function GetTokenList(ByRef sdb As SDB) As List(Of UoToken)
      Dim NewTokenList As New List(Of UoToken)

      ' Start from the beginning and handle each token
      Reset()
      While IsTokenAvailable()
        NewTokenList.Add(GetToken())
      End While

      ' Done
      Return NewTokenList
    End Function
  End Class

  Public Shared Function ProduceTokenList(ByVal Contents() As Byte, ByRef SDB As SDB, Optional ByVal PostProcess As Boolean = False) As List(Of UoToken)
    Dim NewTokenList As New List(Of UoToken)

    ' Use a temponary tokenizer
    Dim Tokenizer As New Tokenizer
    Tokenizer.Source = Contents
    Tokenizer.SDB = SDB
    Do While Tokenizer.Source(Tokenizer.Address) <> 0
      NewTokenList.Add(Tokenizer.GetToken())
    Loop

    ' Post-Process?
    If PostProcess Then
      Decompiler.PostProcessTokenList(NewTokenList)
    End If

    ' Done
    Return NewTokenList
  End Function

  Public Shared Sub PostProcessTokenList(ByRef TokenList As List(Of UoToken))
    Dim InsertEventParameters As String = Nothing

    Dim i As Integer = 0
    Do While i < TokenList.Count
      Dim Token As UoToken = TokenList(i)
      If TypeOf Token Is UoToken.UoKeyword_trigger Then
        ' convert "on chance eventname" 
        ' to "on <chance> eventname"
        If i + 1 < TokenList.Count Then
          If TypeOf TokenList(i + 1) Is UoToken.UoConstantEventChance Then
            TokenList.Insert(i + 1, New UoToken.UoPunctuator_OpenChevron)
            TokenList.Insert(i + 3, New UoToken.UoPunctuator_CloseChevron)
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoEventName Then
        ' convert "on eventname(code)" 
        ' to "on eventname<code>"
        If i + 1 < TokenList.Count Then
          If Not TypeOf TokenList(i + 1) Is UoToken.UoPunctuator_OpenBrace Then
            ' Extra checks...
            If TypeOf TokenList(i + 1) Is UoToken.UoPunctuator_OpenParenthesis Then
              TokenList(i + 1) = New UoToken.UoPunctuator_OpenChevron
            End If
            If i + 3 < TokenList.Count Then
              If TypeOf TokenList(i + 3) Is UoToken.UoPunctuator_CloseParenthesis Then
                TokenList(i + 3) = New UoToken.UoPunctuator_CloseChevron
              End If
            End If
          End If
        End If

        ' convert "on eventname {" 
        ' to "on eventname(parameters) {"
        InsertEventParameters = Token.Value
      End If

      If InsertEventParameters IsNot Nothing Then
        If TypeOf Token Is UoToken.UoSemicolon Then
          ' Something is wrong anyways if this happens
          InsertEventParameters = Nothing
        End If
        If TypeOf Token Is UoToken.UoPunctuator_OpenBrace Then
          Dim EventDefinition As Events.EventDefinition = Events.GetEventDefinition(InsertEventParameters)
          Dim ParamList As New List(Of UoToken)

          ' Insert the parameters now
          ParamList.Add(New UoToken.UoPunctuator_OpenParenthesis)
          If EventDefinition IsNot Nothing Then
            If EventDefinition.Tokens IsNot Nothing Then
              ParamList.AddRange(EventDefinition.Tokens)
            End If
          Else
            ' Should not happen...
            ParamList.Add(New UoToken.UoComment("/* UNKNOWN EVENT */"))
          End If
          ParamList.Add(New UoToken.UoPunctuator_CloseParenthesis)

          ' Insertion done
          TokenList.InsertRange(i, ParamList)
          InsertEventParameters = Nothing
        End If
      End If
      If TypeOf Token Is UoToken.UoKeyword_case Then
        ' convert "case X" 
        ' to "case X:"
        If i + 2 < TokenList.Count Then
          If Not TypeOf TokenList(i + 2) Is UoToken.UoColon Then
            TokenList.Insert(i + 2, New UoToken.UoColon)
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoKeyword_return Then
        ' convert return();
        ' to return;
        If i + 2 < TokenList.Count Then
          If TypeOf TokenList(i + 1) Is UoToken.UoPunctuator_OpenParenthesis AndAlso TypeOf TokenList(i + 2) Is UoToken.UoPunctuator_CloseParenthesis Then
            TokenList.RemoveAt(i + 2)
            TokenList.RemoveAt(i + 1)
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoConstantString Then
        ' convert "xxx" "yyy"
        ' to "xxx" + "yyy"
        If i + 1 < TokenList.Count Then
          If TypeOf TokenList(i + 1) Is UoToken.UoConstantString Then
            TokenList.Insert(i + 1, New UoToken.UoOperator_Plus)
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoConstantNumber Then
        ' convert x y
        ' to x, y
        ' NOTE: this grammar error occurs a few times in list assignments
        If i > 0 Then
          If TypeOf TokenList(i - 1) Is UoToken.UoConstantNumber OrElse TypeOf TokenList(i - 1) Is UoToken.UoConstantString Then
            TokenList.Insert(i, New UoToken.UoPunctuator_Comma)
            i = i + 1
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoKeyword_default Then
        ' convert "default" 
        ' to "default:"
        If i + 1 < TokenList.Count Then
          If Not TypeOf TokenList(i + 1) Is UoToken.UoColon Then
            TokenList.Insert(i + 1, New UoToken.UoColon)
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoKeyword_break OrElse TypeOf Token Is UoToken.UoKeyword_return OrElse TypeOf Token Is UoToken.UoTypeName OrElse TypeOf Token Is UoToken.UoFunctionName OrElse TypeOf Token Is UoToken.UoVariableName Then
        ' ensure that break/return/types/identifiers are preceeded by ; or : or { or }
        If i > 0 Then
          If TypeOf TokenList(i - 1) Is UoToken.UoOperator_CloseParenthesis OrElse TypeOf TokenList(i - 1) Is UoToken.UoPunctuator_CloseParenthesis Then
            TokenList.Insert(i, New UoToken.UoSemicolon)
            TokenList.Insert(i, New UoToken.UoComment("/* semicolon added by the decompiler post-processor */"))
            i = i + 2
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoPunctuator_CloseBrace Then
        ' ensure that } is preceeded by ;
        If i > 0 Then
          If Not (TypeOf TokenList(i - 1) Is UoToken.UoSemicolon OrElse TypeOf TokenList(i - 1) Is UoToken.UoPunctuator_Brace) Then
            TokenList.Insert(i, New UoToken.UoSemicolon)
            TokenList.Insert(i, New UoToken.UoComment("/* semicolon added by the decompiler post-processor */"))
            i = i + 2
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoConstantString Then
        ' fix the OSI grammar bug where a string is preceeded by this keyword
        If i > 0 Then
          If TypeOf TokenList(i - 1) Is UoToken.UoKeyword_this Then
            TokenList.Insert(i, New UoToken.UoPunctuator_Comma)
            TokenList.Insert(i, New UoToken.UoComment("/* comma added by the decompiler post-processor */"))
            i = i + 2
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoPunctuator_CloseParenthesis Then
        ' ensure that ) is not preceeded by ,
        If i > 0 Then
          If TypeOf TokenList(i - 1) Is UoToken.UoPunctuator_Comma Then
            TokenList(i - 1) = New UoToken.UoComment("/* , *//* comma removed by decompiler post-processor */")
          End If
        End If
      End If

      ' Now fix some coolor OSI bugs
      If TypeOf Token Is UoToken.UoSemicolon Then
        ' Replace == and - by an = operator there where needed
        Dim prvI As Integer = i
        Do
          prvI = prvI - 1
        Loop Until prvI < 0 OrElse (TypeOf TokenList(prvI) Is UoToken.UoPunctuator_Brace OrElse TypeOf TokenList(prvI) Is UoToken.UoSemicolon OrElse TypeOf TokenList(prvI) Is UoToken.UoColon)
        If prvI >= 0 Then
          prvI = prvI + 1
          If TypeOf TokenList(prvI) Is UoToken.UoTypeName Then
            prvI = prvI + 1
          End If
          If TypeOf TokenList(prvI) Is UoToken.UoVariableName AndAlso (TypeOf TokenList(prvI + 1) Is UoToken.UoOperator_IsEqual OrElse TypeOf TokenList(prvI + 1) Is UoToken.UoOperator_Minus) Then
            TokenList.Insert(i, New UoToken.UoComment("/* replaced " + TokenList(prvI + 1).Value + " with = */"))
            i = i + 1
            TokenList(prvI + 1) = New UoToken.UoOperator_Assignment
          End If
        End If
      End If

      i = i + 1
    Loop
  End Sub
  Public Shared Function DecompileM(ByVal Contents() As Byte, ByRef SDB As SDB, Optional ByVal TargetLanguage As TargetLanguage = TargetLanguage.EnhancedUOSL) As List(Of UoToken)
    Return ProduceTokenList(Contents, SDB, TargetLanguage <> Decompiler.TargetLanguage.NativeUOSL)
  End Function
  Public Shared Function DecompileM(ByVal FilePath As String, ByRef SDB As SDB, Optional ByVal TargetLanguage As TargetLanguage = TargetLanguage.EnhancedUOSL) As List(Of UoToken)
    ' Open the file
    Dim File As New IO.FileStream(FilePath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)

    ' Return an empty decompiled string if the file length is zero
    If File.Length = 0 Then
      Return Nothing
    End If

    ' Read into a buffer
    Dim TempBuffer(File.Length) As Byte
    File.Read(TempBuffer, 0, File.Length)

    ' Add 0-Byte at the end of the buffer
    TempBuffer(File.Length) = 0

    ' Close the file
    File.Close()

    Return DecompileM(TempBuffer, SDB, TargetLanguage)
  End Function
  Public Shared Function ProduceSourceCode(ByVal TokenList As List(Of UoToken), Optional ByVal TargetLanguage As TargetLanguage = TargetLanguage.EnhancedUOSL) As String
    ' Generate the user listing
    Dim Helper As New System.Text.StringBuilder
    Select Case TargetLanguage
      Case TargetLanguage.NativeUOSL
        Helper.Append("// UOSL (native)")
      Case TargetLanguage.EnhancedUOC
        Helper.Append("// UO-C (old)")
      Case TargetLanguage.EnhancedUOSL
        Helper.Append("// UOSL (enhanced)")
      Case TargetLanguage.RealC
        Helper.Append("// Real-C")
        Helper.Append(vbNewLine)
        Helper.Append("#include ""ENGINE.hpp""")
        Helper.Append(vbNewLine)
      Case TargetLanguage.RealC
        Helper.Append("// ... (unkwown style)")
    End Select
    Helper.Append(vbNewLine)

    Dim CurrentGroup As UoToken = Nothing
    Dim CurrentDepth As Integer = 0, IsBeginOfNewLine As Boolean = True
    Dim ParenthesesDepth As Integer = 0
    Dim CurrentLineIsCase As Boolean = False ' used by Native-UOSL only
    Dim CurrentLineIsInclude As Boolean = False ' Used by Real-C only
    Dim CurrentLineIsOn As Boolean = False ' Used by Real-C only
    Dim LocationVariables As New List(Of String) ' Used by Real-C only
    Dim ListVariables As New List(Of String) ' Used by Real-C only
    Dim CurrentLineIsConstructorAssignment As Boolean = False ' Used by Real-C only
    Dim LastTokenWasCloseParenthesis As Boolean = False ' used by Native-UOSL only
    Dim LastTokenWasConstantString As Boolean = False ' used by Native-UOSL only
    Dim LastTokenWasEventConstant As Boolean = False

    Dim SecondLastToken As UoToken = Nothing
    Dim LastToken As UoToken = Nothing
    For Each Token As UoToken In TokenList
      Dim PostAdd As String = ""
      Dim PreAdd As String = ""

      ' Add depth
      If TypeOf Token Is UoToken.UoPunctuator_CloseBrace Then
        CurrentDepth -= 1
      End If
      If TypeOf Token Is UoToken.UoKeyword_case OrElse TypeOf Token Is UoToken.UoKeyword_default Then
        ' "case" and "default" are placed at the same scope depth as the switch statement
        CurrentDepth -= 1
      End If
      If TypeOf Token Is UoToken.UoComment AndAlso (TypeOf LastToken Is UoToken.UoColon OrElse (TypeOf LastToken Is UoToken.UoComment AndAlso TypeOf SecondLastToken Is UoToken.UoColon) OrElse (TypeOf LastToken Is UoToken.UoSemicolon AndAlso TypeOf SecondLastToken Is UoToken.UoKeyword_break)) Then
        ' Handle a comment after a case statement
        CurrentDepth -= 1
        PostAdd = vbNewLine
      End If
      If TypeOf Token Is UoToken.UoPunctuator_Brace Then
        ' Normally we only need to handle UoPunctuator_OpenBrace here
        ' but due to bugged OSI-scripts, we will also handle UoPunctuator_CloseBrace
        If Not IsBeginOfNewLine Then
          Helper.Append(vbNewLine)
          IsBeginOfNewLine = True
        End If
      End If
      If TargetLanguage = Decompiler.TargetLanguage.NativeUOSL AndAlso TypeOf Token Is UoToken.UoKeyword_break Then
        ' Due to bugged OSI-scripts we must ensure that the break keyword is always on a new line
        ' (only for Native UOSL because for the other languages the post-processor does this job)
        If Not IsBeginOfNewLine Then
          Helper.Append(vbNewLine)
          IsBeginOfNewLine = True
        End If
      End If
      If IsBeginOfNewLine Then
        '
        CurrentLineIsCase = False
        CurrentLineIsOn = False
        CurrentLineIsInclude = False
        CurrentLineIsConstructorAssignment = False

        ' Ident
        For i As Integer = 1 To CurrentDepth
          Helper.Append(vbTab)
        Next
        IsBeginOfNewLine = False
      End If
      If TypeOf Token Is UoToken.UoPunctuator_OpenBrace Then
        CurrentDepth += 1
      End If

      ' Clean-up the output visually
      If TypeOf Token Is UoToken.UoOperator Then
        If TypeOf Token Is UoToken.UoOperator_Parenthesis OrElse TypeOf Token Is UoToken.UoOperator_Bracket Then
        ElseIf TypeOf Token Is UoToken.UoUnaryOperator Then
          If TypeOf Token Is UoToken.UoOperator_LogicalNegation Then
            ' The "location negation" operator does not require a whitespace
          Else
            ' All other unary opators require a space in front of them
            PreAdd = " "
          End If
        Else
          ' Binary operators get a space in front of and behind them
          PreAdd = " "
          PostAdd = " "
        End If
      End If
      If TypeOf Token Is UoToken.UoEventName Then
        If TargetLanguage <> TargetLanguage.RealC AndAlso TargetLanguage <> TargetLanguage.EnhancedUOC Then
          If LastTokenWasEventConstant Then
          Else
            ' Add a space before the event name
            PreAdd = " "
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoConstantEventChance Then
        LastTokenWasEventConstant = True
        If TargetLanguage = TargetLanguage.RealC Then
          PostAdd = " , "
        ElseIf TargetLanguage = TargetLanguage.NativeUOSL Then
          ' Add a space before and after the event chance token
          PreAdd = " "
          PostAdd = " "
        End If
      Else
        If Not TypeOf Token Is UoToken.UoPunctuator_Chevron Then
          LastTokenWasEventConstant = False
        End If
      End If
      If TypeOf Token Is UoToken.UoComment AndAlso (TypeOf LastToken Is UoToken.UoColon OrElse (TypeOf LastToken Is UoToken.UoComment AndAlso TypeOf SecondLastToken Is UoToken.UoColon) OrElse (TypeOf LastToken Is UoToken.UoSemicolon AndAlso TypeOf SecondLastToken Is UoToken.UoKeyword_break)) Then
        ' Increase the depth again!
        CurrentDepth += 1
      End If
      If TypeOf Token Is UoToken.UoKeyword Then
        If TypeOf Token Is UoToken.UoKeyword_default Then
          '  Increase the scope depth
          CurrentDepth += 1
        ElseIf TypeOf Token Is UoToken.UoKeyword_case Then
          ' Add a space after the "case" keyword and increase the scope depth
          CurrentDepth += 1
          PostAdd = " "
        ElseIf TypeOf Token Is UoToken.UoTypeName Then
          ' Add a space after the "variable type" keyword
          PostAdd = " "
        ElseIf TypeOf Token Is UoToken.UoKeyword_trigger OrElse TypeOf Token Is UoToken.UoKeyword_function Then
          If TargetLanguage = TargetLanguage.RealC Then
            If TypeOf Token Is UoToken.UoKeyword_trigger Then
              CurrentLineIsOn = True
              PostAdd = "( "
            Else
              PostAdd = " "
            End If
          Else
            If TypeOf Token Is UoToken.UoKeyword_trigger Then
              If TargetLanguage = TargetLanguage.EnhancedUOC Then
                ' Add a space after the "on" keywords
                PostAdd = " "
              End If
            Else
              ' Add a space after the "implement" keywords
              PostAdd = " "
            End If
          End If

          ' Also, place them apart from other sections at the root scope
          If CurrentDepth = 0 Then
            If CurrentGroup IsNot Nothing Then
              PreAdd = vbNewLine
            End If
            CurrentGroup = Token
          End If
        ElseIf TypeOf Token Is UoToken.UoKeyword_inherits OrElse TypeOf Token Is UoToken.UoKeyword_member OrElse TypeOf Token Is UoToken.UoKeyword_forward Then
          '
          If TypeOf Token Is UoToken.UoKeyword_inherits Then
            CurrentLineIsInclude = True
          End If

          If TargetLanguage <> TargetLanguage.RealC OrElse Not TypeOf Token Is UoToken.UoKeyword_inherits Then
            ' Add a space after the "include", "shared" and "prototype" keywords
            PostAdd = " "
          End If

          ' Also, group them together with similar sections at the root scope
          If CurrentDepth = 0 Then
            If CurrentGroup IsNot Nothing AndAlso Not CurrentGroup.GetType() Is Token.GetType() Then
              PreAdd = vbNewLine
            End If
            CurrentGroup = Token
          End If
        End If
      End If
      If TypeOf Token Is UoToken.UoPunctuator Then
        If TypeOf Token Is UoToken.UoPunctuator_OpenParenthesis Then
          ' Keep track of punctuator parentheses (to detect ; inside a for)
          ParenthesesDepth += 1
        End If
        If TypeOf Token Is UoToken.UoPunctuator_CloseParenthesis Then
          ' Keep track of punctuator parentheses (to detect ; inside a for)
          ParenthesesDepth -= 1
        End If
        If TypeOf Token Is UoToken.UoColon Then
          ' After a colon (case and default-keywords only?) we begin a new line
          PostAdd = vbNewLine
        End If
        If TypeOf Token Is UoToken.UoPunctuator_Comma Then
          ' After a comma follows a simple space
          PostAdd = " "
        End If
        If TypeOf Token Is UoToken.UoPunctuator_Brace Then
          ' A brace (square bracket) is always a single item on a line
          PostAdd = vbNewLine
        End If
        If TypeOf Token Is UoToken.UoSemicolon Then
          If ParenthesesDepth = 0 Then
            ' Assume that we are at the end of a statement
            PostAdd = vbNewLine
          Else
            ' Assume that we are inside a "for"-looop
            PostAdd = " "
          End If
        End If
      End If

      Dim Value As String = Token.Value
      If TargetLanguage = TargetLanguage.EnhancedUOC Then
        ' Value-fixes
        ' Use the keywords from the original Mass M Decompiler!
        If TypeOf Token Is UoToken.UoKeyword_forward Then
          Value = "extern"
        End If
        If TypeOf Token Is UoToken.UoKeyword_function Then
          Value = "function"
        End If
        If TypeOf Token Is UoToken.UoKeyword_inherits Then
          Value = "include"
        End If
        If TypeOf Token Is UoToken.UoKeyword_trigger Then
          Value = "on"
        End If
        If TypeOf Token Is UoToken.UoKeyword_member Then
          Value = "scriptvar"
        End If
        If TypeOf Token Is UoToken.UoTypeName_int Then
          Value = "integer"
        End If
        If TypeOf Token Is UoToken.UoTypeName_loc Then
          Value = "location"
        End If
        If TypeOf Token Is UoToken.UoTypeName_ustring Then
          Value = "ust"
        End If

        ' Remove unsupported comments!
        If TypeOf Token Is UoToken.UoComment Then
          If Not Token.Value.StartsWith("//") Then
            Continue For
          End If
        End If
      End If
      If TargetLanguage = TargetLanguage.RealC Then
        ' Value-fixes
        If TypeOf Token Is UoToken.UoKeyword_inherits Then
          Value = "#include """
        End If
        If TypeOf Token Is UoToken.UoScriptName Then
          Value = Value & ".h"""
        End If
        If TypeOf Token Is UoToken.UoPunctuator_OpenParenthesis Then
          If CurrentLineIsOn Then
            Value = " )" & Value
          End If
        End If
        If TypeOf Token Is UoToken.UoConstantEventChance Then
          ' Already handled above...
        ElseIf TypeOf Token Is UoToken.UoConstant Then
          If CurrentLineIsOn Then
            Value = " , " & Value
          End If
        End If
        If TypeOf Token Is UoToken.UoKeyword_trigger Then
          Value = "TRIGGER"
        End If
        If TypeOf Token Is UoToken.UoKeyword_forward Then
          Value = "FORWARD"
        End If
        If TypeOf Token Is UoToken.UoKeyword_function Then
          Value = "FUNCTION"
        End If
        If TypeOf Token Is UoToken.UoKeyword_member Then
          Value = "MEMBER"
        End If

        '
        If TypeOf Token Is UoToken.UoVariableName AndAlso TypeOf LastToken Is UoToken.UoTypeName_list Then
          ListVariables.Add(Token.Value)
        End If
        If TypeOf Token Is UoToken.UoOperator_Assignment AndAlso ListVariables.Contains(LastToken.Value) Then
          PostAdd = " list( "
          CurrentLineIsConstructorAssignment = True
        End If
        If TypeOf Token Is UoToken.UoVariableName AndAlso TypeOf LastToken Is UoToken.UoTypeName_loc Then
          LocationVariables.Add(Token.Value)
        End If
        If TypeOf Token Is UoToken.UoOperator_Assignment AndAlso LocationVariables.Contains(LastToken.Value) Then
          PostAdd = " loc( "
          CurrentLineIsConstructorAssignment = True
        End If
        If TypeOf Token Is UoToken.UoSemicolon AndAlso CurrentLineIsConstructorAssignment Then
          PreAdd = " )"
        End If

        ' Ignore chevrons & semicolon on the include line
        If CurrentLineIsInclude Then
          If Value = ";" Then
            Value = ""
          End If
        End If
        If TypeOf Token Is UoToken.UoPunctuator_Chevron Then
          Continue For
        End If
      End If
      If TargetLanguage = TargetLanguage.NativeUOSL Then
        ' "case" and "default" require special handling by Native UOSL
        If TypeOf Token Is UoToken.UoKeyword_default Then
          PostAdd &= vbNewLine
        End If
        If TypeOf Token Is UoToken.UoKeyword_case Then
          CurrentLineIsCase = True
        End If
        If CurrentLineIsCase AndAlso TypeOf Token Is UoToken.UoConstantNumber Then
          PostAdd &= vbNewLine
        End If
        If TypeOf Token Is UoToken.UoComment AndAlso Token.Value = "/* ) */" Then
          PreAdd = vbNewLine
          For i As Integer = 1 To CurrentDepth
            PreAdd &= vbTab
          Next
          Value = ")"
        End If
        If LastTokenWasCloseParenthesis AndAlso TypeOf Token Is UoToken.UoFunctionName Then
          PreAdd = vbNewLine
          For i As Integer = 1 To CurrentDepth
            PreAdd &= vbTab
          Next
        End If
        If LastTokenWasConstantString AndAlso TypeOf Token Is UoToken.UoConstantString Then
          PreAdd = " "
        End If

        '
        LastTokenWasCloseParenthesis = TypeOf Token Is UoToken.UoPunctuator_CloseParenthesis
        LastTokenWasConstantString = TypeOf Token Is UoToken.UoConstantString

        ' Ignore colons
        If TypeOf Token Is UoToken.UoColon Then
          Continue For
        End If
      End If
      If TypeOf Token Is UoToken.UoConstantEventChance Then
        If Value.StartsWith("0x") Then
          Value = Int32.Parse(Value.Substring(2), Globalization.NumberStyles.AllowHexSpecifier)
        End If
      End If

      ' Add
      Helper.Append(PreAdd)
      Helper.Append(Value)
      Helper.Append(PostAdd)
      IsBeginOfNewLine = PostAdd.EndsWith(vbNewLine)

      ' Move to the next token
      SecondLastToken = LastToken
      LastToken = Token
    Next

    ' Remove the last new line
    Dim RetString As String = Helper.ToString
    If RetString.EndsWith(vbNewLine) Then
      Helper.Remove(Helper.Length - vbNewLine.Length, vbNewLine.Length)
      RetString = RetString.Substring(0, RetString.Length - vbNewLine.Length)
    End If

    Return RetString
  End Function
End Class