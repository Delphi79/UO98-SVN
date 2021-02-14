' UO Demo Class Library / UODemoSDK : A general class library for the UO Demo
' Copyright ©  2010-2011 JoinUO | batlin@joinuo.com 
' Licensed under the Open Software License version 3.0 (COPYING.txt).

Public Class Obfuscator
  Private Shared SillyLookupTable() As UInt16 = {&H129, &H18BE, &H2CD6, &H26E9, &H1EB, &HBB3, &H2EA6, &H12DB, &H153C, &H7E87, &H390C, &HF3E, &H199, &H124, &H305E, &H39B3, &H2D12, &H26A6, &H5D03, &H1238, &H3B25, &H1E1F, &H1AD4, &H7F96, &H7FF5, &H323B, &H260D, &H30A, &H301C, &HBDB, &H732, &H120, &H5CFD, &H3E12, &H3BF6, &H3A9E, &HDDC, &H5E14, &H2E40, &H1CD0, &H7EB7, &H6032, &H2C3B, &H15A1, &H3EF6, &H409D, &H12E1, &H121F, &H26CA, &H3699, &H902, &H7BB9, &H139D, &H187E, &H16C5, &H3CD5, &H13E9, &H4080, &H5DB2, &H33EA, &H23C9, &H60BF, &H3CD6, &HFBF, &H2F14, &H47E, &H368E, &H2FFF, &H288F, &H7DD1, &H261E, &H5E9D, &H1916, &H32E6, &H401D, &H384, &H18D7, &HFC9, &HE12, &H2833, &H249E, &H2B0C, &H11F4, &H5DD5, &H127E, &H135, &H7CF, &H1AF4, &HECC, &H1D3, &HE90, &H3A2D, &H37E6, &H19D9, &H252A, &H37E5, &H1DC0, &H1481, &H4087, &H2B01, &H16D4, &H3A8D, &H7FBE, &HC7B, &HC15, &H3807, &H633, &H251F, &H1D18, &H3492, &H19DA, &H39CE, &H3BB1, &H3004, &H1796, &H1F16, &H182F, &H2CF7, &H5ED0, &H1316, &H5D24, &H588, &H7CFE, &H2725, &HDE5, &H13D3, &H29D8, &HA28, &H9CE, &H3960, &H263D, &H3B97, &H4027, &H138A, &H282D, &H5CCD, &H940, &H293B, &H40A5, &H1D11, &H2528, &HEA9, &H3F0B, &H3087, &H3F97, &H30F1, &H3295, &H1C1, &HCE1, &H3EE9, &H3F9A, &H30A7, &H2DB5, &H169A, &H2FE7, &H10D9, &H390, &H2A38, &H728, &H5C5E, &H1E1, &H1030, &H1BD9, &H159F, &H2BA5, &H28E2, &H2F0C, &H1289, &H3382, &H36C2, &H26B1, &H1CDF, &H27DA, &HE29, &H113E, &H2E39, &H1D3F, &H1D5E, &H1FF1, &H7E0E, &H6E3, &H36A1, &HC1E, &H2120, &H1DCB, &H12C2, &H1003, &H607, &H784, &H2B0F, &H3305, &H32E7, &H212C, &H18E, &H3308, &H1EDC, &H20A8, &H37BE, &H1EB, &H123B, &H3106, &H18C, &H357E, &HA87, &H5D2B, &H3FA, &HAF0, &H786, &H2332, &H1295, &H7DAA, &H2F0B, &H1BFC, &H13F5, &H1ECA, &HD9F, &H388A, &H15FD, &H7CB8, &H1AF6, &H17B, &H6014, &HE99, &H33CD, &H27D3, &H7F0D, &H4F0, &H183A, &H1FB4, &H13A6, &H190B, &H3605, &H20AD, &H32CF, &H2CD5, &H4B0, &H1927, &H8FF, &H31D8, &H914, &H13F4, &H3A27, &H387C, &H32C1, &H198C, &H3223, &H17B8, &H3895, &H248D, &H342D, &H5D3D, &H3260, &H32DE, &H2780, &H31AD, &H5DE9, &H5EA5, &H11D5, &H199F, &H2F15, &HE01, &H19FE, &H3821, &HB93, &HA2F, &H9B3, &H38F, &H328A, &H8AF, &H5CCA, &HC95, &H7CBE, &H7C27, &H5D2A, &H2FA1, &H31BE, &H15B4, &H7C9, &H27C0, &H1B32, &H2934, &H3E09, &H12C, &H2CC6, &H7FA6, &H6D8, &H3181, &H2738, &H3212, &H3F9, &H5D17, &HA1D, &H3B29, &H2BFA, &H2BB8, &H1DB5, &H2F95, &HEF5, &H5CDF, &H7B8B, &H17BD, &H21EB, &H2015, &H5DB8, &H15E1, &H5B60, &H3D8F, &HFF4, &H275B, &H3C8A, &H188F, &H5D27, &H7F5C, &H1F7, &H93B, &H2F84, &H1DC3, &H1DA7, &H1A30, &H2410, &H2EE, &H603, &H12F, &H2C9E, &H2BEF, &H3510, &HB9B, &H6E9, &H3B9E, &HB31, &H2581, &HDC7, &H36BF, &H15BD, &H11C, &H828, &HB7F, &H315D, &H13B9, &H634, &H260, &H7D3C, &H6DE, &H2B2, &H1F0D, &H322B, &H2EC, &H11B8, &H7DE2, &H1B7E, &H267D, &H2BD8, &H3D84, &HD1F, &H7E01, &H60BE, &H5B16, &H7FD6, &H2518, &H252B, &H5B2E, &H1F8B, &H3F0E, &H390E, &H7E94, &H1A31, &H40B5, &H3EA4, &H3990, &H2040, &H19FC, &H5D80, &H3921, &H7DB, &H7F7D, &H161E, &H2A8D, &H348C, &H36B8, &H1C5E, &H3FAF, &H40FA, &H37B0, &H194, &H301D, &H5DDC, &H5DF2, &H5E5B, &H3BA0, &H2E7F, &HE5C, &H3A36, &HDE9, &H502, &H8AC, &H151A, &H1B0B, &HEF7, &HF26, &H5DA3, &H2635, &H7FAD, &H1A2A, &H15D5, &H31B2, &H1732, &H190A, &H30DC, &H195D, &H5EB, &H13A, &H3693, &H3B0D, &H1E87, &HBDF, &H7A2, &H3ED5, &H60C6, &H20BC, &H198D, &H68F, &H314, &H5DA9, &H2718, &H328D, &H3DAE, &H6BB, &H60CA, &H1C20, &H13CF, &H75D, &H42F, &H37D7, &H7DA8, &H25CC, &H3D0D, &HA26, &HCED, &H2784, &H2FD9, &H5F8, &H1EB8, &H20D5, &H1AA0, &H1D5C, &H53C, &H1636, &H1785, &H2D8E, &H3981, &H17B0, &H5D85, &H7FE9, &H7C12, &H12C6, &H2FEC, &H15E2, &H7CA3, &H5EF3, &H5BB9, &H3F4, &H5E32, &H5C04, &H3002, &H41E, &H1F8, &H3590, &H140B, &H4037, &H6092, &H41C, &H1D37, &H7EF2, &H1D26, &H2F7E, &H2EF6, &H32ED, &H2997, &H19CA, &H1002, &H3908, &H363A, &H21C, &H11DF, &H361B, &H5EA6, &H292, &H1E5, &H2ABC, &H3B2B, &H318, &H7C0A, &H400E, &H21BE, &H3AE3, &H2D01, &H1DB, &H3CFF, &H33B2, &H311A, &H389, &H24F8, &H3105, &H7CB3, &H37FD, &H2A9E, &H3FD0, &H4FE, &H1DD4, &H9C9, &H3419, &H2714, &H3FD1, &HA3F, &H26F2, &H1BAD, &H23C0, &H2002, &H2694, &H5D12, &H1D94, &H3930, &H3AF, &H795, &H20E3, &HEDD, &H1DA1, &HA31, &H2B38, &H23CE, &H3ECA, &H34C5, &H3CE5, &H28B6, &H5C01, &H3C87, &H1684, &H1DA, &H1B3C, &H1F0E, &H3FD7, &H2F2, &H9B1, &H2D98, &H5B8F, &H1718, &H113F, &H2DEF, &H10DF, &H3BE, &H1739, &H1FE6, &H31B3, &H39B8, &H2404, &H7DB6, &H5D30, &H10E4, &H2D7B, &H3D3D, &H7DC4, &H1E2F, &H3DFF, &H29C9, &H2B13, &H2C90, &H15A9, &H2524, &H1C11, &H5EAD, &H83F, &H5AE, &H3DFD, &H2A9, &H1439, &H3635, &H2BE, &H381C, &H40CE, &H8BD, &H7DB3, &H40D3, &H917, &H2F0A, &H2B32, &H1EF6, &H250F, &H686, &H7F10, &H1613, &H8D2, &H1C2D, &H2684, &HAB6, &H21A2, &H1BD8, &H16D1, &HDAF, &H3A13, &H2ABA, &H2429, &H7FF4, &H18DD, &H1DA2, &H1E99, &H3412, &H3091, &H5D20, &H6C7, &H2E3D, &H7CEE, &H1CA0, &H19D0, &H2424, &H2D7F, &H253F, &H1C28, &HCC0, &HDA9, &H1BD, &H3EE4, &H2533, &H2706, &H1337, &H3283, &H7F20, &HD0C, &H34AF, &H383, &H2123, &H15A2, &H1831, &H618, &H3DDA, &H730, &H7B09, &HAE1, &H7B34, &H339A, &H34ED, &H243D, &H3602, &H19B6, &H350A, &H2B97, &H19F, &H327C, &H288C, &H27E0, &H3C0A, &H2F2A, &H359A, &H2B30, &H3594, &HAB5, &H1A9D, &HB2E, &H289C, &H2A98, &HA27, &H23EE, &H3833, &HDBD, &H21DE, &H1F1, &HDC3, &H7C0E, &H1C80, &H26BC, &H3D3E, &H27C2, &H350B, &H31CE, &H36B7, &H5CB1, &H5E11, &H2E08, &H3CCD, &H3528, &H181C, &H2BEB, &H27B8, &H13BF, &H2BB7, &H1A5C}

  Private Shared Function Random(ByVal Min As Integer, ByVal Max As Integer) As Integer
    Return CInt(Int((Max * Rnd()) + Min))
  End Function

  Public Shared Function Obfuscate(ByVal b As Byte, Optional ByVal SillyIndex As Integer = -1) As UInt16
    ' The byte must be within the range of the table
    If b >= SillyLookupTable.Length \ 5 Then
      Throw New Exception("Obfuscator.Obfuscate: byte cannot be obfuscated!")
    End If

    ' Obfuscate using the table and a random number (if needed)
    If SillyIndex < 0 Then
      SillyIndex = Random(0, 4)
    Else
      SillyIndex = SillyIndex Mod 5
    End If
    Return SillyLookupTable(b * 5 + SillyIndex)
  End Function

  Public Shared Function Deobfuscate(ByRef Contents() As Byte, ByVal Index As Integer) As Byte
    Return Deobfuscate(Contents(Index + 1) * 256 + Contents(Index))
  End Function

  Public Shared Function Deobfuscate(ByVal w As UInt16) As Byte
    ' Look up
    For i As Integer = SillyLookupTable.GetLowerBound(0) To SillyLookupTable.GetUpperBound(0)
      If SillyLookupTable(i) = w Then
        Return i \ 5
      End If
    Next

    ' Not found
    Return &HFF
  End Function

  Public Shared Function IsObfuscated(ByRef Contents() As Byte, ByVal Index As Integer, ByVal b As Byte) As Boolean
    Return IsObfuscated(Contents(Index + 1) * 256 + Contents(Index), b)
  End Function

  Public Shared Function IsObfuscated(ByVal w As UInt16, ByVal b As Byte) As Boolean
    ' Convert to byte to an index and verify the index
    Dim baseindex = b * 5
    If baseindex >= SillyLookupTable.Length Then
      Return False
    End If

    ' Look up (only 5 indexes)
    For i As Integer = baseindex To baseindex + 5 - 1
      If SillyLookupTable(i) = w Then
        Return True
      End If
    Next

    ' Not found
    Return False
  End Function
End Class
