' UO Demo Class Library / UODemoSDK : A general class library for the UO Demo
' Copyright ©  2010-2011 JoinUO | batlin@joinuo.com 
' Licensed under the Open Software License version 3.0 (COPYING.txt).

Public Class Events
  Public Class EventDefinition
#Region "Properties"
    Private _EventCode As Integer
    Public ReadOnly Property EventCode As Integer
      Get
        Return _EventCode
      End Get
    End Property

    Private _Name As String
    Public ReadOnly Property Name As String
      Get
        Return _Name
      End Get
    End Property

    Private _Parameters As String()
    Public ReadOnly Property Parameters As String()
      Get
        Return _Parameters
      End Get
    End Property

    Private _EventType As EventParamType
    ''' <summary>The Recomended event parameter type based on usage.</summary>
    ''' <remarks>This is not a strict requirement, but a hint for proper operation</remarks>
    Public ReadOnly Property EventType As EventParamType
      Get
        Return _EventType
      End Get
    End Property

    Private _Tokens As UoToken()
    Public ReadOnly Property Tokens As UoToken()
      Get
        Return _Tokens
      End Get
    End Property
#End Region

    Enum EventParamType
      Undefined = 0
      None = 1
      Number = 2
      Text = 3
      Time = 4
    End Enum

#Region "Constructors"
    Public Sub New(ByVal Initial_EventCode As Integer, ByVal Initial_Name As String, Optional ByVal Parameters() As String = Nothing, Optional ByVal Event_Type As EventParamType = EventParamType.Undefined)
      _EventCode = Initial_EventCode
      _Name = Initial_Name
      If Parameters IsNot Nothing AndAlso Parameters.Length > 0 Then
        Dim tmpParameters As New List(Of String)(Parameters.Length)
        Dim tmpTokens As New List(Of UoToken)(Parameters.Length)
        For Each Parameter As String In Parameters
          Parameter = Parameter.Trim
          If Parameter <> "" Then
            tmpParameters.Add(Parameter)

            Dim split() As String = Parameter.Split(" ")
            If split.Length = 2 Then
              Select Case split(0)
                Case "integer"
                  tmpTokens.Add(New UoToken.UoTypeName_int)
                Case "list"
                  tmpTokens.Add(New UoToken.UoTypeName_list)
                Case "location"
                  tmpTokens.Add(New UoToken.UoTypeName_loc)
                Case "object"
                  tmpTokens.Add(New UoToken.UoTypeName_object)
                Case "string"
                  tmpTokens.Add(New UoToken.UoTypeName_string)
                Case Else
                  Throw New Exception("Event Definition error!")
              End Select
              tmpTokens.Add(New UoToken.UoVariableName(split(1)))
              tmpTokens.Add(New UoToken.UoPunctuator_Comma) ' add a comma as seperator
            Else
              Throw New Exception("Event Definition error!")
            End If
          End If
        Next
        If tmpParameters.Count > 0 Then
          _Parameters = tmpParameters.ToArray
        Else
          _Parameters = Nothing
        End If
        If tmpTokens.Count > 0 Then
          tmpTokens.RemoveAt(tmpTokens.Count - 1) ' remove the last comma
          _Tokens = tmpTokens.ToArray
        Else
          _Tokens = Nothing
        End If
      Else
        _Parameters = Nothing
        _Tokens = Nothing
      End If
    End Sub

    Public Sub New(ByVal Initial_EventCode As Integer, ByVal Initial_Name As String, ByVal Parameters As String, Optional ByVal Param_Type As EventParamType = EventParamType.None)
      Me.New(Initial_EventCode, Initial_Name, Parameters.Split(","), Param_Type)
    End Sub
