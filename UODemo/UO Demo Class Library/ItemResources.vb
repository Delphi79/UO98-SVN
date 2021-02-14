' UO Demo Class Library / UODemoSDK : A general class library for the UO Demo
' Copyright ©  2010-2011 JoinUO | batlin@joinuo.com 
' Licensed under the Open Software License version 3.0 (COPYING.txt).

'Imports System.IO
'Imports System.ComponentModel
'Imports JoinUO.WombatSDK

'Public Enum Types
'  Darkness
'End Enum

'Public Class ItemResourceType
'  Inherits MUL.ItemType

'  Private _ignore(4 - 1) As Int32
'  Public ReadOnly Property ignore As Int32()
'    Get
'      Return _ignore
'    End Get
'  End Property

'  Private Shadows Sub RaisePropertyChanged(ByVal propertyName As String)
'    MyBase.RaisePropertyChanged(Me, propertyName)
'  End Sub
'End Class

'Public Class ItemResource
'  Inherits MUL.Item

'  Public Overrides ReadOnly Property BinarySize As Integer
'    Get
'      If Content Is Nothing Then
'        Return 4
'      End If
'      Return 4 + (4 + 4 + 4 + 1 + 16 + 4)
'    End Get
'  End Property

'  Public Overrides Sub Read(ByVal SourceStream As BinaryReader)
'    Dim count As Int32 = SourceStream.ReadInt32
'    Dim NewContent As New ItemResourceType
'    For i As Integer = 1 To count
'      SourceStream.ReadInt32() '1
'      SourceStream.ReadInt32() '2
'      SourceStream.ReadInt32() '3
'      SourceStream.ReadByte() '4

'      ' Not sure why, but the core uses "seek" to skip over these 4 dwords aka 0x10 bytes
'      NewContent.ignore(0) = SourceStream.ReadInt32
'      NewContent.ignore(1) = SourceStream.ReadInt32
'      NewContent.ignore(2) = SourceStream.ReadInt32
'      NewContent.ignore(3) = SourceStream.ReadInt32

'      SourceStream.ReadInt32() '5
'    Next
'    Content = NewContent
'  End Sub
'End Class

'Public Class ItemResources
'  Inherits MUL.ItemList


'End Class
