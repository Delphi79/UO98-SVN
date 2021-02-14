' UO Demo Class Library / UODemoSDK : A general class library for the UO Demo
' Copyright ©  2010-2011 JoinUO | batlin@joinuo.com 
' Licensed under the Open Software License version 3.0 (COPYING.txt).

Public Class SDB
  Implements IEnumerable

  Private sdbList As New List(Of String)
  Private sdbEncoder As New System.Text.ASCIIEncoding

  Public Shared Function LoadSDB(ByVal SDBpath As String, Optional ByVal ThrowException As Boolean = True) As SDB
    Try
      ' Pass-Through
      Return New SDB(SDBpath)
    Catch ex As Exception
      ' Exception handling
      If ThrowException Then
        Throw ex
      End If
      Return Nothing
    End Try
  End Function

  Public Shared Function LoadSDB(ByRef StreamSDB As IO.Stream, Optional ByVal ThrowException As Boolean = True) As SDB
    Try
      ' Pass-Through
      Dim sdb As New SDB
      sdb.Load(StreamSDB)
      Return sdb
    Catch ex As Exception
      ' Exception handling
      If ThrowException Then
        Throw ex
      End If
      Return Nothing
    End Try
  End Function

  Public Sub New()
  End Sub

  Public Sub New(ByVal SDBpath As String)
    ' Pass-Through
    Load(SDBpath, True)
  End Sub

  Public Sub Load(ByRef StreamSDB As IO.Stream, Optional ByRef KeepCurrentSDB As Boolean = False)
    ' Clear the current SDB if needed
    If Not KeepCurrentSDB Then
      sdbList.Clear()
    End If

    ' Read
    Dim Reader As New IO.StreamReader(StreamSDB, sdbEncoder, False)
    Do While Not Reader.EndOfStream
      sdbList.Add(Reader.ReadLine)
    Loop

    ' Support for the 'tutdragon' SDB bug!
    ' NOTE: this does not fix the bug, but prevents the decompiler from failing
    If sdbList.Count = 8850 Then
      Dim tmp As String = sdbList(8850 - 1)
      sdbList.Add("""" & Mid(tmp, 2, Len(tmp) - 2) & """")
    End If
  End Sub

  Public Sub Load(ByVal SDBpath As String, Optional ByRef KeepCurrentSDB As Boolean = False)
    ' Read the SDB file
    Dim ex As Exception = Nothing
    Dim StreamSDB As New IO.FileStream(SDBpath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
    Try
      Load(StreamSDB, KeepCurrentSDB)
    Catch tmpex As Exception
      ex = tmpex
    End Try
    Try
      StreamSDB.Close()
    Catch
    End Try
    If ex IsNot Nothing Then
      Throw ex
    End If
  End Sub

  Public Sub Save(ByRef StreamSDB As IO.FileStream)
    ' Write
    Dim Writer As New IO.StreamWriter(StreamSDB, sdbEncoder)
    For i As Integer = 0 To sdbList.Count - 1
      Writer.WriteLine(CType(sdbList(i), String))
    Next

    ' Very important! Flush the toilet!
    Writer.Flush()
  End Sub

  Public Sub Save(ByVal SDBpath As String)
    ' Write the SDB file
    Dim ex As Exception = Nothing
    Dim StreamSDB As New IO.FileStream(SDBpath, IO.FileMode.Create, IO.FileAccess.ReadWrite, IO.FileShare.Read)
    Try
      Save(StreamSDB)
    Catch tmpex As Exception
      ex = tmpex
    End Try
    Try
      StreamSDB.Close()
    Catch
    End Try
    If ex IsNot Nothing Then
      Throw ex
    End If
  End Sub

  Public Function GetByIndex(ByVal i As Integer) As String
    ' Return the string (if the given index is valid)
    If i < sdbList.Count Then
      Return sdbList(i)
    End If
    Return Nothing
  End Function

  Public Function FindExact(ByVal s As String) As Integer
    ' Find the item in the list and return
    Return sdbList.IndexOf(s)

    ' This was my original version until I discovered IndexOf
    ' (someone wants to buy me .NET reference book ?)
    'For i As Integer = 0 To sdbList.Count - 1
    '  If sdbList(i) = s Then
    '    Return i
    '  End If
    'Next
    'Return -1
  End Function

  Public Function AddAlways(ByVal s As String) As Integer
    ' Add the item always and return the index
    sdbList.Add(s)
    Return sdbList.Count - 1
  End Function

  Public Function Add(ByVal s As String) As Integer
    ' Add the item (if it does not yet exist) and return its index
    Dim i As Integer = FindExact(s)
    If i < 0 Then
      i = AddAlways(s)
    End If
    Return i
  End Function

  Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
    ' Pass-Through
    Return sdbList.GetEnumerator()
  End Function
End Class