#End Region

    Private _asString As String = Nothing
    Public Overrides Function ToString() As String
      If (_asString Is Nothing) Then
        Dim params As String
        If (Parameters Is Nothing) Then
          params = String.Empty
        ElseIf (Parameters.Length = 0) Then
          params = String.Empty
        Else
          params = String.Intern(String.Join(", ", Parameters))
        End If
        _asString = String.Intern(String.Format("{0}({1})", _Name, params))
      End If
      Return _asString
    End Function

  End Class

  ' REFERENCE=005EE45C
  ' Event Type Params for Events which are not used in original OSI demo scripts are left undefined.
  Public Shared ReadOnly SupportedEvents() As EventDefinition = _
  { _
    New EventDefinition(&H0, "speech", "object speaker, string arg", EventDefinition.EventParamType.Text), _
    New EventDefinition(&H1, "gotattacked", "object attacker", EventDefinition.EventParamType.None), _
    New EventDefinition(&H2, "killedtarget", "object attacker", EventDefinition.EventParamType.None), _
    New EventDefinition(&H3, "aversion", "object target"), _
    New EventDefinition(&H4, "death", "object attacker, object corpse", EventDefinition.EventParamType.None), _
    New EventDefinition(&H5, "sawdeath", "object attacker, object victim, object corpse", EventDefinition.EventParamType.None), _
    New EventDefinition(&H6, "fightpulse", "object target"), _
    New EventDefinition(&H7, "washit", "object attacker, integer damamt", EventDefinition.EventParamType.None), _
    New EventDefinition(&H8, "failfood"), _
    New EventDefinition(&H9, "faildesire"), _
    New EventDefinition(&HA, "failshelter"), _
    New EventDefinition(&HB, "foundfood", "object target", EventDefinition.EventParamType.None), _
    New EventDefinition(&HC, "founddesire", "object target"), _
    New EventDefinition(&HD, "foundshelter", "object target"), _
    New EventDefinition(&HE, "time", , EventDefinition.EventParamType.Time), _
    New EventDefinition(&HF, "creation", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H10, "enterrange", "object target", EventDefinition.EventParamType.Number), _
    New EventDefinition(&H11, "leaverange", "object target", EventDefinition.EventParamType.Number), _
    New EventDefinition(&H12, "loiter"), _
    New EventDefinition(&H13, "seekfood", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H14, "seekdesire", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H15, "seekshelter", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H16, "message", "object sender, list args", EventDefinition.EventParamType.Text), _
    New EventDefinition(&H17, "use", "object user", EventDefinition.EventParamType.None), _
    New EventDefinition(&H18, "targetobj", "object user, object usedon", EventDefinition.EventParamType.None), _
    New EventDefinition(&H19, "targetloc", "object user, location place, integer objtype", EventDefinition.EventParamType.None), _
    New EventDefinition(&H1A, "weather"), _
    New EventDefinition(&H1B, "wasdropped", "object dropper", EventDefinition.EventParamType.None), _
    New EventDefinition(&H1C, "lookedat", "object looker", EventDefinition.EventParamType.None), _
    New EventDefinition(&H1D, "give", "object giver, object givenobj"), _
    New EventDefinition(&H1E, "wasgotten", "object getter", EventDefinition.EventParamType.None), _
    New EventDefinition(&H1F, "pathfound", , EventDefinition.EventParamType.Number), _
    New EventDefinition(&H20, "pathnotfound", , EventDefinition.EventParamType.Number), _
    New EventDefinition(&H21, "callback", , EventDefinition.EventParamType.Number), _
    New EventDefinition(&H22, "ishitting", "object victim, integer damamt", EventDefinition.EventParamType.None), _
    New EventDefinition(&H23, "convofunc", "object talker, string arg", EventDefinition.EventParamType.None), _
    New EventDefinition(&H24, "typeselected", "object user, integer listindex, integer objtype, integer objhue", EventDefinition.EventParamType.Number), _
    New EventDefinition(&H25, "hueselected", "object user, integer objhue", EventDefinition.EventParamType.Number), _
    New EventDefinition(&H26, "moon", "integer trammelchange, integer feluccachange"), _
    New EventDefinition(&H27, "minrangeattack", "object defender"), _
    New EventDefinition(&H28, "minrangedefend", "object attacker"), _
    New EventDefinition(&H29, "maxrangeattack", "object defender"), _
    New EventDefinition(&H2A, "maxrangedefend", "object attacker"), _
    New EventDefinition(&H2B, "destroyed", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H2C, "equip", "object equippedon", EventDefinition.EventParamType.None), _
    New EventDefinition(&H2D, "unequip", "object unequippedfrom", EventDefinition.EventParamType.None), _
    New EventDefinition(&H2E, "isstackableon", "object stackon", EventDefinition.EventParamType.None), _
    New EventDefinition(&H2F, "stackonto", "object stackon"), _
    New EventDefinition(&H30, "multirecycle", "integer oldtype, integer newtype", EventDefinition.EventParamType.None), _
    New EventDefinition(&H31, "decay", "integer oldvalue, integer newvalue", EventDefinition.EventParamType.None), _
    New EventDefinition(&H32, "serverswitch", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H33, "ooruse", "object user", EventDefinition.EventParamType.None), _
    New EventDefinition(&H34, "acquiredesire", "object target", EventDefinition.EventParamType.None), _
    New EventDefinition(&H35, "logout", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H36, "objectloaded", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H37, "genericgump", "object user, integer closeId, list selectList, list entryList", EventDefinition.EventParamType.Number), _
    New EventDefinition(&H38, "oortargetobj", "object user, object usedon", EventDefinition.EventParamType.None), _
    New EventDefinition(&H39, "pkpost", "object killer, object killee"), _
    New EventDefinition(&H3A, "textentry", "object sender, integer button, string text", EventDefinition.EventParamType.Number), _
    New EventDefinition(&H3B, "shop", "integer func", EventDefinition.EventParamType.None), _
    New EventDefinition(&H3C, "stolenfrom", "object stealer"), _
    New EventDefinition(&H3D, "objaccess", "object user, object usedon", EventDefinition.EventParamType.Number), _
    New EventDefinition(&H3E, "ishealthy", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H3F, "online", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H40, "transaccountcheck", "object target, integer transok", EventDefinition.EventParamType.None), _
    New EventDefinition(&H41, "transresponse", "object target, integer transok", EventDefinition.EventParamType.None), _
    New EventDefinition(&H42, "canbuy", "object buyer, object seller, integer quantity", EventDefinition.EventParamType.None), _
    New EventDefinition(&H43, "mobishitting", "object victim, integer damage", EventDefinition.EventParamType.None), _
    New EventDefinition(&H44, "famechanged", , EventDefinition.EventParamType.None), _
    New EventDefinition(&H45, "karmachanged"), _
    New EventDefinition(&H46, "murdercountchanged", , EventDefinition.EventParamType.None) _
  }

  Public Shared Function GetEventCode(ByVal EventName As String) As Integer
    For i As Integer = SupportedEvents.GetLowerBound(0) To SupportedEvents.GetUpperBound(0)
      If SupportedEvents(i).Name = EventName Then
