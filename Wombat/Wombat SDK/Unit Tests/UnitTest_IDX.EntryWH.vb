Imports System.Text

<TestClass()>
Public Class UnitTest_IDXentryWH

	Private testContextInstance As TestContext

	'''<summary>
	'''Gets or sets the test context which provides
	'''information about and functionality for the current test run.
	'''</summary>
	Public Property TestContext() As TestContext
		Get
			Return testContextInstance
		End Get
		Set(ByVal value As TestContext)
			testContextInstance = value
		End Set
	End Property

	<TestMethod()>
	Public Sub TheBinarySizeOf_IDXentryWH_MustBeEqualToThatOf_IDXentry()
		Assert.AreEqual(IDX.EntryWH.BinarySize, IDX.Entry.BinarySize)
	End Sub

	<TestMethod()>
	Public Sub TheEmptyConstructor_IDXentryWH_SetsAllMembersTo_Minus1()
		Dim totest As New IDX.EntryWH
		Assert.AreEqual(-1, totest.lookup)
		Assert.AreEqual(-1, totest.length)
		Assert.AreEqual(-1, totest.extra)
	End Sub

	<TestMethod()>
	Public Sub TheRelationOf_IDXentryWHextra_IsCorrectWith_WidthAndHeight()
		Dim totest As New IDX.EntryWH(0, -1, &H1234ABCDUI)
		Assert.AreEqual(&HABCDS, totest.width)
		Assert.AreEqual(&H1234S, totest.height)
	End Sub
End Class
