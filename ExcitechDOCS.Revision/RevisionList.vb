Public Class RevisionList
    Inherits List(Of DocumentRevision)

    'Public Function GetMostRecent(RevisionID As String, Count As Integer, includeInternalRev As Boolean) As List(Of DocumentRevision)

    '    Dim _sr As New IO.StreamWriter("c:\temp\revision.log")
    '    _sr.WriteLine("RevisionId = " & RevisionID)
    '    _sr.WriteLine("Count = " & Count)
    '    _sr.WriteLine("IncludeInternalRev = " & includeInternalRev)

    '    Return MyBase.Where(Function(_dr)
    '                            If includeInternalRev Then
    '                                '' _sr.WriteLine("Found: " & _dr.RevisionID.ToString & " this is includeinternalrev: " & includeInternalRev)
    '                                Return _dr.RevisionID.StartsWith(RevisionID)
    '                            Else
    '                                '' _sr.WriteLine("Found: " & _dr.RevisionID.ToString & " this is includeinternalrev: " & includeInternalRev)
    '                                Return _dr.RevisionID.StartsWith(RevisionID) And Not _dr.RevisionID.Contains(".")
    '                            End If
    '                        End Function).OrderBy(Function(_dr)
    '                                                  Return _dr.ApproveDate
    '                                              End Function).Take(Count).ToList

    '    _sr.Dispose()

    'End Function

    Public Function GetMostRecent(RevisionID As String, Count As Integer, includeInternalRev As Boolean, wasShared As Boolean) As List(Of DocumentRevision)
        Throw New Exception("Method 'GetMostRecent' has been depreciated, use GetAll method instead")

        Return MyBase.Where(Function(_dr)
                                If Not includeInternalRev Then
                                    '' _sr.WriteLine("Found: " & _dr.RevisionID.ToString & " this is includeinternalrev: " & includeInternalRev)
                                    Return _dr.RevisionID.StartsWith(RevisionID) And _dr.WasShared = wasShared
                                Else
                                    '' _sr.WriteLine("Found: " & _dr.RevisionID.ToString & " this is includeinternalrev: " & includeInternalRev)
                                    Return _dr.RevisionID.StartsWith(RevisionID) And Not _dr.RevisionID.Contains(".") And _dr.WasShared = wasShared
                                End If
                            End Function).OrderByDescending(Function(_dr)
                                                                Return _dr.ApproveDate
                                                            End Function).Take(Count).ToList

    End Function

    Public Function GetAll(RevisionID As String, includeInternalRev As Boolean, wasShared As Boolean) As List(Of DocumentRevision)

        Return MyBase.Where(Function(_dr)
                                If Not includeInternalRev Then
                                    ''_sr.WriteLine("Found: " & _dr.RevisionID.ToString & " this is includeinternalrev: " & includeInternalRev)
                                    Return _dr.RevisionID.StartsWith(RevisionID) And _dr.WasShared = wasShared
                                Else
                                    ''_sr.WriteLine("Found: " & _dr.RevisionID.ToString & " this is includeinternalrev: " & includeInternalRev)
                                    Return _dr.RevisionID.StartsWith(RevisionID) And Not _dr.RevisionID.Contains(".") And _dr.WasShared = wasShared
                                End If
                            End Function).ToList



    End Function

    Public Function RevisionItem(itemIndex As Integer) As DocumentRevision
        Return MyBase.Item(itemIndex)
    End Function

    Public Function Count() As Integer
        Return MyBase.Count
    End Function

    Public Shared Function SeraliseRevisionList(RevisionList As RevisionList) As String
        Dim _jsonStr As String = Newtonsoft.Json.JsonConvert.SerializeObject(RevisionList)
        Return _jsonStr
    End Function

    Public Shared Function DeSeraliseRevisionList(jsonString As String) As RevisionList
        Dim _revList As New RevisionList
        _revList = Newtonsoft.Json.JsonConvert.DeserializeObject(Of RevisionList)(jsonString)

        If _revList Is Nothing Then _revList = New RevisionList
        Return _revList
    End Function

    Public Overloads Sub Add(Revision As DocumentRevision)
        Dim _index As Integer = -1
        _index = MyBase.FindIndex(Function(rv)
                                      If Revision Is Nothing Then Return False

                                      Return rv.RevisionID = Revision.RevisionID
                                  End Function)

        If _index > -1 Then Exit Sub

        MyBase.Add(Revision)
    End Sub

    Public Function IsNothing(itemIndex As Integer) As Boolean
        Return MyBase.Item(itemIndex) Is Nothing
    End Function

    Public Sub SortRevisions(sortOrder As String, sortBy As Integer)
        ''0 = sort asc
        ''1 = sort desc

        'Public Enum SortBy
        '    RevisionID
        '    CheckedBy
        '    CheckedDate
        '    ApproveBy
        '    ApproveDate
        'End Enum

        Debugger.Launch()

        Select Case sortOrder.ToUpperInvariant
            Case "ASC"
                'Me.OrderBy(Function(_dr)

                '               Return _dr.ApproveDate
                '           End Function)

                Me.Sort(Function(x, y)
                            Dim _ret As Integer = 0

                            Select Case sortBy
                                Case 0 ''revisionID
                                    _ret = String.Compare(x.RevisionID, y.RevisionID)
                                Case 1 ''checked by
                                    _ret = String.Compare(x.CheckedBy, y.CheckedBy)
                                Case 2 ''checkeddate
                                    _ret = x.CheckedDate.CompareTo(y.CheckedDate)
                                Case 3 ''approvedby
                                    _ret = String.Compare(x.ApproveBy, y.ApproveBy)
                                Case 4 ''approveDate
                                    _ret = x.ApproveDate.CompareTo(y.ApproveDate)
                            End Select

                            Return _ret
                        End Function)
            Case "DESC"
                Me.Sort(Function(x, y)
                            Dim _ret As Integer = 0

                            Select Case sortBy
                                Case 0 ''revisionID
                                    _ret = String.Compare(y.RevisionID, x.RevisionID)
                                Case 1 ''checked by
                                    _ret = String.Compare(y.CheckedBy, x.CheckedBy)
                                Case 2 ''checkeddate
                                    _ret = y.CheckedDate.CompareTo(x.CheckedDate)
                                Case 3 ''approvedby
                                    _ret = String.Compare(y.ApproveBy, x.ApproveBy)
                                Case 4 ''approveDate
                                    _ret = y.ApproveDate.CompareTo(x.ApproveDate)
                            End Select

                            Return _ret
                        End Function)
        End Select

        Debugger.Break()

    End Sub

    Public Sub GetTopRevisions(revisionCount As Integer)
        Dim _newList As New List(Of DocumentRevision)

        Debugger.Break()

        _newList = Me.Take(revisionCount).ToList

        Dim _pad As Integer = _newList.Count() - revisionCount
        For _p As Integer = 0 To _pad - 1
            _newList.Add(Nothing)
        Next

        Me.Clear()
        Me.AddRange(_newList)
    End Sub


End Class