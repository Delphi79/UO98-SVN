Imports System.Text

<TestClass()>
Public Class UnitTest__IDXentry
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
	Public Sub BinarySize_MustBe_12()
		Assert.AreEqual(12, IDX.Entry.BinarySize)
	End Sub

	<TestMethod()>
	Public Sub TheEmptyConstructor_IDXentry_SetsAllMembersTo_Minus1()
		Dim totest As New IDX.Entry
		Assert.AreEqual(-1, totest.lookup)
		Assert.AreEqual(-1, totest.length)
		Assert.AreEqual(-1, totest.extra)
	End Sub

	<TestMethod()>
	Public Sub IDXentry_WithNegativeLength_ShouldBeConsideredPatched()
		Dim totest As New IDX.Entry(0, -2, 0)
		Assert.IsTrue(totest.IsPatched)
	End Sub

	<TestMethod()>
	Public Sub IDXentry_WithExceptionalLengthOf_Minus1_ShouldNotBeConsideredPatched()
		Dim totest As New IDX.Entry(0, -1, 0)
		Assert.IsFalse(totest.IsPatched)
	End Sub

	<TestMethod()>
	Public Sub IDXentry_WithLengthOf_1_ShouldNotBeConsideredPatched()
		Dim totest As New IDX.Entry(0, 1, 0)
		Assert.IsFalse(totest.IsPatched)
	End Sub

	<TestMethod()>
	Public Sub aSaved_IDXentry_CanBeReRead()
		Dim towrite As New IDX.Entry(1, 2, 3)
		Dim s As New IO.MemoryStream(IDX.Entry.BinarySize)
		towrite.Write(New IO.BinaryWriter(s))
		s.Seek(0, IO.SeekOrigin.Begin)
		Dim totest As New IDX.Entry
		totest.Read(New IO.BinaryReader(s))
		Assert.AreEqual(1, totest.lookup)
		Assert.AreEqual(2, totest.length)
		Assert.AreEqual(3, totest.extra)
	End Sub

	<TestMethod()>
	Public Sub IDXentry_CanBePatchedCorrectly()
		Dim totest As New IDX.Entry(1, 2, 3)
		Dim toapply As New IDX.Entry(4, 5, 6)
		totest.ApplyPatch(toapply)
		Assert.AreEqual(4, totest.lookup)
		Assert.AreEqual(Extensions.SetHighestBit(5), totest.length)
		Assert.AreEqual(6, totest.extra)
	End Sub
End Class