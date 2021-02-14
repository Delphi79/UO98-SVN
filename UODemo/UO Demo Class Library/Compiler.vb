' UO Demo Class Library / UODemoSDK : A general class library for the UO Demo
' Copyright ©  2010-2011 JoinUO | batlin@joinuo.com 
' Licensed under the Open Software License version 3.0 (COPYING.txt).

Public Class Compiler
  Public Class Tokenizer
    Public Shared Function GetToken(ByVal SourceCode As String, ByRef Index As Integer) As String
      ' Get the length
      Dim SourceLength As Integer = SourceCode.Length

      ' Left trim
      Do While (Index < SourceLength) AndAlso (SourceCode(Index) = " "c OrElse SourceCode(Index) = vbTab OrElse SourceCode(Index) = Chr(10) OrElse SourceCode(Index) = Chr(13))
        Index = Index + 1
      Loop

      ' Exit if end reached
      If Index >= SourceLength Then
        Return ""
      End If

      ' Do we need to extract a number?
      Dim FirstChar As Char = SourceCode(Index)
      Select Case FirstChar
        Case "$"c
          Index = Index + 1

HexNumber:
          ' The number is a hexadecimal number
          Dim StartIndex As Integer = Index, ContainsInvalidCharacter As Boolean = False
          Do While Index < SourceLength
            Dim TheChar As Char = SourceCode(Index)
            If TheChar >= "0"c AndAlso TheChar <= "9"c Then
              GoTo ValidHexNumber
            End If
            If TheChar >= "a"c AndAlso TheChar <= "z"c Then
              If TheChar > "f"c Then
                ContainsInvalidCharacter = True
              End If
              GoTo ValidHexNumber
            End If
            If TheChar >= "A"c AndAlso TheChar <= "Z"c Then
              If TheChar > "F"c Then
                ContainsInvalidCharacter = True
              End If
              GoTo ValidHexNumber
            End If
            If TheChar = "_"c Then
              ContainsInvalidCharacter = True
              GoTo ValidHexNumber
            End If
            Exit Do
ValidHexNumber:
            Index = Index + 1
          Loop

          '
          If ContainsInvalidCharacter Then
            Return ""
          End If
          Return "$" & SourceCode.Substring(StartIndex, Index - StartIndex)
        Case "0"c To "9"c
          ' Is the number a hexadecimal number by any chance?
          If FirstChar = "0"c Then
            If (Index + 1) < SourceLength AndAlso SourceCode(Index + 1) = "x" Then
              Index = Index + 2
              GoTo HexNumber
            End If
          End If

          ' The number is a decimal number
          Dim StartIndex As Integer = Index, ContainsInvalidCharacter As Boolean = False
          Do While Index < SourceLength
            Dim TheChar As Char = SourceCode(Index)
            If TheChar >= "0"c AndAlso TheChar <= "9"c Then
              GoTo ValidNumber
            End If
            If TheChar >= "a"c AndAlso TheChar <= "z"c Then
              ContainsInvalidCharacter = True
              GoTo ValidNumber
            End If
            If TheChar >= "A"c AndAlso TheChar <= "Z"c Then
              ContainsInvalidCharacter = True
              GoTo ValidNumber
            End If
            If TheChar = "_"c Then
              ContainsInvalidCharacter = True
              GoTo ValidNumber
            End If
            Exit Do
ValidNumber:
            Index = Index + 1
          Loop

          '
          If ContainsInvalidCharacter Then
            Return ""
          End If
          Return "0" & SourceCode.Substring(StartIndex, Index - StartIndex)
        Case "a"c To "z"c, "A"c To "Z"c, "_"c
          ' A name follows
          Dim StartIndex As Integer = Index
          Do While Index < SourceLength
            Dim TheChar As Char = SourceCode(Index)
            If TheChar >= "0"c AndAlso TheChar <= "9"c Then
              GoTo ValidName
            End If
            If TheChar >= "a"c AndAlso TheChar <= "z"c Then
              GoTo ValidName
            End If
            If TheChar >= "A"c AndAlso TheChar <= "Z"c Then
              GoTo ValidName
            End If
            If TheChar = "_"c Then
              GoTo ValidName
            End If
            Exit Do
ValidName:
            Index = Index + 1
          Loop

          '
          Return "_" & SourceCode.Substring(StartIndex, Index - StartIndex)
        Case """"c
          Dim StartIndex = Index + 1
          Index = SourceCode.IndexOf(""""c, StartIndex)
          If Index < 0 Then
            Index = SourceLength
            Return ""
          End If
          Index = Index + 1
          Return SourceCode.Substring(StartIndex - 1, Index - StartIndex + 1)
        Case "+"c, "-"c, "*"c, "/"c, "%"c, "!"c, "="c, "<"c, ">"c, "&"c, "|"c, "^"c
          ' Keep scanning
          Dim StartIndex As Integer = Index
          Do While Index < SourceLength
            Dim TheChar As Char = SourceCode(Index)
            If TheChar = "+"c OrElse TheChar = "-"c OrElse TheChar = "*"c OrElse TheChar = "/"c OrElse TheChar = "%"c Then
              GoTo KeepScanning
            End If
            If TheChar = "!"c OrElse TheChar = "="c OrElse TheChar = "<"c OrElse TheChar = ">"c Then
              GoTo KeepScanning
            End If
            If TheChar = "&"c OrElse TheChar = "|"c OrElse TheChar = "^"c Then
              GoTo KeepScanning
            End If
            If TheChar = "/"c Then
              GoTo KeepScanning
            End If
            Exit Do
KeepScanning:
            Index = Index + 1
          Loop

          '
          Dim Result As String = SourceCode.Substring(StartIndex, Index - StartIndex)
          If Result = "//" Then
            ' This is a comment, keep scanning until the end of the line
            Do While (Index < SourceLength) AndAlso (SourceCode(Index) <> Chr(10) AndAlso SourceCode(Index) <> Chr(13))
              Result &= SourceCode(Index)
              Index = Index + 1
            Loop
          End If
          Return Result
        Case Else
          ' Return the character
          Index = Index + 1
          Return SourceCode(Index - 1)
      End Select
    End Function
  End Class
End Class