#If DEBUG Then
        If SupportedEvents(i).EventCode <> i Then
          Throw New ExecutionEngineException
        End If
#End If
        Return i
      End If
    Next
    Return -1
  End Function

  Public Shared Function IsEventCodeValid(ByVal EventCode As Integer) As Boolean
    Return EventCode >= SupportedEvents.GetLowerBound(0) AndAlso EventCode <= SupportedEvents.GetUpperBound(0)
  End Function

  Public Shared Function GetEventName(ByVal EventCode As Integer) As String
    If Not IsEventCodeValid(EventCode) Then
      Return Nothing
    End If
    Return SupportedEvents(EventCode).Name
  End Function

  Public Shared Function GetEventParameters(ByVal EventCode As Integer) As String()
    If Not IsEventCodeValid(EventCode) Then
      Return Nothing
    End If
    Return SupportedEvents(EventCode).Parameters
  End Function

  Public Shared Function GetEventParameters(ByVal EventName As String) As String()
    Return GetEventParameters(GetEventCode(EventName))
  End Function

  Public Shared Function GetEventDefinition(ByVal EventCode As Integer) As EventDefinition
    If Not IsEventCodeValid(EventCode) Then
      Return Nothing
    End If
    Return SupportedEvents(EventCode)
  End Function

  Public Shared Function GetEventDefinition(ByVal EventName As String) As EventDefinition
    Return GetEventDefinition(GetEventCode(EventName))
  End Function
End Class
